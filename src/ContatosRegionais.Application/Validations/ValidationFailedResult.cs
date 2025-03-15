using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc;

namespace ContatosRegionais.Application.Validations;

public class ValidationFailedResult : ObjectResult
{
    public ValidationFailedResult(ModelStateDictionary modelState)
        : base(new ValidationResultModel(modelState))
    {
        StatusCode = StatusCodes.Status422UnprocessableEntity;
    }
}
