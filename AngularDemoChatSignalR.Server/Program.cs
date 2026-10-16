using AngularDemoChatSignalR.Server.Database;
using AngularDemoChatSignalR.Server.Database.Entities;
using AngularDemoChatSignalR.Server.Database.Repositories;
using AngularDemoChatSignalR.Server.HubsWebsocket;
using Microsoft.EntityFrameworkCore;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSignalR(); //RealTime (websocket)
builder.Services.AddScoped<RealtimeNotificationHubService>(); //For RealTime (websocket) connections by userId

string connectionString = builder.Configuration.GetConnectionString("demoproject");
builder.Services.AddDbContext<DatabaseContext>(options => options.UseSqlServer(connectionString));


var app = builder.Build();

app.UseDefaultFiles();
app.UseStaticFiles();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.MapHub<RealtimeNotificationHub>("/sigR/notis"); //RealTime (websocket)

app.MapFallbackToFile("/notfound.html");

//############################# DEMO INIT DB #############################################

void INITDEMO()
{
    var options = new DbContextOptionsBuilder<DatabaseContext>()
.UseSqlServer(connectionString)
.Options;


    DatabaseContext dbContext = new DatabaseContext(options);
    UserRepository userRepository = new UserRepository(dbContext);
    if (userRepository.CountAll() < 1)
    {
        List<User> users = new List<User>();
        users.Add(InitAddUser("Tom", userRepository));
        users.Add(InitAddUser("Max", userRepository));
        users.Add(InitAddUser("John", userRepository));
        users.Add(InitAddUser("John", userRepository));
        users.Add(InitAddUser("Anna", userRepository));
        users.Add(InitAddUser("Julia", userRepository));
        users.Add(InitAddUser("Pedro", userRepository));
        users.Add(InitAddUser("David", userRepository));
        users.Add(InitAddUser("Martin", userRepository));
        users.Add(InitAddUser("Fabian", userRepository));
    }
}
static User InitAddUser(string name, UserRepository userRepository)
{
    Task<Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry<User>> task
        = userRepository.AddUser(new User(name));
    task.Wait();
    return task.Result.Entity;
}

INITDEMO();
//############################# END DEMO INIT DB #########################################

app.Run();
