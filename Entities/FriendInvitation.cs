using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Entities
{
    public class FriendInvitation : BaseEntity
    {
        [ForeignKey("Sender")]
        public Guid SenderId { get; set; }
        public Profile Sender { get; set; }
        public string Email { get; set; }

        public string PhoneNumber { get; set; }

        public bool Processed { get; set; } = false;
    }
}
