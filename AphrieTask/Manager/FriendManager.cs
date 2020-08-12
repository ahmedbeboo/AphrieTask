using Entities;
using Entities.Enums;
using Repository;
using Services.MailServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace AphrieTask.Manager
{
    public class FriendManager
    {
        private IBaseRepository<Friend> _friendRepository;
        private IBaseRepository<Profile> _profileRepository;
        private IBaseRepository<FriendInvitation> _friendInvitationRepository;

        private EmailSender _emailSender;

        public FriendManager(IBaseRepository<Friend> friendRepository,
            IBaseRepository<FriendInvitation> friendInvitationRepository,
            IBaseRepository<Profile> profileRepository)
        {
            _friendRepository = friendRepository;
            _friendInvitationRepository = friendInvitationRepository;
            _profileRepository = profileRepository;

            _emailSender = new EmailSender();
        }


        private string toEnglishNumber(string input)
        {
            string EnglishNumbers = "";

            for (int i = 0; i < input.Length; i++)
            {
                if (Char.IsDigit(input[i]))
                {
                    EnglishNumbers += char.GetNumericValue(input, i);
                }
                else
                {
                    EnglishNumbers += input[i].ToString();
                }
            }
            return EnglishNumbers;
        }

        public async void AddPendingFriendsInvitations(Profile newjoiner)
        {
            Guid invitedByGuid;
            bool isGuid = Guid.TryParse(newjoiner.InvitedBy, out invitedByGuid);
            if (isGuid)
            {
                List<FriendInvitation> friendInvitations =await _friendInvitationRepository.Where(x => x.SenderId == invitedByGuid && x.PhoneNumber == newjoiner.PhoneNumber);

                var invitedByInvitation = friendInvitations.FirstOrDefault();
                if (invitedByInvitation == null)
                {
                    _friendRepository.Insert(
                        new Friend()
                        {
                            ReceiverProfile = newjoiner,
                            RelationStatus = RelationStatus.Accepted,
                            SenderProfileId = invitedByGuid
                        });
                }
            }

            var invitations =await _friendInvitationRepository.Where(x => x.PhoneNumber == newjoiner.PhoneNumber);
            foreach (var invitation in invitations)
            {
                var friend = new Friend()
                {
                    ReceiverProfile = newjoiner,
                    Invitation = invitation,
                    SenderProfileId = invitation.SenderId
                };
                if (isGuid)
                {
                    friend.RelationStatus = (invitation.SenderId == invitedByGuid) ? RelationStatus.Accepted : RelationStatus.Pending;
                }
                else
                {
                    friend.RelationStatus = RelationStatus.Pending;
                }
                _friendRepository.Insert(friend);
            }
        }

        internal async Task<int> InviteFriend(FriendInvitation input)
        {
            List<FriendInvitation> friendInvitations =await _friendInvitationRepository.Where(x => x.SenderId == input.SenderId && x.PhoneNumber == input.PhoneNumber && x.Processed == false);

            var invitaionexists = friendInvitations.FirstOrDefault();
            if (invitaionexists == null)
            {
                input.PhoneNumber = toEnglishNumber(input.PhoneNumber);

                if (!input.PhoneNumber.StartsWith("+2"))
                    input.PhoneNumber = "+2" + input.PhoneNumber;

                List<Profile> receiverInfo =await _profileRepository.Where(x => (x.PhoneNumber == input.PhoneNumber));
                var receiver = receiverInfo.FirstOrDefault();

                var sender =await _profileRepository.GetById(input.SenderId);
                if (receiver == null)
                {
                    _friendInvitationRepository.Insert(input);

                    // Send Email Invitation
                    SendInvitation(sender.FullName, sender.MyInvitationCode, input);
                    return 1; // sent invitation to external user
                }
                else
                {
                    if (receiver.PhoneNumber == sender.PhoneNumber)
                    {
                        return 7; // same number
                    }

                    var previousSentRequest =await _friendRepository.Where(x => x.SenderProfileId == sender.Id && x.receiverProfileId == receiver.Id);
                    var previousReceivedRequest =await _friendRepository.Where(x => x.SenderProfileId == receiver.Id && x.receiverProfileId == sender.Id);

                    if (previousSentRequest != null && previousSentRequest.Count > 0)
                    {
                        foreach (var item in previousSentRequest)
                        {
                            if (item.RelationStatus == RelationStatus.Pending || item.RelationStatus == RelationStatus.WaitingForAcceptance)
                            {
                                return 3; // you have sent a friend request before, please wait until your friend's approval
                            }
                            else if (item.RelationStatus == RelationStatus.Accepted)
                            {
                                return 4; // already friends
                            }
                            else if (item.RelationStatus == RelationStatus.Blocked)
                            {
                                return 6; // blocked
                            }
                        }
                    }

                    if (previousReceivedRequest != null && previousReceivedRequest.Count > 0)
                    {
                        foreach (var item in previousReceivedRequest)
                        {
                            if (item.RelationStatus == RelationStatus.Pending || item.RelationStatus == RelationStatus.WaitingForAcceptance)
                            {
                                return 5; // please accept his request
                            }
                            else if (item.RelationStatus == RelationStatus.Accepted)
                            {
                                return 4; // already friends
                            }
                            else if (item.RelationStatus == RelationStatus.Blocked)
                            {
                                return 6; // blocked
                            }
                        }
                    }


                    input.Processed = true;
                    _friendRepository.Insert(
                        new Friend()
                        {
                            ReceiverProfile = receiver,
                            RelationStatus = RelationStatus.Pending,
                            Invitation = input,
                            SenderProfile = sender
                        });


                    return 2; // send friend request to internal user
                }
            }
            else
            {
                return 0; // invitation exists
            }
        }

        private void SendInvitation(string senderFullName, string code, FriendInvitation input)
        {

            if (input.Email != null && input.Email != "")
            {
                SendEmailInvitation(senderFullName, input.Email);
            }
        }
        private void SendEmailInvitation(string sendarFullName, string email)
        {


            string htmlString = $"<html> <body> Hello, <br/> {sendarFullName} invites you to join us, please follow this link to download the app</body> </html>";


            var result = _emailSender.SendEmail("Invitation Sent", htmlString, "tt436209@gmail.com", "Test App", email, email,
                                    null, null, null, null, null,
                                    null, 0, null
                                    );
        }


        internal async Task<List<Friend>> GetMyCurrentlyFriendsAsync(Guid? clientId)
        {
            var navfriendProperties = new string[] { "ReceiverProfile" };
            var navsenderProperties = new string[] { "SenderProfile" };
            var mymadefriends = await _friendRepository.Where(x => x.SenderProfile.Id == clientId.Value &&
            x.RelationStatus == RelationStatus.Accepted, navfriendProperties);
            var myivnitelist = await _friendRepository.Where(x => x.ReceiverProfile.Id == clientId.Value &&
            x.RelationStatus == RelationStatus.Accepted, navsenderProperties);
            List<Friend> rslt = new List<Friend>();
            rslt.AddRange(mymadefriends);
            rslt.AddRange(myivnitelist);
            return rslt;
        }

        internal async Task<List<Friend>> GetMySentFriendRequestsAsync(Guid? clientId)
        {
            var navProperties = new string[] { "ReceiverProfile" };

            var myrequestlist = await _friendRepository.Where(x => x.SenderProfile.Id == clientId.Value &&
            (x.RelationStatus == RelationStatus.Pending), navProperties);
            foreach (var item in myrequestlist)
            {
                item.RelationStatus = RelationStatus.WaitingForAcceptance;
            }
            List<Friend> rslt = new List<Friend>();
            rslt.AddRange(myrequestlist);
            return rslt;
        }

        internal async Task<List<Friend>> GetMyReceivedFriendRequestsAsync(Guid? clientId)
        {
            var navProperties = new string[] { "SenderProfile" };
            var myivnitelist = await _friendRepository.Where(x => x.ReceiverProfile.Id == clientId.Value &&
            (x.RelationStatus == RelationStatus.Pending), navProperties);
            List<Friend> rslt = new List<Friend>();
            rslt.AddRange(myivnitelist);
            return rslt;
        }

        internal async void RemoveFriend(Guid id)
        {
            var friend =await GetFriendshipById(id);
            _friendRepository.Delete(friend);
        }

        internal async void RemoveFriendInvitation(Guid id)
        {
            var invitation =await GetFriendInvitationById(id);
            _friendInvitationRepository.Delete(invitation);
        }

        internal async Task<Friend> GetFriendshipById(Guid friendshipId)
        {
            try
            {
                return await _friendRepository.GetById(friendshipId);
            }
            catch
            {
                return null;
            }
        }


        internal async Task<Profile> GetFriendById(Guid friendId)
        {
            try
            {
                return await _profileRepository.GetById(friendId);
            }
            catch
            {
                return null;
            }
        }


        internal async Task<List<FriendInvitation>> GetMyFriendInvitations(Guid? ProfileId, int limit, int page)
        {
            List<FriendInvitation> invitations = await _friendInvitationRepository.WhereOrdered(x => x.SenderId == ProfileId && x.Processed == false, y => y.CreatedDate);
            return invitations;
        }


        internal async Task<FriendInvitation> GetFriendInvitationById(Guid friendId)
        {
            try
            {
                return await _friendInvitationRepository.GetById(friendId);
            }
            catch
            {
                return null;
            }
        }

        internal void UpdateFriend(Friend friend)
        {
            try
            {
                _friendRepository.Update(friend);
            }
            catch { }
        }


    }
}
