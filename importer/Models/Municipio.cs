namespace Ageeml.Importer.Models;

public sealed class Municipio
{
    public int Id { get; set; }
    public int EstadoId { get; set; }
    public string Clave { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public bool Activo { get; set; }
}
