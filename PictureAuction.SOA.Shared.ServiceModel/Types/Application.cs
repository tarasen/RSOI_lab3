#pragma warning disable 1591

using ServiceStack.DataAnnotations;
using ServiceStack.OrmLite;
using System;
using System.ComponentModel.DataAnnotations;

namespace PictureAuction.SOA.Shared.ServiceModel.Types
{
    [Alias("Application")]
    public class Application
    {
        [Required]
        [PrimaryKey]
        public Guid ClientId { get; set; }

        [Required]
        public string RedirectUri { get; set; }

        [Required]
        public Guid SecretKey { get; set; }

        [Required]
        [ForeignKey(typeof (User))]
        public Guid UserId { get; set; }
    }
}

#pragma warning restore 1591