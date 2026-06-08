using FinanzAladin.Components;
using FinanzAladin.Database;
using FinanzAladin.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddDbContextFactory<FinanceDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Register Services
builder.Services.AddScoped<TransactionService>();
builder.Services.AddScoped<AuthService>();

var app = builder.Build();

// Initialize database
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var dbContextFactory = services.GetRequiredService<IDbContextFactory<FinanceDbContext>>();
    try
    {
        using var context = dbContextFactory.CreateDbContext();
        context.Database.EnsureCreated();

        // Ensure Users table exists
        var createUserTableSql = @"
            CREATE TABLE IF NOT EXISTS ""Users"" (
                ""Id"" SERIAL PRIMARY KEY,
                ""Email"" text NOT NULL UNIQUE,
                ""Username"" text NOT NULL UNIQUE,
                ""PasswordHash"" text NOT NULL,
                ""FullName"" text,
                ""CreatedAt"" timestamp with time zone NOT NULL,
                ""LastLogin"" timestamp with time zone,
                ""IsActive"" boolean NOT NULL DEFAULT true
            );";

        context.Database.ExecuteSqlRaw(createUserTableSql);

        // Ensure Transactions table exists with all columns for PostgreSQL
        var createTableSql = @"
            CREATE TABLE IF NOT EXISTS ""Transactions"" (
                ""Id"" SERIAL PRIMARY KEY,
                ""Title"" text NOT NULL,
                ""Amount"" numeric(18,2) NOT NULL,
                ""Date"" timestamp with time zone NOT NULL,
                ""Type"" integer NOT NULL,
                ""Category"" text NOT NULL,
                ""Note"" text NOT NULL,
                ""UserId"" integer NOT NULL,
                CONSTRAINT ""FK_Transactions_Users"" FOREIGN KEY (""UserId"") REFERENCES ""Users""(""Id"") ON DELETE CASCADE
            );";

        context.Database.ExecuteSqlRaw(createTableSql);

        // Create index on UserId for better query performance
        var createIndexSql = @"
            CREATE INDEX IF NOT EXISTS ""IX_Transactions_UserId"" ON ""Transactions""(""UserId"");";

        context.Database.ExecuteSqlRaw(createIndexSql);

        Console.WriteLine("? Database initialized successfully");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"? Database initialization error: {ex.Message}");
        Console.WriteLine($"StackTrace: {ex.StackTrace}");
    }
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);

app.UseHttpsRedirection();

app.UseAntiforgery();

app.MapStaticAssets();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();