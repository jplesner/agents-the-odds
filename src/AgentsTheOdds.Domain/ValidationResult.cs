namespace AgentsTheOdds.Domain;

public sealed class ValidationResult
{
    public bool IsValid { get; private init; }
    public string? Error { get; private init; }

    public static ValidationResult Ok() => new() { IsValid = true };
    public static ValidationResult Fail(string error) => new() { IsValid = false, Error = error };
}
