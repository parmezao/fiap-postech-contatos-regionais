namespace ContatosRegionais.Application.Events;

public class UpdateContatoEvent
{
    public long Id { get; set; }
    public string Nome { get; set; }
    public string Telefone { get; set; }
    public string Email { get; set; }
    public int Ddd { get; set; }

    public UpdateContatoEvent() { }

    public UpdateContatoEvent(long id, string nome, string telefone, string email, int ddd)
    {
        Id = id;
        Nome = nome;
        Telefone = telefone;
        Email = email;
        Ddd = ddd;
    }
}
