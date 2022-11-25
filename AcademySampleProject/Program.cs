using AcademySampleProject;
using Microsoft.EntityFrameworkCore;
using Prometheus;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddJsonFile("appsecrets.json");

// Add services to the container.
builder.Services.AddDbContext<BloggingContext>(options =>
   options.UseSqlServer(builder.Configuration.GetConnectionString("BloggingDatabase")));

builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());
    c.IgnoreObsoleteActions();
    c.IgnoreObsoleteProperties();
    c.CustomSchemaIds(type => type.FullName);
});
builder.Services.AddHealthChecks().AddDbContextCheck<BloggingContext>();

var app = builder.Build();

using (var serviceScope = app.Services.CreateScope())
{
    var context = serviceScope.ServiceProvider.GetRequiredService<BloggingContext>();
    context.Database.EnsureCreated();
    context.Database.Migrate();
}

var basePath = app.Configuration.GetValue<string>("ASPNETCORE_APPL_PATH")?.Trim('/').ToLower();
if (!string.IsNullOrWhiteSpace(basePath))
    app.UsePathBase($"/{basePath}");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapHealthChecks("/health");
app.UseMetricServer();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();
app.Run();