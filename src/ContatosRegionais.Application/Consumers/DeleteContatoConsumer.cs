using ContatosRegionais.Application.Events;
using MassTransit;

namespace ContatosRegionais.Application.Consumers;

public class DeleteContatoConsumer : IConsumer<DeleteContatoEvent>
{
    public Task Consume(ConsumeContext<DeleteContatoEvent> context)
    {
        throw new NotImplementedException();
    }
}
