using ContatosRegionais.Application.Events;
using ContatosRegionais.Application.Interfaces;
using MassTransit;

namespace ContatosRegionais.Application.UseCases;

public class ContatoPublisher(IPublishEndpoint publishEndpoint) : IContatoPublisher
{
    private readonly IPublishEndpoint _publishEndpoint = publishEndpoint;

    public async Task PublishDeleteContatoAsync(DeleteContatoEvent message)
    {
        await _publishEndpoint.Publish(message);
    }

    public async Task PublishInsertContatoAsync(InsertContatoEvent message)
    {
        await _publishEndpoint.Publish(message);
    }

    public async Task PublishUpdateContatoAsync(UpdateContatoEvent message)
    {
        await _publishEndpoint.Publish(message);
    }
}