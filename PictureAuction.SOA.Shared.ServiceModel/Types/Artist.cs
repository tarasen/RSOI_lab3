#pragma warning disable 1591

using ServiceStack.DataAnnotations;
using ServiceStack.DesignPatterns.Model;
using ServiceStack.OrmLite;
using System.ComponentModel.DataAnnotations;

namespace PictureAuction.SOA.Shared.ServiceModel.Types
{
    [Alias("Artist")]
    public class Artist : IHasId<int>
    {
        [Required]
        public string BirthDate { get; set; }

        public string DeathDate { get; set; }
        public string FirstName { get; set; }

        [Required]
        [ForeignKey(typeof (Nation))]
        public int NationId { get; set; }

        [Required]
        public string SecondName { get; set; }

        [Alias("ArtistId")]
        [AutoIncrement]
        [PrimaryKey]
        public int Id { get; set; }
    }
}

#pragma warning restore 1591