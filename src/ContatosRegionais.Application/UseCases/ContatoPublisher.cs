using ContatosRegionais.Application.Events;
using ContatosRegionais.Application.Interfaces;
using MassTransit;

namespace ContatosRegionais.Application.UseCases;

public class ContatoPublisher(IPublishEndpoint publishEndpoint) : IContatoPublisher
{
    private readonly IPublishEndpoint _publishEndpoint = publishEndpoint;

    public Task PublishDeleteContatoAsync(DeleteContatoEvent message)
    {
        throw new NotImplementedException();
    }

    public async Task PublishInsertContatoAsync(InsertContatoEvent message)
    {
        await _publishEndpoint.Publish(message);
    }

    public Task PublishUpdateContatoAsync(UpdateContatoEvent message)
    {
        throw new NotImplementedException();
    }
}