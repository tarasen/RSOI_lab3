#pragma warning disable 1591

using ServiceStack.DataAnnotations;
using ServiceStack.DesignPatterns.Model;
using ServiceStack.OrmLite;
using System.ComponentModel.DataAnnotations;

namespace PictureAuction.SOA.Shared.ServiceModel.Types
{
    [Alias("Genre")]
    public class Genre : IHasId<int>
    {
        public string Date { get; set; }

        [Required]
        public string Name { get; set; }

        [ForeignKey(typeof (Nation))]
        public int? NationId { get; set; }

        [Alias("GenreId")]
        [AutoIncrement]
        [PrimaryKey]
        public int Id { get; set; }
    }
}

#pragma warning restore 1591