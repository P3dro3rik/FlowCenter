using System.Collections.Concurrent;
using FlowCenter.Application.Interfaces;
using FlowCenter.Domain.Entities;
using FlowCenter.Domain.Enums;

namespace FlowCenter.Infrastructure.Persistence;

/// <summary>
/// Implementação em memória do repositório de tarefas.
/// Mantém os dados localmente, no processo da aplicação, sem depender de
/// nenhum banco de dados externo. Registrada como singleton (RNF04) para que
/// os dados persistam entre requisições enquanto a aplicação estiver em execução.
/// </summary>
public class TarefaRepository : ITarefaRepository
{
    private readonly ConcurrentDictionary<Guid, Tarefa> _tarefas = new();

    public Task<IReadOnlyList<Tarefa>> ListarAsync(
        StatusTarefa? status,
        PrioridadeTarefa? prioridade,
        CancellationToken cancellationToken = default)
    {
        var query = _tarefas.Values.AsEnumerable();

        if (status.HasValue)
            query = query.Where(t => t.Status == status.Value); // RF07

        if (prioridade.HasValue)
            query = query.Where(t => t.Prioridade == prioridade.Value); // RF08

        IReadOnlyList<Tarefa> resultado = query.OrderBy(t => t.DataCriacao).ToList();
        return Task.FromResult(resultado);
    }

    public Task<Tarefa?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        _tarefas.TryGetValue(id, out var tarefa);
        return Task.FromResult(tarefa);
    }

    public Task AdicionarAsync(Tarefa tarefa, CancellationToken cancellationToken = default)
    {
        _tarefas[tarefa.Id] = tarefa;
        return Task.CompletedTask;
    }

    public void Remover(Tarefa tarefa) => _tarefas.TryRemove(tarefa.Id, out _);

    /// <summary>
    /// Não há operação a realizar: as alterações no armazenamento em memória
    /// já são aplicadas de forma síncrona pelos demais métodos deste repositório.
    /// </summary>
    public Task SalvarAlteracoesAsync(CancellationToken cancellationToken = default) =>
        Task.CompletedTask;
}
