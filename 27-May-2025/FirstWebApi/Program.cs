var builder = WebApplication.CreateBuilder(args);


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();


builder.Services.AddSingleton<IRepository<int, Doctor>, DoctorRepository>();
builder.Services.AddSingleton<IRepository<int, Patient>, PatientRepository>();
builder.Services.AddSingleton<IRepository<int, Appointment>, AppointmentRepository>();


builder.Services.AddSingleton<DoctorService>();
builder.Services.AddSingleton<PatientService>();
builder.Services.AddSingleton<AppointmentService>();

var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();

app.Run();