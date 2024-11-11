using Microsoft.EntityFrameworkCore;
using Tourism.Data;
using Tourism.Services;


var builder = WebApplication.CreateBuilder(args);

// Register the UserServices for dependency injection
builder.Services.AddScoped<UserServices>(); 

// Add DbContext
builder.Services.AddDbContext<TourismDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Register AutoMapper profiles
builder.Services.AddAutoMapper(typeof(MappingProfile));

// Add other services like controllers, Swagger, etc.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Swagger UI and middlewares
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();

app.MapControllers();

app.Run();