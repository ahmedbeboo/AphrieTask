using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Entities
{
    public class RegisterAttempt : BaseEntity
    {
        [JsonProperty("profileId")]
        [ForeignKey("Profile")]
        public Guid profileId { get; set; }
        [JsonProperty("Profile")]
        public Profile Profile { get; set; }


        [JsonProperty("OTP")]
        public string OTP { get; set; }
        public bool IsUsed { get; set; }

    }
}
