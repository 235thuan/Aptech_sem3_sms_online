using Microsoft.EntityFrameworkCore;
using Sem03.Config;
using Sem03.Repositories;
using Sem03.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<DataFriendShipContext>
    (options => options.UseSqlServer
        (builder.Configuration.GetConnectionString
            ("DefaultConnection")
        )
    );
// Đăng ký các repository
builder.Services.AddScoped<IFriendshipRepository, FriendshipRepository>();

// Đăng ký FriendshipService
builder.Services.AddScoped<FriendshipService>();


var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<DataFriendShipContext>();
    var seeder = new DataSeeder(context);
    await seeder.SeedAsync(); // Seed the database
}
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
