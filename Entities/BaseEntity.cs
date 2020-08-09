using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Entities
{
    public abstract class BaseEntity : IBaseEntity
    {
        [JsonProperty("id")]
        public Guid Id { get; set; }

        [JsonProperty("createdDate")]
        public DateTime CreatedDate { get; set; }

        [JsonProperty("updatedDate")]
        public DateTime UpdatedDate { get; set; }



        public BaseEntity()
        {
            Id = Guid.NewGuid();
            CreatedDate = DateTime.Now;

        }
    }

}
