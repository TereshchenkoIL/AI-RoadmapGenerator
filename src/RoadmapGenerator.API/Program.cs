using RoadmapGenerator.API.Extensions;
using RoadmapGenerator.BL;
using RoadmapGenerator.BL.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpoints(typeof(Program).Assembly);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<IRoadmapGenerator, RoadmapGenerator.BL.Services.RoadmapGenerator>();
builder.Services.AddScoped<ILanguageModelService, ChatGpt3ModelService>();
builder.Services.AddScoped<ILanguageModelFactory, LanguageModelFactory>();
builder.Services.AddScoped<ChatGpt3ModelService>();
builder.Services.AddScoped<ChatGpt4oModelService>();
builder.Services.AddScoped<ChatGpt4oMiniModelService>();
builder.Services.AddScoped<ChatGpto3MiniModelService>();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy
            .AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});
builder.Services.Configure<OpenApiSettings>(builder.Configuration.GetSection(OpenApiSettings.SectionName));

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseCors("AllowAll");
app.UseHttpsRedirection();
var apiGroup = app.MapGroup("api/v1");
app.MapEndpoints(apiGroup);

app.Run();