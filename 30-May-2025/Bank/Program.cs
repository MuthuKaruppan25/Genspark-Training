
using SecondWebApi.Contexts;
using Microsoft.EntityFrameworkCore;
using BankApi.Interfaces;
using BankApi.Models;
using BankApi.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();
builder.Services.AddDbContext<BankContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
});


builder.Services.AddControllers().AddJsonOptions(options =>
 {
     options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
     options.JsonSerializerOptions.WriteIndented = true;
 });


builder.Services.AddScoped<IRepository<string, Branch>, BranchRepository>();
builder.Services.AddScoped<IRepository<string, Account>, AccountRepository>();
builder.Services.AddScoped<IAccountTransactionService, AccountTransactionService>();
builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<IBranchService, BranchService>();
builder.Services.AddScoped<ITransactionService, TransactionService>();

var app = builder.Build();



if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();




app.Run();
