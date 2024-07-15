using AutoMapper;
using CheckIN.Data.Model;
using CheckIN.Models.TITo.Event;

namespace CheckIN.Common
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<EventResponse, Event>()
            .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt))
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => src.UpdatedAt))
            .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title))
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
            .ForMember(dest => dest.SecurityToken, opt => opt.MapFrom(src => src.SecurityToken))
            .ForMember(dest => dest.Url, opt => opt.MapFrom(src => src.Url))
            .ForMember(dest => dest.Currency, opt => opt.MapFrom(src => src.Currency))
            .ForMember(dest => dest.TestMode, opt => opt.MapFrom(src => src.TestMode))
            .ForMember(dest => dest.DiscountCodesCount, opt => opt.MapFrom(src => src.DiscountCodesCount))
            .ForMember(dest => dest.ShowDiscountCodeField, opt => opt.MapFrom(src => src.ShowDiscountCodeField))
            .ForMember(dest => dest.AccountSlug, opt => opt.MapFrom(src => src.AccountSlug))
            .ForMember(dest => dest.Locales, opt => opt.MapFrom(src => src.Locales))
            .ForMember(dest => dest.Location, opt => opt.MapFrom(src => src.Location))
            .ForMember(dest => dest.Private, opt => opt.MapFrom(src => src.Private))
            .ForMember(dest => dest.Slug, opt => opt.MapFrom(src => src.Slug))
            .ForMember(dest => dest.Live, opt => opt.MapFrom(src => src.Live))
            .ForMember(dest => dest.StartDate, opt => opt.MapFrom(src => src.StartDate))
            .ForMember(dest => dest.EndDate, opt => opt.MapFrom(src => src.EndDate));
        }
    }
}
