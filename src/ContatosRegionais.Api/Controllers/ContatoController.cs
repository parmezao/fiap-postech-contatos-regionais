using ContatosRegionais.Application.DTO;
using ContatosRegionais.Application.Extensions;
using ContatosRegionais.Application.Validations;
using ContatosRegionais.Application.ViewModels;
using ContatosRegionais.Domain.Entities;
using ContatosRegionais.Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ContatosRegionais.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ContatoController(
    IBaseService<Contato> baseService, ILogger<ContatoController> logger) : ControllerBase
{
    private readonly IBaseService<Contato> _baseService = baseService;
    private readonly ILogger<ContatoController> _logger = logger;

    /// <summary>
    /// Endpoint utilizado para cadastrar um novo Contato
    /// </summary>
    /// <param name="contatoDto">Necessário informar o Nome, Endereço de Email, Telefone e DDD do Contato para o cadastro</param>
    /// <returns>Retorna o objeto ContatoDto informado com o Id preenchido</returns>
    [HttpPost]
    [Authorize]
    [ValidateModel]
    [ProducesResponseType<ContatoDto>(StatusCodes.Status201Created)]
    [ProducesResponseType<ContatoDto>(StatusCodes.Status422UnprocessableEntity)]
    public async Task<ActionResult<ResponseModel>> Create(ContatoDto contatoDto)
    {
        var responseModel = new ResponseModel();

        try
        {
            var contato = contatoDto.ToContato();
            var result = await _baseService.AddAsync(contato);

            return CreatedAtAction(nameof(GetById), new { id = result.Id },
                responseModel.Result(StatusCodes.Status201Created, "Criado", result));
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
            var contatosDto = contatos.ToDto();

            // Paginação dos resultados
            var pagedResult = contatosDto.AsQueryable()
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

            var contatoDto = contato.ToDto();
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
            var contatosDddDto = contatosDdd.ToDto();

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
    /// <returns>Retorna o objeto Contato informado com o Id preenchido</returns>
    [HttpPut("{id:int}")]
    [Authorize]
    [ValidateModel]
    [ProducesResponseType<ContatoDto>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ContatoDto>(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ContatoDto>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ContatoDto>(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ResponseModel>> Update(int id, ContatoDto contatoDto)
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

            contatoExistente = contatoDto.ToContato();
            await _baseService.UpdateAsync(contatoExistente);

            return NoContent();
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
    /// <returns>Retorna o objeto do tipo Contato</returns>
    [HttpDelete("{id:int}")]
    [Authorize]
    [ProducesResponseType<ContatoDto>(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ContatoDto>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ContatoDto>(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Delete(int id)
    {
        var responseModel = new ResponseModel();

        var contatoExistente = await _baseService.GetByIdAsync(id);
        if (contatoExistente is null)
            return NotFound(responseModel.Result(StatusCodes.Status404NotFound, "Não Encontrado", default!));

        try
        {
            await _baseService.DeleteAsync(contatoExistente.Id);
            return NoContent();
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message);
            return StatusCode(500, responseModel.Result(
                StatusCodes.Status500InternalServerError, "Erro Interno do Servidor", default!));
        }
    }
}

