using Microsoft.AspNetCore.Mvc;
using chatApp.Server.Domain.Models;
using chatApp.Server.Domain.Interfaces.Bases;
using chatApp.Server.Domain.Interfaces.UoW;

namespace chatApp.Server.Presentation.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoomController : ControllerBase
    {
        private readonly IUnitOfWork _uow;

        public RoomController(IUnitOfWork unityOfWork)
        {
            _uow = unityOfWork;
        }

        [HttpGet]
        public async Task<IActionResult> GetRooms()
        {
            var rooms = await _uow.Rooms.GetAllAsync();

            var roomsData = rooms.Select(room => new
            {
                roomId = room.RoomId,
                roomName = room.Name,
                createdAt = room.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss")
            });

            return Ok(roomsData);
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateRoom([FromBody] RoomData rd)
        {
            var room = new Room
            {
                Name = rd.RoomName,
                Password = rd.Password
            };

            await _uow.Rooms.AddAsync(room);
            await _uow.SaveChangesAsync();

            return Ok();
        }

        [HttpPost("Validate")]
        public async Task<IActionResult> ValidateRoom([FromBody] RoomData rd)
        {
            var room = await _uow.Rooms.GetRoomByNameAsync(rd.RoomName);
            if (room == null)
            {
                return NotFound();
            }

            if (!string.IsNullOrEmpty(room.Password) && room.Password != rd.Password)
            {
                return Unauthorized();
            }

            return Ok();
        }

        [HttpPost("PublicRoom")]
        public async Task<IActionResult> PublicRoom([FromBody] RoomData rd)
        {
            var room = await _uow.Rooms.GetRoomByNameAsync(rd.RoomName);
            if (room == null)
            {
                return NotFound(new { message = "Sala não encontrada", isPublic = false });
            }

            if (string.IsNullOrEmpty(room.Password))
            {
                return Ok();
            }

            return Unauthorized();
        }

        public class RoomData
        {
            public string RoomName { get; set; } = "";
            public string? Password { get; set; }
        }
    }
}
