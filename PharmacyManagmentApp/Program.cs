using Application.IServices.ActiveIngredient;
using Application.IServices.Department;
using Application.IServices.Diagnosis;
using Application.IServices.Doctor;
using Application.IServices.MedicationForm;
using Application.IServices.Supplier;
using Application.MappingProfiles;
using Application.Services;
using Domain.IRepositories;
using Infrastructure.Context;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Infrastructure.DataSeeders;
using Microsoft.OpenApi.Models;
using Application.IServices.Auth;
using Application.Services.Auth;
using Application.IServices.Employee;
using Application.IServices.FamilyMember;
using Application.IServices.InsuredPerson;
using Application.IServices.Manufacturer;
using Application.IServices.MedicationClass;
using Application.IServices.Medication;

var builder = WebApplication.CreateBuilder(args);

// Registering DbContext 
builder.Services.AddDbContext<PharmacyDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));


// Registering ASP.NET Core Identity
builder.Services.AddIdentity<User, Role>(
    options =>
    {
        options.Password.RequiredUniqueChars = 0;
        options.Password.RequireUppercase = false;
        options.Password.RequiredLength = 6;
        options.Password.RequireNonAlphanumeric = false;
    })
    .AddEntityFrameworkStores<PharmacyDbContext>()
    .AddDefaultTokenProviders();


// Configure JWT Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
.AddJwtBearer(options =>
{
    options.SaveToken = true;
    options.RequireHttpsMetadata = false;
    options.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidAudience = builder.Configuration["Jwt:Audience"],
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
    };

    options.Events = new JwtBearerEvents
    {
        OnChallenge = context =>
        {
            context.HandleResponse();
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            context.Response.ContentType = "application/json";
            return context.Response.WriteAsync("{\"message\": \"Unauthorized. Token is missing or invalid.\"}");
        }
    };
});


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter your token: Bearer"
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

builder.Services.AddAutoMapper(
    typeof(DepartmentProfile),
    typeof(DoctorProfile),
    typeof(ActiveIngredientProfile),
    typeof(SupplierProfile),
    typeof(DiagnosisProfile),
    typeof(MedicationFormProfile),
    typeof(MedicationProfile),
    typeof(EmployeeProfile),
    typeof(InsuredPersonProfile),
    typeof(FamilyMemberProfile)    

    );

// Registering all the services
builder.Services.AddScoped<IDepartmentService, DepartmentService>();
builder.Services.AddScoped<IDoctorService, DoctorService>();
builder.Services.AddScoped<IActiveIngredientService, ActiveIngredientService>();
builder.Services.AddScoped<ISupplierService, SupplierService>();
builder.Services.AddScoped<IDiagnosisService, DiagnosisService>();
builder.Services.AddScoped<IMedicationFormService, MedicationFormService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IEmployeeService, EmployeeService>();
builder.Services.AddScoped<IFamilyMemberService, FamilyMemberService>();
builder.Services.AddScoped<IInsuredPersonService, InsuredPersonService>();
builder.Services.AddScoped<IManufacturerService, ManufacturerService>();
builder.Services.AddScoped<IMedicationClassService, MedicationClassService>();
builder.Services.AddScoped<IMedicationService, MedicationService>();
builder.Services.AddScoped<IRepository<MedicationActiveIngredient, int>, Repository<MedicationActiveIngredient, int>>();






// Registering the Repository
builder.Services.AddScoped(typeof(IRepository<,>), typeof(Repository<,>));
builder.Services.AddScoped(typeof(IMedicationRepository), typeof(MedicationRepository));



var app = builder.Build();



if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    await IdentitySeedData.Initialize(services);
}

app.Run();
