using AutoMapper;
using MoreLinq;
using PictureAuction.SOA.Artists.ServiceModel.Routes;
using PictureAuction.SOA.Shared.DTOs;
using PictureAuction.SOA.Shared.ServiceModel;
using PictureAuction.SOA.Shared.ServiceModel.Types;
using ServiceStack.Common.Web;
using ServiceStack.OrmLite;
using ServiceStack.ServiceInterface;
using System;
using System.Linq;
using System.Net;

namespace PictureAuction.SOA.Artists.ServiceInterface.Services
{
    public class ArtistService : Service
    {
        static ArtistService()
        {
            Mapper.CreateMap<Artist, ArtistsDTO.ArtistExtendedDTO>()
                .ForMember(x => x.Name,
                    expression => expression.MapFrom(e => $"{e.SecondName}, {e.FirstName}"));
            Mapper.CreateMap<Artist, ArtistsDTO.ArtistDTO>()
                .ForMember(x => x.Name,
                    expression => expression.MapFrom(e => $"{e.SecondName}, {e.FirstName}"));
            Mapper.CreateMap<ArtistsDTO.ArtistExtendedDTO, Artist>()
                .ForMember(x => x.FirstName,
                    expression =>
                        expression.MapFrom(e => e.Name.Substring(0, e.Name.IndexOf(", ", StringComparison.Ordinal))))
                .ForMember(x => x.SecondName,
                    expression =>
                        expression.MapFrom(e => e.Name.Substring(e.Name.IndexOf(", ", StringComparison.Ordinal) + 2)));
        }

        public object Delete(ArtistRoutes.DeleteArtist request)
        {
            try
            {
                if (Db.Any<PicturesByArtist>(x => x.ArtistId == request.Id))
                    return HttpError.Conflict("Can't remove artist with existing pictures");

                var deleted = Db.Delete<ArtistsByPeriod>(x => x.ArtistId == request.Id)
                              + Db.Delete<Artist>(x => x.Id == request.Id);

                return deleted == 0
                    ? HttpError.NotFound("")
                    : new HttpError(HttpStatusCode.NoContent, "");
            }
            catch
            {
                return new HttpError(HttpStatusCode.InternalServerError, "Internal Server Error");
            }
        }

        public object Get(ArtistRoutes.GetArtists request)
        {
            try
            {
                var count = Db.GetScalar<Artist, int>(r => Sql.Count(r.Id));
                var skip = (request.PageNumber - 1)*request.PageSize;

                if (skip >= count)
                    return
                        new HttpResult(new PageResult<ArtistsDTO.ArtistDTO>(new ArtistsDTO.ArtistDTO[0], 0, 1, count),
                            MimeTypes.Json);

                var artists = Db.Select<Artist>(q => q.Limit(skip, request.PageSize));
                var minId = artists.First().Id;
                var maxId = artists.Last().Id;

                var pba = Db.Select<PicturesByArtist>(p => p.ArtistId >= minId && p.ArtistId <= maxId)
                    .ToLookup(x => x.ArtistId, x => x.PictureId);

                var dto = new PageResult<ArtistsDTO.ArtistDTO>(
                    artists.Select(arg =>
                    {
                        var artistDto = Mapper.Map<ArtistsDTO.ArtistDTO>(arg);
                        artistDto.Pictures =
                            pba.Where(x => x.Key == artistDto.Id)
                                .SelectMany(x => x)
                                .Select(x => new CustomEntity {Id = x})
                                .ToArray();
                        return artistDto;
                    }).ToList(),
                    request.PageNumber,
                    request.PageSize, count);

                return new HttpResult(dto, $"{MimeTypes.Json}; charset=utf-8");
            }
            catch
            {
                return new HttpError(HttpStatusCode.InternalServerError, "Internal Server Error");
            }
        }

        public object Get(ArtistRoutes.GetArtist request)
        {
            try
            {
                var artist = Db.GetByIdOrDefault<Artist>(request.Id);
                if (artist == null)
                    return HttpError.NotFound("Artist with this id is not found");

                var dto = Mapper.Map<ArtistsDTO.ArtistExtendedDTO>(artist);
                dto.Pictures =
                    Db.Select<PicturesByArtist>(x => x.ArtistId == artist.Id)
                        .Select(x => new CustomEntity {Id = x.PictureId})
                        .ToArray();
                dto.Nation = Db.GetByIdOrDefault<Nation>(artist.NationId)?.Name;
                dto.Periods = Db.Select<ArtistsByPeriod>(x => x.ArtistId == artist.Id)
                    .Join(Db.Select<Period>(), pb => pb.PeriodId, g => g.Id, (_, g) => g.Name).ToArray();
                return new HttpResult(dto, $"{MimeTypes.Json}; charset=utf-8");
            }
            catch
            {
                return new HttpError(HttpStatusCode.InternalServerError, "Internal Server Error");
            }
        }

