using Entities.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Entities
{
    public class Log:BaseEntity
    {
        public string requestInfo { get; set; }

        public string responseInfo { get; set; }

        public string userInfo { get; set; }

        public LogLevel Level { get; set; }

        public string MSG { get; set; }

    }
}
