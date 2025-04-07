using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Net.Http;
using ContatosRegionais.Domain.Entities;
using System.Text.Json;
using System.Web.Http;
using System.Net.Http.Headers;
using ContatosRegionais.Application.DTO;

namespace ContatosRegionais.ContatosFunction
{
    public static class GetAllContacts
    {
        private static readonly JsonSerializerOptions jsonOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };

        [FunctionName("GetAllContacts")]
        public static async Task<ActionResult<ResponseModel>> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            var _apiUrl = Environment.GetEnvironmentVariable("API_URL");
            var _tokenUrl = Environment.GetEnvironmentVariable("TOKEN_URL");

            log.LogInformation($"Azure Function executada em: {DateTime.UtcNow}");

            try
            {
                var httpClientHandler = new HttpClientHandler
                {
                    ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
                }; 
                var _httpClient = new HttpClient(httpClientHandler);

                HttpResponseMessage responseToken = await _httpClient.PostAsJsonAsync(_tokenUrl, new { username = "admin", password = "admin@123" });
                var jwtToken = await responseToken.Content.ReadAsStringAsync();

                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);
                HttpResponseMessage response = await _httpClient.GetAsync(_apiUrl);

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var contatoResponse = JsonSerializer.Deserialize<ResponseModel>(content, jsonOptions)!;
                    using var jDoc = JsonDocument.Parse(contatoResponse.Data!.ToString()!);

                    var contatoResponseData = jDoc.RootElement.GetProperty("data").Deserialize<List<ContatoDto>>(jsonOptions);

                    var responseData = new ResponseModel();
                    return responseData.Result(StatusCodes.Status200OK, "OK", contatoResponseData);
                }
                else
                {
                    log.LogError($"Erro ao acessar API: {response.StatusCode}");
                    return new BadRequestObjectResult("Erro ao acessar API");
                }
            }
            catch (Exception ex)
            {
                log.LogError($"Erro: {ex.Message}");
                return new InternalServerErrorResult();
            }
        }

    }
}
