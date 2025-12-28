using Ageeml.Service.Data;
using Ageeml.Service.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.AspNetCore.OData.Results;

namespace Ageeml.Service.Controllers;

[Route("api/v1")]
[ApiController]
public sealed class MunicipiosController(ApplicationDbContext db) : ODataController
{
    [EnableQuery(PageSize = 500)]
    [HttpGet("Municipios")]
    public IQueryable<Municipio> Get()
    {
        return db.Municipios;
    }

    [EnableQuery]
    [HttpGet("Municipios({key})")]
    public SingleResult<Municipio> Get([FromRoute] int key)
    {
        return SingleResult.Create(db.Municipios.Where(municipio => municipio.Id == key));
    }
}
