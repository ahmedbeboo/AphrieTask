using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace AphrieTask.Manager
{
    public class ConfigurationsManager
    {
        public string Token { get; set; }
        public string ConnectionString { get; set; }
        public string ApiUrl { get; set; }
        public string SiteUrl { get; set; }
        public string redirectAfterConfirmMail { get; set; }

        public ConfigurationsManager()
        {

            using (StreamReader r = new System.IO.StreamReader("appsettings.json"))
            {
                string json = r.ReadToEnd();

                dynamic item = JsonConvert.DeserializeObject(json);

                Token = item.Keys.Secret;
                ConnectionString = item.DatabaseSettings.ConnectionString;
                ApiUrl = item.URLs.ApiUrl;
                SiteUrl = item.URLs.siteUrl;
                redirectAfterConfirmMail = item.URLs.redirectAfterConfirmMail;
            }
        }

    }

}
