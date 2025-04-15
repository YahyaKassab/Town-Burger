// Importing necessary namespaces for JWT, Identity, Token handling, configuration, etc.
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Text;
using Town_Burger.Models;
using Town_Burger.Models.Context;
using Town_Burger.Models.Identity;
using Town_Burger.Services;

// Entry point of the application. `WebApplication.CreateBuilder` initializes the app with defaults.
var builder = WebApplication.CreateBuilder(args);

// Register controllers for handling incoming HTTP requests (API endpoints).
builder.Services.AddControllers();

// Enables MVC views and configures JSON serialization to ignore circular references.
builder.Services.AddControllersWithViews()
    .AddNewtonsoftJson(options =>
        options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
    );

// Adds minimal API explorer (for Swagger/OpenAPI documentation).
builder.Services.AddEndpointsApiExplorer();

// Adds Swagger support for generating API documentation and testing endpoints.
builder.Services.AddSwaggerGen();

// Dependency Injection for custom services
builder.Services.AddTransient<IBalanceService, BalanceService>();
builder.Services.AddTransient<ICustomerService, CustomerService>();
builder.Services.AddTransient<IEmployeeService, EmployeeService>();
builder.Services.AddTransient<IReviewService, ReviewService>();
builder.Services.AddTransient<IMailService, MailService>();
builder.Services.AddTransient<IMenuService, MenuService>();
builder.Services.AddTransient<IOrdersService, OrdersService>();
builder.Services.AddTransient<ISecondarySevice, SecondaryService>();
builder.Services.AddTransient<IUserService, UserService>();

// Registers `AppDbContext` with the service container for Entity Framework Core (database access).
builder.Services.AddDbContext<AppDbContext>();

// Adds and configures ASP.NET Core Identity system for user management and authentication.
builder.Services.AddIdentity<User, IdentityRole>(options =>
{
    options.Password.RequireDigit = true;       // Require at least one digit in password
    options.Password.RequireLowercase = true;   // Require at least one lowercase letter
    options.Password.RequiredLength = 5;        // Minimum password length
})
// Uses EF Core to store identity data in AppDbContext and adds token support (for password reset, etc.).
.AddEntityFrameworkStores<AppDbContext>()
.AddDefaultTokenProviders();

// Loads the connection string named "DefaultConnection" from appsettings.json
builder.Configuration.GetConnectionString("DefaultConnection");

// Configures authentication using JWT Bearer scheme
builder.Services.AddAuthentication(options =>
{
    // Default scheme for authentication and challenge responses
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
})
// Adds JWT Bearer authentication with token validation settings
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true, // Ensure the token has a valid issuer
        ValidateAudience = true, // Ensure the token has a valid audience
        ValidAudience = builder.Configuration["AuthSettings:Audience"], // Allowed audience
        ValidIssuer = builder.Configuration["AuthSettings:Issuer"],     // Allowed issuer
        RequireExpirationTime = true, // Token must have expiration time
        // Symmetric key used to validate token signature (from configuration)
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(builder.Configuration["AuthSettings:Key"])
        ),
        ValidateIssuerSigningKey = true // Ensure signing key is valid
    };
});

// Builds the WebApplication object (finalizes configuration)
var app = builder.Build();

// Configure middleware pipeline
if (app.Environment.IsDevelopment())
{
    // Enable Swagger UI in development environment
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Enables CORS (Cross-Origin Resource Sharing) for all headers, methods, and origins.
app.UseCors(builder => builder
    .AllowAnyHeader()
    .AllowAnyMethod()
    .SetIsOriginAllowed((host) => true) // Allow all origins
    .AllowCredentials()
);

// Forces redirection from HTTP to HTTPS
app.UseHttpsRedirection();

// Enables authentication middleware (JWT, Identity, etc.)
app.UseAuthentication();

// Enables authorization middleware (check if user has access to resources)
app.UseAuthorization();

// Maps controller endpoints to their respective routes
app.MapControllers();

// Runs the application (starts listening for HTTP requests)
app.Run();
