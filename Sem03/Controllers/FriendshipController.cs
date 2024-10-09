using Microsoft.AspNetCore.Mvc;
using Sem03.Entities;
using Sem03.Services;
using Sem03.DTOs;

namespace Sem03.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FriendshipController : ControllerBase
    {
        private readonly FriendshipService _friendshipService;

        // Khởi tạo FriendshipService thông qua Dependency Injection
        public FriendshipController(FriendshipService friendshipService)
        {
            _friendshipService = friendshipService;
        }

        // POST: api/Friendship/request
        // Gửi yêu cầu kết bạn
        [HttpPost("request")]
        public async Task<IActionResult> SendFriendRequest([FromBody] FriendshipDto friendshipDto)
        {
            try
            {
                await _friendshipService.SendFriendRequest(friendshipDto);
                return CreatedAtAction(nameof(GetFriendshipStatus), new { userId1 = friendshipDto.RequesterId, userId2 = friendshipDto.AccepterId }, "Yêu cầu kết bạn đã được gửi.");
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // PUT: api/Friendship/respond
        // Phản hồi yêu cầu kết bạn
        [HttpPut("respond")]
        public async Task<IActionResult> RespondToFriendRequest([FromBody] FriendshipDto friendshipDto)
        {
            try
            {
                var result = await _friendshipService.RespondToFriendRequest(friendshipDto);
                return Ok(new { message = $"Yêu cầu kết bạn {result.Status.ToString().ToLower()}." });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        // GET: api/Friendship/status/1/2
        // Lấy trạng thái kết bạn giữa hai người dùng
        [HttpGet("status/{userId1}/{userId2}")]
        public async Task<IActionResult> GetFriendshipStatus(int userId1, int userId2)
        {
            try
            {
                var result = await _friendshipService.GetFriendshipStatus(userId1, userId2);
                return Ok(result);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        // GET: api/Friendship/1 (Tất cả bạn bè của một người dùng)
        [HttpGet("{userId}")]
        public async Task<IActionResult> GetFriends(int userId)
        {
            try
            {
                var friends = await _friendshipService.GetFriends(userId);
                return Ok(friends);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = ex.Message });
            }
        }

        // DELETE: api/Friendship/cancel
        // Hủy yêu cầu kết bạn
        [HttpDelete("cancel")]
        public async Task<IActionResult> CancelFriendRequest([FromBody] FriendshipDto friendshipDto)
        {
            try
            {
                await _friendshipService.CancelFriendRequest(friendshipDto);
                return Ok(new { message = "Yêu cầu kết bạn đã được hủy." });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // DELETE: api/Friendship/remove
        // Xóa mối quan hệ kết bạn
        [HttpDelete("remove")]
        public async Task<IActionResult> RemoveFriendship([FromBody] FriendshipDto friendshipDto)
        {
            try
            {
                await _friendshipService.RemoveFriendship(friendshipDto);
                return Ok(new { message = "Mối quan hệ kết bạn đã được xóa." });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // GET: api/Friendship/mutual/1/2 (Bạn chung)
        [HttpGet("mutual/{userId1}/{userId2}")]
        public async Task<IActionResult> GetMutualFriends(int userId1, int userId2)
        {
            try
            {
                var mutualFriends = await _friendshipService.GetMutualFriends(userId1, userId2);
                return Ok(mutualFriends);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = ex.Message });
            }
        }

        // GET: api/Friendship/search
        // Tìm kiếm các yêu cầu kết bạn
        [HttpGet("search")]
        public async Task<IActionResult> SearchFriendships(
            [FromQuery] int? requesterId,
            [FromQuery] int? accepterId,
            [FromQuery] FriendshipStatus? status,
            [FromQuery] DateTime? createdAfter,
            [FromQuery] DateTime? updatedAfter)
        {
            try
            {
                var friendships = await _friendshipService.SearchFriendships(
                    requesterId, accepterId, status, createdAfter, updatedAfter);
                return Ok(friendships);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = ex.Message });
            }
        }

        // GET: api/Friendship/filter
        // Lọc danh sách yêu cầu kết bạn
        [HttpGet("filter")]
        public async Task<IActionResult> FilterFriendships(
            [FromQuery] int? requesterId,
            [FromQuery] int? accepterId,
            [FromQuery] FriendshipStatus? status,
            [FromQuery] DateTime? createdAfter,
            [FromQuery] DateTime? updatedAfter)
        {
            try
            {
                var friendships = await _friendshipService.FilterFriendships(
                    requesterId, accepterId, status, createdAfter, updatedAfter);
                return Ok(friendships);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = ex.Message });
            }
        }
    }
}
