using Ageeml.Service.Models;
using Microsoft.EntityFrameworkCore;

namespace Ageeml.Service.Data;

public sealed class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    public DbSet<Estado> Estados { get; set; }
    public DbSet<Municipio> Municipios { get; set; }
    public DbSet<Localidad> Localidades { get; set; }
}
