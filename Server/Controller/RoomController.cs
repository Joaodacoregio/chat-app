
using chatApp.Server.Data;
using Microsoft.AspNetCore.Mvc;
using chatApp.Server.Models;
 

namespace chatApp.Server.Controllers
{
    //Classe para gerenciar as salas
    [Route("api/[controller]")]
    [ApiController]
    public class RoomController : ControllerBase
    {
        //Ja é registrado automaticamente
        private readonly IAppDbContext _context;

        public RoomController(IAppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult GetRooms()
        {
            var rooms = _context.Rooms.ToList();

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
            _context.Rooms.Add(room);
            await _context.SaveChangesAsync();

            return Ok();
        }


        [HttpPost("Validate")]
        public async Task<IActionResult> ValidateRoom([FromBody] RoomData rd)
        {
            var room = _context.Rooms.FirstOrDefault(r => r.Name == rd.RoomName);
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
            var room = _context.Rooms.FirstOrDefault(r => r.Name == rd.RoomName);
            if (room == null)
            {
                return NotFound(new { message = "Sala não encontrada", isPublic = false });
            }

            bool isPublic = string.IsNullOrEmpty(room.Password);

            return Ok(new { isPublic = isPublic });
        }

        public class RoomData
        {
            public string RoomName { get; set; } = "";
            public string? Password { get; set; }  
        }

    }
        
        
}