using Ecommerce.Shared.Configuration;
using Ecommerce.Data.Context;
using Ecommerce.Data.IRepository;
using Ecommerce.Data.Repository;
using Ecommerce.Model;
using Ecommerce.Services;
using Ecommerce.Services.IServices;
using Ecommerce.ServicesDependencies.Email;
using Ecommerce.Shared.Mapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using Ecommerce.ServicesDependencies.MercadoPago;

var builder = WebApplication.CreateBuilder(args);

//Read secrets
string connectionString = builder.Configuration["ConnectionStrings:SqlServerConnection"];

//Add Connection SqlServer
builder.Services.AddDbContext<ApplicationDbContext>(options =>
     options.UseSqlServer(connectionString)
);

//Add Identity
builder.Services.AddIdentity<ApplicationUser, IdentityRole>().AddEntityFrameworkStores<ApplicationDbContext>().AddDefaultTokenProviders();

builder.Services.Configure<JwtConfig>(builder.Configuration.GetSection("JwtConfig"));
builder.Services.Configure<SmtpSettings>(builder.Configuration.GetSection("SmtpSettings"));

var key = Encoding.ASCII.GetBytes(builder.Configuration.GetSection("JwtConfig:Secret").Value);

var tokenValidationParameters = new TokenValidationParameters()
{
    ValidateIssuerSigningKey = true,
    IssuerSigningKey = new SymmetricSecurityKey(key),
    ValidateIssuer = false,
    ValidateAudience = false,
    RequireExpirationTime = false,
    ValidateLifetime = true
};

// Add Authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
})
    .AddJwtBearer(jwt =>
    {
        jwt.SaveToken = true;
        jwt.TokenValidationParameters = tokenValidationParameters;
    });


//Add Dependence Inyection
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IProductRepository, ProductRepository>(); 
builder.Services.AddScoped<IAdminRepository, AdminRepository>();
builder.Services.AddScoped<IPurchaseRepository, PurchaseRepository>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

builder.Services.AddScoped<IProductServices, ProductServices>();
builder.Services.AddScoped<IUserServices, UserServices>();
builder.Services.AddScoped<IAdminServices, AdminServices>();
builder.Services.AddScoped<IPurchaseOrdersServices, PurchaseOrdersServices>();
builder.Services.AddScoped<IGenerateJWTServices, GenerateJWTServices>();
builder.Services.AddScoped<ISendEmail, SendEmail>();

builder.Services.AddScoped<IPurchaseOrdersServices, PurchaseOrdersServices>();
builder.Services.AddScoped<IMercadoPago, MP>();

builder.Services.AddTransient<IMapperDto, MapperDto>();
builder.Services.AddSingleton<IEmailSender, EmailService>();
builder.Services.AddSingleton(tokenValidationParameters);

// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Furniture_Store_API",
        Version = "v1"
    });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = $@"JWT Authorization header using the Bearer scheme. 
                        \r\n\r\n Enter prefix (Bearer), space, and then your token. 
                        Example: 'Bearer 1231233kjsdlkajdksad'"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement {
        {
            new OpenApiSecurityScheme {
            Reference = new OpenApiReference{
                Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string [] { }
        }
    });
});

builder.Services.AddCors(p => p.AddPolicy("PolicyCors", build =>
{
    build.WithOrigins("*").AllowAnyMethod().AllowAnyHeader();
}));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseCors("PolicyCors");

app.UseAuthorization();

app.MapControllers();

app.Run();
