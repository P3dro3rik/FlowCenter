using FlowCenter.Domain.Entities;
using FlowCenter.Domain.Enums;

namespace FlowCenter.Application.Interfaces;

/// <summary>
/// Contrato de persistência para a entidade Tarefa.
/// A implementação concreta reside na camada Infrastructure.
/// </summary>
public interface ITarefaRepository
{
    /// <summary>Lista tarefas, opcionalmente filtradas por status e/ou prioridade. (RF07, RF08)</summary>
    Task<IReadOnlyList<Tarefa>> ListarAsync(
        StatusTarefa? status,
        PrioridadeTarefa? prioridade,
        CancellationToken cancellationToken = default);

    /// <summary>Obtém uma tarefa pelo identificador, ou null caso não exista. (RF03, RB09)</summary>
    Task<Tarefa?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>Adiciona uma nova tarefa. (RF01)</summary>
    Task AdicionarAsync(Tarefa tarefa, CancellationToken cancellationToken = default);

    /// <summary>Remove uma tarefa. (RF06)</summary>
    void Remover(Tarefa tarefa);

    /// <summary>Persiste as alterações pendentes.</summary>
    Task SalvarAlteracoesAsync(CancellationToken cancellationToken = default);
}
