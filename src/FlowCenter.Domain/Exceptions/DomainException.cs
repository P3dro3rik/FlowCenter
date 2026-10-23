namespace FlowCenter.Domain.Exceptions;

/// <summary>
/// Representa uma violação de regra de negócio no domínio do FlowCenter.
/// Lançada quando dados inválidos ou transições de estado proibidas são detectadas.
/// </summary>
public class DomainException : Exception
{
    public DomainException(string message) : base(message)
    {
    }

    public DomainException(string message, Exception innerException) : base(message, innerException)
    {
    }
}
