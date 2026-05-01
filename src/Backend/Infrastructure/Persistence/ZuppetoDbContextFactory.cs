using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Zuppeto.Infrastructure.Persistence;

public sealed class ZuppetoDbContextFactory : IDesignTimeDbContextFactory<ZuppetoDbContext>
{
    public ZuppetoDbContext CreateDbContext(string[] args)
    {
        var connectionString = Environment.GetEnvironmentVariable("ConnectionStrings__Zuppeto")
            ?? Environment.GetEnvironmentVariable("ZUPPETO_DB_CONNECTION_STRING")
            ?? "Host=localhost;Port=5433;Database=zuppeto;Username=app;Password=app";

        var optionsBuilder = new DbContextOptionsBuilder<ZuppetoDbContext>();
        optionsBuilder.UseNpgsql(connectionString);

        return new ZuppetoDbContext(optionsBuilder.Options);
    }
}
