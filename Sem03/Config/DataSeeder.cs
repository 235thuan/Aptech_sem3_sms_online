using Bogus;
using Sem03.Entities;
using Sem03.DTOs;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace Sem03.Config
{
    public class DataSeeder
    {
        private readonly DataFriendShipContext _context;
        private readonly Faker _faker;

        public DataSeeder(DataFriendShipContext context)
        {
            _context = context;
            _faker = new Faker();
        }

        public async Task SeedAsync()
        {
            if (await _context.Roles.AnyAsync() || await _context.Users.AnyAsync())
            {
                return;
            }

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                await SeedRolesAsync();
                var users = await SeedUsersAsync();
                await SeedProfilesAndStatusesAsync(users);
                await SeedFriendshipsAsync(users);
                await SeedConversationsAsync(users);
                await SeedMessagesAsync();
                await SeedPaymentsAsync();

                await transaction.CommitAsync();
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                Console.WriteLine($"Lỗi: {ex.Message} - Chi tiết: {ex.InnerException?.Message}");
            }
        }

        private async Task SeedRolesAsync()
        {
            var roleFaker = new Faker<Role>()
                .RuleFor(r => r.Name, f => TruncateString(f.Name.JobTitle(), 50)); // Giới hạn độ dài 50 ký tự

            var roles = roleFaker.Generate(5);
            Console.WriteLine($"Tạo {roles.Count} vai trò.");

            await _context.Roles.AddRangeAsync(roles);
            await _context.SaveChangesAsync();

            // Log the created roles for verification
            foreach (var role in roles)
            {
                Console.WriteLine($"Đã tạo vai trò: {role.Name}");
            }
        }

        private async Task<List<User>> SeedUsersAsync()
        {
            var roleIds = await _context.Roles.Select(r => r.RoleId).ToListAsync();
            // Check the role IDs for debugging
            Console.WriteLine($"Role IDs: {string.Join(", ", roleIds)}");
            // Check if there are any roles to assign to users
            if (roleIds.Count == 0)
            {
                throw new InvalidOperationException("No roles exist to assign to users."); // Throw an error if no roles are available
            }

            try
            {
                var userFaker = new Faker<User>()
                     .RuleFor(u => u.RoleId, f => f.PickRandom(roleIds))
                     .RuleFor(u => u.UserName, f => TruncateString(f.Internet.UserName(), 10))
                     .RuleFor(u => u.PasswordHash, f => f.Internet.Password(8, true))
                     .RuleFor(u => u.Email, f => TruncateString(f.Internet.Email(), 50))
                     .RuleFor(u => u.PhoneNumber, f => TruncateString(f.Phone.PhoneNumber(), 15));

                var users = userFaker.Generate(10000);
                Console.WriteLine($"Tạo {users.Count} người dùng.");

                if (users.Count == 0)
                {
                    throw new InvalidOperationException("User generation failed, no users created."); // Throw an error if no users were generated
                }

                await _context.Users.AddRangeAsync(users);
                await _context.SaveChangesAsync();
                Console.WriteLine($"Đã lưu {users.Count} người dùng vào cơ sở dữ liệu.");
                return users;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving users: {ex.Message}");
                throw; // Re-throw the exception after logging it
            }
        }



        private async Task SeedProfilesAndStatusesAsync(List<User> users)
        {
            var profiles = new List<Profile>();
            var userStatuses = new List<UserStatus>();

            foreach (var user in users)
            {
                var profile = new Profile
                {
                    UserId = user.Id,
                    FullName = TruncateString(_faker.Name.FullName(), 100),
                    Sex = _faker.PickRandom<Gender>(),
                    DOB = _faker.Date.Past(30, DateTime.Now),
                    Address = TruncateString(_faker.Address.FullAddress(), 200),
                    Hobbies = TruncateString(_faker.Lorem.Sentence(5), 200),
                    Marriaged = _faker.PickRandom<MaritalStatus>(),
                    Liked = TruncateString(_faker.Lorem.Sentence(), 200),
                    Disliked = TruncateString(_faker.Lorem.Sentence(), 200),
                    Avatar = _faker.Internet.Avatar()
                };

                var userStatus = new UserStatus
                {
                    UserId = user.Id,
                    IsOnline = _faker.Random.Bool(),
                    LastOnlineAt = _faker.Date.Recent(30),
                    IsPublic = _faker.Random.Bool(),
                    StatusUpdatedAt = DateTime.UtcNow
                };

                profiles.Add(profile);
                userStatuses.Add(userStatus);
            }

            await _context.Profiles.AddRangeAsync(profiles);
            await _context.UserStatuses.AddRangeAsync(userStatuses);
            await _context.SaveChangesAsync();
        }

        private async Task SeedFriendshipsAsync(List<User> users)
        {
            var friendshipFaker = new Faker<FriendshipDto>()
                .RuleFor(f => f.RequesterId, f => f.PickRandom(users.Select(u => u.Id).ToList()))
                .RuleFor(f => f.AccepterId, (f, u) =>
                {
                    int accepterId;
                    do
                    {
                        accepterId = f.PickRandom(users.Select(u => u.Id).ToList());
                    } while (accepterId == u.RequesterId);
                    return accepterId;
                })
                .RuleFor(f => f.Status, f => f.PickRandom(FriendshipStatus.Pending, FriendshipStatus.Accepted, FriendshipStatus.Rejected))
                .RuleFor(f => f.CreatedAt, f => f.Date.Recent(30))
                .RuleFor(f => f.UpdatedAt, f => f.Date.Recent(15));

            var friendships = friendshipFaker.Generate(5000);
            Console.WriteLine($"Tạo {friendships.Count} tình bạn.");

            foreach (var friendship in friendships)
            {
                if (!await _context.Friendships.AsNoTracking().AnyAsync(f => f.RequesterId == friendship.RequesterId && f.AccepterId == friendship.AccepterId))
                {
                    var friendshipEntity = new Friendship
                    {
                        RequesterId = friendship.RequesterId,
                        AccepterId = friendship.AccepterId,
                        Status = friendship.Status,
                        CreatedAt = friendship.CreatedAt,
                        UpdatedAt = friendship.UpdatedAt
                    };

                    await _context.Friendships.AddAsync(friendshipEntity);
                }
            }
            await _context.SaveChangesAsync();
        }

        private async Task SeedConversationsAsync(List<User> users)
        {
            var conversationFaker = new Faker<Conversation>()
                .RuleFor(c => c.ConversationName, f => f.Commerce.ProductName())
                .RuleFor(c => c.IsGroupChat, f => f.Random.Bool())
                .RuleFor(c => c.CreatedAt, f => f.Date.Recent(10))
                .RuleFor(c => c.CreatedBy, f => f.PickRandom(users.Select(u => u.Id).ToList()));

            var conversations = conversationFaker.Generate(1000);
            Console.WriteLine($"Tạo {conversations.Count} trò chuyện.");
            await _context.Conversations.AddRangeAsync(conversations);
            await _context.SaveChangesAsync();
        }

        private async Task SeedMessagesAsync()
        {
            var messages = new List<Message>();

            // Lấy danh sách ConversationId và UserId trước
            var conversationIds = await _context.Conversations.Select(c => c.ConversationId).ToListAsync();
            var userIds = await _context.Users.Select(u => u.Id).ToListAsync();

            var messageFaker = new Faker<Message>()
                .RuleFor(m => m.ConversationId, f => f.PickRandom(conversationIds))
                .RuleFor(m => m.SenderId, f => f.PickRandom(userIds))
                .RuleFor(m => m.Content, f => TruncateString(f.Lorem.Sentence(10), 500))
                .RuleFor(m => m.MessageType, f => f.PickRandom(MessageType.Text, MessageType.Image, MessageType.File))
                .RuleFor(m => m.SendAt, f => f.Date.Recent(30))
                .RuleFor(m => m.EditAt, f => f.Date.Recent(15));

            messages.AddRange(messageFaker.Generate(50000));
            Console.WriteLine($"Tạo {messages.Count} tin nhắn.");
            await _context.Messages.AddRangeAsync(messages);
            await _context.SaveChangesAsync();
        }


        private async Task SeedPaymentsAsync()
        {
            // Lấy danh sách UserId trước
            var userIds = await _context.Users.Select(u => u.Id).ToListAsync();

            var paymentFaker = new Faker<Payment>()
                .RuleFor(p => p.UserId, f => f.PickRandom(userIds)) // Sử dụng danh sách UserId đã lấy
                .RuleFor(p => p.Amount, f => f.Finance.Amount(1, 1000))
                .RuleFor(p => p.PaymentDate, f => f.Date.Recent(30))
                .RuleFor(p => p.CardNumber, f => f.Finance.CreditCardNumber())
                .RuleFor(p => p.CardHolderName, f => f.Name.FullName())
                .RuleFor(p => p.ExpiryDate, f => f.Date.Future(1))
                .RuleFor(p => p.CVV, f => f.Random.Int(100, 999).ToString())
                .RuleFor(p => p.PaymentStatus, f => f.PickRandom<PaymentStatus>())
                .RuleFor(p => p.Description, f => f.Lorem.Sentence());

            var payments = paymentFaker.Generate(1000);
            Console.WriteLine($"Tạo {payments.Count} thanh toán.");
            await _context.Payments.AddRangeAsync(payments);
            await _context.SaveChangesAsync();
        }


  
        // Phương thức TruncateString
        private string TruncateString(string input, int maxLength)
        {
            if (string.IsNullOrEmpty(input))
            {
                return input; // Trả về chuỗi gốc nếu nó trống hoặc null
            }

            return input.Length <= maxLength ? input : input.Substring(0, maxLength);
        }
    }
}
