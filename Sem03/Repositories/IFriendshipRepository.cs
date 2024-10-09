using Sem03.Entities;

namespace Sem03.Repositories
{
    public interface IFriendshipRepository
    {
        // Lấy mối quan hệ kết bạn theo ID
        Task<Friendship> GetByIdAsync(int id);

        // Lấy tất cả các mối quan hệ kết bạn
        Task<IEnumerable<Friendship>> GetAllAsync();

        // Thêm một yêu cầu kết bạn mới
        Task AddAsync(Friendship friendship);

        // Cập nhật thông tin mối quan hệ kết bạn
        Task UpdateAsync(Friendship friendship);

        // Xóa một mối quan hệ kết bạn
        Task DeleteAsync(Friendship friendship);

        // Lấy danh sách mối quan hệ kết bạn của người dùng theo ID
        Task<IEnumerable<Friendship>> GetFriendshipsByUserIdAsync(int userId);

        // Lấy mối quan hệ kết bạn giữa hai người dùng
        Task<Friendship> GetFriendshipBetweenUsersAsync(int userId1, int userId2);

        // Tìm kiếm các yêu cầu kết bạn dựa trên các tiêu chí khác nhau
        Task<IEnumerable<Friendship>> SearchAsync(
            int? requesterId = null,
            int? accepterId = null,
            FriendshipStatus? status = null,
            DateTime? createdAfter = null,
            DateTime? updatedAfter = null);

        // Lọc danh sách yêu cầu kết bạn dựa trên các tiêu chí cụ thể
        Task<IEnumerable<Friendship>> FilterAsync(
            int? requesterId = null,
            int? accepterId = null,
            FriendshipStatus? status = null,
            DateTime? createdAfter = null,
            DateTime? updatedAfter = null);
    }
}
