using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ageeml.Service.Models;

public sealed class Estado
{
    [Column("id")]
    public int Id { get; set; }
    [Column("clave")]
    public string Clave { get; set; } = string.Empty;
    [Column("nombre")]
    public string Nombre { get; set; } = string.Empty;
    [Column("abrev")]
    public string Abrev { get; set; } = string.Empty;
    [Column("activo")]
    public bool Activo { get; set; }

    public ICollection<Municipio> Municipios { get; set; } = new List<Municipio>();
}
