using System.ComponentModel.DataAnnotations;
using FlowCenter.Domain.Enums;

namespace FlowCenter.Application.DTOs;

/// <summary>
/// Dados de entrada para o cadastro de uma nova tarefa. (RF01, RF12)
/// </summary>
public class CriarTarefaRequest
{
    /// <summary>Título da tarefa. Obrigatório, entre 3 e 100 caracteres. (RB01, RB02)</summary>
    [Required(ErrorMessage = "O título da tarefa é obrigatório.")]
    [StringLength(100, MinimumLength = 3, ErrorMessage = "O título deve possuir entre 3 e 100 caracteres.")]
    public string Titulo { get; set; } = string.Empty;

    /// <summary>Descrição opcional da tarefa. (RB03)</summary>
    public string? Descricao { get; set; }

    /// <summary>Prioridade da tarefa: Baixa, Media ou Alta. (RB04)</summary>
    public PrioridadeTarefa Prioridade { get; set; }
}
