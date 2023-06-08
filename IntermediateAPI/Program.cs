using IntermediateAPI;
using IntermediateAPI.Extensions;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.Identity.Web;
using Serilog;
using Serilog.Events;

Log.Logger = new LoggerConfiguration().CreateBootstrapLogger();
        

var builder = WebApplication.CreateBuilder(args);

// Add AAD bearer token authentication
builder.Services.AddMicrosoftIdentityWebApiAuthentication(builder.Configuration);

// Add services to the container.
builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddMemoryCache();
// Add data services
builder.Services.AddDataServices();

builder.Services.AddApplicationInsightsTelemetry();

builder.Host.UseSerilog((context, serviceProvider, loggerConfig) =>
{
    loggerConfig
        .MinimumLevel.Override("Microsoft", LogEventLevel.Error)
        .MinimumLevel.Information()
        .Enrich.FromLogContext()
        .WriteTo.Console(outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level}] [{SourceContext}] {Message}{NewLine}{Exception}")
        .WriteTo.ApplicationInsights(serviceProvider.GetRequiredService<TelemetryConfiguration>(), TelemetryConverter.Traces);
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.Use(async (context, next) => {
    context.Request.EnableBuffering();
    await next();
});

app.UseMiddleware<ExceptionHandlingMiddleware>();

//app.UseSerilogRequestLogging();

app.UseAuthentication();
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers().RequireAuthorization();

app.Run();
