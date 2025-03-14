using AutoMapper;
using ContatosRegionais.Application.DTO;
using ContatosRegionais.Domain.Entities;

namespace ContatosRegionais.Application.Mapping;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Contato, ContatoDto>()
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email.Endereco))
            .ReverseMap();
    }
}
