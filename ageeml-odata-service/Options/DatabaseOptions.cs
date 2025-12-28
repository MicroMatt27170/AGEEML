namespace Ageeml.Service.Options;

public sealed record DatabaseOptions
{
    public const string SectionName = "Database";

    private string _provider = "sqlite";
    
    public string Provider 
    { 
        get => _provider;
        init => _provider = value.ToLowerInvariant().Trim();
    }
    public string ConnectionString { get; init; } = "Data Source=sqlite/ageeml.db";
}
