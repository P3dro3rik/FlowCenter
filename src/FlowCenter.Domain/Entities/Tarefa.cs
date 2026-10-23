using FlowCenter.Domain.Enums;
using FlowCenter.Domain.Exceptions;

namespace FlowCenter.Domain.Entities;

/// <summary>
/// Entidade central do domínio FlowCenter.
/// Encapsula os dados e as regras de negócio de uma tarefa.
/// </summary>
public class Tarefa
{
    // ── Propriedades ────────────────────────────────────────────────────────────

    /// <summary>Identificador único da tarefa. (RB11)</summary>
    public Guid Id { get; private set; }

    /// <summary>
    /// Título da tarefa. Obrigatório, entre 3 e 100 caracteres. (RB01, RB02)
    /// </summary>
    public string Titulo { get; private set; }

    /// <summary>Descrição opcional da tarefa. (RB03)</summary>
    public string? Descricao { get; private set; }

    /// <summary>
    /// Status atual da tarefa. Sempre inicia como Pendente. (RB05, RB06)
    /// </summary>
    public StatusTarefa Status { get; private set; }

    /// <summary>Prioridade da tarefa. (RB04)</summary>
    public PrioridadeTarefa Prioridade { get; private set; }

    /// <summary>Data e hora (UTC) em que a tarefa foi criada. (RF09)</summary>
    public DateTime DataCriacao { get; private set; }

    /// <summary>
    /// Data e hora (UTC) em que a tarefa foi concluída.
    /// Nula enquanto a tarefa não estiver concluída. (RB07, RB08)
    /// </summary>
    public DateTime? DataConclusao { get; private set; }

    // ── Construtor ───────────────────────────────────────────────────────────────

    /// <summary>
    /// Cria uma nova tarefa com status inicial Pendente. (RB05)
    /// </summary>
    /// <param name="titulo">Título obrigatório (3–100 caracteres).</param>
    /// <param name="prioridade">Prioridade da tarefa.</param>
    /// <param name="descricao">Descrição opcional.</param>
    public Tarefa(string titulo, PrioridadeTarefa prioridade, string? descricao = null)
    {
        Id = Guid.NewGuid();
        DataCriacao = DateTime.UtcNow;
        Status = StatusTarefa.Pendente; // RB05

        ValidarTitulo(titulo);
        Titulo = titulo;
        Descricao = descricao;
        Prioridade = prioridade;
    }

    /// <summary>
    /// Construtor privado para uso exclusivo do EF Core (hidratação do banco).
    /// O EF Core não utiliza o construtor público; este existe para garantir que
    /// as propriedades sejam preenchidas corretamente sem acionar validações.
    /// </summary>
    private Tarefa()
    {
        Titulo = string.Empty;
    }

    // ── Comportamentos / Regras de Negócio ───────────────────────────────────────

    /// <summary>
    /// Atualiza as informações editáveis da tarefa.
    /// Aplica as mesmas validações do construtor. (RB01, RB02)
    /// </summary>
    /// <param name="titulo">Novo título (3–100 caracteres).</param>
    /// <param name="prioridade">Nova prioridade.</param>
    /// <param name="descricao">Nova descrição (opcional).</param>
    public void Atualizar(string titulo, PrioridadeTarefa prioridade, string? descricao = null)
    {
        ValidarTitulo(titulo);
        Titulo = titulo;
        Descricao = descricao;
        Prioridade = prioridade;
    }

    /// <summary>
    /// Altera o status da tarefa.
    /// Uma tarefa Concluída não pode retornar ao status Pendente. (RB10)
    /// </summary>
    /// <param name="novoStatus">Status desejado.</param>
    public void AlterarStatus(StatusTarefa novoStatus)
    {
        // RB10: tarefa concluída não pode voltar para Pendente
        if (Status == StatusTarefa.Concluida && novoStatus == StatusTarefa.Pendente)
        {
            throw new DomainException(
                "Uma tarefa já concluída não pode retornar ao status Pendente.");
        }

        Status = novoStatus;
    }

    /// <summary>
    /// Conclui a tarefa, registrando a data de conclusão. (RB07, RB10)
    /// Se já estiver concluída, nenhuma ação é realizada.
    /// </summary>
    public void Concluir()
    {
        if (Status == StatusTarefa.Concluida)
            return;

        Status = StatusTarefa.Concluida;
        DataConclusao = DateTime.UtcNow; // RB07
    }

    // ── Validações Privadas ──────────────────────────────────────────────────────

    /// <summary>
    /// Valida o título conforme as regras RB01 e RB02.
    /// </summary>
    private static void ValidarTitulo(string titulo)
    {
        // RB01: o título é obrigatório
        if (string.IsNullOrWhiteSpace(titulo))
            throw new DomainException("O título da tarefa é obrigatório.");

        // RB02: entre 3 e 100 caracteres
        if (titulo.Trim().Length < 3)
            throw new DomainException("O título deve possuir no mínimo 3 caracteres.");

        if (titulo.Trim().Length > 100)
            throw new DomainException("O título deve possuir no máximo 100 caracteres.");
    }
}
