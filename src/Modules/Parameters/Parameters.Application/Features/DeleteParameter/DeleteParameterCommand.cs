using MediatR;

namespace Parameters.Application.Features.DeleteParameter;


public record DeleteParameterCommand(string ParaStamp) : IRequest<bool>;
