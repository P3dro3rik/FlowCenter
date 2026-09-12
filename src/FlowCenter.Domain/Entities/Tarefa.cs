namespace FlowCenter.Domain.Entities;

public enum PrioridadeTarefa { Baixa, Media, Alta }
public enum StatusTarefa { Pendente, EmAndamento, Concluida }

public class Tarefa
{
    public int Id { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public string? Descricao { get; set; }
    public PrioridadeTarefa Prioridade { get; set; }
    public StatusTarefa Status { get; set; } = StatusTarefa.Pendente;
    public DateTime DataCriacao { get; set; } = DateTime.UtcNow;
    public DateTime? DataConclusao { get; set; }
}