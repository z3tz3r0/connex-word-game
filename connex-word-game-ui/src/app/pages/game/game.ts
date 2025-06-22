import { CommonModule } from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { Component, inject, OnInit } from '@angular/core';
import {
  FormBuilder,
  FormGroup,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';

@Component({
  selector: 'app-game',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './game.html',
  styleUrl: './game.css',
})
export class Game implements OnInit {
  private http = inject(HttpClient);
  private fb = inject(FormBuilder);
  private apiUrl = 'http://localhost:5012/api/words';

  wordForm: FormGroup;
  history: any[] = [];
  topPlayers: any[] = [];

  constructor() {
    this.wordForm = this.fb.group({
      word: ['', Validators.required],
    });
  }

  ngOnInit(): void {
    this.loadData();
  }

  loadData(): void {
    this.http.get<any[]>(`${this.apiUrl}/history`).subscribe((data) => {
      this.history = data;
    });

    this.http.get<any[]>(`${this.apiUrl}/top5`).subscribe((data) => {
      this.topPlayers = data;
    });
  }

  onWordSubmit(): void {
    if (this.wordForm.invalid) return;

    const wordData = { word: this.wordForm.value.word };
    this.http.post(this.apiUrl, wordData).subscribe({
      next: () => {
        alert('Word submitted successfully!');
        this.wordForm.reset();
        this.loadData();
      },
      error: (err) => {
        alert(err.error?.message || 'Failed to submit word.');
      },
    });
  }
}
