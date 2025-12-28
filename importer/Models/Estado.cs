namespace Ageeml.Importer.Models;

public sealed class Estado
{
    public int Id { get; set; }
    public string Clave { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public string Abrev { get; set; } = string.Empty;
    public bool Activo { get; set; }
}
