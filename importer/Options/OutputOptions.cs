namespace Ageeml.Importer.Options;

public sealed class OutputOptions
{
    public string RootPath { get; set; } = string.Empty;
    public string WorkDir { get; set; } = ".ageeml-files";
    public string MysqlSqlPath { get; set; } = "mysql/ageeml.sql";
    public string PostgreSqlSqlPath { get; set; } = "postgresql/ageeml.sql";
    public string SqliteSqlPath { get; set; } = "sqlite/ageeml.sql";
    public string SqliteDbPath { get; set; } = "sqlite/ageeml.db";
}
