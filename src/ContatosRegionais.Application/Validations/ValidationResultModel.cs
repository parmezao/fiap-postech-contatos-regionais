using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace ContatosRegionais.Application.Validations;

public class ValidationResultModel(ModelStateDictionary modelState)
{
    public string Message { get; } = "Falha na Validação";

    public List<ValidationError> Errors { get; } = [.. modelState.Keys
            .SelectMany(key => modelState[key]!.Errors
                .Select(x => new ValidationError(key, x.ErrorMessage)))];
}

