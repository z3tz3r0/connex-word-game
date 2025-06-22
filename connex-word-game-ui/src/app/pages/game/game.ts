import { CommonModule } from '@angular/common';
import { Component, inject, OnInit } from '@angular/core';
import {
  FormBuilder,
  FormGroup,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { Auth } from '../../services/auth';
import { Words } from '../../services/words';

@Component({
  selector: 'app-game',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './game.html',
  styleUrl: './game.css',
})
export class Game implements OnInit {
  private wordsService = inject(Words);
  private fb = inject(FormBuilder);
  private apiUrl = 'http://localhost:5012/api/words';
  authService = inject(Auth);

  wordForm: FormGroup;
  history: any[] = [];
  topPlayers: any[] = [];
  submissionError: string | null = null;

  constructor() {
    this.wordForm = this.fb.group({
      word: ['', [Validators.required, Validators.pattern(/^[a-zA-Z]+$/)]],
    });
  }

  ngOnInit(): void {
    this.loadData();
  }

  loadData(): void {
    this.wordsService.getHistory().subscribe((data) => {
      console.log('Received history data:', data);
      this.history = data;
    });

    this.wordsService.getTop5().subscribe((data) => {
      this.topPlayers = data;
    });
  }

  onWordSubmit(): void {
    if (this.wordForm.invalid) return;
    this.submissionError = null;

    const wordData = { word: this.wordForm.value.word };
    this.wordsService.addWord({ word: this.wordForm.value.word }).subscribe({
      next: () => {
        this.wordForm.reset();
        this.loadData();
      },
      error: (err) => {
        // alert(err.error?.message || 'Failed to submit word.');
        this.submissionError = err.error?.message || 'Failed to submit word.';
      },
    });
  }

  onDelete(id: number): void {
    console.log('Attempting to delete ID:', id);
    if (confirm('are you sure?')) {
      this.wordsService.deleteWord(id).subscribe({
        next: () => {
          this.loadData();
        },
        error: (err) => {
          this.submissionError = err.error?.message || 'Failed to delete word.';
        },
      });
    }
  }

  onEdit(item: any): void {
    const newWord = prompt('enter the new word: ', item.word);
    if (newWord && newWord.trim() !== '') {
      this.wordsService.editWord(item.id, { word: newWord }).subscribe({
        next: () => this.loadData(),
        error: (err) => alert(err.error?.message || 'Failed to edit word.'),
      });
    }
  }
}
