var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
        policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
});
builder.Services.AddControllers();
var app = builder.Build();




if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}
app.UseCors("AllowAll");


app.UseHttpsRedirection();

app.MapControllers();

app.Run();

