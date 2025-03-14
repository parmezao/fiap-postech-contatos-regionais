namespace ContatosRegionais.Application.Events;

public class InsertContatoEvent
{
    public string? Nome { get; set; }
    public string? Telefone { get; set; }
    public string? Email { get; set; }
    public int Ddd { get; set; }
}