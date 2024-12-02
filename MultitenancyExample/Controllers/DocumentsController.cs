using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MultitenancyExample.Helpers;

namespace MultitenancyExample.Controllers;

public class DocumentsController : ControllerBase
{
    private readonly ISharedContextAccessor _sharedContextAccessor;

    public DocumentsController(ISharedContextAccessor sharedContextAccessor)
    {
        _sharedContextAccessor = sharedContextAccessor;
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var sharedContext = await _sharedContextAccessor.GetSharedContextAsync();
        var documents = await sharedContext.Documents.ToListAsync();
        return Ok(documents);
    }
}