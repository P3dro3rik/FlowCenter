using FlowCenter.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FlowCenter.Infrastructure.Persistence.Configurations;

/// <summary>
/// Mapeamento explícito da entidade Tarefa para o Entity Framework Core.
/// Necessário porque a entidade expõe apenas construtores/propriedades encapsulados. (RB01–RB11)
/// </summary>
public class TarefaConfiguration : IEntityTypeConfiguration<Tarefa>
{
    public void Configure(EntityTypeBuilder<Tarefa> builder)
    {
        builder.ToTable("tarefas");

        builder.HasKey(t => t.Id);

        builder.Property(t => t.Titulo)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(t => t.Descricao);

        builder.Property(t => t.Status)
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(t => t.Prioridade)
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(t => t.DataCriacao)
            .IsRequired();

        builder.Property(t => t.DataConclusao);
    }
}
