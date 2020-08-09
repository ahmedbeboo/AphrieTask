using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AphrieTask.BE
{
    public class PasswordModel
    {
        public Guid profileId { get; set; }

        public string oldPassword { get; set; }

        public string newPassword { get; set; }
    }
}
