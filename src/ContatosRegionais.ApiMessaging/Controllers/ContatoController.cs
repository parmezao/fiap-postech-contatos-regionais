using AutoMapper;
using ContatosRegionais.Application.DTO;
using ContatosRegionais.Application.Events;
using ContatosRegionais.Application.Interfaces;
using ContatosRegionais.Application.ViewModels;
using ContatosRegionais.Domain.Entities;
using ContatosRegionais.Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ContatosRegionais.ApiMessaging.Controllers;

[Route("api/messaging/[controller]")]
[ApiController]
public class ContatoController(
    IBaseService<Contato> baseService, IMapper mapper, ILogger<ContatoController> logger) : ControllerBase
{
    private readonly IBaseService<Contato> _baseService = baseService;
    private readonly IMapper _mapper = mapper;
    private readonly ILogger<ContatoController> _logger = logger;

    /// <summary>
    /// Endpoint utilizado para cadastrar um novo Contato
    /// </summary>
    /// <param name="contatoPublisher"></param>
    /// <param name="contatoDto">Necessário informar o Nome, Endereço de Email, Telefone e DDD do Contato para o cadastro</param>
    /// <returns>Retorna o objeto ContatoDto informado com o Id preenchido</returns>
    [HttpPost]
    [Authorize]
    public async Task<ActionResult<ResponseModel>> Create(
        [FromServices] IContatoPublisher contatoPublisher,
        ContatoDto contatoDto)
    {
        var responseModel = new ResponseModel();
        try
        {
            var contato = _mapper.Map<Contato>(contatoDto);

            // Cria a mensagem e publica na fila
            await contatoPublisher.PublishInsertContatoAsync(new InsertContatoEvent
            {
                Nome = contato.Nome,    
                Email = contato.Email.Endereco,
                Telefone = contato.Telefone,
                Ddd = contato.DDD
            });

            return new ResponseModel
            {
                StatusCode = StatusCodes.Status201Created,
                Message = "Cadastro em processamento.",
                Data = new
                {
                    contato.Nome,
                    contato.Email,
                    contato.Telefone,
                    contato.DDD
                }
            };
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message);
            return StatusCode(500, responseModel.Result(
                StatusCodes.Status500InternalServerError, "Internal Server Error", default!));
        }
    }

    /// <summary>
    /// Endpoint utilizado para listar todos os Contatos cadastrados
    /// </summary>
    /// <returns>Retorna a lista de objetos do tipo ContatoDto</returns>
    [HttpGet]
    [Authorize]
    [ProducesResponseType<List<ContatoDto>>(StatusCodes.Status200OK)]
    [ProducesResponseType<List<ContatoDto>>(StatusCodes.Status500InternalServerError)]
    [ProducesResponseType<List<ContatoDto>>(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<ResponseModel>> GetAll([FromQuery] PaginationParameters paginationParameters)
    {
        var responseModel = new ResponseModel();

        try
        {
            var contatos = await _baseService.GetAllAsync();
            var contatosDto = _mapper.Map<List<ContatoDto>>(contatos);

            // Paginação dos resultados
            var pagedResult = contatosDto!.AsQueryable()
                .ToPagedResult(paginationParameters.PageNumber, paginationParameters.PageSize);
            Response.Headers.Append("X-Total-Count", pagedResult.TotalItems.ToString());
            Response.Headers.Append("X-Total-Pages", pagedResult.TotalPages.ToString());

            return Ok(responseModel.Result(StatusCodes.Status200OK, "OK", pagedResult));
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message);
            return StatusCode(500, responseModel.Result(
                StatusCodes.Status500InternalServerError, "Erro Interno do Servidor", default!));
        }
    }

    /// <summary>
    /// Endpoint utilizado para localizar o Contato de acordo com o Id informado
    /// </summary>
    /// <param name="id">Id do objeto Contato. Necessário informar para localizar o Contato</param>
    /// <returns>Retorna o objeto do tipo Contato</returns>
    /// <response code="200">Contato encontrado com sucesso</response>
    /// <response code="404">Contato não encontrado</response>
    [HttpGet("{id:int}")]
    [Authorize]
    [ProducesResponseType<ContatoDto>(StatusCodes.Status200OK)]
    [ProducesResponseType<ContatoDto>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ContatoDto>(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ResponseModel>> GetById(int id)
    {
        var responseModel = new ResponseModel();

        try
        {
            var contato = await _baseService.GetByIdAsync(id);
            if (contato is null)
                return NotFound(responseModel.Result(StatusCodes.Status404NotFound, "Não Encontrado", default!));

            var contatoDto = _mapper.Map<ContatoDto>(contato);
            return Ok(responseModel.Result(StatusCodes.Status200OK, "OK", contatoDto));
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message);
            return StatusCode(500, responseModel.Result(
                StatusCodes.Status500InternalServerError, "Erro Interno do Servidor", default!));
        }
    }   

    /// <summary>
    /// Endpoint utilizado para filtrar os Contatos de acordo com o DDD informado
    /// </summary>
    /// <param name="ddd">DDD do objeto Contato. Necessário informar para localizar o Contato</param>
    /// <param name="paginationParameters">Parâmetros utilizados na paginação dos resultados</param>
    /// <returns>Retorna o objeto do tipo Contato</returns>
    [HttpGet, Route("ddd/{ddd:int}")]
    [Authorize]
    [ProducesResponseType<List<ContatoDto>>(StatusCodes.Status200OK)]
    [ProducesResponseType<List<ContatoDto>>(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ResponseModel>> GetByDdd(int ddd,
        [FromQuery] PaginationParameters paginationParameters)
    {
        var responseModel = new ResponseModel();
        try
        {
            var contatosDdd = await _baseService.FilterAsync(c => c.DDD == ddd);
            var contatosDddDto = _mapper.Map<List<ContatoDto>>(contatosDdd);

            // Paginação dos resultados
            var pagedResult = contatosDddDto
                .AsQueryable()
                .ToPagedResult(paginationParameters.PageNumber, paginationParameters.PageSize);
            Response.Headers.Append("X-Total-Count", pagedResult.TotalItems.ToString());
            Response.Headers.Append("X-Total-Pages", pagedResult.TotalPages.ToString());

            return Ok(responseModel.Result(StatusCodes.Status200OK, "OK", pagedResult));
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message);
            return StatusCode(500, responseModel.Result(
                StatusCodes.Status500InternalServerError, "Erro Interno do Servidor", default!));
        }
    }     

    /// <summary>
    /// Endpoint utilizado para alterar um Contato existente
    /// </summary>
    /// <param name="id">Id do objeto Contato. Necessário informar para localizar o Contato que será alterado</param>
    /// <param name="contatoDto">Objeto Contato. Necessário informar para aplicar as alterações no Contato que será alterado</param>
    /// <param name="contatoPublisher"></param>
    /// <returns>Retorna o objeto Contato informado com o Id preenchido</returns>
    [HttpPut("{id:int}")]
    [Authorize]
    [ProducesResponseType<ContatoDto>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ContatoDto>(StatusCodes.Status200OK)]
    [ProducesResponseType<ContatoDto>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ContatoDto>(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ResponseModel>> Update(
        int id, 
        ContatoDto contatoDto,
        [FromServices] IContatoPublisher contatoPublisher)
    {
        var responseModel = new ResponseModel();

        if (id != contatoDto.Id)
            return BadRequest(responseModel.Result(
                StatusCodes.Status400BadRequest, "Erro na Requisição", contatoDto));
        try
        {
            var contatoExistente = await _baseService.GetByIdAsync(id);
            if (contatoExistente is null)
                return NotFound(responseModel.Result(StatusCodes.Status404NotFound, "Não Encontrado", default!));

            _mapper.Map(contatoDto, contatoExistente);

            // Cria a mensagem e publica na fila
            await contatoPublisher.PublishUpdateContatoAsync(new UpdateContatoEvent(            
                contatoExistente.Id,
                contatoExistente.Nome!,    
                contatoExistente.Telefone!,
                contatoExistente.Email.Endereco,
                contatoExistente.DDD
            ));

            return new ResponseModel
            {
                StatusCode = StatusCodes.Status200OK,
                Message = "Atualização em processamento.",
                Data = new
                {
                    contatoExistente.Nome,
                    contatoExistente.Email,
                    contatoExistente.Telefone,
                    contatoExistente.DDD
                }
            };
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message);
            return StatusCode(500, responseModel.Result(
                StatusCodes.Status500InternalServerError, "Erro Interno do Servidor", default!));
        }
    }

    /// <summary>
    /// Endpoint utilizado para excluir o Contato de acordo com o Id informado
    /// </summary>
    /// <param name="id">Id do objeto Contato. Necessário informar para localizar o Contato que será excluído</param>
    /// <param name="contatoPublisher"></param>
    /// <returns>Retorna o objeto do tipo Contato</returns>
    [HttpDelete("{id:int}")]
    [Authorize]
    [ProducesResponseType<ContatoDto>(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ContatoDto>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ContatoDto>(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ResponseModel>> Delete(
        int id,
        [FromServices] IContatoPublisher contatoPublisher)
    {
        var responseModel = new ResponseModel();

        var contatoExistente = await _baseService.GetByIdAsync(id);
        if (contatoExistente is null)
            return NotFound(responseModel.Result(StatusCodes.Status404NotFound, "Não Encontrado", default!));

        try
        {
            // Cria a mensagem e publica na fila
            await contatoPublisher.PublishDeleteContatoAsync(new DeleteContatoEvent{ Id = contatoExistente.Id });            

            return new ResponseModel
            {
                StatusCode = StatusCodes.Status204NoContent,
                Message = "Exclusão em processamento.",
                Data = new()  
            };             
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message);
            return StatusCode(500, responseModel.Result(
                StatusCodes.Status500InternalServerError, "Erro Interno do Servidor", default!));
        }
    }

    /// <summary>
    /// Endpoint utilizado para listar todos os Contatos cadastrados, através da Azure Function
    /// </summary>
    /// <returns>Retorna a lista de objetos do tipo ContatoDto</returns>
    [HttpGet, Route("function")]
    public async Task<ActionResult<ResponseModel>> GetAllContacts(
        [FromQuery] PaginationParameters paginationParameters)
    {
        var responseModel = new ResponseModel();

        try
        {
            string functionUrl = Environment.GetEnvironmentVariable("FUNCTION_URL")?? string.Empty;
            var functionResult = await CallHttpFunction<ResponseModel>(functionUrl, HttpMethod.Get);

            var contatosDto = JsonConvert.DeserializeObject<List<ContatoDto>>(JsonConvert.SerializeObject(functionResult.Data));

            // Paginação dos resultados
            var pagedResult = contatosDto!
                .AsQueryable()
                .ToPagedResult(paginationParameters.PageNumber, paginationParameters.PageSize);
            Response.Headers.Append("X-Total-Count", pagedResult.TotalItems.ToString());
            Response.Headers.Append("X-Total-Pages", pagedResult.TotalPages.ToString());

            return Ok(responseModel.Result(StatusCodes.Status200OK, "OK", pagedResult));
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message);
            return StatusCode(500, responseModel.Result(
                StatusCodes.Status500InternalServerError, "Erro Interno do Servidor", default!));
        }
    }

    private static async Task<TResponse> CallHttpFunction<TResponse>(string functionUrl, HttpMethod method, object? requestBody = null)
    {
        try
        {
            HttpRequestMessage request = new(method, functionUrl);

            if (requestBody != null)
            {
                string jsonBody = JsonConvert.SerializeObject(requestBody);
                request.Content = new StringContent(jsonBody, Encoding.UTF8, "application/json");
            }

            var _httpClient = new HttpClient();
            HttpResponseMessage response = await _httpClient.SendAsync(request);

            response.EnsureSuccessStatusCode(); // Lança uma exceção para códigos de status de erro
            string responseContent = await response.Content.ReadAsStringAsync();

            if (typeof(TResponse) == typeof(string))
            {
                return (TResponse)(object)responseContent;
            }
            else if (!string.IsNullOrEmpty(responseContent))
            {
                return JsonConvert.DeserializeObject<TResponse>(responseContent)!;
            }
            else
            {
                return default!;
            }
        }
        catch (HttpRequestException ex)
        {
            throw new HttpRequestException($"Erro na requisição HTTP: {ex.Message}");
        }
        catch (JsonException ex)
        {
            throw new JsonException($"Erro na requisição HTTP: {ex.Message}");
        }
    }
}
