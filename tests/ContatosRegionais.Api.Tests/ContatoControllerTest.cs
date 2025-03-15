using Bogus;
using ContatosRegionais.Api.Tests.Abstractions;
using ContatosRegionais.Application.DTO;
using ContatosRegionais.Domain.Entities;
using FluentAssertions;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace ContatosRegionais.Api.Tests;

public class ContatoControllerTest : BaseFunctionalTests
{
    private readonly Faker _faker;
    private readonly FunctionalTestWebAppFactory _testsFixture;

    public ContatoControllerTest(FunctionalTestWebAppFactory testsFixture) : base(testsFixture)
    {
        _faker = new Faker(locale: "pt_BR");
        _testsFixture = testsFixture;
    }

    private string validPhoneNumber => "999999999";
    private string validPhoneNumberToUpdate => "988888888";
    private int validDddPhoneNumber => 11;

    private readonly JsonSerializerOptions jsonOptions = new JsonSerializerOptions
    {
        PropertyNameCaseInsensitive = true
    };

    [Fact(DisplayName = "Deve inserir um contato com sucesso")]
    [Trait("Functional", "ContatoController")]
    public async Task Should_Return_ContactCreatedWithSuccess()
    {
        //Arrange
        var payload = new ContatoDto
        {
            Nome = _faker.Name.FullName(),
            Email = _faker.Internet.Email(),
            Telefone = validPhoneNumber,
            Ddd = validDddPhoneNumber
        };

        //Act
        var response = await HttpClient.PostAsJsonAsync("api/contato", payload);

        //Assert
        response.EnsureSuccessStatusCode();
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var content = await response.Content.ReadAsStringAsync();
        var contatoResponse = JsonSerializer.Deserialize<ResponseModel>(content, jsonOptions)!;
        contatoResponse.Should().NotBeNull();

        var contatoResponseData = JsonSerializer.Deserialize<Contato>(contatoResponse.Data!.ToString()!, jsonOptions)!;
        contatoResponseData?.Nome.Should().BeEquivalentTo(payload.Nome);

        var formatedPhone = Regex.Replace(payload.Telefone, "[^0-9]+", "");
        contatoResponseData?.Telefone.Should().BeEquivalentTo(formatedPhone);
        contatoResponseData?.Email.Endereco.Should().BeEquivalentTo(payload.Email);
    }

    [Fact(DisplayName = "Deve retornar uma lista com todos os contatos")]
    [Trait("Functional", "ContatoController")]
    public async Task Should_Return_ListWithAllContacts()
    {
        //Act
        var response = await HttpClient.GetAsync($"api/contato");

        //Assert
        response.EnsureSuccessStatusCode();
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var content = await response.Content.ReadAsStringAsync();
        var contatoResponse = JsonSerializer.Deserialize<ResponseModel>(content, jsonOptions)!;
        using var jDoc = JsonDocument.Parse(contatoResponse.Data!.ToString()!);
        var contatoResponseData = jDoc.RootElement.GetProperty("data").Deserialize<List<ContatoDto>>(jsonOptions);

        contatoResponseData.Should().NotBeNull();
        contatoResponseData.Should().HaveCountGreaterThan(0);
    }

    [Fact(DisplayName = "Deve retornar uma lista com filtro de ddd")]
    [Trait("Functional", "ContatoController")]
    public async Task Should_Return_ListWithContactsFiltered()
    {
        //Act
        var response = await HttpClient.GetAsync($"api/contato/ddd/{validDddPhoneNumber}");

        //Assert
        response.EnsureSuccessStatusCode();
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var content = await response.Content.ReadAsStringAsync();
        var contatoResponse = JsonSerializer.Deserialize<ResponseModel>(content, jsonOptions)!;
        using var jDoc = JsonDocument.Parse(contatoResponse.Data!.ToString()!);
        var contatoResponseData = jDoc.RootElement.GetProperty("data").Deserialize<List<ContatoDto>>(jsonOptions);

        contatoResponseData.Should().NotBeNull();
        contatoResponseData.Should().HaveCountGreaterThan(0);
    }

