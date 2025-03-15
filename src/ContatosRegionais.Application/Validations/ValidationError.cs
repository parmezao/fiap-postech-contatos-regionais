using System.Text.Json.Serialization;

namespace ContatosRegionais.Application.Validations;

public class ValidationError(string field, string message)
{
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? Field { get; } = field != string.Empty ? field : null;

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public int Code { get; set; }

    public string Message { get; } = message;
}