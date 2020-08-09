using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Entities
{
    [JsonObject(Title = "profilePassword")]
    public class ProfilePassword : BaseEntity
    {

        [JsonProperty("profileId")]
        [ForeignKey("Profile")]
        public Guid profileId { get; set; }
        [JsonProperty("Profile")]
        public Profile Profile { get; set; }


        [JsonProperty("hashPassword")]
        public string HashPassword
        {
            get; set;
        }
    }
}
