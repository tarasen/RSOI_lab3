using PictureAuction.SOA.Shared.DTOs;
using PictureAuction.SOA.Shared.ServiceModel;
using ServiceStack.Common.Web;
using ServiceStack.ServiceHost;
using System.Runtime.Serialization;

namespace PictureAuction.SOA.Artists.ServiceModel.Routes
{
    public static class ArtistRoutes
    {
        [Route("/artists", HttpMethods.Post)]
        public class CreateArtist : ArtistsDTO.ArtistExtendedDTO, IReturn<ArtistsDTO.ArtistExtendedDTO>
        {
        }

        [Route("/artists/{Id}", HttpMethods.Delete)]
        public class DeleteArtist : IReturnVoid
        {
            public int Id { get; set; }
        }

        [Route("/artists/{Id}", HttpMethods.Get)]
        public class GetArtist : IReturn<ArtistsDTO.ArtistExtendedDTO>
        {
            public int Id { get; set; }
        }

        [Route("/artists", HttpMethods.Get)]
        [DataContract]
        public class GetArtists : IReturn<PageResult<ArtistsDTO.ArtistDTO>>
        {
            [DataMember(Name = "page")]
            public int PageNumber { get; set; } = 1;

            [DataMember(Name = "page_size")]
            public int PageSize { get; set; } = 15;
        }

        [Route("/artists/{Id}", HttpMethods.Put)]
        public class UpdateArtist : ArtistsDTO.ArtistExtendedDTO, IReturn<ArtistsDTO.ArtistExtendedDTO>
        {
        }
    }
}