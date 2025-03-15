namespace ContatosRegionais.Application.Events;

public class UpdateContatoEvent(long id, string nome, string telefone, string email, int ddd)
{
    public long Id { get; set; } = id;
    public string Nome { get; set; } = nome;
    public string Telefone { get; set; } = telefone;
    public string Email { get; set; } = email;
    public int Ddd { get; set; } = ddd;
}
