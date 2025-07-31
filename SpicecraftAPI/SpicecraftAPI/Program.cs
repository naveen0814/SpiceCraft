using SpicecraftAPI.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using SpicecraftAPI.Data;
using SpicecraftAPI.Interface;
using SpicecraftAPI.Repository;
using System.Text;
using System.Reflection;
using System.Text.Json.Serialization;
using SpicecraftAPI.Swagger;


var builder = WebApplication.CreateBuilder(args);

// ✅ Register ApplicationDbContext
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});
// ✅ Register services from Repository implementing Interface
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IRestaurantService, RestaurantService>();
builder.Services.AddScoped<IMenuCategoryService, MenuCategoryService>();
builder.Services.AddScoped<IMenuItemService, MenuItemService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IOrderItemService, OrderItemService>();
builder.Services.AddScoped<ICartService, CartService>();
builder.Services.AddScoped<ICartItemService, CartItemService>();
builder.Services.AddScoped<IPaymentService, PaymentService>();
builder.Services.AddScoped<IDiscountService, DiscountService>();
builder.Services.AddScoped<IReviewService, ReviewService>();
builder.Services.AddSingleton<TokenService>();
builder.Services.AddControllers();
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddScoped<IDeliveryPartnerService, DeliveryPartnerService>();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend",
        policy =>
        {
            policy.WithOrigins("http://localhost:3001")
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
});
// ✅ Token service for JWT generation
builder.Services.AddSingleton<TokenService>();
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });
// ✅ Configure JWT Authentication
var jwtSettings = builder.Configuration.GetSection("Jwt");
var key = Encoding.UTF8.GetBytes(jwtSettings["Key"]);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(key)
    };
});



// ✅ Swagger/OpenAPI Setup
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Spicecraft API",
        Version = "v1"
    });

    // 🔐 JWT setup remains the same
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT using Bearer scheme (e.g., 'Bearer your-token')",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                },
                Scheme = "oauth2",
                Name = "Bearer",
                In = ParameterLocation.Header,
            },
            new List<string>()
        }
    });

    // ✅ Register the custom filter
    //c.DocumentFilter<HideAuthorizedDocumentFilter>();
});


var app = builder.Build();

// ✅ Enable Swagger in development
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Spicecraft API v1");
        c.RoutePrefix = string.Empty; // Serve at root
    });
}
app.UseStaticFiles();
app.UseCors("AllowAll");
app.UseAuthentication();
app.UseAuthorization();
app.UseHttpsRedirection();

// ✅ Authentication + Authorization middleware
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.Run();
