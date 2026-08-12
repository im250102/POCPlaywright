import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, BehaviorSubject } from 'rxjs';
import { tap } from 'rxjs/operators';
import { Router } from '@angular/router';
import { environment } from '../../environments/environment';

export interface AuthUser {
  id: string;
  name: string;
  email: string;
  role: string;
  token: string;
}

@Injectable({ providedIn: 'root' })
export class AuthService {
  private base = `${environment.apiUrl}/auth`;
  private userSubject = new BehaviorSubject<AuthUser | null>(null);
  user$ = this.userSubject.asObservable();

  constructor(private http: HttpClient, private router: Router) {
    this.restoreSession();
  }

  get user(): AuthUser | null {
    return this.userSubject.value;
  }

  get isLoggedIn(): boolean {
    const token = localStorage.getItem('auth_token');
    if (!token) {
      return false;
    }

    if (!this.isTokenValid(token)) {
      this.clearSession();
      return false;
    }

    return true;
  }

  register(name: string, email: string, password: string, confirmPassword: string): Observable<AuthUser> {
    return this.http.post<AuthUser>(`${this.base}/register`, { name, email, password, confirmPassword })
      .pipe(tap(user => this.setUser(user)));
  }

  login(email: string, password: string): Observable<AuthUser> {
    return this.http.post<AuthUser>(`${this.base}/login`, { email, password })
      .pipe(tap(user => this.setUser(user)));
  }

  logout(): void {
    this.clearSession();
    this.router.navigate(['/login']);
  }

  private restoreSession(): void {
    const token = localStorage.getItem('auth_token');
    if (!token || !this.isTokenValid(token)) {
      this.clearSession();
      return;
    }

    const storedUser = localStorage.getItem('auth_user');
    if (storedUser) {
      try {
        const parsedUser = JSON.parse(storedUser) as AuthUser;
        this.userSubject.next(parsedUser);
        return;
      } catch {
        localStorage.removeItem('auth_user');
      }
    }

    const decodedUser = this.decodeToken(token);
    if (decodedUser.id || decodedUser.email || decodedUser.name) {
      const fallbackUser: AuthUser = {
        id: decodedUser.id ?? '',
        name: decodedUser.name ?? '',
        email: decodedUser.email ?? '',
        role: decodedUser.role ?? 'Medico',
        token
      };
      this.setUser(fallbackUser);
      return;
    }

    this.clearSession();
  }

  private setUser(user: AuthUser): void {
    const normalizedUser: AuthUser = {
      ...user,
      token: user.token
    };

    localStorage.setItem('auth_user', JSON.stringify(normalizedUser));
    localStorage.setItem('auth_token', normalizedUser.token);
    this.userSubject.next(normalizedUser);
  }

  private clearSession(): void {
    localStorage.removeItem('auth_user');
    localStorage.removeItem('auth_token');
    this.userSubject.next(null);
  }

  private isTokenValid(token: string): boolean {
    try {
      const payload = token.split('.')[1];
      const normalizedPayload = payload.replace(/-/g, '+').replace(/_/g, '/');
      const decodedPayload = JSON.parse(atob(normalizedPayload));

      if (typeof decodedPayload.exp === 'number') {
        return Date.now() < decodedPayload.exp * 1000;
      }

      return true;
    } catch {
      return false;
    }
  }

  private decodeToken(token: string): Partial<AuthUser> {
    try {
      const payload = token.split('.')[1];
      const normalizedPayload = payload.replace(/-/g, '+').replace(/_/g, '/');
      const decodedPayload = JSON.parse(atob(normalizedPayload));

      return {
        id: decodedPayload.nameid ?? decodedPayload.sub ?? decodedPayload.id ?? '',
        name: decodedPayload.name ?? '',
        email: decodedPayload.email ?? '',
        role: decodedPayload.role ?? 'Medico'
      };
    } catch {
      return {};
    }
  }
}
