import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { jwtDecode, JwtPayload } from 'jwt-decode';
import { Observable, tap } from 'rxjs';
import {
  LoginDto,
  LoginResponse,
  RegisterDto,
  RegisterResponse,
} from '../shared/interfaces/auth.interfaces';

@Injectable({
  providedIn: 'root',
})
export class Auth {
  private apiUrl = 'http://localhost:5012/api';

  constructor(private http: HttpClient) {}

  login(loginData: LoginDto): Observable<LoginResponse> {
    return this.http
      .post<LoginResponse>(`${this.apiUrl}/Users/login`, loginData)
      .pipe(tap((response) => this.setToken(response.token)));
  }

  register(userData: RegisterDto): Observable<RegisterResponse> {
    return this.http.post<RegisterResponse>(
      `${this.apiUrl}/Users/register`,
      userData
    );
  }

  isLoggedIn(): boolean {
    const token = this.getToken();
    if (!token) {
      return false;
    }
    try {
      const decodedToken: JwtPayload = jwtDecode(token);
      return decodedToken.exp ? decodedToken.exp * 1000 > Date.now() : true;
    } catch (error) {
      return false;
    }
  }

  getToken(): string | null {
    return localStorage.getItem('authToken');
  }

  getUsername(): string | null {
    const token = this.getToken();
    if (!token) {
      return null;
    }

    try {
      const payload = JSON.parse(atob(token.split('.')[1]));

      return payload.name;
    } catch (e) {
      console.error('Invalid token:', e);
      return null;
    }
  }

  logout(): void {
    localStorage.removeItem('authToken');
  }

  private setToken(token: string): void {
    localStorage.setItem('authToken', token);
  }
}
