#pragma warning disable 1591

using ServiceStack.DataAnnotations;
using ServiceStack.DesignPatterns.Model;
using ServiceStack.OrmLite;
using System;
using System.ComponentModel.DataAnnotations;

namespace PictureAuction.SOA.Shared.ServiceModel.Types
{
    [Alias("Lot")]
    public class Lot : IHasId<int>
    {
        [Required]
        public DateTime EndTime { get; set; }

        [Required]
        [ForeignKey(typeof (Picture))]
        public int PictureId { get; set; }

        [Alias("LotId")]
        [AutoIncrement]
        [PrimaryKey]
        public int Id { get; set; }
    }
}

#pragma warning restore 1591