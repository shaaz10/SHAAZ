import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { AgentPortalService } from '../../core/services/agent-portal.service';
import { AuthService } from '../../core/services/auth.service';

@Component({
  selector: 'app-agent-portal',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './agent-portal.component.html',
  styleUrl: './agent-portal.component.scss'})
export class AgentPortalComponent implements OnInit {
  loading = false;
  applications: any[] = [];

  constructor(
    private agentService: AgentPortalService,
    private authService: AuthService,
    private cdr: ChangeDetectorRef
  ) { }

  ngOnInit() {
    if (this.isAgent()) {
      this.loadPendingApplications();
    }
  }

  isAgent(): boolean {
    return this.authService.hasRole('Agent') || this.authService.hasRole('Admin');
  }

  loadPendingApplications() {
    this.loading = true;
    this.cdr.detectChanges();
    this.agentService.getApplicationsByStatus('Submitted').subscribe({
      next: (res) => {
        this.applications = res;
        this.loading = false;
        this.cdr.detectChanges();
      },
      error: (err) => {
        this.loading = false;
        this.cdr.detectChanges();
      }
    });
  }

  review(app: any, status: 'Approved' | 'Rejected') {
    this.loading = true;
    this.cdr.detectChanges();

    const agentId = this.authService.currentUserId();
    if (!agentId) {
      alert("Authentication error: Could not identify agent.");
      this.loading = false;
      this.cdr.detectChanges();
      return;
    }

    const payload = {
      isApproved: status === 'Approved',
      agentNotes: app.notes || ''
    };

    this.agentService.reviewApplication(app.id, agentId, payload).subscribe({
      next: (res) => {
        alert('Application ' + status + ' successfully!');
        this.applications = this.applications.filter(a => a.id !== app.id);
        this.loading = false;
        this.cdr.detectChanges();
      },
      error: (err) => {
        alert('Error reviewing application');
        this.loading = false;
        this.cdr.detectChanges();
      }
    });
  }
}
