using ContatosRegionais.Application.Events;
using ContatosRegionais.Domain.Entities;
using ContatosRegionais.Domain.Interfaces;
using ContatosRegionais.Domain.ValueObjects;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;

namespace ContatosRegionais.Application.Consumers;

public class UpdateContatoConsumer(IServiceProvider serviceProvider) : IConsumer<UpdateContatoEvent>
{
    public async Task Consume(ConsumeContext<UpdateContatoEvent> context)
    {
        var message = context.Message;

        try
        {
            // Atualiza o contato no banco de dados
            var contato = new Contato
            {
                Id = message.Id,
                Nome = message.Nome,
                Email = new Email(message.Email!),
                Telefone = message.Telefone,
                DDD = message.Ddd
            };

            using var scope = serviceProvider.CreateScope();
            var baseService = scope.ServiceProvider.GetRequiredService<IBaseRepository<Contato>>();
            await baseService.UpdateAsync(contato);

            Console.WriteLine($"Contato atualizado com sucesso: {contato.Id}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao processar mensagem: Email:{message.Email} {ex.Message}");
        }
    }
}