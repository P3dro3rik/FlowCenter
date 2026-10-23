using FlowCenter.Application.Interfaces;
using FlowCenter.Domain.Entities;
using FlowCenter.Domain.Enums;
using FlowCenter.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace FlowCenter.Infrastructure.Persistence;

/// <summary>
/// Implementação do repositório de tarefas sobre PostgreSQL via EF Core
/// (RNF03, RNF04).
/// </summary>
public class TarefaRepository : ITarefaRepository
{
    private readonly AppDbContext _context;

    public TarefaRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<Tarefa>> ListarAsync(
        StatusTarefa? status,
        PrioridadeTarefa? prioridade,
        CancellationToken cancellationToken = default)
    {
        var query = _context.Tarefas.AsQueryable();

        if (status.HasValue)
            query = query.Where(t => t.Status == status.Value); // RF07

        if (prioridade.HasValue)
            query = query.Where(t => t.Prioridade == prioridade.Value); // RF08

        return await query.OrderBy(t => t.DataCriacao).ToListAsync(cancellationToken);
    }

    public Task<Tarefa?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        _context.Tarefas.SingleOrDefaultAsync(t => t.Id == id, cancellationToken);

    public async Task AdicionarAsync(Tarefa tarefa, CancellationToken cancellationToken = default) =>
        await _context.Tarefas.AddAsync(tarefa, cancellationToken);

    public void Remover(Tarefa tarefa) => _context.Tarefas.Remove(tarefa);

    public async Task SalvarAlteracoesAsync(CancellationToken cancellationToken = default) =>
        await _context.SaveChangesAsync(cancellationToken);
}
