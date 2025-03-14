using ContatosRegionais.Application.Events;
using MassTransit;

namespace ContatosRegionais.Application.Consumers;

public class UpdateContatoConsumer : IConsumer<UpdateContatoEvent>
{
    public Task Consume(ConsumeContext<UpdateContatoEvent> context)
    {
        throw new NotImplementedException();
    }
}