#pragma warning disable 1591

using ServiceStack.DataAnnotations;
using ServiceStack.OrmLite;
using System.ComponentModel.DataAnnotations;

namespace PictureAuction.SOA.Shared.ServiceModel.Types
{
    [Alias("PicturesByGenre")]
    public class PicturesByGenre
    {
        [Required]
        [ForeignKey(typeof (Genre))]
        public int GenreId { get; set; }

        [Required]
        [ForeignKey(typeof (Picture))]
        public int PictureId { get; set; }
    }
}

#pragma warning restore 1591