using ContatosRegionais.Application.Events;
using ContatosRegionais.Domain.Entities;
using ContatosRegionais.Domain.Interfaces;
using ContatosRegionais.Domain.ValueObjects;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;

namespace ContatosRegionais.Application.Consumers;

public class InsertContatoConsumer(IServiceProvider serviceProvider) : IConsumer<InsertContatoEvent>
{
    private readonly IServiceProvider serviceProvider = serviceProvider;

    public async Task Consume(ConsumeContext<InsertContatoEvent> context)
    {
        var message = context.Message;

        try
        {
            // Cria o contato no banco de dados
            var contato = new Contato
            {
                Nome = message.Nome,
                Email = new Email(message.Email!),
                Telefone = message.Telefone,
                DDD = message.Ddd
            };

            using var scope = serviceProvider.CreateScope();
            var baseService = scope.ServiceProvider.GetRequiredService<IBaseRepository<Contato>>();
            await baseService.InsertAsync(contato);

            //Thread.Sleep(5000);
            Console.WriteLine($"Contato inserido com sucesso: {contato.Id}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao processar mensagem: Email:{message.Email} {ex.Message}");
        }
    }
}
