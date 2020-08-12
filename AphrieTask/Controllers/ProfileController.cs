using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Threading.Tasks;
using AphrieTask.BE;
using Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Repository;
using Services.MailServices;

namespace AphrieTask.Controllers
{
    [Authorize]
    [Produces("application/json")]
    [Route("api/Profile")]
    public class ProfileController : Controller
    {
        private IBaseRepository<Friend> _friendRepository;
        private IBaseRepository<Profile> _profileRepository;
        private IBaseRepository<FriendInvitation> _friendInvitationRepository;
        private Manager.ProfileManager _profileManager;
        private IBaseRepository<ProfilePassword> _passwordRepository;
        private IBaseRepository<RegisterAttempt> _registerAttemptRepository;

        private static HttpClient client;


        public ProfileController(IBaseRepository<Profile> repository, IBaseRepository<Friend> friendRepository,
        IBaseRepository<FriendInvitation> friendInvitationRepository, IBaseRepository<ProfilePassword> passwordRepository, IBaseRepository<RegisterAttempt> registerAttemptRepository)
        {

            _profileRepository = repository;
            _friendRepository = friendRepository;
            _friendInvitationRepository = friendInvitationRepository;
            _passwordRepository = passwordRepository;
            _registerAttemptRepository = registerAttemptRepository;


            _profileManager = new Manager.ProfileManager(_friendRepository, _friendInvitationRepository, _profileRepository, _passwordRepository, _registerAttemptRepository);
        }

