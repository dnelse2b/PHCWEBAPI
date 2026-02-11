using Mapster;
using Parameters.Domain.Entities;
using Parameters.Application.Features.CreateParameter;
using Parameters.Application.Features.UpdateParameter;
using Shared.Kernel.Extensions;

namespace Parameters.Application.Mappings;

public static class Para1Mappings
{
    public static Para1 ToEntity(this CreateParameterDto dto, string? createdBy)
    {
        return new Para1(
            25.GenerateStamp(),
            dto.Descricao,
            dto.Valor,
            dto.Tipo,
            dto.Dec,
            dto.Tam,
            createdBy
        );
    }

    public static void UpdateEntity(this Para1 entity, UpdateParameterDto dto, string? updatedBy)
    {
        entity.Update(
            dto.Descricao,
            dto.Valor,
            dto.Tipo,
            dto.Dec,
            dto.Tam,
            updatedBy
        );
    }

    public static TDto ToDto<TDto>(this Para1 entity) where TDto : class
    {
        return entity.Adapt<TDto>();
    }

    public static IEnumerable<TDto> ToDtos<TDto>(this IEnumerable<Para1> entities) where TDto : class
    {
        return entities.Select(e => e.Adapt<TDto>());
    }
}
