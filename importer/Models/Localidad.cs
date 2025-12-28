namespace Ageeml.Importer.Models;

public sealed class Localidad
{
    public int Id { get; set; }
    public int MunicipioId { get; set; }
    public string Clave { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public int Mapa { get; set; }
    public string Ambito { get; set; } = string.Empty;
    public string Latitud { get; set; } = string.Empty;
    public string Longitud { get; set; } = string.Empty;
    public double Lat { get; set; }
    public double Lng { get; set; }
    public string Altitud { get; set; } = string.Empty;
    public string Carta { get; set; } = string.Empty;
    public int Poblacion { get; set; }
    public int Masculino { get; set; }
    public int Femenino { get; set; }
    public int Viviendas { get; set; }
    public bool Activo { get; set; }
}
