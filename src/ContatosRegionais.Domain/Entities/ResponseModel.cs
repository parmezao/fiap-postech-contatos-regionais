namespace ContatosRegionais.Domain.Entities;

public class ResponseModel
{
    public string? Message { set; get; }
    public int? StatusCode { set; get; }
    public object? Data { set; get; }

    public ResponseModel Result(int statusCode, string message, object data)
    {
        StatusCode = statusCode;
        Message = message;
        Data = data;

        return this;
    }
}
