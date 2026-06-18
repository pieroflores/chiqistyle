var builder = WebApplication.CreateBuilder(args);
// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// 🔥 Agregar CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngular", policy =>
    {
        policy.WithOrigins("http://localhost:4200", "http://localhost", "http://localhost:80") // ✅ Múltiples orígenes
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials(); // 👈 Agrega esto si usas cookies/auth
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// 🔥 Usar CORS antes de Authorization
app.UseCors("AllowAngular");

app.UseAuthorization();

app.MapControllers();
app.UseStaticFiles();

app.Run();