        private static void PrepareClient(string Token)
        {
            client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            var token = Token;
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
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


        [HttpGet("{profileId}")]
        public async Task<IActionResult> GetProfile(Guid profileId)
        {
            // Get the claims values
            var Claim = User.Claims.Where(c => c.Type == ClaimTypes.PrimarySid)
                               .Select(c => c.Value).SingleOrDefault();

            if (profileId.ToString() != Claim)
            {
                return Unauthorized();
            }

            // get user profile by id
            Profile result = await _profileManager.GetProfileByGuid(profileId);
            return Ok(result);
        }

        [HttpGet("PhoneNumber/{phoneNumber}")]
        public async Task<IActionResult> GetProfileByPhoneNumber(string phoneNumber)
        {
            try
            {
                phoneNumber = toEnglishNumber(phoneNumber);
            }
            catch
            {
                return Unauthorized();
            }

            if (!phoneNumber.StartsWith("+"))
            {
                if (phoneNumber.StartsWith("010") || phoneNumber.StartsWith("011") || phoneNumber.StartsWith("012") || phoneNumber.StartsWith("015"))
                {
                    phoneNumber = "+2" + phoneNumber;
                }

                else
                {
                    phoneNumber = "+" + phoneNumber;
                }
            }



            Profile result = await _profileManager.GetProfileByPhoneNumber(phoneNumber);

            // Get the claims values
            var Claim = User.Claims.Where(c => c.Type == ClaimTypes.PrimarySid)
                               .Select(c => c.Value).SingleOrDefault();

            if (result.Id.ToString() != Claim)
            {
                return BadRequest();
            }

            if (result != null)
            {
                return Ok(result);
            }
            else
            {
                return NotFound();
            }
        }


        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> CreateProfile([FromBody] Profile input)
        {
            try
            {
                input.PhoneNumber = toEnglishNumber(input.PhoneNumber);
            }
            catch
            {
                return BadRequest(false);
            }

            var result = await _profileManager.GetProfileByPhoneNumber(input.PhoneNumber);
            if (result != null)
            {
                return BadRequest(false);
            }
            else
            {
                var checkMail =await _profileRepository.Where(p => p.Email == input.Email);

                
                if (input.Email == "" || checkMail.Count == 0)
                {
                    if (!input.PhoneNumber.StartsWith("+2"))
                        input.PhoneNumber = "+2" + input.PhoneNumber;

                    if (!string.IsNullOrWhiteSpace(input.InvitedBy))
                    {
                        try
                        {

                            input.InvitedBy =await _profileManager.GetUserIdByInvitationCode(input.InvitedBy);

                        }
                        catch
                        {
                            return BadRequest(false);
                        }
                    }


                    _profileManager.InsertProfile(input);
                    Guid invitedByGuid;
                    bool isGuid = Guid.TryParse(input.InvitedBy, out invitedByGuid);
                    if (isGuid)
                    {
                        var profile =await _profileManager.GetProfileByGuid(Guid.Parse(input.InvitedBy));
                        
                    }
                    _profileManager.InsertProfilePassword(input.Id, input.Password);

                    // send Mail With Confirmation Code
                    _profileManager.sendConfirmationMail(input.Email, input.FullName,input.Id);

                    return Ok(true);


                }

                else
                {
                    return BadRequest("Email Already Existed");
                }


            }
        }

        [HttpPost("verifyRegister")]
        [AllowAnonymous]
        public async Task<IActionResult> verifyRegister([FromBody] verifyRegister input)
        {
            input.phone= toEnglishNumber(input.phone);

            if (!input.phone.StartsWith("+2"))
                input.phone = "+2" + input.phone;


            var user = await _profileManager.GetProfileByPhoneNumber(input.phone);

            if (user != null)
            {
                
                var userLastRegisterAttempt =await _profileManager.getLastUserRegisterAttempt(user.Id);
                if(userLastRegisterAttempt != null)
                {
                    if (user.Id == userLastRegisterAttempt.profileId && input.OTP == userLastRegisterAttempt.OTP)
                    {

                        user.EmailConfirmed = true;
                        userLastRegisterAttempt.IsUsed = true;

                        int updatedProfile =await _profileManager.UpdateProfile(user);
                        if (updatedProfile == 0)
                        {
                            string token =await _profileManager.verifyUser(user);
                            _profileManager.UpdateUserRegisterAttempt(userLastRegisterAttempt);

                            return Ok(token);

                        }
                        else
                        {
                            return BadRequest("Erroor in Verify Your Account... try again");
                        }
                    }
                    else
                    {
                        return NotFound("unvalid Phone or OTP");
                    }
                }
                else
                {
                    return BadRequest("unvalid Phone or OTP");
                }
            }
            else
            {
                return BadRequest("Wrong User Phone");
            }
        }

        [HttpPost("Login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginModel loginModel)
        {
            
            //try
            //{
            //    loginModel.phoneNumber = toEnglishNumber(loginModel.phoneNumber);
            //}
            //catch
            //{
            //    return BadRequest("Issue In Phone Number");
            //}

            //if (!loginModel.phoneNumber.StartsWith("+"))
            //{
            //    if (loginModel.phoneNumber.StartsWith("010") || loginModel.phoneNumber.StartsWith("011") || loginModel.phoneNumber.StartsWith("012") || loginModel.phoneNumber.StartsWith("015"))
            //    {
            //        loginModel.phoneNumber = "+2" + loginModel.phoneNumber;
            //    }

            //    else
            //    {
            //        loginModel.phoneNumber = "+" + loginModel.phoneNumber;
            //    }
            //}

            Profile result = await _profileManager.GetProfileByUserName(loginModel.userName);
            if (result != null && result.EmailConfirmed)
            {
                bool correct =await _profileManager.IsPasswordCorrect(result.Id, loginModel.password);
                if (!correct)
                {
                    return BadRequest("Wrong Password");
                }

                result.Token = _profileManager.GetAuthToken(result);
                return Ok(result.Token);

            }

            return NotFound("This User Is Not Found");


        }

        [HttpPut]
        public async Task<IActionResult> EditProfile([FromBody] Profile input)
        {
            try
            {
                try
                {
                    input.PhoneNumber = toEnglishNumber(input.PhoneNumber);
                }
                catch
                {
                    return BadRequest("Issue In Phone Number");
                }

                // Get the claims values
                var Claim = User.Claims.Where(c => c.Type == ClaimTypes.PrimarySid)
                                   .Select(c => c.Value).SingleOrDefault();

                if (input.Id.ToString() != Claim)
                {
                    return Unauthorized();
                }


                if (!string.IsNullOrWhiteSpace(input.InvitedBy))
                {
                    try
                    {

                        input.InvitedBy =await _profileManager.GetUserIdByInvitationCode(input.InvitedBy);

                    }
                    catch
                    { }
                }
                var checkUpdate =await _profileManager.UpdateProfile(input);

                if (checkUpdate == 1)
                {
                    return BadRequest("Email Must Not Empty");
                }

                if (checkUpdate == 2)
                {
                    return BadRequest("Email Already Exist");
                }


                return Ok(input);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpPut("ChangePassword")]
        public async Task<IActionResult> ChangePassword([FromBody] PasswordModel input)
        {
            var result=await _profileManager.ChangePassword(input);

            if (result)
            {
                return Ok(true);
            }

            return Ok(false);
        }
    }
}
