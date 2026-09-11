using FlowCenter.Domain.Entities;
using FlowCenter.Domain.Enums;
using FlowCenter.Domain.Exceptions;
using FluentAssertions;
using Xunit;

namespace FlowCenter.Tests.Domain.Entities;

/// <summary>
/// Testes unitários da entidade Tarefa.
/// Cobre as regras de negócio definidas no SDD: RB01–RB12.
/// </summary>
public class TarefaTests
{
    // ── Criação ──────────────────────────────────────────────────────────────────

    [Fact]
    public void Criar_ComTituloValido_DeveCriarTarefaComSucesso()
    {
        // Arrange & Act
        var tarefa = new Tarefa("Implementar API", PrioridadeTarefa.Alta);

        // Assert
        tarefa.Titulo.Should().Be("Implementar API");
        tarefa.Prioridade.Should().Be(PrioridadeTarefa.Alta);
        tarefa.Id.Should().NotBe(Guid.Empty);         // RB11
        tarefa.DataCriacao.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5)); // RF09
    }

    [Fact]
    public void Criar_DeveTerStatusPendente_AoCriar() // RB05
    {
        var tarefa = new Tarefa("Nova tarefa", PrioridadeTarefa.Baixa);

        tarefa.Status.Should().Be(StatusTarefa.Pendente);
    }

    [Fact]
    public void Criar_NaoDeveTerDataConclusao_QuandoNovaTarefa() // RB08
    {
        var tarefa = new Tarefa("Nova tarefa", PrioridadeTarefa.Baixa);

        tarefa.DataConclusao.Should().BeNull();
    }

    [Fact]
    public void Criar_ComDescricaoOpcional_DevePermitirDescricaoNula() // RB03
    {
        var tarefa = new Tarefa("Tarefa sem descrição", PrioridadeTarefa.Media);

        tarefa.Descricao.Should().BeNull();
    }

    [Fact]
    public void Criar_ComDescricao_DevePreencherDescricao() // RB03
    {
        var tarefa = new Tarefa("Tarefa com descrição", PrioridadeTarefa.Media, "Detalhes da tarefa");

        tarefa.Descricao.Should().Be("Detalhes da tarefa");
    }

    // ── Validação de Título (RB01, RB02) ────────────────────────────────────────

    [Fact]
    public void Criar_ComTituloVazio_DeveLancarDomainException() // RB01
    {
        Action acao = () => new Tarefa("", PrioridadeTarefa.Baixa);

        acao.Should().Throw<DomainException>()
            .WithMessage("*título*obrigatório*");
    }

    [Fact]
    public void Criar_ComTituloNulo_DeveLancarDomainException() // RB01
    {
        Action acao = () => new Tarefa(null!, PrioridadeTarefa.Baixa);

        acao.Should().Throw<DomainException>()
            .WithMessage("*título*obrigatório*");
    }

    [Fact]
    public void Criar_ComTituloApenasEspacos_DeveLancarDomainException() // RB01
    {
        Action acao = () => new Tarefa("   ", PrioridadeTarefa.Baixa);

        acao.Should().Throw<DomainException>()
            .WithMessage("*título*obrigatório*");
    }

    [Theory]
    [InlineData("ab")]    // 2 caracteres — abaixo do mínimo
    [InlineData("a")]     // 1 caractere
    public void Criar_ComTituloMenorQue3Caracteres_DeveLancarDomainException(string titulo) // RB02
    {
        Action acao = () => new Tarefa(titulo, PrioridadeTarefa.Baixa);

        acao.Should().Throw<DomainException>()
            .WithMessage("*mínimo 3*");
    }

    [Fact]
    public void Criar_ComTituloMaiorQue100Caracteres_DeveLancarDomainException() // RB02
    {
        var tituloLongo = new string('x', 101);
        Action acao = () => new Tarefa(tituloLongo, PrioridadeTarefa.Baixa);

        acao.Should().Throw<DomainException>()
            .WithMessage("*máximo 100*");
    }

    [Theory]
    [InlineData("abc")]                            // mínimo exato (3 chars)
    [InlineData("Implementar endpoints da API")]   // normal
    public void Criar_ComTituloNosLimitesPermitidos_DeveSerValido(string titulo) // RB02
    {
        Action acao = () => new Tarefa(titulo, PrioridadeTarefa.Baixa);

        acao.Should().NotThrow();
    }

    [Fact]
    public void Criar_ComTituloDe100Caracteres_DeveSerValido() // RB02 — limite exato superior
    {
        var titulo = new string('a', 100);
        Action acao = () => new Tarefa(titulo, PrioridadeTarefa.Alta);

        acao.Should().NotThrow();
    }

    // ── Conclusão (RB07, RB08, RB10) ────────────────────────────────────────────

    [Fact]
    public void Concluir_DeveAlterarStatusParaConcluida() // RB06
    {
        var tarefa = new Tarefa("Tarefa", PrioridadeTarefa.Alta);

        tarefa.Concluir();

        tarefa.Status.Should().Be(StatusTarefa.Concluida);
    }

    [Fact]
    public void Concluir_DevePreencherDataConclusao() // RB07
    {
        var tarefa = new Tarefa("Tarefa", PrioridadeTarefa.Alta);

        tarefa.Concluir();

        tarefa.DataConclusao.Should().NotBeNull();
        tarefa.DataConclusao.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Fact]
    public void Concluir_QuandoChamadaDuasVezes_NaoDeveLancarExcecao() // idempotência
    {
        var tarefa = new Tarefa("Tarefa", PrioridadeTarefa.Alta);
        tarefa.Concluir();

        Action acao = () => tarefa.Concluir();

        acao.Should().NotThrow();
    }

    // ── AlterarStatus (RB06, RB10) ───────────────────────────────────────────────

    [Fact]
    public void AlterarStatus_DePendenteParaEmAndamento_DevePermitir() // RB06
    {
        var tarefa = new Tarefa("Tarefa", PrioridadeTarefa.Media);

        tarefa.AlterarStatus(StatusTarefa.EmAndamento);

        tarefa.Status.Should().Be(StatusTarefa.EmAndamento);
    }

    [Fact]
    public void AlterarStatus_DeConcluida_ParaPendente_DeveLancarDomainException() // RB10
    {
        var tarefa = new Tarefa("Tarefa", PrioridadeTarefa.Alta);
        tarefa.Concluir();

        Action acao = () => tarefa.AlterarStatus(StatusTarefa.Pendente);

        acao.Should().Throw<DomainException>()
            .WithMessage("*já concluída*não pode retornar*Pendente*");
    }

    [Fact]
    public void AlterarStatus_DeConcluida_ParaEmAndamento_DevePermitir() // RB10 — só bloqueia Pendente
    {
        var tarefa = new Tarefa("Tarefa", PrioridadeTarefa.Baixa);
        tarefa.Concluir();

        tarefa.AlterarStatus(StatusTarefa.EmAndamento);

        tarefa.Status.Should().Be(StatusTarefa.EmAndamento);
    }

    // ── Atualizar (RB01, RB02) ───────────────────────────────────────────────────

    [Fact]
    public void Atualizar_ComDadosValidos_DeveAtualizarPropriedades()
    {
        var tarefa = new Tarefa("Título original", PrioridadeTarefa.Baixa, "Descrição original");

        tarefa.Atualizar("Título atualizado", PrioridadeTarefa.Alta, "Nova descrição");

        tarefa.Titulo.Should().Be("Título atualizado");
        tarefa.Prioridade.Should().Be(PrioridadeTarefa.Alta);
        tarefa.Descricao.Should().Be("Nova descrição");
    }

    [Fact]
    public void Atualizar_ComTituloInvalido_DeveLancarDomainException() // RB01
    {
        var tarefa = new Tarefa("Título original", PrioridadeTarefa.Baixa);

        Action acao = () => tarefa.Atualizar("", PrioridadeTarefa.Alta);

        acao.Should().Throw<DomainException>();
    }

    // ── Identificador Único (RB11) ───────────────────────────────────────────────

    [Fact]
    public void Criar_DuasTarefas_DevemTerIdsDistintos() // RB11
    {
        var tarefa1 = new Tarefa("Tarefa 1", PrioridadeTarefa.Baixa);
        var tarefa2 = new Tarefa("Tarefa 2", PrioridadeTarefa.Alta);

        tarefa1.Id.Should().NotBe(tarefa2.Id);
    }
}
