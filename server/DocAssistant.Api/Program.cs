var builder = WebApplication.CreateBuilder(args);

// 1. SERVICES REGISTRIEREN
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Services
builder.Services.AddScoped<DocumentService>();
builder.Services.AddHttpClient<ChatService>();


// HIER kommt dein CORS-Code hin (muss vor builder.Build() stehen!)
builder.Services.AddCors(options => 
{
    options.AddPolicy("AllowAngular",
        policy => policy.WithOrigins("http://localhost:4200") // Angular Standard-Port
                        .AllowAnyMethod()
                        .AllowAnyHeader());
});


// Jetzt wird die App gebaut
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();


app.UseCors("AllowAngular");
app.Run();
