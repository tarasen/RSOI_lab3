using PictureAuction.SOA.Frontend.Extentions;
using PictureAuction.SOA.Shared.DTOs;
using PictureAuction.SOA.Shared.ServiceModel;
using ServiceStack.Service;
using ServiceStack.ServiceClient.Web;
using System;
using System.Threading.Tasks;

namespace PictureAuction.SOA.Frontend.Networks
{
    public class PictureAuctionClient : IDisposable
    {
        private readonly IRestClientAsync _artistsClient = new JsonServiceClient(Configuration.ArtistsBackendUri);
        private readonly IRestClientAsync _imagesClient = new JsonServiceClient(Configuration.ImagesBackendUri);
        private readonly IRestClientAsync _picturesClient = new JsonServiceClient(Configuration.PicturesBackendUri);

        public void Dispose()
        {
            _artistsClient.Dispose();
            _picturesClient.Dispose();
            _imagesClient.Dispose();
        }

        public async Task<int> CreateArtistAsync(ArtistsDTO.ArtistExtendedDTO artist)
        {
            var artists =
                await _artistsClient.PostItemAsync(artist).ConfigureAwait(false);
            return artists?.Id ?? 0;
        }

        public async Task<int> CreatePictureAsync(PicturesDTO.PictureExtendedDTO picture)
        {
            var pictureDto =
                await _picturesClient.PostItemAsync(picture).ConfigureAwait(false);
            return pictureDto?.Id ?? 0;
        }

        public async Task<bool> DeleteArtistAsync(int id)
        {
            return await _artistsClient.DeleteItemAsync(id).ConfigureAwait(false);
        }

        public async Task<bool> DeletePictureAsync(int id)
        {
            return await _picturesClient.DeleteItemAsync(id).ConfigureAwait(false);
        }

        public async Task<ArtistsDTO.ArtistExtendedDTO> GetArtistAsync(int id)
        {
            return await _artistsClient.GetItemAsync<ArtistsDTO.ArtistExtendedDTO>(id.ToString()).ConfigureAwait(false);
        }

        public async Task<PageResult<ArtistsDTO.ArtistDTO>> GetArtistsAsync(int? page, int? pageSize)
        {
            return
                await
                    _artistsClient.GetPageAsync<ArtistsDTO.ArtistDTO>(page, pageSize)
                        .ConfigureAwait(false);
        }

        public async Task<string> GetPhotoAsync(string fileName)
        {
            return await _imagesClient.GetImageAsync(fileName).ConfigureAwait(false);
        }

        public async Task<PicturesDTO.PictureExtendedDTO> GetPictureAsync(int id)
        {
            return
                await _picturesClient.GetItemAsync<PicturesDTO.PictureExtendedDTO>(id.ToString()).ConfigureAwait(false);
        }

        public async Task<PageResult<PicturesDTO.PictureDTO>> GetPicturesAsync(int? page, int? pageSize)
        {
            return
                await
                    _picturesClient.GetPageAsync<PicturesDTO.PictureDTO>(page,
                        pageSize).ConfigureAwait(false);
        }

        public async Task<bool> UpdateArtistAsync(ArtistsDTO.ArtistExtendedDTO artist)
        {
            return
                await
                    _artistsClient.PutItemAsync(artist, artist.Id)
                        .ConfigureAwait(false);
        }

        public async Task<bool> UpdatePictureAsync(PicturesDTO.PictureExtendedDTO picture)
        {
            return
                await
                    _picturesClient.PutItemAsync(picture, picture.Id)
                        .ConfigureAwait(false);
        }
    }
}