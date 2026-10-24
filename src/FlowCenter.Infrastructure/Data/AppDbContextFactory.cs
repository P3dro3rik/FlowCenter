using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace FlowCenter.Infrastructure.Data;

/// <summary>
/// Fábrica usada apenas pelas ferramentas de design-time do EF Core (ex.: `dotnet ef
/// migrations add`). A connection string nunca é hardcoded: deve ser exportada na
/// variável de ambiente ConnectionStrings__DefaultConnection (veja .env.example).
/// </summary>
public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        var connectionString = Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection");

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException(
                "Defina a variável de ambiente ConnectionStrings__DefaultConnection " +
                "antes de rodar ferramentas de design-time do EF Core (veja .env.example).");
        }

        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
        optionsBuilder.UseNpgsql(connectionString);

        return new AppDbContext(optionsBuilder.Options);
    }
}