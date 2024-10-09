using Microsoft.EntityFrameworkCore;
using Sem03.Config;
using Sem03.Entities;

namespace Sem03.Repositories
{
    public class FriendshipRepository : IFriendshipRepository
    {
        private readonly DataFriendShipContext _context;

        public FriendshipRepository(DataFriendShipContext context)
        {
            _context = context;
        }

        // Lấy mối quan hệ kết bạn theo ID
        public async Task<Friendship> GetByIdAsync(int id)
        {
            var friendship = await _context.Friendships.FindAsync(id);

            if (friendship == null)
            {
                throw new KeyNotFoundException($"Không tìm thấy mối quan hệ kết bạn với ID {id}.");
            }

            return friendship;
        }

        // Lấy tất cả các mối quan hệ kết bạn
        public async Task<IEnumerable<Friendship>> GetAllAsync()
        {
            return await _context.Friendships.ToListAsync();
        }

        // Thêm một yêu cầu kết bạn mới
        public async Task AddAsync(Friendship friendship)
        {
            await _context.Friendships.AddAsync(friendship);
            await _context.SaveChangesAsync();
        }

        // Cập nhật thông tin mối quan hệ kết bạn
        public async Task UpdateAsync(Friendship friendship)
        {
            _context.Friendships.Update(friendship);
            await _context.SaveChangesAsync();
        }

        // Xóa một mối quan hệ kết bạn
        public async Task DeleteAsync(Friendship friendship)
        {
            _context.Friendships.Remove(friendship);
            await _context.SaveChangesAsync();
        }

        // Lấy danh sách mối quan hệ kết bạn của người dùng theo ID
        public async Task<IEnumerable<Friendship>> GetFriendshipsByUserIdAsync(int userId)
        {
            return await _context.Friendships
                .Where(f => f.RequesterId == userId || f.AccepterId == userId)
                .ToListAsync();
        }

        // Lấy mối quan hệ kết bạn giữa hai người dùng
        public async Task<Friendship> GetFriendshipBetweenUsersAsync(int userId1, int userId2)
        {
            var friendship = await _context.Friendships
                .FirstOrDefaultAsync(f => (f.RequesterId == userId1 && f.AccepterId == userId2)
                                        || (f.RequesterId == userId2 && f.AccepterId == userId1));
           
            return friendship; // Simply return the friendship object (can be null if not found)
        }


        // Tìm kiếm các yêu cầu kết bạn dựa trên các tiêu chí khác nhau
        public async Task<IEnumerable<Friendship>> SearchAsync(
            int? requesterId = null,
            int? accepterId = null,
            FriendshipStatus? status = null,
            DateTime? createdAfter = null,
            DateTime? updatedAfter = null)
        {
            var query = _context.Friendships.AsQueryable();

            if (requesterId.HasValue)
            {
                query = query.Where(f => f.RequesterId == requesterId.Value);
            }

            if (accepterId.HasValue)
            {
                query = query.Where(f => f.AccepterId == accepterId.Value);
            }

            if (status.HasValue)
            {
                query = query.Where(f => f.Status == status.Value);
            }

            if (createdAfter.HasValue)
            {
                query = query.Where(f => f.CreatedAt >= createdAfter.Value);
            }

            if (updatedAfter.HasValue)
            {
                query = query.Where(f => f.UpdatedAt >= updatedAfter.Value);
            }

            return await query.ToListAsync();
        }

        // Lọc danh sách yêu cầu kết bạn dựa trên các tiêu chí
        public async Task<IEnumerable<Friendship>> FilterAsync(
            int? requesterId = null,
            int? accepterId = null,
            FriendshipStatus? status = null,
            DateTime? createdAfter = null,
            DateTime? updatedAfter = null)
        {
            var query = _context.Friendships.AsQueryable();

            if (requesterId.HasValue)
            {
                query = query.Where(f => f.RequesterId == requesterId.Value);
            }

            if (accepterId.HasValue)
            {
                query = query.Where(f => f.AccepterId == accepterId.Value);
            }

            if (status.HasValue)
            {
                query = query.Where(f => f.Status == status.Value);
            }

            if (createdAfter.HasValue)
            {
                query = query.Where(f => f.CreatedAt >= createdAfter.Value);
            }

            if (updatedAfter.HasValue)
            {
                query = query.Where(f => f.UpdatedAt >= updatedAfter.Value);
            }

            return await query.ToListAsync();
        }
    }
}
