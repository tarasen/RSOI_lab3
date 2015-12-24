#pragma warning disable 1591

using ServiceStack.DataAnnotations;
using ServiceStack.OrmLite;
using System.ComponentModel.DataAnnotations;

namespace PictureAuction.SOA.Shared.ServiceModel.Types
{
    [Alias("PicturesByArtist")]
    public class PicturesByArtist
    {
        [Required]
        [ForeignKey(typeof (Artist))]
        public int ArtistId { get; set; }

        [Required]
        [ForeignKey(typeof (PicturesByArtist))]
        public int PictureId { get; set; }
    }
}

#pragma warning restore 1591