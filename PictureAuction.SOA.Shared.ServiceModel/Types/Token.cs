#pragma warning disable 1591

using ServiceStack.DataAnnotations;
using ServiceStack.OrmLite;
using System;
using System.ComponentModel.DataAnnotations;

namespace PictureAuction.SOA.Shared.ServiceModel.Types
{
    [Alias("Token")]
    public class Token
    {
        [Required]
        [PrimaryKey]
        public Guid AccessToken { get; set; }

        [Required]
        [ForeignKey(typeof (OAuthCode))]
        public Guid Code { get; set; }

        [Required]
        public DateTime ExpiresIn { get; set; }
    }
}

#pragma warning restore 1591