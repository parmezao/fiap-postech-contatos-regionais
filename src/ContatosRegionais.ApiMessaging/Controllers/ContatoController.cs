using AutoMapper;
using ContatosRegionais.Application.DTO;
using ContatosRegionais.Application.Events;
using ContatosRegionais.Application.Interfaces;
using ContatosRegionais.Domain.Entities;
using ContatosRegionais.Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ContatosRegionais.ApiMessaging.Controllers;

[Route("api/messaging/[controller]")]
[ApiController]
public class ContatoController(
    IBaseService<Contato> baseService, IMapper mapper, ILogger<ContatoController> logger) : ControllerBase
{
    private readonly IBaseService<Contato> _baseService = baseService;
    private readonly IMapper _mapper = mapper;
    private readonly ILogger<ContatoController> _logger = logger;

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

    [HttpGet]
    public IActionResult Get()
    {
        return Ok(new { message = "OK!" });
    }

}
