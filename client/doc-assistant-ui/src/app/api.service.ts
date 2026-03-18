import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class ApiService {
  // Hier die URL deines C#-Backends (Port 5220 haben wir vorhin im Terminal gesehen)
  private baseUrl = 'http://localhost:5220/api/docs';

  constructor(private http: HttpClient) { }

  // Holt die Liste der PDFs
  scanDocuments(): Observable<any[]> {
    return this.http.get<any[]>(`${this.baseUrl}/scan`);
  }

  // Schickt die Frage an Ollama
  askQuestion(question: string, fileName: string): Observable<any> {
    return this.http.post<any>(`${this.baseUrl}/ask`, { 
      question: question, 
      fileName: fileName 
    });
  }
}