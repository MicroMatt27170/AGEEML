using Ageeml.Importer.Extensions;
using Ageeml.Importer.Models;
using Ageeml.Importer.Options;
using Ageeml.Importer.Services;
using Ageeml.Importer;
using Microsoft.Extensions.Configuration;

var projectDir = PathUtilities.FindProjectDirectory();
var configuration = new ConfigurationBuilder()
    .SetBasePath(projectDir)
    .AddJsonFile("appsettings.json", optional: true)
    .AddUserSecrets<Program>(optional: true)
    .AddEnvironmentVariables()
    .AddCommandLine(args)
    .Build();

var downloadOptions = configuration.GetSection("Download").Get<DownloadOptions>() ?? new DownloadOptions();
var outputOptions = configuration.GetSection("Output").Get<OutputOptions>() ?? new OutputOptions();
var sqlOptions = configuration.GetSection("Sql").Get<SqlOptions>() ?? new SqlOptions();

var importer = new AgeemlImporter(projectDir, downloadOptions, outputOptions, sqlOptions);
await importer.RunAsync();
