using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Instrumented.Models;

public class Database : DbContext
{
    public Database(DbContextOptions options)
        :base(options)
    { }

    public DbSet<Person> People { get; set; } = null!;
}

public record Person
{
    [JsonIgnore]
    public int Id { get; set; }
    [JsonPropertyName("first_name")]
    public string FirstName { get; set; }
    [JsonPropertyName("last_name")]
    public string LastName { get; set; }
}