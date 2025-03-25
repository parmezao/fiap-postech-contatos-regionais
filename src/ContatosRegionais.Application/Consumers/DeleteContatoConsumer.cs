using ContatosRegionais.Application.Events;
using ContatosRegionais.Domain.Entities;
using ContatosRegionais.Domain.Interfaces;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;

namespace ContatosRegionais.Application.Consumers;

public class DeleteContatoConsumer(IServiceProvider serviceProvider) : IConsumer<DeleteContatoEvent>
{
    public async Task Consume(ConsumeContext<DeleteContatoEvent> context)
    {
        var message = context.Message;

        try
        {
            using var scope = serviceProvider.CreateScope();
            var baseService = scope.ServiceProvider.GetRequiredService<IBaseRepository<Contato>>();
            await baseService.DeleteAsync(message.Id);

            Console.WriteLine($"Contato excluído com sucesso: {message.Id}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao processar mensagem: Id: {message.Id} {ex.Message}");
        }
    }
}
