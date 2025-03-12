namespace ContatosRegionais.Domain.ValueObjects;

public class Email(string endereco)
{
    public string Endereco { get; set; } = endereco;

    public bool ChangeEmail(string email)
    {
        Endereco = email;
        return Endereco.Equals(email);
    }
}

