#pragma warning disable 1591

using ServiceStack.DataAnnotations;
using ServiceStack.OrmLite;
using System.ComponentModel.DataAnnotations;

namespace PictureAuction.SOA.Shared.ServiceModel.Types
{
    [Alias("ArtistsByPeriod")]
    public class ArtistsByPeriod
    {
        [Required]
        [ForeignKey(typeof (Artist))]
        public int ArtistId { get; set; }

        [Required]
        [ForeignKey(typeof (Period))]
        public int PeriodId { get; set; }
    }
}

#pragma warning restore 1591