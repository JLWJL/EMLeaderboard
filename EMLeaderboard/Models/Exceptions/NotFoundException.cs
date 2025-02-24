namespace EMLeaderboard.Models.Exceptions;

public abstract class NotFoundException(string entityName, object identifier)
    : Exception($"{entityName}: {identifier} not found.")
{
    public string EntityName { get; } = entityName;
    public object Identifier { get; } = identifier;
}