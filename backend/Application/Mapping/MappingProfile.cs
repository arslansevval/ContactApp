using AutoMapper;
using ContactApp.Application.DTOs;
using ContactApp.Core.Entities;

namespace ContactApp.Application.Mapping;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<EmployeeCreateDto, Employee>();
        CreateMap<Employee, EmployeeReadDto>();

        CreateMap<ContactInfoCreateDto, ContactInfo>();
        CreateMap<ContactInfo, ContactInfoCreateDto>();
        
        CreateMap<ContactInfoReadDto, ContactInfo>()
            .ForMember(dest => dest.Id, opt => opt.Ignore());
        CreateMap<ContactInfo, ContactInfoReadDto>();

        CreateMap<CompanyCreateDto, Company>();
        CreateMap<Company, CompanyReadDto>();
        
        CreateMap<EmployeeWithContactInfoDto, Employee>();
        CreateMap<Employee, EmployeeWithContactInfoDto>();
    }
}
