using Entities.Enums;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Entities
{
    public class Friend : BaseEntity
    {

        [JsonProperty("senderProfileId")]
        [ForeignKey("SenderProfile")]
        public Guid SenderProfileId { get; set; }
        [JsonProperty("senderProfile")]
        public Profile SenderProfile { get; set; }

        [JsonProperty("receiverProfileId")]
        [ForeignKey("ReceiverProfile")]
        public Guid receiverProfileId { get; set; }
        [JsonProperty("recieverProfile")]
        public Profile ReceiverProfile { get; set; }

        

        [JsonProperty("relationStatus")]
        public RelationStatus RelationStatus { get; set; }

        [JsonProperty("invitationId")]
        [ForeignKey("Invitation")]
        public Guid? InvitationId { get; set; }
        [JsonProperty("invitation")]
        public FriendInvitation Invitation { get; set; }
    }

}
