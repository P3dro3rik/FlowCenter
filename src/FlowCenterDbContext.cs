namespace FlowCenter;

using Microsoft.EntityFrameworkCore;

public class FlowCenterDbContext : DbContext
{
    public FlowCenterDbContext(DbContextOptions<FlowCenterDbContext> options) : base(options)
    {
    }

    public DbSet<Tarefa> Tarefas { get; set; } = null!;
}
