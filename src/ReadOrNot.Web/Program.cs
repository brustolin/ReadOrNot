using ReadOrNot.Web.Endpoints;

var builder = WebApplication.CreateBuilder(args);

var app = builder.Build();

app.UseHttpsRedirection();
app.UseDefaultFiles();
app.UseStaticFiles();

app.MapGet("/client-config", (IConfiguration configuration) =>
{
    var apiBaseUrl = configuration["FrontendClient:ApiBaseUrl"] ?? "https://localhost:7040/api/v1";
    return Results.Ok(new ClientConfigResponse(apiBaseUrl));
});

app.MapFallbackToFile("index.html");

app.Run();
