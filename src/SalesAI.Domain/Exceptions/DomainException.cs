namespace SalesAI.Domain.Exceptions;

public class DomainException : Exception
{
    public DomainException(string message) : base(message) { }
    public DomainException(string message, Exception innerException) : base(message, innerException) { }
}

public class BusinessRuleViolationException : DomainException
{
    public string Rule { get; }

    public BusinessRuleViolationException(string rule, string message) : base(message)
    {
        Rule = rule;
    }
}
