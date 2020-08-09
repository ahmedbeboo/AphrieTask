using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Entities
{
    public class LoacalizProperty:BaseEntity
    {

        [JsonProperty("localizeLanguge")]
        [ForeignKey("localizeLangugeId")]
        public Language localizeLanguge { get; set; }

        [JsonProperty("localizeLangugeId")]
        public Guid localizeLangugeId { get; set; }

        [JsonProperty("localizeTableName")]
        public string localizeTableName { get; set; }

        [JsonProperty("localizeAttributeName")]
        public string localizeAttributeName { get; set; }

        [JsonProperty("localizeValue")]
        public string localizeValue { get; set; }

        [JsonProperty("localizeEntityId")]
        public Guid localizeEntityId { get; set; }

        [JsonProperty("isDeleted")]
        public bool isDeleted { get; set; }
    }

}
