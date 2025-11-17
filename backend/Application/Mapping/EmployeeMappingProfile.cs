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
                       opt => opt.MapFrom(src => src.Company.Name))
            .ForMember(dest => dest.CreatedAt,
                       opt => opt.MapFrom(src => src.CreatedAt)) // 🔥 Zorunlu
            .ForMember(dest => dest.ContactInfos,
                       opt => opt.MapFrom(src => src.ContactInfos));

        // DTO → ENTITY (update ve create için)
        CreateMap<EmployeeWithContactInfoDto, Employee>()
            .ForMember(dest => dest.Company, opt => opt.Ignore()) // company navigation EF tarafından doldurulur
            .ForMember(dest => dest.ContactInfos, opt => opt.Ignore()) // contactler manuel handle edilir
            .ForMember(dest => dest.CreatedAt,
                       opt => opt.Ignore()); // 🔥 Create sırasında EF verecek, update'te dokunma
    }
}
