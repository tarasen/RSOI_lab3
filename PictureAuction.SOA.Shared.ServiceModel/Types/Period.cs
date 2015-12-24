#pragma warning disable 1591

using ServiceStack.DataAnnotations;
using ServiceStack.DesignPatterns.Model;
using ServiceStack.OrmLite;
using System.ComponentModel.DataAnnotations;

namespace PictureAuction.SOA.Shared.ServiceModel.Types
{
    [Alias("Period")]
    public class Period : IHasId<int>
    {
        [Required]
        public string Name { get; set; }

        [ForeignKey(typeof (Nation))]
        public int? NationId { get; set; }

        public string StartDate { get; set; }

        [Alias("PeriodId")]
        [AutoIncrement]
        [PrimaryKey]
        public int Id { get; set; }
    }
}

#pragma warning restore 1591