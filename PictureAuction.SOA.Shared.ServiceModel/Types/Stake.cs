#pragma warning disable 1591

using ServiceStack.DataAnnotations;
using ServiceStack.DesignPatterns.Model;
using ServiceStack.OrmLite;
using System;
using System.ComponentModel.DataAnnotations;

namespace PictureAuction.SOA.Shared.ServiceModel.Types
{
    [Alias("Stake")]
    public class Stake : IHasId<int>
    {
        [Required]
        public decimal Cost { get; set; }

        [Required]
        [ForeignKey(typeof (Lot))]
        public int LotId { get; set; }

        [Required]
        public DateTime Time { get; set; }

        [Required]
        [ForeignKey(typeof (User))]
        public Guid UserId { get; set; }

        [Alias("StakeId")]
        [AutoIncrement]
        [PrimaryKey]
        public int Id { get; set; }
    }
}

#pragma warning restore 1591