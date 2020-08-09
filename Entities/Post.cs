using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Entities
{
    [JsonObject(Title = "post")]
    public class Post : BaseEntity
    {
        //[JsonProperty("text")]
        //public string text { get; set; }

        [JsonProperty("imagesUrl")]
        public string imagesUrl { get; set; }

        [NotMapped]
        IList<IFormFile> images { get; set; }


        [JsonProperty("profileId")]
        [ForeignKey("Profile")]
        public Guid profileId { get; set; }
        [JsonProperty("Profile")]
        public Profile Profile { get; set; }

        [JsonProperty("isDeleted")]
        public bool isDeleted { get; set; }
    }
}
