using System.IO.Compression;

namespace Ageeml.Importer.Extensions;

public static class FileUtilities
{
    public static async Task GzipAsync(string sourcePath, string destinationPath)
    {
        await using var sourceStream = File.OpenRead(sourcePath);
        await using var destinationStream = File.Create(destinationPath);
        await using var gzipStream = new GZipStream(destinationStream, CompressionLevel.Optimal);
        await sourceStream.CopyToAsync(gzipStream);
    }

    public static bool LooksLikeZip(string path)
    {
        if (!File.Exists(path))
        {
            return false;
        }

        Span<byte> signature = stackalloc byte[4];
        using var fs = File.OpenRead(path);
        if (fs.Length < 4)
        {
            return false;
        }

        fs.ReadExactly(signature);
        return signature[0] == 'P' && signature[1] == 'K' && signature[2] == 3 && signature[3] == 4;
    }

    public static async Task WriteInsertBatches<T>(TextWriter writer, string tableName, string columns, IEnumerable<T> items, Func<T, string> rowFactory, int batchSize = 1000)
    {
        var buffer = new List<string>(batchSize);
        foreach (var item in items)
        {
            buffer.Add(rowFactory(item));
            if (buffer.Count >= batchSize)
            {
                await FlushBatch(writer, tableName, columns, buffer);
            }
        }

        if (buffer.Count > 0)
        {
            await FlushBatch(writer, tableName, columns, buffer);
        }
    }

    private static async Task FlushBatch(TextWriter writer, string tableName, string columns, List<string> buffer)
    {
        await writer.WriteLineAsync($"INSERT INTO {tableName} ({columns}) VALUES");
        await writer.WriteLineAsync(string.Join(",\n", buffer) + ";");
        buffer.Clear();
    }
}
