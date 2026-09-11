using FlowCenter.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FlowCenter.Infrastructure.Persistence;

/// <summary>
/// Contexto do Entity Framework Core para o FlowCenter. (RNF03, RNF04)
/// </summary>
public class FlowCenterDbContext : DbContext
{
    public FlowCenterDbContext(DbContextOptions<FlowCenterDbContext> options)
        : base(options)
    {
    }

    public DbSet<Tarefa> Tarefas => Set<Tarefa>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(FlowCenterDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
