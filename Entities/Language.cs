using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Entities
{
    [JsonObject(Title = "language")]
    public class Language : BaseEntity
    {
        //[JsonProperty]
        //[Key]
        //[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        //public int languageId { get; set; }

        [JsonProperty("languageName")]
        [Required(ErrorMessage = "This Field Is Required")]
        [Display(Name = "Language Name")]
        public string languageName { get; set; }



        [JsonProperty("languageCulture")]
        [Required(ErrorMessage = "This Field Is Required")]
        [Display(Name = "Language Culture")]
        public string languageCulture { get; set; }
    }
}
