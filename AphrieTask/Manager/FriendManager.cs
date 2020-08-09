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

        public void AddPendingFriendsInvitations(Profile newjoiner)
        {
            Guid invitedByGuid;
            bool isGuid = Guid.TryParse(newjoiner.InvitedBy, out invitedByGuid);
            if (isGuid)
            {
                var invitedByInvitation = _friendInvitationRepository.Where(x => x.SenderId == invitedByGuid && x.PhoneNumber == newjoiner.PhoneNumber).Result.FirstOrDefault();
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

            var invitations = _friendInvitationRepository.Where(x => x.PhoneNumber == newjoiner.PhoneNumber).Result;
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

        internal int InviteFriend(FriendInvitation input)
        {
            var invitaionexists = _friendInvitationRepository.Where(x => x.SenderId == input.SenderId && x.PhoneNumber == input.PhoneNumber && x.Processed == false).Result.FirstOrDefault();
            if (invitaionexists == null)
            {
                var receiver = _profileRepository.Where(x => (x.PhoneNumber == input.PhoneNumber)).Result.FirstOrDefault();
                var sender = _profileRepository.GetById(input.SenderId).Result;
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

                    var previousSentRequest = _friendRepository.Where(x => x.SenderProfileId == sender.Id && x.receiverProfileId == receiver.Id).Result.ToList();
                    var previousReceivedRequest = _friendRepository.Where(x => x.SenderProfileId == receiver.Id && x.receiverProfileId == sender.Id).Result.ToList();

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
            var navfriendProperties = new string[] { "FriendProfile" };
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
            var navProperties = new string[] { "FriendProfile" };

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

        internal void RemoveFriend(Guid id)
        {
            var friend = GetFriendById(id).Result;
            _friendRepository.Delete(friend);
        }

        internal void RemoveFriendInvitation(Guid id)
        {
            var invitation = GetFriendInvitationById(id).Result;
            _friendInvitationRepository.Delete(invitation);
        }

        internal async Task<Friend> GetFriendById(Guid friendId)
        {
            return await _friendRepository.GetById(friendId);
        }

        internal async Task<List<FriendInvitation>> GetMyFriendInvitations(Guid? ProfileId, int limit, int page)
        {
            List<FriendInvitation> invitations = await _friendInvitationRepository.WhereOrdered(x => x.SenderId == ProfileId && x.Processed == false, y => y.CreatedDate);
            return invitations;
        }


        internal async Task<FriendInvitation> GetFriendInvitationById(Guid friendId)
        {
            return await _friendInvitationRepository.GetById(friendId);
        }

        internal void UpdateFriend(Friend friend)
        {
            _friendRepository.Update(friend);
        }


    }
}
