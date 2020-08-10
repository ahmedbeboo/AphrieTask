using Entities.Enums;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Entities
{
    public class PostInteraction : BaseEntity
    {
        [JsonProperty("postId")]
        [ForeignKey("Post")]
        public Guid postId { get; set; }

        [JsonProperty("Post")]
        public Post Post { get; set; }

        [JsonProperty("postReact")]
        public Reacts postReact { get; set; }

        [JsonProperty("postComments")]
        public string postComments { get; set; }



        [JsonProperty("userInteractId")]
        [ForeignKey("UserInteract")]
        public Guid userInteractId { get; set; }
        [JsonProperty("UserInteract")]
        public Profile UserInteract { get; set; }


        [JsonProperty("isDeleted")]
        public bool isDeleted { get; set; }
    }
}
