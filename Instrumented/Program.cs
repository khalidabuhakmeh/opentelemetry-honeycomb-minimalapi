using System.Diagnostics;
using Honeycomb.OpenTelemetry;
using Instrumented.Models;
using Microsoft.EntityFrameworkCore;
using OpenTelemetry.Trace;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpClient();
builder.Services.AddDbContext<Database>(options =>
{
    options.UseSqlite(builder.Configuration.GetConnectionString("sqlite"));
});

builder.Services.AddOpenTelemetryTracing(tpb =>
{
    tpb
        .AddAspNetCoreInstrumentation()
        .AddEntityFrameworkCoreInstrumentation(ef => {
            ef.SetDbStatementForText = true;
        })
        .AddHoneycomb(HoneycombOptions.FromConfiguration(builder.Configuration))
        .AddConsoleExporter();
});

var app = builder.Build();

app.MapGet("/", async (Database db, HttpClient http) =>
{
    Activity.Current!.DisplayName = "Home";
    
    var person = await http.GetFromJsonAsync<Person>("https://random-data-api.com/api/name/random_name");
    //Activity.Current.AddTag(nameof(person), person);

    if (person == null)
        return Results.Problem();

    db.People.Add(person);
    await db.SaveChangesAsync();

    return Results.Ok(person);
});

app.Run();