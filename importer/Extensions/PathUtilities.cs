namespace Ageeml.Importer.Extensions;

public static class PathUtilities
{
    public static string FindProjectDirectory()
    {
        var dir = new DirectoryInfo(Environment.CurrentDirectory);
        while (dir != null)
        {
            var csprojPath = Path.Combine(dir.FullName, "importer", "importer.csproj");
            if (File.Exists(csprojPath))
            {
                return Path.Combine(dir.FullName, "importer");
            }

            csprojPath = Path.Combine(dir.FullName, "importer.csproj");
            if (File.Exists(csprojPath))
            {
                return dir.FullName;
            }

            dir = dir.Parent;
        }

        return Environment.CurrentDirectory;
    }

    public static string FindRoot(string startDirectory)
    {
        var dir = new DirectoryInfo(startDirectory);
        while (dir != null)
        {
            var hasTargets = Directory.Exists(Path.Combine(dir.FullName, "mysql")) &&
                             Directory.Exists(Path.Combine(dir.FullName, "postgresql")) &&
                             Directory.Exists(Path.Combine(dir.FullName, "sqlite"));
            if (hasTargets)
            {
                return dir.FullName;
            }

            dir = dir.Parent;
        }

        return startDirectory;
    }

    public static string ResolvePath(string baseDir, string path)
    {
        if (string.IsNullOrWhiteSpace(path))
        {
            return baseDir;
        }

        return Path.IsPathRooted(path) ? path : Path.Combine(baseDir, path);
    }
}
