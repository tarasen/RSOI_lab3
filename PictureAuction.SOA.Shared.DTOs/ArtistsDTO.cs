using PictureAuction.SOA.Shared.ServiceModel.Types;
using System.Collections.Generic;

namespace PictureAuction.SOA.Shared.DTOs
{
    public static class ArtistsDTO
    {
        public class ArtistDTO
        {
            public int Id { get; set; }

            public string Name { get; set; }

            public ICollection<CustomEntity> Pictures { get; set; }
        }

        public class ArtistExtendedDTO : ArtistDTO
        {
            public string BirthDate { get; set; }

            public string DeathDate { get; set; }

            public string Nation { get; set; }

            public ICollection<string> Periods { get; set; }
        }
    }
}