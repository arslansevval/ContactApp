using AutoMapper;
using ContactApp.Core.Entities;
using ContactApp.Application.DTOs;

public class EmployeeMappingProfile : Profile
{
    public EmployeeMappingProfile()
    {
        // ENTITY → DTO
        CreateMap<Employee, EmployeeWithContactInfoDto>()
            .ForMember(dest => dest.CompanyName,
                       opt => opt.MapFrom(src => src.Company != null ? src.Company.Name : null))
            .ForMember(dest => dest.CreatedAt,
                       opt => opt.MapFrom(src => src.CreatedAt));

        CreateMap<ContactInfo, ContactInfoReadDto>();

        // DTO → ENTITY (create / update)
        CreateMap<EmployeeWithContactInfoDto, Employee>()
            .ForMember(dest => dest.Company, opt => opt.Ignore()) // navigation property ignore
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore()); // EF Core set ediyor
            
    }
}
