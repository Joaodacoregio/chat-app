using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using chatApp.Server.Domain.Models;
using chatApp.Server.Domain.Interfaces.UoW;
using chatApp.Server.Application.DTOs;
using Microsoft.AspNetCore.Authorization;

//TODO: CRIAR PAGINAÇÃO 

namespace chatApp.Server.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoomController : ControllerBase
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;

        public RoomController(IUnitOfWork unityOfWork, IMapper mapper)
        {
            _uow = unityOfWork;
            _mapper = mapper;
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetRooms()
        {
            var rooms = await _uow.Rooms.GetAllAsync();

            var roomsData = rooms.Select(room => _mapper.Map<RoomDto>(room));  

            return Ok(roomsData);
        }

        [Authorize]
        [HttpPost("create")]
        public async Task<IActionResult> CreateRoom([FromBody] RoomValidationDto rd)
        {
            var room = _mapper.Map<Room>(rd);  

            await _uow.Rooms.AddAsync(room);
            await _uow.SaveChangesAsync();

            return Ok();
        }

        [Authorize]
        [HttpPost("Validate")]
        public async Task<IActionResult> ValidateRoom([FromBody] RoomValidationDto rd)
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

        [Authorize]
        [HttpPost("PublicRoom")]
        public async Task<IActionResult> PublicRoom([FromBody] RoomValidationDto rd)
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
    }
}
