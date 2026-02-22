using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Security.Claims;
using TaskApi;

var builder = WebApplication.CreateBuilder(args);

// Get the connection string from environment variables (provided by Docker)
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<TaskDb>(options =>
    options.UseSqlServer(connectionString));

// 1. Add Services (Dependency Injection)
// builder.Services.AddDbContext<TaskDb>(opt => opt.UseSqlite("Data Source=tasks.db"));

builder.Services.AddOpenApi(); // .NET 10's built-in documentation

// 1. Add Security Services
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
      options.TokenValidationParameters = new TokenValidationParameters
      {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = "your-app",
        ValidAudience = "your-app-users",
        // This is the "Secret Key" only your server knows
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("SuperSecretKey123456789!"))
      };
    });

builder.Services.AddAuthorization();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails(); // Adds standard formatting for errors



var app = builder.Build();

// --- APP STARTUP PHASE ---

// Create a "Sandbox" (Manual Scope)
using (var scope = app.Services.CreateScope())
{
  var services = scope.ServiceProvider;
  try
  {
    var context = services.GetRequiredService<TaskDb>();

    // 1. Ensure the schema is ready
    await context.Database.MigrateAsync();

    // 2. The Seeding Logic: Check if we already have data
    if (!context.Tasks.Any())
    {
      Console.WriteLine("--> Seeding data...");
      context.Tasks.AddRange(
          new TaskItem { Title = "Install Docker", IsCompleted = true },
          new TaskItem { Title = "Learn C# Dependency Injection", IsCompleted = true },
          new TaskItem { Title = "Build a Portfolio API", IsCompleted = false }
      );

      await context.SaveChangesAsync();
      Console.WriteLine("--> Database seeded successfully!");
    }
  }
  catch (Exception ex)
  {
    Console.WriteLine($"--> Error during startup: {ex.Message}");
  }
} // <--- Sandbox is destroyed here. DB connection is closed.

// --- APP RUNNING PHASE ---
// Now the app sits and waits for a REAL web request to create an AUTOMATIC scope.

// 2. Put the security checkpoints in the "Pipeline"
app.UseAuthentication(); // "Show me your ID"
app.UseAuthorization();  // "Are you allowed in this room?"
app.UseExceptionHandler(); // This must be at the very top of the pipeline!

// 2. Configure the Pipeline
if (app.Environment.IsDevelopment())
{
  app.MapOpenApi(); // Generates your API documentation
}

app.MapGet("/tasks", async (TaskDb db) => await db.Tasks.ToListAsync());

// 3. API Endpoints (CRUD)
app.MapGet("/tasks/search", async (string? q, TaskDb db) =>
{
  var query = db.Tasks.AsQueryable();

  if (!string.IsNullOrWhiteSpace(q))
  {
    // Search for titles that CONTAIN the string (case-insensitive by default in SQLite)
    query = query.Where(t => t.Title.Contains(q));
  }

  return await query.ToListAsync();
});

app.MapPost("/tasks", async (TaskItem task, TaskDb db, ClaimsPrincipal user) =>
{
  // Get the User ID from the JWT token automatically
  var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;

  // Logic: Assign this task to the logged-in user
  // (You would add a 'UserId' column to your TaskItem model first!)

  db.Tasks.Add(task);
  await db.SaveChangesAsync();
  return Results.Created($"/tasks/{task.Id}", task);
}).RequireAuthorization();

// Update a task (e.g., mark as completed or change title)
app.MapPut("/tasks/{id}", async (int id, TaskItem inputTask, TaskDb db) =>
{
  // 1. Try to find the existing task in the database
  var task = await db.Tasks.FindAsync(id);

  // 2. If it's not there, return a 404
  if (task is null) return Results.NotFound();

  // 3. Update the properties of the FOUND task with the NEW data
  task.Title = inputTask.Title;
  task.IsCompleted = inputTask.IsCompleted;

  // 4. Save the changes!
  await db.SaveChangesAsync();

  // 5. Return a "204 No Content" (Success, but nothing to send back)
  return Results.NoContent();
});

app.MapDelete("/tasks/{id}", async (int id, TaskDb db) =>
{
  if (await db.Tasks.FindAsync(id) is TaskItem task)
  {
    db.Tasks.Remove(task);
    await db.SaveChangesAsync();
    return Results.NoContent();
  }
  return Results.NotFound();
});

app.Run();

