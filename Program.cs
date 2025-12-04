using Microsoft.EntityFrameworkCore;
using CatalogWebApi.Data;
using CatalogWebApi.Repository;
using CatalogWebApi.Repository.RepositoryImplement;
using CatalogWebApi.Service;
using CatalogWebApi.Service.ServiceImplement;
using CatalogWebApi.Mapper;
using CatalogWebApi.Utils.MapperImplement;
using Microsoft.Extensions.FileProviders;
using CatalogWebApi.Mapper.MapperImplement;
using CatalogWebApi.Models;
using QuestPDF.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

QuestPDF.Settings.License = LicenseType.Community;

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.Configure<SmtpSettings>(
    builder.Configuration.GetSection("EmailConfigurations")
);

builder.Services.AddDbContext<AppDbContext>(options => options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IEndowmentRepository, EndowmentRepository>();
builder.Services.AddScoped<IEndowmentService, EndowmentService>();
builder.Services.AddScoped<IEndowmentMapper, EndowmentMapper>();
builder.Services.AddScoped<IImagesService, ImagesService>();
builder.Services.AddScoped<IImagesRepository, ImagesRepository>();
builder.Services.AddScoped<IEndowmentTypeRepository, EndowmentTypeRepository>();
builder.Services.AddScoped<IEndowmentTypeService, EndowmentTypeService>();
builder.Services.AddScoped<IEndowmentTypeMapper, EndowmentTypeMapper>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<ICategoryMapper, CategoryMapper>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IEmailTemplateBuilder, EmailTemplateBuilder>();
builder.Services.AddScoped<IColorsRepository, ColorRepository>();
builder.Services.AddScoped<IColorService, ColorService>();
builder.Services.AddScoped<IColorMapper, ColorMapper>();
builder.Services.AddScoped<ISizeRepository, SizeRepository>();
builder.Services.AddScoped<ISizeService, SizeService>();
builder.Services.AddScoped<ISizeMapper, SizeMapper>();
builder.Services.AddScoped<IQuotationRepository, QuotationRepository>();
builder.Services.AddScoped<IQuotationsService, QuotationService>();
builder.Services.AddScoped<IQuotationMapper, QuotationMapper>();
builder.Services.AddScoped<IPdfService, PdfService>();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins("https://seguridad-industrial-y-suministros-sas.azurewebsites.net", "http://localhost:4200").AllowAnyMethod().AllowAnyHeader();
    });
});

builder.Services.AddControllers();

var app = builder.Build();

var webRootPath = builder.Environment.WebRootPath
                   ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");

var uploadPath = Path.Combine(webRootPath, "uploads");

if (!Directory.Exists(uploadPath))
{
    Directory.CreateDirectory(uploadPath);
}

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(uploadPath),
    RequestPath = "/uploads"

});

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowFrontend");

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
