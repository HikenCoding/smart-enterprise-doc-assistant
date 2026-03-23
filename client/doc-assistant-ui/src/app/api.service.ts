// Zentrale Stelle, womit mein Angular mit C# kommuniziert

//Logik aus Komponente auslagern
//HTTP-Request zu bündeln
//Wiederverwendbare Funktionen bereitzustellen
//Komponente auch schlank zu halten


import { Injectable } from '@angular/core'; //Markiert Klasse als Service, damit die Klasse Injizieren kann von Angular
import { HttpClient } from '@angular/common/http'; //Ermöglicht HTTP-Request
import { Observable } from 'rxjs'; //Rückgabetyp für async Datenströme

//Injectable: Kann als Service verwendet und für andere Klassen injiziert werden
@Injectable({
  providedIn: 'root'//erstellt eine einzige Instanz dieses Services und ist global Verfügbar. Singleton-Service
})

export class ApiService {
  // Hier die URL deines C#-Backends (Port 5220 haben wir vorhin im Terminal gesehen)
  private baseUrl = 'http://localhost:5220/api/docs';

  constructor(private http: HttpClient) { }

  // Holt die Liste der PDFs (rückgabetyp is Oberservable, weil HTTP-Requests async sind)
  scanDocuments(): Observable<any[]> {
    return this.http.get<any[]>(`${this.baseUrl}/scan`);
  }

  // Schickt die Frage an Ollama + Dateiname ans Backend
  askQuestion(question: string, fileName: string): Observable<any> {
    return this.http.post<any>(`${this.baseUrl}/ask`, { 
      question: question, 
      fileName: fileName 
    });
  }
}


//Unterscheid zu Oberservable und Promises
//Promise ist wie ein einmaliger Anruf, liefert nur einen Wert, kann nicht abgebrochen werden.Stelle
//Oberservable ist wie ein Telefonat, liefert mehrere Werte, kann jederzeit gestoppt werden.