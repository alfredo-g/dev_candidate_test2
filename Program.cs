using EmployeeProject;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);

//
// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddApiVersioning(options => {
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = true;
    options.ApiVersionReader = new UrlSegmentApiVersionReader();
});
builder.Services.AddVersionedApiExplorer(options => {
    options.GroupNameFormat = "'v'VVV";
    options.SubstituteApiVersionInUrl = true;
});

builder.Services.AddDbContext<AppDbContext>(options => options.UseInMemoryDatabase("Testing"));
//builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddTransient<EmployeeRepository>();

builder.Services.AddResponseCompression(options => options.EnableForHttps = true);

var app = builder.Build();

//
// Configure the HTTP request pipeline.

app.UseResponseCompression();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// NOTE: Im using this to host an example of a simple SPA, its just to setup a quick UI, you can switch 
// this simple implementation with a more complete SPA Framework like Angular, React, Svelte, etc.
app.UseDefaultFiles();
app.UseStaticFiles();

// NOTE: You need to add additional configuration to swagger if you have other versions a controller
app.UseApiVersioning();
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

using(var scope = app.Services.CreateScope()) 
    scope.ServiceProvider.GetRequiredService<AppDbContext>().Database.EnsureCreatedAsync().Wait();

app.Run();