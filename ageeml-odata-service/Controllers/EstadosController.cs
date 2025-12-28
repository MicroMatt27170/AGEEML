using Ageeml.Service.Data;
using Ageeml.Service.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.AspNetCore.OData.Results;

namespace Ageeml.Service.Controllers;

[Route("api/v1")]
[ApiController]
public sealed class EstadosController(ApplicationDbContext db) : ODataController
{
    [EnableQuery(PageSize = 500)]
    [HttpGet("Estados")]
    public IQueryable<Estado> Get()
    {
        return db.Estados;
    }

    [EnableQuery]
    [HttpGet("Estados({key})")]
    public SingleResult<Estado> Get([FromRoute] int key)
    {
        return SingleResult.Create(db.Estados.Where(estado => estado.Id == key));
    }
}
