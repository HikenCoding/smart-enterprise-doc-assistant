//Bauplan für die Anwendung und bereitet alles vor
var builder = WebApplication.CreateBuilder(args);

// 1. SERVICES REGISTRIEREN
// Ohne das können keine HTTP-Anfragen verstanden werden aus dem Internet
builder.Services.AddControllers();
//Hier erstellen wir einen Handbuch. Sind Werkzeuge (Swagger), die automatisch eine Webseite bauen.
//Zeigt welche Befehle beherrscht werden können.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


// Services
// Das ist wie ein Akten-Sortierer. Wenn jemand nach Dokumenten fragt, weiß es wo es den speziellen Mitarbeiten rufen kann
builder.Services.AddScoped<DocumentService>();
// Das ist ein Chat-Übersetzer und den geben wir ein Telefon. Ruft Ollama an um Antworten zu holen.
builder.Services.AddHttpClient<ChatService>();


//Sicherheitszettel für den Türsteher. Nur mein Freund, der auf Port 4200 wohnt darf mit mir reden.
//Nur er darf alles fragen und schicken.
builder.Services.AddCors(options => 
{
    options.AddPolicy("AllowAngular",
        policy => policy.WithOrigins("http://localhost:4200") // Angular Standard-Port
                        .AllowAnyMethod()
                        .AllowAnyHeader());
});


//Aus dem Bauplan wird ein echtes fertiges Gebäude. Ab hier können wir KEINE neuen Werkzeuge hinzufügen
var app = builder.Build();

//Hier werden Hausregeln festgelegt.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


//Alle Gäste müssen durch den Sicherheits-Check.
app.UseHttpsRedirection();
//Hier wird geprüft, ob man die Erlaubnis hat in den Aktenraum kann.
app.UseAuthorization();
//Wegweiser, wenn eine Nachricht für 'api/docs' kommt, weiß man in welchen Controller die Nachricht bringen muss.
app.MapControllers();

//Nur Angular wird akzeptiert.
app.UseCors("AllowAngular");
//Licht an und Roboter ist jetzt wach wartet das jemand klingelt.
app.Run();
