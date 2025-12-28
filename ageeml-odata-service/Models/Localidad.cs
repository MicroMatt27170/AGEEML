using System.ComponentModel.DataAnnotations.Schema;

namespace Ageeml.Service.Models;

public sealed class Localidad
{
    [Column("id")]
    public int Id { get; set; }
    [Column("municipio_id")]
    public int MunicipioId { get; set; }
    [Column("clave")]
    public string Clave { get; set; } = string.Empty;
    [Column("nombre")]
    public string Nombre { get; set; } = string.Empty;
    [Column("mapa")]
    public int Mapa { get; set; }
    [Column("ambito")]
    public string Ambito { get; set; } = string.Empty;
    [Column("latitud")]
    public string Latitud { get; set; } = string.Empty;
    [Column("longitud")]
    public string Longitud { get; set; } = string.Empty;
    [Column("lat")]
    public double Lat { get; set; }
    [Column("lng")]
    public double Lng { get; set; }
    [Column("altitud")]
    public string Altitud { get; set; } = string.Empty;
    [Column("carta")]
    public string Carta { get; set; } = string.Empty;
    [Column("poblacion")]
    public int Poblacion { get; set; }
    [Column("masculino")]
    public int Masculino { get; set; }
    [Column("femenino")]
    public int Femenino { get; set; }
    [Column("viviendas")]
    public int Viviendas { get; set; }
    [Column("activo")]
    public bool Activo { get; set; }

    public Municipio Municipio { get; set; } = null!;
}
