#pragma warning disable 1591

using ServiceStack.DataAnnotations;
using ServiceStack.OrmLite;
using System;
using System.ComponentModel.DataAnnotations;

namespace PictureAuction.SOA.Shared.ServiceModel.Types
{
    [Alias("OAuthCode")]
    public class OAuthCode
    {
        [Required]
        [ForeignKey(typeof (Application))]
        public Guid ClientId { get; set; }

        [Required]
        [PrimaryKey]
        public Guid Code { get; set; }

        [Required]
        [ForeignKey(typeof (User))]
        public Guid UserId { get; set; }
    }
}

#pragma warning restore 1591