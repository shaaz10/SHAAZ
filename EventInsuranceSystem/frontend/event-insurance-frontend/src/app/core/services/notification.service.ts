import { Injectable, signal, computed, effect, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, of } from 'rxjs';
import { catchError, tap } from 'rxjs/operators';
import { environment } from '../../../environments/environment';
import { AuthService } from './auth.service';

// ==========================================
// NotificationService — Uses Angular Signals for State Management
// Manages notification state using Signals instead of RxJS BehaviorSubjects.
// Polls for new notifications when user is logged in and exposes signal-based state.
// ==========================================
@Injectable({
    providedIn: 'root'
})
export class NotificationService {
    private apiUrl = `${environment.apiUrl}/notification`;
    private http = inject(HttpClient);
    private authService = inject(AuthService);
    private pollingInterval: any = null;

    // Signal-based state management — replaces BehaviorSubject
    notifications = signal<any[]>([]);
    unreadCount = computed(() => this.notifications().filter(n => !n.isRead).length);

    constructor() {
        // Use effect to watch authService.currentUser signal and start/stop polling
        effect(() => {
            const user = this.authService.currentUser();
            if (user) {
                this.startPolling();
            } else {
                this.stopPolling();
                this.notifications.set([]);
            }
        });
    }

    private startPolling() {
        this.refreshNotifications();
        this.stopPolling(); // Clear any existing interval
        this.pollingInterval = setInterval(() => {
            this.refreshNotifications();
        }, 30000); // Poll every 30 seconds
    }

    private stopPolling() {
        if (this.pollingInterval) {
            clearInterval(this.pollingInterval);
            this.pollingInterval = null;
        }
    }

    public refreshNotifications() {
        this.http.get<any[]>(this.apiUrl).pipe(
            catchError(() => of([]))
        ).subscribe(notifications => {
            this.notifications.set(notifications);
        });
    }

    markAsRead(id: number): Observable<any> {
        return this.http.post(`${this.apiUrl}/${id}/read`, {}).pipe(
            tap(() => this.refreshNotifications())
        );
    }

    markAllAsRead(): Observable<any> {
        return this.http.post(`${this.apiUrl}/read-all`, {}).pipe(
            tap(() => this.refreshNotifications())
        );
    }

    deleteNotification(id: number): Observable<any> {
        return this.http.delete(`${this.apiUrl}/${id}`).pipe(
            tap(() => this.refreshNotifications())
        );
    }
}
