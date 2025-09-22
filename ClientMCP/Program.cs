using Azure;
using Azure.AI.OpenAI;
using Azure.Identity;
using Microsoft.Extensions.AI;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var endpoint = builder.Configuration["AI:Endpoint"];
var apiKey = builder.Configuration["AI:ApiKey"];
var model = builder.Configuration["AI:ModelName"];

builder.Services.AddChatClient(services =>
  new ChatClientBuilder(
    (
      !string.IsNullOrEmpty(apiKey)
        ? new AzureOpenAIClient(new Uri(endpoint!), new AzureKeyCredential(apiKey))
        : new AzureOpenAIClient(new Uri(endpoint!), new DefaultAzureCredential())
    ).GetChatClient(model).AsIChatClient()
  )
  .UseFunctionInvocation()
  .Build());

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(options => {
        options.SwaggerEndpoint("/openapi/v1.json", "MCP Server");
        options.RoutePrefix = "";
    });   
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
