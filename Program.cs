using Ishurim.Services;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Server.IISIntegration;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Scheme = "Bearer",
        Description = "Enter only the token.",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
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
                }
            },
            Array.Empty<string>()
        }
    });
});

//builder.Services.AddCors(options =>
//{
//    options.AddPolicy("AllowAll", builder =>
//    {
//        builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
//    });
//});

var useNegotiate =
    builder.Configuration.GetValue<bool>("Authorization:UseNegotiate");

if (useNegotiate)
{
    builder.Services.AddAuthentication("Negotiate")
        .AddNegotiate();
}
else
{
    builder.Services.AddAuthentication(IISDefaults.AuthenticationScheme);
}

builder.Services.AddAuthorization();

builder.Services.AddScoped<AccountService>();
builder.Services.AddScoped<ApprovalService>();
builder.Services.AddScoped<ApproverService>();
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<DbService>();
builder.Services.AddScoped<DepartmentService>();
builder.Services.AddScoped<HospitalService>();
builder.Services.AddScoped<InstituteService>();
builder.Services.AddScoped<PdfService>();
builder.Services.AddScoped<TestService>();
builder.Services.AddScoped<VehicleService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseDefaultFiles();
app.UseStaticFiles();

//app.UseCors("AllowAll");

app.UseAuthorization();

app.MapControllers();
app.MapFallbackToFile("index.html");

app.Run();