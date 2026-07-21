using SalesAI.Domain.Exceptions;

namespace SalesAI.Domain.ValueObjects;

public record Email
{
    public string Value { get; }

    public Email(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new DomainException("Email cannot be empty.");

        if (!value.Contains('@') || !value.Contains('.'))
            throw new DomainException($"'{value}' is not a valid email address.");

        Value = value.Trim().ToLowerInvariant();
    }

    public static implicit operator string(Email email) => email.Value;
    public override string ToString() => Value;
}

public record PhoneNumber
{
    public string Value { get; }

    public PhoneNumber(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new DomainException("Phone number cannot be empty.");

        Value = value.Trim();
    }

    public static implicit operator string(PhoneNumber phone) => phone.Value;
    public override string ToString() => Value;
}

public record Money
{
    public decimal Amount { get; }
    public string Currency { get; }

    public Money(decimal amount, string currency = "USD")
    {
        if (amount < 0)
            throw new DomainException("Money amount cannot be negative.");

        Amount = amount;
        Currency = currency.ToUpperInvariant();
    }

    public static Money Zero(string currency = "USD") => new(0, currency);
}

public record LeadScore
{
    public Enums.LeadScoreCategory Category { get; }
    public int NumericScore { get; }
    public string Reasoning { get; }

    public LeadScore(Enums.LeadScoreCategory category, int numericScore, string reasoning)
    {
        if (numericScore < 0 || numericScore > 100)
            throw new DomainException("Lead score must be between 0 and 100.");

        Category = category;
        NumericScore = numericScore;
        Reasoning = reasoning;
    }
}
