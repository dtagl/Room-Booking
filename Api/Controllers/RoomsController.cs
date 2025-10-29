using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Application.Interfaces;
using Domain.Entities;

[ApiController]
[Route("companies/{companyId}/rooms")]
public class RoomsController : ControllerBase
{
    private readonly IUnitOfWork _uow;
    public RoomsController(IUnitOfWork uow) { _uow = uow; }

    [HttpGet]
    public IActionResult GetRooms(Guid companyId)
    {
        var rooms = _uow.Rooms.Query().Where(r => r.CompanyId == companyId).Select(r => new { r.Id, r.Name, r.Capacity, r.Description }).ToList();
        return Ok(rooms);
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> CreateRoom(Guid companyId, [FromBody] RoomCreateModel model)
    {
        // check role from claims
        var roleClaim = User.Claims.FirstOrDefault(c => c.Type == System.Security.Claims.ClaimTypes.Role)?.Value;
        if (roleClaim != "Admin") return Forbid();
        var room = new Room{ Id = Guid.NewGuid(), CompanyId = companyId, Name = model.Name, Capacity = model.Capacity, Description = model.Description };
        await _uow.Rooms.AddAsync(room);
        await _uow.SaveChangesAsync();
        return Ok(new { room.Id });
    }

    public class RoomCreateModel { public string Name { get; set; } = default!; public int Capacity { get; set; } = 1; public string? Description { get; set; } }
}