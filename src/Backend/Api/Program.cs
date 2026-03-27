using YepPet.Application;
using YepPet.Api.Endpoints;
using YepPet.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddProblemDetails();

var app = builder.Build();

app.UseExceptionHandler();

app.MapGet("/", () => "Hello World!");
app.MapGet("/health/db", () => Results.Ok(new { status = "configured" }));
app.MapPlaceEndpoints();
app.MapFavoriteEndpoints();
app.MapUserEndpoints();
app.MapReviewEndpoints();

app.Run();
