using Application.IServices.ActiveIngredient;
using Application.IServices.Department;
using Application.IServices.Diagnosis;
using Application.IServices.Doctor;
using Application.IServices.Supplier;
using Application.MappingProfiles;
using Application.Services;
using Domain.IRepositories;
using Infrastructure.Context;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Registering DbContext 
builder.Services.AddDbContext<PharmacyDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));


builder.Services.AddAutoMapper(
    typeof(DepartmentProfile),
    typeof(DoctorProfile),
    typeof(ActiveIngredientProfile),
    typeof(SupplierProfile),
    typeof(DiagnosisProfile)
    

    );

// Registering all the services
builder.Services.AddScoped<IDepartmentService, DepartmentService>();
builder.Services.AddScoped<IDoctorService, DoctorService>();
builder.Services.AddScoped<IActiveIngredientService, ActiveIngredientService>();
builder.Services.AddScoped<ISupplierService, SupplierService>();
builder.Services.AddScoped<IDiagnosisService, DiagnosisService>();

// Registering the Repository
builder.Services.AddScoped(typeof(IRepository<,>), typeof(Repository<,>));



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

app.Run();
