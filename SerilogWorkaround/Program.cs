using Microsoft.AspNetCore.Identity;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .Destructure.ByTransforming<IdentityUser>(u => new { u.Id, u.UserName })
    .CreateBootstrapLogger();


var serilogSelfLogWriter =
    File.CreateText(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Logs",
        "serilog.log"));
Serilog.Debugging.SelfLog.Enable(msg => TextWriter.Synchronized(serilogSelfLogWriter));


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


var host = builder.Host;
builder.Logging.AddConsole();

host.UseSerilog((ctx, lc) => lc.ReadFrom.Configuration(ctx.Configuration));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();