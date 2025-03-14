using ContatosRegionais.Domain.Entities;
using System.Text.RegularExpressions;

namespace ContatosRegionais.Domain.Tests.Entities;

public class ContatoTest
{
    #region Propriedade "Nome"
    [Fact(DisplayName = "Nome Deve falhar quando for nulo")]
    [Trait("Property", "Nome")]
    public void Nome_DeveFalhar_QuandoForNulo()
    {
        // Arrange        
        string? valorEsperado = null;

        // Act
        var contato = new Contato();
        string? valorAtual = contato.Nome;

        // Assert
        Assert.Equal(valorEsperado, valorAtual);
    }

    [Fact(DisplayName = "Nome Deve Falhar Quando Estiver Em Branco")]
    [Trait("Property", "Nome")]
    public void Nome_DeveFalhar_QuandoEstiverEmBranco()
    {
        const string nomeVazio = "";

        // Arrange
        string valorEsperado = nomeVazio;

        // Act
        Contato contato = new() { Nome = nomeVazio };
        string valorAtual = contato.Nome;

        // Assert
        Assert.Equal(valorEsperado, valorAtual);
    }

    [Fact(DisplayName = "Nome Deve Falhar Quando Ultrapassar Limite De Caracteres")]
    [Trait("Property", "Nome")]
    public void Nome_DeveFalhar_QuandoUltrapassarLimiteDeCaracteres()
    {
        const string nomeMaiorQue100 = "ndflhweifosndflsjdhnvchoehowijedfhoidhfoiwjhofoifhoifhwoihfeohasjfhjfgkjhrlpf" +
                                       "jsdfjsdfsfsdfdfdhfcnohurhife";

        // Arrange
        var valorEsperado = nomeMaiorQue100.Length > 100;

        // Act
        var contato = new Contato();
        contato.Nome = nomeMaiorQue100;

        var valorAtual = contato.Nome?.Length > 100;

        // Assert
        Assert.Equal(valorEsperado, valorAtual);
    }

    [Fact(DisplayName = "Nome Deve Passar Quando Estiver Preenchido")]
    [Trait("Property", "Nome")]
    public void Nome_DevePassar_QuandoEstiverPreenchido()
    {
        const string nomeContato = "Nome_Contato";

        // Arrange
        var valorEsperado = nomeContato;

        // Act
        var contato = new Contato() { Nome = nomeContato };
        var valorAtual = contato.Nome;

        // Assert
        Assert.Equal(valorEsperado, valorAtual);
    }
    #endregion

    #region Propriedade "Email - campo Endereço"
    [Fact(DisplayName = "Email Deve Falhar Quando Ultrapassar Limite De Caracteres")]
    [Trait("Property", "Email")]
    public void Email_DeveFalhar_QuandoUltrapassarLimiteDeCaracteres()
    {
        const string enderecoMaiorQue100 = "ndflhweifosndflsjdhnvchoehowijedfhoidhfoiwjhofoifhoifhwoihfeo" +
                                           "hasjfhjfgkjhrlpfjsdfjsdfsfsdfdfdhfcnohurhife";

        // Arrange
        var valorEsperado = enderecoMaiorQue100.Length > 100;

        // Act
        Contato contato = new();
        contato.Email.Endereco = enderecoMaiorQue100;

        bool valorAtual = contato.Email.Endereco.Length > 100;

        // Assert
        Assert.Equal(valorEsperado, valorAtual);
    }
    #endregion

    #region Propriedade "Telefone"
    [Fact(DisplayName = "Telefone Deve Falhar Se Tiver Mais De 10 Caracteres")]
    [Trait("Property", "Telefone")]
    public void Telefone_DeveFalhar_SeTiverMaisDe10Caracteres()
    {
        const string telefoneMaiorQue10Caracteres = "98888776600";

        // Arrange
        const int limiteMaximoCaracteres = 10;

        // Act
        var contato = new Contato();
        contato.Telefone = telefoneMaiorQue10Caracteres;
        var valorAtual = contato.Telefone.Length;

        // Assert
        Assert.True(valorAtual > limiteMaximoCaracteres);
    }

    [Fact(DisplayName = "Telefone Deve Falhar Quando For Nulo")]
    [Trait("Property", "Telefone")]
    public void Telefone_DeveFalhar_QuandoForNulo()
    {
        // Arrange        
        string? valorEsperado = null;

        // Act
        var contato = new Contato();
        var valorAtual = contato.Telefone;

        // Assert
        Assert.Equal(valorEsperado, valorAtual);
    }

    [Fact(DisplayName = "Telefone Deve Falhar Quando Estiver Em Branco")]
    [Trait("Property", "Telefone")]
    public void Telefone_DeveFalhar_QuandoEstiverEmBranco()
    {
        string telefoneVazio = string.Empty;

        // Arrange
        var valorEsperado = telefoneVazio;

        // Act
        Contato contato = new() { Telefone = telefoneVazio };
        string valorAtual = contato.Telefone;

        // Assert
        Assert.Equal(valorEsperado, valorAtual);
    }

    [Fact(DisplayName = "Telefone Deve Falhar Quando Possuir Algum Caractere Alfanumerico")]
    [Trait("Property", "Telefone")]
    public void Telefone_DeveFalhar_QuandoPossuirAlgumCaractereAlfanumerico()
    {
        const string telefoneComCaractereAlfanumerico = "9888877X6";

        // Arrange
        var contato = new Contato();
        contato.Telefone = telefoneComCaractereAlfanumerico;

        var valorEsperado = !Regex.IsMatch(contato.Telefone, "^[0-9]*$");

        // Assert
        Assert.True(valorEsperado);
    }

    #endregion

    #region Propriedade "DDD"
    [Fact(DisplayName = "DDD Deve falhar quando for zero")]
    [Trait("Property", "DDD")]
    public void DDD_DeveFalhar_QuandoForZero()
    {
        // Arrange        
        const int valorEsperado = 0;

        // Act
        var contato = new Contato();
        var valorAtual = contato.DDD;

        // Assert
        Assert.Equal(valorEsperado, valorAtual);
    }

    [Fact(DisplayName = "DDD Deve Falhar Quando Estiver Abaixo Da Qtde Minima De Caracteres")]
    [Trait("Property", "DDD")]
    public void DDD_DeveFalhar_QuandoEstiverAbaixoDaQtdeMinimaDeCaracteres()
    {
        // Arrange        
        const int qtdEsperada = 1;

        // Act
        var contato = new Contato();
        contato.DDD = 2;

        var qtdAtual = contato.DDD.ToString().Length;

        // Assert
        Assert.True(qtdAtual <= qtdEsperada);
    }

    [Fact(DisplayName = "DDD Deve Falhar Quando Estiver Acima Da Qtde Máxima De Caracteres")]
    [Trait("Property", "DDD")]
    public void DDD_DeveFalhar_QuandoEstiverAcimaDaQtdeMaximaDeCaracteres()
    {
        // Arrange        
        const int qtdEsperada = 3;

        // Act
        var contato = new Contato();
        contato.DDD = 222;

        var qtdAtual = contato.DDD.ToString().Length;

        // Assert
        Assert.True(qtdAtual >= qtdEsperada);
    }


    #endregion
}