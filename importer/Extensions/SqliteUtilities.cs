using Microsoft.Data.Sqlite;

namespace Ageeml.Importer.Extensions;

public static class SqliteUtilities
{
    public static void InsertSqlite<T>(SqliteConnection connection, SqliteTransaction transaction, string sql, IEnumerable<T> items, Action<SqliteCommand, T> binder)
    {
        using var cmd = connection.CreateCommand();
        cmd.CommandText = sql;
        cmd.Transaction = transaction;
        foreach (var item in items)
        {
            cmd.Parameters.Clear();
            binder(cmd, item);
            cmd.ExecuteNonQuery();
        }
    }

    public static void ExecuteNonQuery(SqliteConnection connection, SqliteTransaction? transaction, string sql)
    {
        using var cmd = connection.CreateCommand();
        cmd.CommandText = sql;
        cmd.Transaction = transaction;
        cmd.ExecuteNonQuery();
    }
}
