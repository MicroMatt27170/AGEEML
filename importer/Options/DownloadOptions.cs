namespace Ageeml.Importer.Options;

public sealed class DownloadOptions
{
    public string EstadosUrl { get; set; } = "https://www.inegi.org.mx/contenidos/app/ageeml/catun_entidad.zip";
    public string MunicipiosUrl { get; set; } = "https://www.inegi.org.mx/contenidos/app/ageeml/catun_municipio.zip";
    public string LocalidadesUrl { get; set; } = "https://www.inegi.org.mx/contenidos/app/ageeml/min_con_acento.zip";
}
