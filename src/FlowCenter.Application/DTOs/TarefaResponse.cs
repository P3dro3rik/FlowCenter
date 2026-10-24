using FlowCenter.Domain.Entities;
using FlowCenter.Domain.Enums;

namespace FlowCenter.Application.DTOs;

/// <summary>
/// Representação de uma tarefa retornada pela API.
/// </summary>
public class TarefaResponse
{
    public Guid Id { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public string? Descricao { get; set; }
    public StatusTarefa Status { get; set; }
    public PrioridadeTarefa Prioridade { get; set; }
    public DateTime DataCriacao { get; set; }
    public DateTime? DataConclusao { get; set; }

    /// <summary>Converte uma entidade de domínio no seu contrato de saída.</summary>
    public static TarefaResponse FromDomain(Tarefa tarefa) => new()
    {
        Id = tarefa.Id,
        Titulo = tarefa.Titulo,
        Descricao = tarefa.Descricao,
        Status = tarefa.Status,
        Prioridade = tarefa.Prioridade,
        DataCriacao = tarefa.DataCriacao,
        DataConclusao = tarefa.DataConclusao
    };
}
