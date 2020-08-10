using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Entities;
using Entities.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Repository;

namespace AphrieTask.Controllers
{
    [Authorize]
    [Produces("application/json")]
    [Route("api/Friend")]
    public class FriendController : Controller
    {
        private readonly IBaseRepository<Profile> _profileRepository;
        private readonly IBaseRepository<Friend> _friendRepository;
        private readonly IBaseRepository<FriendInvitation> _friendInvitationRepository;
        private readonly Manager.FriendManager _friendManager;

        public FriendController(IBaseRepository<Profile> repository, IBaseRepository<Friend> friendRepository,
        IBaseRepository<FriendInvitation> friendInvitationRepository)
        {

            _profileRepository = repository;
            _friendRepository = friendRepository;
            _friendInvitationRepository = friendInvitationRepository;

            _friendManager = new Manager.FriendManager(_friendRepository, _friendInvitationRepository, _profileRepository);
        }

        [HttpGet("GetMyFriends")]
        public async Task<IActionResult> GetMyFriends(BE.ParametersModel parameters)
        {
            // Get the claims values
            var Claim = User.Claims.Where(c => c.Type == ClaimTypes.PrimarySid)
                               .Select(c => c.Value).SingleOrDefault();

            if (parameters.ClientId.Value.ToString() != Claim)
            {
                return Unauthorized();
            }

            // get user friends by userid, status, number per page
            // in case of no relationship sent it will return all firends
            List<Friend> friends = await _friendManager.GetMyCurrentlyFriendsAsync(parameters.ClientId);
            return Ok(friends);
        }

        [HttpGet("GetMyPendingFriends")]
        public async Task<IActionResult> GetMyPendingFriends(BE.ParametersModel parameters)
        {
            // Get the claims values
            var Claim = User.Claims.Where(c => c.Type == ClaimTypes.PrimarySid)
                               .Select(c => c.Value).SingleOrDefault();

            if (parameters.ClientId.Value.ToString() != Claim)
            {
                return Unauthorized();
            }

            // get user friends by userid, status, number per page
            // in case of no relationship sent it will return all firends
            List<Friend> sentfriends = await _friendManager.GetMySentFriendRequestsAsync(parameters.ClientId);
            List<Friend> receivedfriends = await _friendManager.GetMyReceivedFriendRequestsAsync(parameters.ClientId);

            List<Friend> allPending = new List<Friend>();
            allPending.AddRange(sentfriends);
            allPending.AddRange(receivedfriends);

            return Ok(allPending);
        }

        [HttpGet("GetInvitationSent")]
        public async Task<IActionResult> GetInvitationSent(BE.ParametersModel parameters)
        {

            // Get the claims values
            var Claim = User.Claims.Where(c => c.Type == ClaimTypes.PrimarySid)
                               .Select(c => c.Value).SingleOrDefault();

            if (parameters.ClientId.Value.ToString() != Claim)
            {
                return Unauthorized();
            }

            // get user friends by userid, status, number per page
            // in case of no relationship sent it will return all firends
            List<FriendInvitation> friends = await _friendManager.GetMyFriendInvitations(parameters.ClientId, parameters.Limit, parameters.Page);
            return Ok(friends);
        }


        [HttpPost("SendFriendInvitation")]
        public IActionResult SendFriendInvitation([FromBody] FriendInvitation input)
        {


            if (String.IsNullOrEmpty(input.PhoneNumber))
            {
                return Unauthorized();
            }


            // Get the claims values
            var Claim = User.Claims.Where(c => c.Type == ClaimTypes.PrimarySid)
                               .Select(c => c.Value).SingleOrDefault();

            if (input.SenderId.ToString() != Claim)
            {
                return Unauthorized();
            }

            // for sending invitation for new user
            // if the user found on our db then send a friend request
            int result = _friendManager.InviteFriend(input);

            if (result == 0)
            {
                return BadRequest("Send Invitaion To Friend Before");
            }
            else if (result == 1)
            {
                string data = "We have send an invitation to " + input.PhoneNumber + ". Make sure that your friends uses your inviation code when register";

                return Json(new { message = data });
            }
            else if (result == 2)
            {
                string data = "your friend has been added ... Invitation sent successfully!";
                return Json(new { message = data });
            }
            else if (result == 3)
            {
                return BadRequest("Send Invitaion To Friend Before Still Pending");
            }
            else if (result == 4)
            {
                return BadRequest("Already Friends");
            }
            else if (result == 5)
            {
                return BadRequest("Already Sent Friend Request");
            }
            
            else if (result == 7)
            {
                return BadRequest("Friend Request To YourSelf");
            }
            else
            {
                return BadRequest("Friend Request Error");
            }


        }

        [HttpGet("GetFriendshipInfo/{friendshipId}")]
        public async Task<IActionResult> GetFriendshipInfo(Guid friendshipId, BE.ParametersModel parameter)
        {
            // Get the claims values
            var Claim = User.Claims.Where(c => c.Type == ClaimTypes.PrimarySid)
                               .Select(c => c.Value).SingleOrDefault();

            if (parameter.ClientId.Value.ToString() != Claim)
            {
                return Unauthorized();
            }

            // return friend profile 
            Friend friend = await _friendManager.GetFriendshipById(friendshipId);
            return Ok(friend);
        }


        [HttpGet("GetFriendInfo/{friendId}")]
        public async Task<IActionResult> GetFriendInfo(Guid friendId, BE.ParametersModel parameter)
        {
            // Get the claims values
            var Claim = User.Claims.Where(c => c.Type == ClaimTypes.PrimarySid)
                               .Select(c => c.Value).SingleOrDefault();

            if (parameter.ClientId.Value.ToString() != Claim)
            {
                return Unauthorized();
            }

            // return friend profile 
            Profile friend = await _friendManager.GetFriendById(friendId);
            return Ok(friend);
        }


        [HttpPut("ChangeFriendStatus")]
        public IActionResult ChangeFriendStatus([FromBody] Friend friend)
        {
            try
            {
                // Get the claims values
                var Claim = User.Claims.Where(c => c.Type == ClaimTypes.PrimarySid)
                                   .Select(c => c.Value).SingleOrDefault();

                if (friend.receiverProfileId.ToString() != Claim)
                {
                    return Unauthorized();
                }

                // change user friend status 
                // accept, reject, block
                _friendManager.UpdateFriend(friend);

                return Ok(friend);
            }
            catch
            {
                return BadRequest();
            }
        }

        [HttpDelete("DeleteFriendRequest/{Id}")]
        public void DeleteFriendRequest(Guid Id)
        {
            _friendManager.RemoveFriend(Id);
        }

        [HttpDelete("FriendInvitation/{Id}")]
        public void DeleteFriendInvitationRequest(Guid Id)
        {
            _friendManager.RemoveFriendInvitation(Id);

        }
    }
}
