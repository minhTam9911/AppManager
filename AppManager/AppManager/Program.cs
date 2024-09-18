using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Project_2_Web_Api.Configurations;
using Project_2_Web_Api.Service;
using Project_2_Web_Api.Service.Impl;
using Project_2_Web_API.Models;
using Swashbuckle.AspNetCore.Filters;
using System.ComponentModel;
using System.Diagnostics;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Services.AddHostedService<BackgroundWorkerService>();
//builder.Services.AddInfrastructrue();
builder.Services.AddEndpointsApiExplorer();
/*builder.Services.Configure<TwilioSettings>(builder.Configuration.GetSection("Twilio"));
builder.Services.AddTransient<SmsService, SmsServiceImpl>();*/
builder.Services.AddSwaggerGen(option =>
{
    option.AddSecurityDefinition("oauth2", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Description = "Standard Authorization Header Using The Bearer sheme(\"bearer {token}\")",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey
    });
    option.OperationFilter<SecurityRequirementsOperationFilter>();
});
var connectString = builder.Configuration["Connection:DefaultString"];
builder.Services.AddDbContext<DatabaseContext>(option => option.UseLazyLoadingProxies().UseSqlServer(connectString), ServiceLifetime.Singleton);
builder.Services.AddCors();
builder.Services.AddHttpContextAccessor();
builder.Services.AddAutoMapper(typeof(Program).Assembly);
builder.Services.AddScoped<PositionGroupService, PositionGroupServiceImpl>();
builder.Services.AddScoped<PositionService, PositionServiceImpl>();
builder.Services.AddScoped<GrantPermissionService, GrantPermissionServiceImpl>();
builder.Services.AddScoped<AreaService, AreaServiceImpl>();
builder.Services.AddScoped<StaffUserService, StaffUserServiceImpl>();
builder.Services.AddScoped<UserServiceAccessor, UserServiceAccessorImpl>();
builder.Services.AddScoped<UserService, UserServiceImpl>();
builder.Services.AddScoped<SupportAccountService, SupportAccountServiceImpl>();
builder.Services.AddScoped<DistributorService, DistributorServiceImpl>();
builder.Services.AddScoped<AuthService, AuthServiceImpl>();
builder.Services.AddScoped<UserServiceAccessor, UserServiceAccessorImpl>();
builder.Services.AddScoped<VisitService, VisitServiceImpl>();
builder.Services.AddScoped<TaskForVisitService, TaskForVisitServiceImpl>();
builder.Services.AddScoped<CommentService, CommentServiceImpl>();
builder.Services.AddScoped<PostService, PostServiceImpl>();
builder.Services.AddScoped<MediaService, MediaServiceImpl>();
builder.Services.AddScoped<NotificationService, NotificationServiceImpl>();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(option =>
{
    option.SaveToken = true;
    option.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        ValidateAudience = false,
        ValidateIssuer = false,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
            builder.Configuration.GetSection("AppSettings:Token").Value!)),
        ClockSkew = TimeSpan.Zero
    };
});
var app = builder.Build();
app.UseStaticFiles();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors(builder => builder
                .AllowAnyHeader()
                .AllowAnyMethod()
                .SetIsOriginAllowed((host) => true)
                .AllowCredentials()
            );
app.UseHttpsRedirection();
app.UseAuthentication();

app.UseRouting();
app.UseAuthorization();
app.UseEndpoints(_ => { });
app.MapControllers();

app.Run();
