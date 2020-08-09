using Entities.Enums;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Entities
{
    [JsonObject(Title = "profile")]
    public class Profile : BaseEntity
    {
        public Profile()
        {
            EmailConfirmed = false;
        }

        [JsonProperty("active")]
        public bool Active { get; set; }
        
        [JsonProperty("birthDate")]
        public Nullable<DateTime> BirthDate { get; set; }
        
        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("emailConfirmed")]
        public bool EmailConfirmed { get; set; }

        [JsonProperty("firstName")]
        public string FirstName { get; set; }
        
        [JsonProperty("lastName")]
        public string LastName { get; set; }

        [JsonProperty("gender")]
        public int Gender { get; set; }
        
        [JsonProperty("phoneNumber")]
        public string PhoneNumber { get; set; }
        
        [JsonProperty("userName")]
        public string UserName { get; set; }
        
        [JsonProperty("loginType")]
        public SocialType LoginType { get; set; }
        
        [JsonProperty("imageUrl")]
        public string ImageUrl { get; set; }
        
        [JsonProperty("invitedBy")]
        public string InvitedBy { get; set; }
        
        
        [JsonProperty("language")]
        public string Language { get; set; }
        
        [NotMapped]
        [JsonProperty("password")]
        public string Password { get; set; }
        
        [NotMapped]
        [JsonProperty("fullName")]
        public string FullName
        {
            get
            {
                string fullName = FirstName;
                if (String.IsNullOrWhiteSpace(LastName))
                {
                    return fullName;
                }
                else
                {
                    return fullName + " " + LastName;
                }
            }
            set { }
        }


        [JsonProperty("myInvitationCode")]
        public string MyInvitationCode { get; set; }


        public string GetFullName()
        {
            return FirstName + " " + LastName;
        }

        [NotMapped]
        [JsonProperty("authToken")]
        public string Token { get; set; }

        [JsonProperty("isBlocked")]
        public bool isBlocked { get; set; }


    }
}
