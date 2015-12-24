#pragma warning disable 1591

using ServiceStack.DataAnnotations;
using ServiceStack.DesignPatterns.Model;
using ServiceStack.OrmLite;
using System;
using System.ComponentModel.DataAnnotations;

namespace PictureAuction.SOA.Shared.ServiceModel.Types
{
    [Alias("Picture")]
    public class Picture : IHasId<int>
    {
        [Required]
        public DateTime Creation { get; set; }

        [ForeignKey(typeof (Gallery))]
        public int? GalleryId { get; set; }

        public double? Height { get; set; }

        [Required]
        public bool IsSaleable { get; set; }

        [Required]
        [ForeignKey(typeof (Material))]
        public int MaterialId { get; set; }

        [Required]
        public string Name { get; set; }

        public decimal? StartCost { get; set; }

        [Required]
        [ForeignKey(typeof (Technique))]
        public int TechniqueId { get; set; }

        public double? Width { get; set; }

        [Alias("PictureId")]
        [AutoIncrement]
        [PrimaryKey]
        public int Id { get; set; }
    }
}

#pragma warning restore 1591