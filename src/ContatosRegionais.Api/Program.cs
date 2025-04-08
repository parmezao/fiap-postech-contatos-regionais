using ContatosRegionais.Application.Extensions;
using ContatosRegionais.Application.Validations;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Adiciona os Servi�os
#region Adiciona Logging
builder.Logging.ClearProviders().AddConsole();
#endregion

#region Configura servi�o de Controllers e Routes
builder.Services.AddControllers().ConfigureApiBehaviorOptions(options =>
{
    options.InvalidModelStateResponseFactory = context =>
    {
        var result = new ValidationFailedResult(context.ModelState);
        return result;
    };
});

builder.Services.Configure<RouteOptions>(options =>
{
    options.LowercaseQueryStrings = options.LowercaseUrls = true;
});
#endregion

#region Adiciona os Servi�os
builder.Services.AddServices();
#endregion

#region Adiciona a Conex�o
builder.Services.AddDbConnection(builder);
#endregion

#region Adiciona a documenta��o (Swagger)
builder.Services.AddDocs();
#endregion

#region Adiciona a Autentica��o
var configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json")
    .Build();

var key = Encoding.ASCII.GetBytes(configuration.GetValue<string>("SecretJWT") ?? string.Empty);
builder.Services.AddAuthentication(x =>
{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(j =>
{
    j.RequireHttpsMetadata = false;
    j.SaveToken = true;
    j.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = false,
        ValidateAudience = false
    };
});
#endregion

var app = builder.Build();

#region Aplica as Migra��es (Migrations)
if (!WasInvoked) app.ApplyMigrations();
#endregion

#region Utiliza M�tricas
app.UseMetrics();
#endregion

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
    app.UseSwagger();
    app.UseSwaggerUI();
//}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();


public partial class Program
{
    public static bool WasInvoked { private get; set; }
}