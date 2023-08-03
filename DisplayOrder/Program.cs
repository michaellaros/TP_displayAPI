using DisplayOrder.Interfaces;
using DisplayOrder.Services;
using NLog.Web;

var logger = NLogBuilder.ConfigureNLog("nlog.config").GetCurrentClassLogger();
try
{
    var builder = WebApplication.CreateBuilder(args);
    builder.Logging.ClearProviders();
    builder.Host.UseNLog();
    // Add services to the container.

    builder.Services.AddControllers();
    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();
    builder.Services.AddSingleton<IDatabaseService, DatabaseService>();
    var app = builder.Build();

    // Configure the HTTP request pipeline.

    app.UseSwagger();
    app.UseSwaggerUI();


    app.UseAuthorization();
    app.UseCors(options => options.AllowAnyHeader()
                                  .AllowAnyMethod()
                                  .AllowAnyOrigin()
                          );

    app.MapControllers();

    app.Run();
}
catch (Exception exception)
{
    // NLog: catch setup errors
    logger.Error(exception, "Stopped program because of exception");
    throw;
}
finally
{
    // Ensure to flush and stop internal timers/threads before application-exit (Avoid segmentation fault on Linux)
    NLog.LogManager.Shutdown();
}