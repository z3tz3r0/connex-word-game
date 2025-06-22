import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class Words {
  private http = inject(HttpClient);
  private apiUrl = 'http://localhost:5012/api/words';

  getHistory(): Observable<any[]> {
    return this.http.get<any[]>(`${this.apiUrl}/history`);
  }

  getTop5(): Observable<any[]> {
    return this.http.get<any[]>(`${this.apiUrl}/top5`);
  }

  addWord(wordData: { word: string }): Observable<any> {
    return this.http.post(this.apiUrl, wordData);
  }

  editWord(id: number, wordData: { word: string }): Observable<any> {
    return this.http.put(`${this.apiUrl}/${id}`, wordData);
  }

  deleteWord(id: number): Observable<any> {
    return this.http.delete(`${this.apiUrl}/${id}`);
  }
}
