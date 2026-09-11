namespace FlowCenter;

public class TarefaRepository
{
    private readonly FlowCenterDbContext _context;

    public TarefaRepository(FlowCenterDbContext context)
    {
        _context = context;
    }
}
