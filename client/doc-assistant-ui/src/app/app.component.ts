import { Component } from '@angular/core';
import { CommonModule } from '@angular/common'; // Wichtig für [ngClass]
import { FormsModule } from '@angular/forms'; // Wichtig für [(ngModel)]
import { ApiService } from './api.service';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [CommonModule, FormsModule], // Diese beiden müssen hier stehen!
  templateUrl: './app.component.html',
  styleUrl: './app.component.scss'
})
export class AppComponent {
  documents: any[] = [];
  chatHistory: {role: string, text: string}[] = [];
  currentQuestion: string = '';
  loading: boolean = false;

  constructor(private api: ApiService) {}

  loadDocs() {
    this.api.scanDocuments().subscribe({
      next: (data) => {
        this.documents = data;
        console.log('Docs geladen:', data);
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