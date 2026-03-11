import { Injectable, signal, computed } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../environments/environment';
import { Router } from '@angular/router';
import { Observable } from 'rxjs';
import { map, tap } from 'rxjs/operators';

// ==========================================
// AuthService — Uses Angular Signals for State Management
// Manages user authentication state using Signals instead of RxJS BehaviorSubjects.
// Handles login, logout, registration, JWT parsing, and role checking.
// ==========================================
@Injectable({
    providedIn: 'root'
})
export class AuthService {
    private apiUrl = environment.apiUrl;

    // Signal-based state management — replaces BehaviorSubject
    currentUser = signal<any>(null);

    // Computed signals for derived state
    isLoggedIn = computed(() => this.currentUser() !== null);
    currentUserId = computed<number | null>(() => {
        const user = this.currentUser();
        if (!user) return null;
        const idStr =
            user['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier'] ||
            user.nameid ||
            user.sub;
        return idStr ? parseInt(idStr, 10) : null;
    });
    currentUserRole = computed<string | null>(() => {
        const user = this.currentUser();
        if (!user) return null;
        return user['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'] || user.role || null;
    });

    constructor(private http: HttpClient, private router: Router) {
        this.initUser();
    }

    private initUser() {
        try {
            const token = localStorage.getItem('token');
            if (token) {
                const decoded = this.parseJwt(token);
                if (decoded) {
                    this.currentUser.set({ ...decoded, token });
                }
            }
        } catch (e) {
            console.error('initUser error', e);
        }
    }

    /**
     * Sends login request, stores JWT token, updates the currentUser signal.
     */
    login(dto: { email: string; password: string }): Observable<any> {
        return this.http.post<{ token: string }>(`${this.apiUrl}/auth/login`, dto).pipe(
            map((res) => {
                const token = res.token;
                if (!token) throw new Error('No token in response');

                const decoded = this.parseJwt(token);
                if (!decoded) throw new Error('Could not decode token');

                localStorage.setItem('token', token);
                this.currentUser.set({ ...decoded, token });
                return decoded;
            })
        );
    }

    logout() {
        localStorage.removeItem('token');
        this.currentUser.set(null);
        this.router.navigate(['/login']);
    }

    register(dto: any): Observable<any> {
        return this.http.post(`${this.apiUrl}/auth/register`, dto);
    }

    createUser(dto: any): Observable<any> {
        return this.http.post(`${this.apiUrl}/auth/create-user`, dto);
    }

    getToken() {
        return localStorage.getItem('token');
    }

    get currentUserValue() {
        return this.currentUser();
    }

    hasRole(role: string): boolean {
        const user = this.currentUser();
        if (!user) return false;
        const userRole =
            user['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'] ||
            user.role;
        return userRole === role;
    }

    private parseJwt(token: string): any {
        try {
            const base64Url = token.split('.')[1];
            const base64 = base64Url.replace(/-/g, '+').replace(/_/g, '/');
            const jsonPayload = decodeURIComponent(
                atob(base64).split('').map(c => '%' + ('00' + c.charCodeAt(0).toString(16)).slice(-2)).join('')
            );
            return JSON.parse(jsonPayload);
        } catch (e) {
            return null;
        }
    }
}
