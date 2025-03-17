using ContatosRegionais.Application.Extensions;
using ContatosRegionais.Infra.Consumer;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<Worker>();

builder.Services.AddInjections(builder.Configuration);

var host = builder.Build();
host.Run();
