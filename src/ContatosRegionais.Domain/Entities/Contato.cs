using ContatosRegionais.Domain.ValueObjects;

namespace ContatosRegionais.Domain.Entities;

public class Contato : BaseEntity
{
    public string? Nome { get; set; }

    public Email Email { get; set; } = new Email(string.Empty);

    public string? Telefone { get; set; }
    
    public int DDD { get; set; }
}