using MediatR;
using Parameters.Domain.Repositories;

namespace Parameters.Application.Features.DeleteParameter;

/// <summary>
/// Handler para deletar Parâmetro
/// </summary>
public class DeleteParameterCommandHandler : IRequestHandler<DeleteParameterCommand, bool>
{
    private readonly IE1Repository _e1Repository;
    private readonly IE4Repository _e4Repository;

    public DeleteParameterCommandHandler(
        IE1Repository e1Repository,
        IE4Repository e4Repository)
    {
        _e1Repository = e1Repository;
        _e4Repository = e4Repository;
    }

    public async Task<bool> Handle(DeleteParameterCommand request, CancellationToken cancellationToken)
    {
        // Get E1
        var e1 = await _e1Repository.GetByStampAsync(request.E1Stamp, cancellationToken);
        if (e1 == null)
            return false;

        // Delete E4 if exists
        var e4 = await _e4Repository.GetByStampAsync(request.E1Stamp, cancellationToken);
        if (e4 != null)
        {
            await _e4Repository.DeleteAsync(e4, cancellationToken);
        }

        // Delete E1
        await _e1Repository.DeleteAsync(e1, cancellationToken);

        return true;
    }
}
