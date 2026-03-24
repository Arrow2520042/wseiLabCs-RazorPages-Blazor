using ApplicationCore.Dto;
using ApplicationCore.Models.ContactAggregate;
using AutoMapper;

namespace ApplicationCore.Mapper;

public class ContactsMappingProfile : Profile
{
    public ContactsMappingProfile()
    {
        CreateMap<Address, AddressDto>().ReverseMap();

        CreateMap<Person, PersonDto>()
            .ForMember(dest => dest.FullName,
                opt => opt.MapFrom(src => $"{src.FirstName} {src.LastName}"))
            .ForMember(dest => dest.EmployerName,
                opt => opt.MapFrom(src => src.Employer != null ? src.Employer.Name : null));

        CreateMap<CreatePersonDto, Person>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(_ => Guid.NewGuid()))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow))
            .ForMember(dest => dest.Status,
                opt => opt.MapFrom(_ => ApplicationCore.Enums.ContactStatus.Active))
            .ForMember(dest => dest.Notes, opt => opt.Ignore())
            .ForMember(dest => dest.Tags, opt => opt.Ignore())
            .ForMember(dest => dest.Employer, opt => opt.Ignore())
            .ForMember(dest => dest.Organization, opt => opt.Ignore());

        CreateMap<UpdatePersonDto, Person>()
            .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));
    }
}
