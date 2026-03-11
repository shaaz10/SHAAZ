import { Component, signal, inject } from '@angular/core';
import { RouterOutlet, RouterModule } from '@angular/router';
import { DatePipe } from '@angular/common';
import { AuthService } from './core/services/auth.service';
import { NotificationService } from './core/services/notification.service';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet, RouterModule, DatePipe],
  templateUrl: './app.html',
  styleUrl: './app.css'
})
export class App {
  authService = inject(AuthService);
  notificationService = inject(NotificationService);
  protected readonly title = signal('event-insurance-frontend');
  showNotifications = false;
  mobileMenuOpen = false;

  logout() {
    this.authService.logout();
  }

  toggleNotifications() {
    this.showNotifications = !this.showNotifications;
    if (this.showNotifications) {
      this.notificationService.refreshNotifications();
    }
  }

  markAsRead(eventOrId: any, id?: number) {
    let notificationId: number;
    if (id !== undefined) {
      if (eventOrId && typeof eventOrId.stopPropagation === 'function') {
        eventOrId.stopPropagation();
      }
      notificationId = id;
    } else {
      notificationId = eventOrId;
    }
    this.notificationService.markAsRead(notificationId).subscribe(() => {
      this.notificationService.deleteNotification(notificationId).subscribe();
    });
  }

  markAllAsRead() {
    this.notificationService.markAllAsRead().subscribe();
  }

  deleteNotification(event: Event, id: number) {
    event.stopPropagation();
    this.notificationService.deleteNotification(id).subscribe();
  }
}
