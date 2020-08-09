using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AphrieTask.BE
{
    public class ParametersModel
    {
        public ParametersModel()
        {
            CreatedAtMin = null;
            CreatedAtMax = null;
            UpdatedAtMin = null;
            UpdatedAtMax = null;
            Limit = 250;
            Page = 1;
            Fields = string.Empty;
            ClientId = null;
            Ids = new List<int>();
        }

        /// <summary>
        /// the client guid who sent this request
        /// </summary>
        [JsonProperty("clientId")]
        public Guid? ClientId { get; set; }

        /// <summary>
        /// Show entities created after date (format: 2008-12-31 03:00)
        /// </summary>
        [JsonProperty("created_at_min")]
        public DateTime? CreatedAtMin { get; set; }

        /// <summary>
        /// Show entities created before date (format: 2008-12-31 03:00)
        /// </summary>
        [JsonProperty("created_at_max")]
        public DateTime? CreatedAtMax { get; set; }

        /// <summary>
        /// Show entities last updated after date (format: 2008-12-31 03:00)
        /// </summary>
        [JsonProperty("updated_at_min")]
        public DateTime? UpdatedAtMin { get; set; }

        /// <summary>
        /// Show entities last updated before date (format: 2008-12-31 03:00)
        /// </summary>
        [JsonProperty("updated_at_max")]
        public DateTime? UpdatedAtMax { get; set; }

        /// <summary>
        /// A comma-separated list of entity ids
        /// </summary>
        [JsonProperty("ids")]
        public List<int> Ids { get; set; }

        /// <summary>
        /// Amount of results (default: 50) (maximum: 250)
        /// </summary>
        [JsonProperty("limit")]
        public int Limit { get; set; }

        /// <summary>
        /// Page to show (default: 1)
        /// </summary>
        [JsonProperty("page")]
        public int Page { get; set; }

        /// <summary>
        /// <ul>
        /// <li>published - Show only published categories</li>
        /// <li>unpublished - Show only unpublished categories</li>
        /// <li>any - Show all categories(default)</li>
        /// </ul>
        /// </summary>
        [JsonProperty("published_status")]
        public bool? PublishedStatus { get; set; }

        /// <summary>
        /// comma-separated list of fields to include in the response
        /// </summary>
        [JsonProperty("fields")]
        public string Fields { get; set; }

        public override string ToString()
        {
            string obj = "";
            obj += "limit=" + Limit.ToString();
            obj += "&page=" + Page.ToString();
            if (ClientId != null)
            {
                obj += "&clientId=" + ClientId.ToString();
            }
            if (CreatedAtMin != null)
            {
                obj += "&created_at_min=" + CreatedAtMin.ToString();
            }
            if (CreatedAtMax != null)
            {
                obj += "&created_at_max=" + CreatedAtMax.ToString();
            }
            if (UpdatedAtMin != null)
            {
                obj += "&updated_at_min=" + UpdatedAtMin.ToString();
            }
            if (UpdatedAtMax != null)
            {
                obj += "&updated_at_max=" + UpdatedAtMax.ToString();
            }
            if (PublishedStatus.HasValue)
            {
                obj += "&published_status=" + PublishedStatus.Value.ToString();
            }
            if (Fields != string.Empty)
            {
                obj += "&fields=" + Fields.ToString();
            }
            if (Ids.Count > 0)
            {
                var strids = "";
                foreach (int item in Ids)
                {
                    strids += item.ToString() + ",";
                }
                obj += "&ids=" + strids;
            }

            return obj;
        }
    }
}
