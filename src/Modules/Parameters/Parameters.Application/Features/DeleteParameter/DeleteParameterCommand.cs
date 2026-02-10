using MediatR;

namespace Parameters.Application.Features.DeleteParameter;

/// <summary>
/// Command para deletar Parâmetro
/// </summary>
public record DeleteParameterCommand(string E1Stamp) : IRequest<bool>;
