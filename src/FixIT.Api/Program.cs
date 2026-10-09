using FixIT.Application.Interfaces;
using FixIT.Application.Services;
using FixIT.Infrastracture;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddScoped<IServiceRequestService, ServiceRequestService>();

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();
// API svarar bara om systemet skickar rätt nyckel
app.Use(async (context, next) =>
{
    var key = context.Request.Headers["X-Api-Key"].ToString();
    if (key != builder.Configuration["ApiKey"])
    {
        context.Response.StatusCode = 401;
        return;
    }
    await next();
});

app.MapControllers();

app.Run();
