using AphrieTask.BE;
using Entities;
using Microsoft.IdentityModel.Tokens;
using Repository;
using Services.MailServices;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace AphrieTask.Manager
{
    public class ProfileManager
    {
        private IBaseRepository<Friend> _friendRepository;
        private IBaseRepository<Profile> _profileRepository;
        private IBaseRepository<FriendInvitation> _friendInvitationRepository;
        private IBaseRepository<ProfilePassword> _passwordRepository;
        private IBaseRepository<RegisterAttempt> _registerAttemptRepository;

        private EmailSender _emailSender;

        public ProfileManager(IBaseRepository<Friend> friendRepository,
            IBaseRepository<FriendInvitation> friendInvitationRepository,
            IBaseRepository<Profile> profileRepository,
            IBaseRepository<ProfilePassword> passwordRepository,
            IBaseRepository<RegisterAttempt> registerAttemptRepository)
        {
            _friendRepository = friendRepository;
            _friendInvitationRepository = friendInvitationRepository;
            _profileRepository = profileRepository;
            _passwordRepository = passwordRepository;
            _registerAttemptRepository = registerAttemptRepository;

            _emailSender = new EmailSender();

        }


        public void InsertProfile(Profile input)
        {
            //input.BirthDate = DateTime.Now.Date;
            input.Language = "en";
            input.MyInvitationCode = input.PhoneNumber.Substring(5, 4) + new Random().Next(9999999).ToString();

            _profileRepository.Insert(input);

        }

        public void InsertProfilePassword(Guid profileid, string password)
        {
            ProfilePassword profilePassword = new ProfilePassword();
            profilePassword.profileId = profileid;
            profilePassword.HashPassword = GetHash(password);
            _passwordRepository.Insert(profilePassword);
        }

        public async Task<bool> IsPasswordCorrect(Guid profileid, string password)
        {
            List<ProfilePassword> passwords =await _passwordRepository.Where(x => x.profileId == profileid);

            var passwordList = passwords.OrderByDescending(x => x.CreatedDate).ToList();
            var userPassword = passwordList.FirstOrDefault();
            return VerifyHash(password, userPassword.HashPassword);
        }


        public async Task<string> verifyUser(Profile input)
        {
            
            input.Token = GetAuthToken(input);

            try
            {
                // add all pending invitations to the profile as pending friends
                new FriendManager(_friendRepository, _friendInvitationRepository, _profileRepository).AddPendingFriendsInvitations(input);

                // update all invitations sent to this phone number to be processed = true
                var invitations =await _friendInvitationRepository.Where(x => x.PhoneNumber == input.PhoneNumber);
                foreach (var item in invitations)
                {
                    item.Processed = true;
                    _friendInvitationRepository.Update(item);
                }
            }
            catch { }

            return input.Token;
        }

        public async Task<RegisterAttempt> getLastUserRegisterAttempt(Guid profileid)
        {
            try
            {
                List<RegisterAttempt> registerAttemptList =await _registerAttemptRepository.Where(r => r.profileId == profileid && !r.IsUsed);

                var registerAttempt = registerAttemptList.OrderByDescending(r => r.CreatedDate).FirstOrDefault();
                return registerAttempt;
            }
            catch
            {
                return null;
            }

        }

        public async Task<List<RegisterAttempt>> getAllUserRegisterAttempt(Guid profileid)
        {
            try
            {
                var registerAttempt =await _registerAttemptRepository.Where(r => r.profileId == profileid);

                return registerAttempt.OrderByDescending(r => r.CreatedDate).ToList();
            }
            catch
            {
                return null;
            }

        }

        public bool UpdateUserRegisterAttempt(RegisterAttempt input)
        {
            try
            {
                _registerAttemptRepository.Update(input);
                return true;
            }
            catch
            {
                return false;
            }

        }


        internal async Task<Profile> GetProfileByPhoneNumber(string phoneNumber)
        {
            var result = await _profileRepository.Where(x => x.PhoneNumber == phoneNumber);
            return result.FirstOrDefault();
        }

        internal async Task<Profile> GetProfileByUserName(string userName)
        {
            var result = await _profileRepository.Where(x => x.UserName == userName);
            return result.FirstOrDefault();
        }


        public async Task<Profile> GetProfileByGuid(Guid profileId)
        {
            var profile =await _profileRepository.GetById(profileId);
            return profile;
        }

        public async Task<string> GetUserIdByInvitationCode(string invitationCode)
        {
            List<Profile> profiles =await _profileRepository.Where(x => x.MyInvitationCode == invitationCode);

            var profile = profiles.FirstOrDefault();
            if (profile != null)
                return profile.Id.ToString();

            throw new Exception("Wrong Invitation Code");

        }

        public async Task<int> UpdateProfile(Profile input)
        {
            // (0) ---> updated succefully
            int updated = 0;

            var savedobj =await _profileRepository.GetById(input.Id);

            if (input.Email != null)
            {

                if (input.Email.Trim() == "")
                {
                    // (1) ---> Mail Must have a value
                    updated = 1;
                    return updated;
                }

                var profiles =await _profileRepository.Where(x => x.Email.ToLower() == input.Email.ToLower());

                var result = profiles.FirstOrDefault();
                if (result != null && result.Id != savedobj.Id)
                {
                    //throw new Exception(ErrorMessagesEnum.EmailAlreadyExisted.ToString());

                    // (2) ---> Email Already Existed
                    updated = 2;
                    return updated;
                }

                savedobj.Email = input.Email;
            }
            savedobj.FirstName = input.FirstName;
            savedobj.LastName = input.LastName;
            savedobj.Gender = input.Gender;
            savedobj.BirthDate = input.BirthDate;
            savedobj.InvitedBy = input.InvitedBy;

            _profileRepository.Update(savedobj);
            input.Token = GetAuthToken(input);

            return updated;
        }


        public async Task<bool> ChangePassword(PasswordModel passwordModel)
        {
            var oldPasswords =await _passwordRepository.Where(p => p.profileId == passwordModel.profileId && p.HashPassword == GetHash(passwordModel.oldPassword));

            var oldPassword = oldPasswords.FirstOrDefault();
            if (oldPassword != null)
            {
                try
                {
                    InsertProfilePassword(passwordModel.profileId, passwordModel.newPassword);
                    return true;
                }
                catch
                {
                    return false;
                }
            }

            return false;
        }
        public bool sendConfirmationMail(string mail, string name,Guid profileID)
        {
            try
            {
                Random generator = new Random();
                string randomCode = generator.Next(0, 999999).ToString("D6");

                string htmlString = $"<html> <body> Hello, <br/> {name} Your Invetation Code Is: {randomCode} </body> </html>";


                var Mailresult = _emailSender.SendEmail("Confirmation Code", htmlString, "tt436209@gmail.com", "Test App", mail, name,
                                        null, null, null, null, null,
                                        null, 0, null
                                        );

                if (Mailresult)
                {
                    try
                    {
                        RegisterAttempt registerAttempt = new RegisterAttempt();
                        registerAttempt.profileId = profileID;
                        registerAttempt.OTP = randomCode;
                        registerAttempt.IsUsed = false;
                        _registerAttemptRepository.Insert(registerAttempt);
                    }
                    catch { }
                }

                return Mailresult;
            }
            catch
            {
                return false;
            }

        }
        private string GetHash(string input)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] data = sha256.ComputeHash(Encoding.UTF8.GetBytes(input));

                StringBuilder result = new StringBuilder();

                for (int i = 0; i < data.Length; i++)
                {
                    result.Append(data[i].ToString("X2"));
                }
                return result.ToString();
            }

        }
        private bool VerifyHash(string input, string hash)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                // Hash the input. 
                string hashOfInput = GetHash(input);

                // Create a StringComparer an compare the hashes.
                StringComparer comparer = StringComparer.OrdinalIgnoreCase;

                if (0 == comparer.Compare(hashOfInput, hash))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
        public string GetAuthToken(Profile profile)
        {
            var confgManager = new ConfigurationsManager();
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    //new Claim("http://schemas.microsoft.com/accesscontrolservice/2010/07/claims/identityprovider", "ASP.NET Identity", "http://www.w3.org/2001/XMLSchema#string"),
                    new Claim(ClaimTypes.Name, profile.FirstName+profile.LastName),
                    new Claim(ClaimTypes.PrimarySid,profile.Id.ToString()),
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.ASCII.GetBytes(confgManager.Token)), SecurityAlgorithms.HmacSha256Signature)
            };

            //foreach (var role in roles)
            //    tokenDescriptor.Subject.AddClaim(new Claim(ClaimTypes.Role, role));
            var tokenHandler = new JwtSecurityTokenHandler();
            return tokenHandler.WriteToken(tokenHandler.CreateToken(tokenDescriptor));
        }

    }
}