    [Fact(DisplayName = "Deve atualizar um contato com sucesso")]
    [Trait("Functional", "ContatoController")]
    public async Task Should_Return_ContactUpdatedWithSuccess()
    {
        //Arrange
        await Should_Return_ContactCreatedWithSuccess();

        var response = await HttpClient.GetAsync($"api/contato");

        var content = await response.Content.ReadAsStringAsync();
        var contatoResponse = JsonSerializer.Deserialize<ResponseModel>(content, jsonOptions)!;

        using var jDoc = JsonDocument.Parse(contatoResponse.Data!.ToString()!);
        var contatoResponseData = jDoc.RootElement.GetProperty("data").Deserialize<List<ContatoDto>>(jsonOptions);
        var id = contatoResponseData!.First().Id;

        var payload = new ContatoDto
        {
            Id = id,
            Nome = _faker.Name.FullName(),
            Email = _faker.Internet.Email(),
            Telefone = validPhoneNumberToUpdate,
            Ddd = validDddPhoneNumber
        };

        //Act
        response = await HttpClient.PutAsJsonAsync($"api/contato/{id}", payload);

        //Assert
        response.EnsureSuccessStatusCode();
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        content = await response.Content.ReadAsStringAsync();
        content.Should().BeNullOrEmpty();
    }

    [Fact(DisplayName = "Deve retornar menagem de não localizado ao tentar atualizar um contato inexistente")]
    [Trait("Functional", "ContatoController")]
    public async Task Should_Return_ErrorToUpdateContactWhenNotExists()
    {
        //Arrange
        var payload = new ContatoDto
        {
            Id = 9999,
            Nome = _faker.Name.FullName(),
            Email = _faker.Internet.Email(),
            Telefone = validPhoneNumberToUpdate,
            Ddd = validDddPhoneNumber
        };

        //Act
        var response = await HttpClient.PutAsJsonAsync($"api/contato/9999", payload);

        //Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        var content = await response.Content.ReadAsStringAsync();

        var notFoundError = JsonSerializer.Deserialize<ResponseModel>(content, jsonOptions)!;
        notFoundError.Should().NotBeNull();
        notFoundError.Message.Should().Be("Não Encontrado");
    }

    [Theory(DisplayName = "Deve retornar erro ao tentar atualizar um contato com um e-mail inválido")]
    [Trait("Functional", "ContatoController")]
    [InlineData("john.emailinvalido.com", "Falha na Validação")]
    [InlineData($"emailcommaisdoquesessentacaracteres@dominiomaiordoqueesperado.com", "Falha na Validação")]
    public async Task Should_Return_ErrorToUpdateContactWhenEmailIsInvalid(string email, string expectedErrorMessage)
    {
        //Arrange
        await Should_Return_ContactCreatedWithSuccess();

        var response = await HttpClient.GetAsync($"api/contato");

        var content = await response.Content.ReadAsStringAsync();
        var contatoResponse = JsonSerializer.Deserialize<ResponseModel>(content, jsonOptions)!;

        using var jDoc = JsonDocument.Parse(contatoResponse.Data!.ToString()!);
        var contatoResponseData = jDoc.RootElement.GetProperty("data").Deserialize<List<ContatoDto>>(jsonOptions);
        var id = contatoResponseData!.First().Id;

        var payload = new ContatoDto
        {
            Id = id,
            Nome = _faker.Name.FullName(),
            Email = email,
            Telefone = validPhoneNumberToUpdate,
            Ddd = validDddPhoneNumber
        };

        //Act
        response = await HttpClient.PutAsJsonAsync($"api/contato/{id}", payload);

        //Assert
        response.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
        content = await response.Content.ReadAsStringAsync();

        var validationProblemDetails = JsonSerializer.Deserialize<ResponseModel>(content, jsonOptions)!;
        validationProblemDetails.Should().NotBeNull();
        validationProblemDetails.Message.Should().Contain(expectedErrorMessage);
    }

    [Fact(DisplayName = "Deve deletar o contato")]
    [Trait("Functional", "ContatoController")]
    public async Task Should_Delete_Contact()
    {
        //Arrange 
        await Should_Return_ContactCreatedWithSuccess();

        var response = await HttpClient.GetAsync($"api/contato");

        var content = await response.Content.ReadAsStringAsync();
        var contatoResponse = JsonSerializer.Deserialize<ResponseModel>(content, jsonOptions)!;

        using var jDoc = JsonDocument.Parse(contatoResponse.Data!.ToString()!);
        var contatoResponseData = jDoc.RootElement.GetProperty("data").Deserialize<List<ContatoDto>>(jsonOptions);
        var id = contatoResponseData!.First().Id;

        //Act
        var requestDelete = await HttpClient.DeleteAsync($"api/contato/{id}");

        //Assert
        requestDelete.EnsureSuccessStatusCode();
        requestDelete.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }
}