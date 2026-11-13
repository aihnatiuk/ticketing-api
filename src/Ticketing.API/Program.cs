using Booking.Presentation;
using Catalog.Presentation;
using Payment.Module;
using Scalar.AspNetCore;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder
    .Services.AddCatalogModule(builder.Configuration)
    .AddBookingModule(builder.Configuration)
    .AddPaymentModule(builder.Configuration);

builder.Services.AddOpenApi();

WebApplication app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
