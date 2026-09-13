using FluentAssertions;
using Xunit;

namespace FlowCenter.Tests.Domain.Entities;

public class SampleDomainTest
{
    [Fact]
    public void Teste_De_Entidade_Deve_Passar()
    {
        // Arrange
        var status = true;

        // Act & Assert
        status.Should().BeTrue();
    }
}