
using Microsoft.EntityFrameworkCore;
using Sem03.Entities;

namespace Sem03.Config
{
    public class DataFriendShipContext : DbContext
    {
        public DataFriendShipContext(DbContextOptions<DataFriendShipContext> options)
        : base(options)
        {
        }
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Profile> Profiles { get; set; }
        public DbSet<UserStatus> UserStatuses { get; set; }
        public DbSet<Friendship> Friendships { get; set; }
        public DbSet<BlockUser> BlockUsers { get; set; }
        public DbSet<Conversation> Conversations { get; set; }
        public DbSet<ConversationMember> ConversationMembers { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<Attachment> Attachments { get; set; }
        public DbSet<Payment> Payments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Định nghĩa khóa chính và khóa ngoại
            modelBuilder.Entity<Friendship>()
        .HasKey(f => new { f.RequesterId, f.AccepterId });

            // Định nghĩa các quan hệ
            modelBuilder.Entity<Friendship>()
                .HasOne(f => f.Requester)
                .WithMany(u => u.Friendships) // Một User có thể là requester của nhiều Friendship
                .HasForeignKey(f => f.RequesterId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Friendship>()
                .HasOne(f => f.Accepter)
                .WithMany() // Một User có thể là accepter của nhiều Friendship
                .HasForeignKey(f => f.AccepterId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<BlockUser>()
                .HasKey(b => new { b.UserId, b.BlockedUserId });

            // Định nghĩa quan hệ 1-n giữa User và BlockUser
            modelBuilder.Entity<BlockUser>()
                .HasOne(b => b.User) // Người dùng thực hiện chặn
                .WithMany(u => u.BlockedUsers) // Một người dùng có thể chặn nhiều người
                .HasForeignKey(b => b.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<BlockUser>()
                .HasOne(b => b.BlockedUser) // Người dùng bị chặn
                .WithMany() // Không cần tập hợp ở bên BlockedUser
                .HasForeignKey(b => b.BlockedUserId)
                .OnDelete(DeleteBehavior.Restrict);


            // Định nghĩa quan hệ n-n giữa User và Conversation qua ConversationMember
            modelBuilder.Entity<ConversationMember>()
                .HasKey(cm => new { cm.ConversationId, cm.UserId }); // Khóa chính tổng hợp

            modelBuilder.Entity<ConversationMember>()
                .HasOne(cm => cm.Conversation)
                .WithMany(c => c.ConversationMembers) // Một Conversation có nhiều ConversationMember
                .HasForeignKey(cm => cm.ConversationId);

            modelBuilder.Entity<ConversationMember>()
                .HasOne(cm => cm.User)
                .WithMany(u => u.ConversationMembers) // Một User có nhiều ConversationMember
                .HasForeignKey(cm => cm.UserId);

            // Định nghĩa quan hệ 1-n giữa User và Conversation (người tạo cuộc trò chuyện)
            modelBuilder.Entity<Conversation>()
                .HasOne(c => c.Creator)
                .WithMany(u => u.CreatedConversations) // Một User có thể tạo nhiều Conversation
                .HasForeignKey(c => c.CreatedBy)
                .OnDelete(DeleteBehavior.Restrict); // Không xóa cascade khi xóa User
                                                    // Định nghĩa kiểu dữ liệu cho Amount trong bảng Payment
            modelBuilder.Entity<Payment>()
                .Property(p => p.Amount)
                .HasColumnType("decimal(18, 2)"); // Định nghĩa kiểu decimal với độ chính xác 18 và tỷ lệ 2

            // Định nghĩa quan hệ 1-n giữa User và Payment
            modelBuilder.Entity<Payment>()
                .HasOne(p => p.User)
                .WithMany(u => u.Payments) // Một User có thể có nhiều Payment
                .HasForeignKey(p => p.UserId)
                .OnDelete(DeleteBehavior.Restrict); // Không xóa cascade nếu xóa User
                                                    // Định nghĩa quan hệ 1-n giữa User và Message
            modelBuilder.Entity<Message>()
                .HasOne(m => m.Sender) // Một Message có một Sender (người gửi)
                .WithMany(u => u.Messages) // Một User có thể gửi nhiều Message
                .HasForeignKey(m => m.SenderId) // FK đến User
                .OnDelete(DeleteBehavior.Restrict); // Không xóa cascade nếu xóa User

            // Định nghĩa quan hệ 1-n giữa Message và Attachment
            modelBuilder.Entity<Attachment>()
                .HasOne(a => a.Message) // Một Attachment thuộc về một Message
                .WithMany(m => m.Attachments) // Một Message có thể có nhiều Attachment
                .HasForeignKey(a => a.MessageId) // FK tới Message
                .OnDelete(DeleteBehavior.Cascade); // Xóa Attachment nếu Message bị xóa

            // Định nghĩa quan hệ 1-1 giữa User và Profile
            modelBuilder.Entity<User>()
                .HasOne(u => u.Profile) // Một User có một Profile
                .WithOne(p => p.User) // Một Profile thuộc về một User
                .HasForeignKey<Profile>(p => p.UserId); // Khóa ngoại trong bảng Profile trỏ tới User

            // Định nghĩa quan hệ 1-n giữa Role và User
            modelBuilder.Entity<User>()
                .HasOne(u => u.Role) // Một User có một Role
                .WithMany(r => r.Users) // Một Role có nhiều User
                .HasForeignKey(u => u.RoleId) // FK tới Role
                .OnDelete(DeleteBehavior.Restrict); // Không xóa cascade khi xóa Role


            base.OnModelCreating(modelBuilder);
        }
    }
}
