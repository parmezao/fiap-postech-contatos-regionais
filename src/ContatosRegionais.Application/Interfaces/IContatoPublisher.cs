using ContatosRegionais.Application.Events;

namespace ContatosRegionais.Application.Interfaces;

public interface IContatoPublisher
{
    Task PublishInsertContatoAsync(InsertContatoEvent message);
    Task PublishUpdateContatoAsync(UpdateContatoEvent message);
    Task PublishDeleteContatoAsync(DeleteContatoEvent message);
}
