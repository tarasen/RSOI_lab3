#pragma warning disable 1591

using ServiceStack.DataAnnotations;
using ServiceStack.DesignPatterns.Model;
using System.ComponentModel.DataAnnotations;

namespace PictureAuction.SOA.Shared.ServiceModel.Types
{
    [Alias("Technique")]
    public class Technique : IHasId<int>
    {
        [Required]
        public string Name { get; set; }

        [Alias("TechniqueId")]
        [AutoIncrement]
        [PrimaryKey]
        public int Id { get; set; }
    }
}

#pragma warning restore 1591