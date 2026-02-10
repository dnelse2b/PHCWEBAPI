using System.Text.Json.Serialization;

namespace Shared.Kernel.Responses;

/// <summary>
/// Resposta padrão da API
/// </summary>
public sealed record ResponseDTO
{
    [JsonPropertyName("response")]
    public ResponseCodeDTO Response { get; init; }

    [JsonPropertyName("data")]
    public object? Data { get; init; }

    [JsonPropertyName("content")]
    public object? Content { get; init; }

    public ResponseDTO(ResponseCodeDTO response, object? data = null, object? content = null)
    {
        Response = response;
        Data = data;
        Content = content;
    }

    /// <summary>
    /// Cria uma resposta de sucesso
    /// </summary>
    public static ResponseDTO Success(object? data = null, object? content = null) 
        => new(ResponseCodes.Success, data, content);

    /// <summary>
    /// Cria uma resposta de erro
    /// </summary>
    public static ResponseDTO Error(ResponseCodeDTO errorCode, object? data = null, object? content = null)
        => new(errorCode, data, content);
}

