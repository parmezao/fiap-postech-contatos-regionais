using ContatosRegionais.Application.DTO;
using ContatosRegionais.Domain.Entities;

namespace ContatosRegionais.Application.Extensions;

public static class ContatoExtensions
{
    public static ContatoDto ToDto(this Contato contato)
    {
        return new ContatoDto
        {
            Id = contato.Id,
            Nome = contato.Nome?? string.Empty,
            Telefone = contato.Telefone?? string.Empty,
            Email = contato.Email.Endereco,
            Ddd = contato.DDD,
        };
    }

    public static IList<ContatoDto> ToDto(this IList<Contato> contatos)
    {
        return [.. contatos.Select(c => c.ToDto())];
    }

    public static IEnumerable<ContatoDto> ToDto(this IEnumerable<Contato> contatos)
    {
        return [.. contatos.Select(c => c.ToDto())];
    }

    public static Contato ToContato(this ContatoDto contatoDto)
    {
        return new Contato
        {
            Id = contatoDto.Id,
            Nome = contatoDto.Nome,
            Telefone = contatoDto.Telefone,
            DDD = contatoDto.Ddd,
            Email = new Domain.ValueObjects.Email(contatoDto.Email)
        };
    }

    public static IList<Contato> ToContato(this IList<ContatoDto> contatosDto)
    {
        return [.. contatosDto.Select(c => c.ToContato())];
    }

    public static IEnumerable<Contato> ToContato(this IEnumerable<ContatoDto> contatosDto)
    {
        return [.. contatosDto.Select(c => c.ToContato())];
    }
}
