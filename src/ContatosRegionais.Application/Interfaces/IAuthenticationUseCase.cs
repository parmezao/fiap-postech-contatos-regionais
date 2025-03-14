using ContatosRegionais.Application.DTO;

namespace ContatosRegionais.Application.Interfaces;

public interface IAuthenticationUseCase
{
    public string GetToken(UserDto usuario);
}
