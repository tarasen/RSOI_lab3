#pragma warning disable 1591

using ServiceStack.DataAnnotations;
using ServiceStack.OrmLite;
using System;
using System.ComponentModel.DataAnnotations;

namespace PictureAuction.SOA.Shared.ServiceModel.Types
{
    [Alias("TUser")]
    public class User
    {
        [ForeignKey(typeof (Artist))]
        public int? ArtistId { get; set; }

        [Required]
        public decimal Bill { get; set; }

        [Required]
        [PrimaryKey]
        [Alias("UserId")]
        public Guid Id { get; set; }

        [Required]
        public string Login { get; set; }

        [Required]
        public string PasswordHash { get; set; }

        [Required]
        public string Salt { get; set; }
    }
}

#pragma warning restore 1591