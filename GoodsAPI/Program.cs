using Microsoft.EntityFrameworkCore;
using GoodsAPI.Data;
using System.Configuration;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// The string can be changed to "SqlServer" and then you can edit the appsettings.json connection string
//or just add the connection string directly here
builder.Services.AddDbContext<GoodsContext>(
    o=> o.UseSqlServer("Server=localhost;Database=goodsDB;Trusted_Connection=True;"));


var app = builder.Build();

// Configure the HTTP request pipeline.
if (builder.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();

}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();