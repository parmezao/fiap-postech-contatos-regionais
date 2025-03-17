using ContatosRegionais.Application.Extensions;
using ContatosRegionais.Application.Validations;
using ContatosRegionais.Infra.IoC;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Reflection;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

#region Adiciona Logging
builder.Logging.ClearProviders().AddConsole();
#endregion

#region Configura serviço de Controllers e Routes
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

builder.Services.AddEndpointsApiExplorer();

#region Adiciona Documentação
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "Cadastro de Contatos com Mensageria", Version = "v1.0" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description =
        "JWT Authorization Header - utilizado com Bearer Authentication.\r\n\r\n" +
        "Digite 'Bearer' [espaço] e então seu token no campo abaixo.\r\n\r\n" +
        "Exemplo (informar sem as aspas): 'Bearer 12345abcdef'",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });

    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);

    c.IncludeXmlComments(xmlPath);
});
#endregion

// Add Dependency Injection
builder.Services.AddMessagingInjections(builder.Configuration);

#region Adiciona Autenticação
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

#region Utiliza Métricas
app.UseMetrics();
#endregion

//if (app.Environment.IsDevelopment())
//{
    app.UseSwagger();
    app.UseSwaggerUI();
//}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();

public partial class Program;
