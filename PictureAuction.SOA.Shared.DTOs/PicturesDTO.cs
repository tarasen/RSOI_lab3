using PictureAuction.SOA.Shared.ServiceModel.Types;
using System.Collections.Generic;

namespace PictureAuction.SOA.Shared.DTOs
{
    public static class PicturesDTO
    {
        public class PictureDTO
        {
            public ICollection<CustomEntity> Artists { get; set; }
            public string CreationDate { get; set; }
            public int Id { get; set; }
            public string Name { get; set; }
        }

        public class PictureExtendedDTO : PictureDTO
        {
            public string Gallery { get; set; }
            public string[] Genres { get; set; }
            public double? Height { get; set; }
            public string Image { get; set; }
            public string Material { get; set; }
            public string Technique { get; set; }
            public double? Width { get; set; }
        }
    }
}