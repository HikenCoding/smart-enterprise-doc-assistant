import { Component } from '@angular/core';
import { CommonModule } from '@angular/common'; // Wichtig für [ngClass]
import { FormsModule } from '@angular/forms'; // Wichtig für [(ngModel)]
import { ApiService } from './api.service'; // der eigene Service von mir der HTTP-Anfragen ans Backend schickt

@Component({
  selector: 'app-root', //HTML Tag, dass diese Komponente repräsentiert
  standalone: true, //Moderne Angular Variante ohne Module
  imports: [CommonModule, FormsModule], // Dadurch dass es Standalone ist, muss man angeben, was es braucht
  templateUrl: './app.component.html', //Verknüpft das HTML-Template
  styleUrl: './app.component.scss' //Verknüpft die SCSS-Datei
})
export class AppComponent {
  documents: any[] = [];
  chatHistory: {role: string, text: string}[] = [];
  currentQuestion: string = '';
  loading: boolean = false;

  constructor(private api: ApiService) {} //Injiziert den ApiService automatisch. 'api' Variable ist nur in dieser Klasse sichtbar

  loadDocs() {
    this.api.scanDocuments().subscribe({ //ruft meinen Backend die Methode 'scanDocuments' auf und mit subscribe startet den HTTP-Request
      next: (data) => { //'next wird ausgeführt, wenn Daten zurückkommen
        this.documents = data; //Die Daten werden in documents gespeichert
        console.log('Docs geladen:', data); //Anzeige in der Console, das Dokumente geladen wurden. Damit man auch debuggen kann und sieht was ankommt.
      },
      error: (err) => console.error('Fehler beim Scannen:', err)
    });
  }

  sendQuestion() {
    if (!this.currentQuestion.trim() || this.loading) return;

    const userQ = this.currentQuestion;
    this.chatHistory.push({ role: 'user', text: userQ });
    this.currentQuestion = '';
    this.loading = true;

    // Wir nehmen das erste Doc oder unser Test-PDF
    const docName = this.documents.length > 0 ? this.documents[0].fileName : 'vertrag_max_mustermann.pdf';

    this.api.askQuestion(userQ, docName).subscribe({
      next: (res) => {
        this.chatHistory.push({ role: 'assistant', text: res.answer });
        this.loading = false;
      },
      error: (err) => {
        this.chatHistory.push({ role: 'assistant', text: 'Backend nicht erreichbar. Läuft der C# Server?' });
        this.loading = false;
      }
    });
  }
}