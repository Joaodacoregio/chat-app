
using chatApp.Server.Data;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using chatApp.Server.Models;
using Newtonsoft.Json;

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


    }
        
        
}