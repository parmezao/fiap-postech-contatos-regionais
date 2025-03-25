namespace ContatosRegionais.Application.Events;

public class UpdateContatoEvent(int id, string nome, string telefone, string email, int ddd)
{
    public int Id { get; set; } = id;
    public string Nome { get; set; } = nome;
    public string Telefone { get; set; } = telefone;
    public string Email { get; set; } = email;
    public int Ddd { get; set; } = ddd;
}
