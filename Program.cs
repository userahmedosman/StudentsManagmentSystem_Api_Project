using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    // ===============================
    // 1) Define the JWT Bearer security scheme
    // ===============================
    //
    // This tells Swagger that our API uses JWT Bearer authentication
    // through the HTTP Authorization header.
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        // The name of the HTTP header where the token will be sent.
        Name = "Authorization",


        // Indicates this is an HTTP authentication scheme.
        Type = SecuritySchemeType.Http,


        // Specifies the authentication scheme name.
        // Must be exactly "Bearer" for JWT Bearer tokens.
        Scheme = "Bearer",


        // Optional metadata to describe the token format.
        BearerFormat = "JWT",


        // Specifies that the token is sent in the request header.
        In = ParameterLocation.Header,


        // Text shown in Swagger UI to guide the user.
        Description = "Enter: Bearer {your JWT token}"
    });


    // ===============================
    // 2) Require the Bearer scheme for secured endpoints
    // ===============================
    //
    // This tells Swagger that endpoints protected by [Authorize]
    // require the Bearer token defined above.
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                // Reference the previously defined "Bearer" security scheme.
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },


            // No scopes are required for JWT Bearer authentication.
            // This array is empty because JWT does not use OAuth scopes here.
            new string[] {}
        }
    });
});

builder.Services.AddCors( options =>
{
    options.AddPolicy("AppPolicy", builder =>
    {
        builder.WithOrigins(
            "https://localhost:7034", 
            "http://localhost:5102",
            "http://127.0.0.1:5500"
            )
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});

var secretKey = builder.Configuration["JWT_SECRET_KEY"];

if (string.IsNullOrWhiteSpace(secretKey))
{
    throw new Exception("JWT secret key is not configured.");
}

// authorization and authentication service
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
    // TokenValidationParameters define how incoming JWTs will be validated.
    options.TokenValidationParameters = new TokenValidationParameters
    {
        // Ensures the token was issued by a trusted issuer.
        ValidateIssuer = true,


        // Ensures the token is intended for this API (audience check).
        ValidateAudience = true,


        // Ensures the token has not expired.
        ValidateLifetime = true,


        // Ensures the token signature is valid and was signed by the API.
        ValidateIssuerSigningKey = true,


        // The expected issuer value (must match the issuer used when creating the JWT).
        ValidIssuer = "StudentApi",


        // The expected audience value (must match the audience used when creating the JWT).
        ValidAudience = "StudentApiUsers",


        // The secret key used to validate the JWT signature.
        // This must be the same key used when generating the token.

        IssuerSigningKey = new SymmetricSecurityKey(
       Encoding.UTF8.GetBytes(secretKey))

    };
       
    });


// ===============================
// Authorization Configuration
// ===============================


// Register authorization services.
// This enables attributes like [Authorize] and role-based authorization.
builder.Services.AddAuthorization();
builder.Services.AddControllers();


// ===============================
// Swagger Configuration
// ===============================


// Enables Swagger endpoint discovery.
builder.Services.AddEndpointsApiExplorer();


// Enables Swagger UI for testing and documentation.
builder.Services.AddSwaggerGen();



var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("AppPolicy");

// IMPORTANT:
// Authentication middleware must run BEFORE authorization middleware.
// Authentication identifies the user.
// Authorization decides what the user is allowed to do.
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.Run();