        public object Post(ArtistRoutes.CreateArtist request)
        {
            try
            {
                var art = Mapper.Map<Artist>(request);
                art.NationId = Db.Select<Nation>(x => x.Name == request.Nation).First().Id;

                Db.Insert(art);
                var id = Db.GetScalar<Artist, int>(r => Sql.Max(r.Id));
                var artist = Db.GetById<Artist>(id);

                if (request.Pictures?.Any() ?? false)
                {
                    var pba =
                        Db.Select<PicturesByArtist>()
                            .Join(request.Pictures, x => x.PictureId, a => a.Id, (x, _) => x)
                            .ToList();

                    Db.DeleteAll(pba);

                    Db.SaveAll(pba.Select(x => new PicturesByArtist {PictureId = x.PictureId, ArtistId = artist.Id}));
                    Db.SaveAll(
                        request.Pictures.Select(x => new PicturesByArtist {PictureId = x.Id, ArtistId = artist.Id})
                            .ExceptBy(pba, o => o.PictureId)
                            .ToList());
                }

                if (request.Periods?.Any() ?? false)
                {
                    var abp = Db.Select<Period>()
                        .Join(request.Periods, x => x.Name, x => x, (x, _) => x.Id)
                        .Join(Db.Select<ArtistsByPeriod>(), x => x, x => x.PeriodId, (_, p) => p)
                        .ToList();

                    Db.DeleteAll(abp);
                    Db.SaveAll(abp.Select(x => new ArtistsByPeriod {PeriodId = x.PeriodId, ArtistId = artist.Id}));
                }

                return new HttpResult(artist, $"{MimeTypes.Json}; charset=utf-8") {StatusCode = HttpStatusCode.Created};
            }
            catch (InvalidOperationException)
            {
                return new HttpError(HttpStatusCode.BadRequest, "Wrong Parameters");
            }
            catch
            {
                return new HttpError(HttpStatusCode.InternalServerError, "Internal Server Error");
            }
        }

        public object Put(ArtistRoutes.UpdateArtist request)
        {
            try
            {
                var artist = Db.GetByIdOrDefault<Artist>(request.Id);
                if (artist == null)
                    return HttpError.NotFound($"Artist '{request.Id}' does not exist");

                Mapper.Map(request, artist, o => o.AfterMap((rq, art) => art.Id = rq.Id));
                artist.NationId = Db.Select<Nation>(x => x.Name == request.Nation).First().Id;
                Db.Update(artist);

                if (request.Pictures?.Any() ?? false)
                {
                    var pba =
                        Db.Select<PicturesByArtist>()
                            .Join(request.Pictures, x => x.PictureId, a => a.Id, (x, _) => x)
                            .ToList();

                    Db.DeleteAll(pba);

                    Db.SaveAll(pba.Select(x => new PicturesByArtist {PictureId = x.PictureId, ArtistId = artist.Id}));
                    Db.SaveAll(
                        request.Pictures.Select(x => new PicturesByArtist {PictureId = x.Id, ArtistId = artist.Id})
                            .ExceptBy(pba, o => o.PictureId)
                            .ToList());
                }
                if (request.Periods?.Any() ?? false)
                {
                    var abp = Db.Select<Period>()
                        .Join(request.Periods, x => x.Name, x => x, (x, _) => x.Id)
                        .Join(Db.Select<ArtistsByPeriod>(), x => x, x => x.PeriodId, (_, p) => p)
                        .ToList();

                    Db.DeleteAll(abp);
                    Db.SaveAll(abp.Select(x => new ArtistsByPeriod {PeriodId = x.PeriodId, ArtistId = artist.Id}));
                }

                return new HttpResult(artist, $"{MimeTypes.Json}; charset=utf-8")
                {
                    StatusCode = HttpStatusCode.OK
                };
            }
            catch
            {
                return new HttpError(HttpStatusCode.InternalServerError, "Internal Server Error");
            }
        }
    }
}