using AutoMapper;
using Nancy;
using Nancy.ModelBinding;
using Nancy.Security;
using PictureAuction.SOA.Frontend.Models;
using PictureAuction.SOA.Frontend.Networks;
using PictureAuction.SOA.Shared.DTOs;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace PictureAuction.SOA.Frontend.Modules
{
    public class ArtistsModule : NancyModule, IDisposable
    {
        private readonly PictureAuctionClient _client = new PictureAuctionClient();

        static ArtistsModule()
        {
            Mapper.CreateMap<ArtistsDTO.ArtistExtendedDTO, ArtistModel>();
        }

        public ArtistsModule() : base("/artists")
        {
            Get["/", true] = GetArtistsAsync;
            Get["/add"] = AddArtist;

            Get["/id{id:int}", true] = GetArtistAsync;

            Get["/id{id:int}/edit", true] = EditArtistAsync;
            Post["/id{id:int}/edit", true] = UpdateArtistAsync;

            Get["/id{id:int}/delete", true] = DeleteArtistAsync;
        }

        public void Dispose()
        {
            _client.Dispose();
        }

        private object AddArtist(object arg)
        {
            this.RequiresAuthentication();

            return View["Views/artist_edit.sshtml", new ArtistsDTO.ArtistExtendedDTO()];
        }

        private async Task<dynamic> CreateArtistAsync(ArtistsDTO.ArtistExtendedDTO artist)
        {
            var i = await _client.CreateArtistAsync(artist);
            if (i > 0)
                return Response.AsRedirect($"~/artists/id{i}");
            return new Response {StatusCode = HttpStatusCode.BadRequest};
        }

        private async Task<dynamic> DeleteArtistAsync(dynamic arg, CancellationToken _)
        {
            this.RequiresAuthentication();
            int id = Convert.ToInt32(arg.id.Value);

            if (await _client.DeleteArtistAsync(id))
                return Response.AsRedirect("~/artists");
            return new Response {StatusCode = HttpStatusCode.BadRequest};
        }

        private async Task<dynamic> EditArtistAsync(dynamic arg, CancellationToken _)
        {
            this.RequiresAuthentication();

            int id = Convert.ToInt32(arg.id.Value);
            var artistsDto = await _client.GetArtistAsync(id);
            if (artistsDto == null)
                return new Response {StatusCode = HttpStatusCode.NotFound};

            return View["Views/Edit/artist.sshtml", Mapper.Map<ArtistModel>(artistsDto)];
        }

        private async Task<dynamic> GetArtistAsync(dynamic arg, CancellationToken _)
        {
            int id = Convert.ToInt32(arg.id.Value);
            var artistsDto = await _client.GetArtistAsync(id);

            var task = Task.Run(async () =>
            {
                var list = new List<PicturesDTO.PictureDTO>();
                foreach (var picture in artistsDto.Pictures)
                {
                    var item = await _client.GetPictureAsync(picture.Id);
                    if (item != null)
                        list.Add(item);
                }
                return list;
            }, CancellationToken.None);

            var artistEx = Mapper.Map<ArtistModel>(artistsDto);
            artistEx.PicturesInfo = await task;
            artistEx.Authorized = Context.CurrentUser != null;

            return View["Views/artist.sshtml", artistEx];
        }

        private async Task<dynamic> GetArtistsAsync(dynamic arg, CancellationToken _)
        {
            DynamicDictionaryValue pageParam = Request.Query["page"];
            var page = pageParam.HasValue ? Convert.ToInt32(pageParam.Value) : 1;

            var artists = await _client.GetArtistsAsync(page, 30);
            if (artists == null)
                return new Response {StatusCode = HttpStatusCode.NotFound};
            var collection = new PageResultModel<ArtistsDTO.ArtistDTO>(artists)
            {
                Authorized = Context.CurrentUser != null
            };

            return View["Views/artists.sshtml", collection];
        }

        private async Task<object> UpdateArtistAsync(object arg, CancellationToken _)
        {
            this.RequiresAuthentication();
            var artist = this.Bind<ArtistsDTO.ArtistExtendedDTO>();

            if (artist.Id == 0)
                return await CreateArtistAsync(artist);
            return await UpdateArtistAsync(artist);
        }

        private async Task<dynamic> UpdateArtistAsync(ArtistsDTO.ArtistExtendedDTO artist)
        {
            if (await _client.UpdateArtistAsync(artist))
                return Response.AsRedirect($"~/artists/id{artist.Id}");
            return new Response {StatusCode = HttpStatusCode.BadRequest};
        }
    }
}