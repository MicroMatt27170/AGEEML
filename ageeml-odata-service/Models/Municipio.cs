using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ageeml.Service.Models;

public sealed class Municipio
{
    [Column("id")]
    public int Id { get; set; }
    [Column("estado_id")]
    public int EstadoId { get; set; }
    [Column("clave")]
    public string Clave { get; set; } = string.Empty;
    [Column("nombre")]
    public string Nombre { get; set; } = string.Empty;
    [Column("activo")]
    public bool Activo { get; set; }

    public Estado Estado { get; set; } = null!;
    public ICollection<Localidad> Localidades { get; set; } = new List<Localidad>();
}
