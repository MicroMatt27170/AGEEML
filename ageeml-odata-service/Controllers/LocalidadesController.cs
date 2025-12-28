using Ageeml.Service.Data;
using Ageeml.Service.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.AspNetCore.OData.Results;

namespace Ageeml.Service.Controllers;

[Route("api/v1")]
[ApiController]
public sealed class LocalidadesController(ApplicationDbContext db) : ODataController
{
    [EnableQuery(PageSize = 500)]
    [HttpGet("Localidades")]
    public IQueryable<Localidad> Get()
    {
        return db.Localidades;
    }

    [EnableQuery]
    [HttpGet("Localidades({key})")]
    public SingleResult<Localidad> Get([FromRoute] int key)
    {
        return SingleResult.Create(db.Localidades.Where(localidad => localidad.Id == key));
    }
}
