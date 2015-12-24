#pragma warning disable 1591

using ServiceStack.DataAnnotations;
using ServiceStack.OrmLite;
using System;
using System.ComponentModel.DataAnnotations;

namespace PictureAuction.SOA.Shared.ServiceModel.Types
{
    [Alias("PictureByUser")]
    public class PictureByUser
    {
        [Required]
        public DateTime BuyingTime { get; set; }

        [Required]
        [ForeignKey(typeof (Picture))]
        public int PictureId { get; set; }

        [Required]
        [ForeignKey(typeof (User))]
        public Guid UserId { get; set; }
    }
}

#pragma warning restore 1591