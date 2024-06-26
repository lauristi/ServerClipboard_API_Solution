using EncryptionLib;
using Microsoft.AspNetCore.HttpOverrides;
using ServerClipboard_API.Service;
using ServerClipboard_API.Service.Interface;

var builder = WebApplication.CreateBuilder(args);

//Set log
builder.Logging.ClearProviders();
builder.Logging.AddConsole();

//==============================================================================================
// Configure o Kestrel para ouvir em todas as interfaces de rede na porta 5020
//==============================================================================================

if (!builder.Environment.IsDevelopment())
{
    builder.WebHost.ConfigureKestrel(options =>
    {
        options.Listen(System.Net.IPAddress.Parse("192.168.0.156"), 5020);
    });
}
//==============================================================================================

// Add services to the container.
builder.Services.AddScoped<IBankStatementService, BankStatementService>();

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        //Sem isso todas as propriedades do Json ficam minusculas no retorno e causam problemas no blazor
        options.JsonSerializerOptions.PropertyNamingPolicy = null;
    });

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Registro da implementação da interface ICrypto de EncryptonLib
builder.Services.AddSingleton<ICrypto, CryptoClass>();

var app = builder.Build();

// Configure the HTTP request pipeline.

//if (app.Environment.IsDevelopment())
//{
app.UseSwagger();
app.UseSwaggerUI();
//}

//REMOVIDO PRA EVITAR ERRO DO CORS
//app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

//==============================================================================================
//Configuracao de Cabecalho encaminhado para funcionar com proxy reverso... Ngnix
//==============================================================================================
app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
});

//==============================================================================================

app.UseAuthentication();
app.Run();