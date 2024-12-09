using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MultitenancyExample.EntityModels.Shared;
using MultitenancyExample.Helpers;
using MultitenancyExample.ViewModels;

namespace MultitenancyExample.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
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

    [HttpPost]
    public async Task<IActionResult> Post(DocumentPostViewModel documentPostViewModel)
    {
        var sharedContext = await _sharedContextAccessor.GetSharedContextAsync();
        var document = new Document
        {
            Title = documentPostViewModel.Title,
            Content = documentPostViewModel.Content
        };
        sharedContext.Documents.Add(document);
        await sharedContext.SaveChangesAsync();
        return Ok(document);
    }
}