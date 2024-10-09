using Sem03.Entities;
using Sem03.Repositories;
using Sem03.DTOs;
namespace Sem03.Services
{
    public class FriendshipService
    {
        private readonly IFriendshipRepository _friendshipRepository;

        public FriendshipService(IFriendshipRepository friendshipRepository)
        {
            _friendshipRepository = friendshipRepository;
        }

        // Gửi yêu cầu kết bạn
        public async Task<FriendshipDto> SendFriendRequest(FriendshipDto friendshipDto)
        {
            // Kiểm tra nếu yêu cầu kết bạn đã tồn tại
            var existingFriendship = await _friendshipRepository.GetFriendshipBetweenUsersAsync(friendshipDto.RequesterId, friendshipDto.AccepterId);

            if (existingFriendship != null)
            {
                throw new InvalidOperationException("Yêu cầu kết bạn đã tồn tại.");
            }

            var friendship = new Friendship
            {
                RequesterId = friendshipDto.RequesterId,
                AccepterId = friendshipDto.AccepterId,
                Status = FriendshipStatus.Pending, // Trạng thái đang chờ
                CreatedAt = DateTime.UtcNow, // Thời gian tạo yêu cầu
                UpdatedAt = DateTime.UtcNow // Thời gian cập nhật
            };

            await _friendshipRepository.AddAsync(friendship); // Thêm yêu cầu kết bạn vào cơ sở dữ liệu

            // Không có Id, nên chỉ trả về các thuộc tính cần thiết
            return friendshipDto;
        }

        // Phản hồi yêu cầu kết bạn
        public async Task<FriendshipDto> RespondToFriendRequest(FriendshipDto friendshipDto)
        {
            var existingFriendship = await _friendshipRepository.GetFriendshipBetweenUsersAsync(friendshipDto.RequesterId, friendshipDto.AccepterId);

            if (existingFriendship == null)
            {
                throw new KeyNotFoundException("Không tìm thấy yêu cầu kết bạn.");
            }

            existingFriendship.Status = friendshipDto.Status; // Cập nhật trạng thái yêu cầu kết bạn
            existingFriendship.UpdatedAt = DateTime.UtcNow; // Cập nhật thời gian

            await _friendshipRepository.UpdateAsync(existingFriendship); // Lưu thay đổi

            // Chuyển đổi entity sang DTO
            return new FriendshipDto
            {
                RequesterId = existingFriendship.RequesterId,
                AccepterId = existingFriendship.AccepterId,
                Status = existingFriendship.Status,
                CreatedAt = existingFriendship.CreatedAt,
                UpdatedAt = existingFriendship.UpdatedAt
            };
        }

        // Lấy trạng thái kết bạn giữa hai người dùng
        public async Task<FriendshipDto> GetFriendshipStatus(int userId1, int userId2)
        {
            var friendship = await _friendshipRepository.GetFriendshipBetweenUsersAsync(userId1, userId2);

            if (friendship == null)
            {
                throw new KeyNotFoundException("Không tìm thấy mối quan hệ kết bạn giữa hai người dùng này.");
            }

            // Chuyển đổi entity sang DTO
            return new FriendshipDto
            {
                RequesterId = friendship.RequesterId,
                AccepterId = friendship.AccepterId,
                Status = friendship.Status,
                CreatedAt = friendship.CreatedAt,
                UpdatedAt = friendship.UpdatedAt
            };
        }

        // Lấy danh sách bạn bè của một người dùng
        public async Task<IEnumerable<FriendshipDto>> GetFriends(int userId)
        {
            var friendships = await _friendshipRepository.GetFriendshipsByUserIdAsync(userId);
            return friendships
                .Where(f => f.Status == FriendshipStatus.Accepted)
                .Select(f => new FriendshipDto
                {
                    RequesterId = f.RequesterId,
                    AccepterId = f.AccepterId,
                    Status = f.Status,
                    CreatedAt = f.CreatedAt,
                    UpdatedAt = f.UpdatedAt
                });
        }

        // Hủy yêu cầu kết bạn
        public async Task CancelFriendRequest(FriendshipDto friendshipDto)
        {
            var existingFriendship = await _friendshipRepository.GetFriendshipBetweenUsersAsync(friendshipDto.RequesterId, friendshipDto.AccepterId);

            if (existingFriendship == null || existingFriendship.Status != FriendshipStatus.Pending)
            {
                throw new InvalidOperationException("Không tìm thấy yêu cầu kết bạn hoặc yêu cầu đã được chấp nhận.");
            }

            await _friendshipRepository.DeleteAsync(existingFriendship); // Xóa yêu cầu kết bạn
        }

        // Xóa mối quan hệ kết bạn
        public async Task RemoveFriendship(FriendshipDto friendshipDto)
        {
            var existingFriendship = await _friendshipRepository.GetFriendshipBetweenUsersAsync(friendshipDto.RequesterId, friendshipDto.AccepterId);

            if (existingFriendship == null || existingFriendship.Status != FriendshipStatus.Accepted)
            {
                throw new InvalidOperationException("Không tìm thấy mối quan hệ kết bạn hoặc chưa được chấp nhận.");
            }

            await _friendshipRepository.DeleteAsync(existingFriendship); // Xóa mối quan hệ kết bạn
        }

        // Lấy danh sách bạn chung giữa hai người dùng
        public async Task<IEnumerable<int>> GetMutualFriends(int userId1, int userId2)
        {
            var friendsOfUser1 = await _friendshipRepository.GetFriendshipsByUserIdAsync(userId1);
            var friendsOfUser2 = await _friendshipRepository.GetFriendshipsByUserIdAsync(userId2);

            return friendsOfUser1.Where(f => f.Status == FriendshipStatus.Accepted)
                .Select(f => f.RequesterId == userId1 ? f.AccepterId : f.RequesterId)
                .Intersect(friendsOfUser2.Where(f => f.Status == FriendshipStatus.Accepted)
                .Select(f => f.RequesterId == userId2 ? f.AccepterId : f.RequesterId));
        }

        // Tìm kiếm các yêu cầu kết bạn dựa trên các tiêu chí khác nhau
        public async Task<IEnumerable<FriendshipDto>> SearchFriendships(
            int? requesterId = null,
            int? accepterId = null,
            FriendshipStatus? status = null,
            DateTime? createdAfter = null,
            DateTime? updatedAfter = null)
        {
            var friendships = await _friendshipRepository.SearchAsync(requesterId, accepterId, status, createdAfter, updatedAfter);
            return friendships.Select(f => new FriendshipDto
            {
                RequesterId = f.RequesterId,
                AccepterId = f.AccepterId,
                Status = f.Status,
                CreatedAt = f.CreatedAt,
                UpdatedAt = f.UpdatedAt
            });
        }

        // Lọc danh sách yêu cầu kết bạn dựa trên các tiêu chí
        public async Task<IEnumerable<FriendshipDto>> FilterFriendships(
            int? requesterId = null,
            int? accepterId = null,
            FriendshipStatus? status = null,
            DateTime? createdAfter = null,
            DateTime? updatedAfter = null)
        {
            var friendships = await _friendshipRepository.FilterAsync(requesterId, accepterId, status, createdAfter, updatedAfter);
            return friendships.Select(f => new FriendshipDto
            {
                RequesterId = f.RequesterId,
                AccepterId = f.AccepterId,
                Status = f.Status,
                CreatedAt = f.CreatedAt,
                UpdatedAt = f.UpdatedAt
            });
        }
    }
}

