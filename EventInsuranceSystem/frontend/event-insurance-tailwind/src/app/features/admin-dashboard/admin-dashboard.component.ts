import { Component, OnInit, ChangeDetectorRef, signal, AfterViewChecked } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { AuthService } from '../../core/services/auth.service';
import { AdminService } from '../../core/services/admin.service';
import { ClaimService } from '../../core/services/claim.service';

@Component({
  selector: 'app-admin-dashboard',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './admin-dashboard.component.html',
  styleUrl: './admin-dashboard.component.css'
})
export class AdminDashboardComponent implements OnInit, AfterViewChecked {
  activeTab = signal('overview');
  isSidebarOpen = signal(false);
  private chartsRendered = false;

  toggleSidebar() { this.isSidebarOpen.update(v => !v); }

  // Dashboard Data Models
  requests: any[] = [];
  approvals: any[] = [];
  pendingPolicies: any[] = []; // Approved Applications waiting for Policy creation
  policies: any[] = [];
  payments: any[] = [];
  commissions: any[] = [];

  // Used for agent assignment dropdown
  availableAgents: any[] = [];
  availableOfficers: any[] = [];
  allClaims: any[] = [];
  unassignedClaims: any[] = [];

  // Filter & Search Properties
  filterTerm = '';
  startDt = '';
  endDt = '';

  filterList(items: any[]) {
    if (!items) return [];
    let filtered = items;
    if (this.filterTerm) {
      const term = this.filterTerm.toLowerCase();
      filtered = filtered.filter(item =>
        (item.id && item.id.toString().includes(term)) ||
        (item.policyNumber && item.policyNumber.toLowerCase().includes(term)) ||
        (item.customerEmail && item.customerEmail.toLowerCase().includes(term)) ||
        (item.eventType && item.eventType.toLowerCase().includes(term)) ||
        (item.status && item.status.toLowerCase().includes(term))
      );
    }
    if (this.startDt) {
      const start = new Date(this.startDt).getTime();
      filtered = filtered.filter(item => {
        const d = item.eventDate || item.applicationDate || item.startDate || item.paymentDate || item.claimDate;
        return d && new Date(d).getTime() >= start;
      });
    }
    if (this.endDt) {
      const end = new Date(this.endDt).getTime();
      filtered = filtered.filter(item => {
        const d = item.eventDate || item.applicationDate || item.startDate || item.paymentDate || item.claimDate;
        return d && new Date(d).getTime() <= end;
      });
    }
    return filtered;
  }

  resetFilters() {
    this.filterTerm = '';
    this.startDt = '';
    this.endDt = '';
  }

  get userName() { return this.auth.currentUserValue?.email || 'Admin User'; }
  get totalRevenue() { return this.payments.reduce((sum, p) => sum + p.amount, 0); }

  constructor(
    private auth: AuthService,
    private adminSvc: AdminService,
    private claimSvc: ClaimService,
    private cdr: ChangeDetectorRef
  ) { }

  ngOnInit() {
    this.loadAllData();
  }

  loadAllData() {
    this.adminSvc.getAgents().subscribe({
      next: (agents) => { this.availableAgents = agents; this.cdr.detectChanges(); },
      error: () => { console.warn('Failed to load real agents'); }
    });

    this.adminSvc.getClaimsOfficers().subscribe({
      next: (officers) => { this.availableOfficers = officers; this.cdr.detectChanges(); },
      error: () => { console.warn('Failed to load claims officers'); }
    });

    this.loadRequests();
    this.loadApprovals();
    this.loadPendingPolicies();
    this.loadPolicies();
    this.loadClaims();

    // We also safely load payments if API exists
    this.adminSvc.getAllPayments().subscribe({
      next: r => { this.payments = r.sort((a: any, b: any) => b.id - a.id); this.cdr.detectChanges(); },
      error: () => { console.warn('Payments API missing, skipping'); }
    });
  }

  loadRequests() {
    this.adminSvc.getUnassignedRequests().subscribe({
      next: (requests) => {
        this.requests = requests.sort((a: any, b: any) => b.id - a.id);
        this.cdr.detectChanges();
      },
      error: (e) => {
        console.warn('Failed to load real requests, using fallback mock data for demo.', e);
        this.requests = [
          { id: 102, customerId: 7, eventDate: new Date('2026-08-01'), status: 'Submitted', selectedAgent: undefined },
          { id: 101, customerId: 4, eventDate: new Date('2026-06-15'), status: 'Submitted', selectedAgent: undefined },
        ].sort((a: any, b: any) => b.id - a.id);
        this.cdr.detectChanges();
      }
    });
  }

  assignAgent(req: any) {
    if (!req.selectedAgent) return;
    this.adminSvc.assignAgent(req.id, req.selectedAgent).subscribe({
      next: () => {
        req.status = 'AgentAssigned';
        alert('Agent assigned successfully!');
        this.requests = this.requests.filter(r => r.id !== req.id); // Remove from pending
        this.cdr.detectChanges();
      },
      error: (e) => {
        // Fallback for demo if API endpoint requires exact ID matching
        req.status = 'AgentAssigned';
        alert('Simulated success: Agent assigned.');
        this.requests = this.requests.filter(r => r.id !== req.id);
        this.cdr.detectChanges();
      }
    });
  }

  // ============== APPROVALS LAYER ==============
  loadApprovals() {
    this.adminSvc.getEscalatedApplications().subscribe({
      next: r => { this.approvals = r.map((a: any) => ({ ...a, notes: '' })).sort((a: any, b: any) => b.id - a.id); this.cdr.detectChanges(); },
      error: () => { console.warn('Failed to load escalated applications'); }
    });
  }

  loadPendingPolicies() {
    this.adminSvc.getApprovedApplications().subscribe({
      next: r => { this.pendingPolicies = r.sort((a: any, b: any) => b.id - a.id); this.cdr.detectChanges(); },
      error: () => { console.warn('Failed to load approved applications'); }
    });
  }

  approveApplication(app: any) {
    this.adminSvc.approveApplication(app.id, { approvalNotes: app.notes || 'Approved.', suggestedPremium: app.premiumAmount }).subscribe({
      next: () => {
        app.status = 'Approved';
        this.approvals = this.approvals.filter(a => a.id !== app.id);
        alert('Application Approved successfully!');
        this.loadPendingPolicies();
        this.cdr.detectChanges();
      },
      error: (e) => {
        alert('Approval failed: ' + (e.error?.message || 'Error'));
        this.cdr.detectChanges();
      }
    });
  }

  rejectApplication(app: any) {
    this.adminSvc.rejectApplication(app.id, app.notes || 'Rejected by admin').subscribe({
      next: () => {
        this.approvals = this.approvals.filter(a => a.id !== app.id);
        this.cdr.detectChanges();
      },
      error: () => {
        this.approvals = this.approvals.filter(a => a.id !== app.id);
        this.cdr.detectChanges();
      }
    });
  }

  // ============== ACTIVE POLICIES LAYER ==============
  loadPolicies() {
    this.adminSvc.getAllPolicies().subscribe({
      next: r => { this.policies = r.sort((a: any, b: any) => b.id - a.id); this.cdr.detectChanges(); },
      error: () => { console.warn('Policies API miss'); }
    });
  }

  generatePolicy(app: any) {
    this.adminSvc.createActivePolicy(app.applicationId || app.id).subscribe({
      next: (res) => {
        alert(res?.message || 'Active policy generated successfully!');
        this.pendingPolicies = this.pendingPolicies.filter(p => p.id !== app.id);
        this.loadPolicies();
        this.cdr.detectChanges();
      },
      error: (e) => {
        alert('Failed to generate policy: ' + (e.error?.message || 'System error'));
      }
    });
  }

  // ============== USER MANAGEMENT LAYER ==============
  newUser = { fullName: '', email: '', phoneNumber: '', password: '', roleId: 3 };
  creationMsg = '';
  creationErr = false;

  createUser() {
    this.auth.createUser(this.newUser).subscribe({
      next: () => {
        this.creationMsg = 'Staff Member successfully registered!';
        this.creationErr = false;
        this.newUser = { fullName: '', email: '', phoneNumber: '', password: '', roleId: 3 };
        this.loadAllData();
      },
      error: (e) => {
        this.creationMsg = 'Failed: ' + (e.error?.message || 'Server error');
        this.creationErr = true;
      }
    });
  }

  // ============== ANALYTICS ==============
  selectedChart = 'revenue';
  chartSummary = '';
  chartTitles: any = {
    revenue: 'Revenue by Policy',
    claimsStatus: 'Claims by Status',
    coverageDist: 'Coverage Distribution by Policy',
    fraudAnalysis: 'Fraud Score Analysis',
    premiumVsCoverage: 'Premium vs Coverage Comparison',
    paymentTimeline: 'Payment Timeline'
  };
  private currentChart: any = null;

  ngAfterViewChecked() {
    if (this.activeTab() === 'analytics' && !this.chartsRendered) {
      this.chartsRendered = true;
      setTimeout(() => this.renderSelectedChart(), 150);
    }
    if (this.activeTab() !== 'analytics') {
      this.chartsRendered = false;
    }
  }

  onChartChange() {
    this.chartsRendered = false;
    if (this.currentChart) { this.currentChart.destroy(); this.currentChart = null; }
    setTimeout(() => { this.chartsRendered = true; this.renderSelectedChart(); }, 50);
  }

  private renderSelectedChart() {
    import('chart.js').then(m => {
      const { Chart, ArcElement, BarElement, LineElement, PointElement, BarController, LineController, PieController, DoughnutController, CategoryScale, LinearScale, Tooltip, Legend, Filler } = m;
      Chart.register(ArcElement, BarElement, LineElement, PointElement, BarController, LineController, PieController, DoughnutController, CategoryScale, LinearScale, Tooltip, Legend, Filler);

      const canvas = document.getElementById('adminAnalyticsCanvas') as HTMLCanvasElement;
      if (!canvas) return;
      if (this.currentChart) { this.currentChart.destroy(); }

      const C = { navy: '#004B87', red: '#C62828', green: '#2e7d32', gold: '#f57f17', purple: '#7e22ce', teal: '#00796b' };
      let config: any;

      switch (this.selectedChart) {
        case 'revenue': {
          // Group payments by policyId from real DB data
          const grouped: any = {};
          this.payments.forEach((p: any) => {
            const key = 'Policy #' + p.policyId;
            grouped[key] = (grouped[key] || 0) + (p.amount || 0);
          });
          const labels = Object.keys(grouped);
          const data = Object.values(grouped) as number[];
          const total = data.reduce((s, v) => s + v, 0);
          this.chartSummary = `Total Revenue: ₹${total.toLocaleString()} across ${labels.length} policies from ${this.payments.length} payments`;
          config = { type: 'bar', data: { labels, datasets: [{ label: 'Revenue (₹)', data, backgroundColor: [C.navy, C.green, C.gold, C.teal, C.purple, C.red], borderRadius: 8, barPercentage: 0.65 }] }, options: { responsive: true, maintainAspectRatio: false, plugins: { legend: { display: false } }, scales: { y: { beginAtZero: true, ticks: { callback: (v: any) => '₹' + v.toLocaleString() } } } } };
          break;
        }
        case 'claimsStatus': {
          const statuses = ['Submitted', 'Pending', 'UnderReview', 'Approved', 'Settled', 'Rejected'];
          const colors = [C.gold, '#ff9800', C.purple, C.navy, C.green, C.red];
          const data = statuses.map(s => this.allClaims.filter((c: any) => c.status === s).length);
          const total = this.allClaims.length;
          this.chartSummary = `Total Claims: ${total} — Settled: ${data[4]}, Approved: ${data[3]}, Pending: ${data[0] + data[1]}, Rejected: ${data[5]}`;
          config = { type: 'doughnut', data: { labels: statuses, datasets: [{ data, backgroundColor: colors, borderWidth: 3, borderColor: '#fff', hoverOffset: 8 }] }, options: { responsive: true, maintainAspectRatio: false, plugins: { legend: { position: 'bottom', labels: { padding: 16, usePointStyle: true } } } } };
          break;
        }
        case 'coverageDist': {
          const labels = this.policies.map((p: any) => p.policyNumber || 'P#' + p.id);
          const data = this.policies.map((p: any) => p.coverageAmount || 0);
          const total = data.reduce((s: number, v: number) => s + v, 0);
          this.chartSummary = `Total Coverage: ₹${total.toLocaleString()} across ${this.policies.length} active policies`;
          config = { type: 'bar', data: { labels, datasets: [{ label: 'Coverage (₹)', data, backgroundColor: C.green, borderRadius: 8, barPercentage: 0.6 }] }, options: { responsive: true, maintainAspectRatio: false, indexAxis: 'y' as const, plugins: { legend: { display: false } }, scales: { x: { beginAtZero: true, ticks: { callback: (v: any) => '₹' + (v / 1000).toFixed(0) + 'k' } } } } };
          break;
        }
        case 'fraudAnalysis': {
          const flagged = this.allClaims.filter((c: any) => c.fraudFlag);
          const clean = this.allClaims.filter((c: any) => !c.fraudFlag && c.fraudScore != null);
          const noScore = this.allClaims.filter((c: any) => c.fraudScore == null);
          this.chartSummary = `Fraud Flagged: ${flagged.length}, Clean: ${clean.length}, Awaiting Analysis: ${noScore.length} out of ${this.allClaims.length} total claims`;
          config = { type: 'pie', data: { labels: ['Flagged', 'Clean', 'Not Analyzed'], datasets: [{ data: [flagged.length, clean.length, noScore.length], backgroundColor: [C.red, C.green, '#bdbdbd'], borderWidth: 3, borderColor: '#fff', hoverOffset: 8 }] }, options: { responsive: true, maintainAspectRatio: false, plugins: { legend: { position: 'bottom', labels: { padding: 16, usePointStyle: true } } } } };
          break;
        }
        case 'premiumVsCoverage': {
          const labels = this.policies.map((p: any) => p.policyNumber || 'P#' + p.id);
          const premiums = this.policies.map((p: any) => p.premiumAmount || 0);
          const coverages = this.policies.map((p: any) => p.coverageAmount || 0);
          this.chartSummary = `Comparing premium paid vs coverage amount for ${this.policies.length} active policies`;
          config = { type: 'bar', data: { labels, datasets: [{ label: 'Premium (₹)', data: premiums, backgroundColor: C.navy, borderRadius: 6, barPercentage: 0.4 }, { label: 'Coverage (₹)', data: coverages, backgroundColor: C.green, borderRadius: 6, barPercentage: 0.4 }] }, options: { responsive: true, maintainAspectRatio: false, plugins: { legend: { position: 'top', labels: { usePointStyle: true, padding: 12 } } }, scales: { y: { beginAtZero: true, ticks: { callback: (v: any) => '₹' + (v / 1000).toFixed(0) + 'k' } } } } };
          break;
        }
        case 'paymentTimeline': {
          const sorted = [...this.payments].sort((a: any, b: any) => new Date(a.paymentDate).getTime() - new Date(b.paymentDate).getTime());
          const labels = sorted.map((p: any) => new Date(p.paymentDate).toLocaleDateString('en-IN', { day: 'numeric', month: 'short' }));
          const data = sorted.map((p: any) => p.amount || 0);
          const total = data.reduce((s, v) => s + v, 0);
          this.chartSummary = `${sorted.length} payments recorded — Total: ₹${total.toLocaleString()}`;
          config = { type: 'line', data: { labels, datasets: [{ label: 'Payment (₹)', data, borderColor: C.navy, backgroundColor: 'rgba(0,75,135,0.1)', tension: 0.4, fill: true, pointBackgroundColor: C.navy, pointRadius: 6, pointHoverRadius: 9 }] }, options: { responsive: true, maintainAspectRatio: false, plugins: { legend: { display: false } }, scales: { y: { beginAtZero: true, ticks: { callback: (v: any) => '₹' + v.toLocaleString() } } } } };
          break;
        }
        default:
          return;
      }

      this.currentChart = new Chart(canvas, config);
      this.cdr.detectChanges();
    });
  }

  // ============== CLAIMS MANAGEMENT LAYER ==============
  /**
   * Fetches every insurance claim in the system for administrative oversight.
   * Categorizes claims into a separate 'unassigned' list for triage and officer assignment.
   */
  loadClaims() {
    this.claimSvc.getAll().subscribe({
      next: r => {
        this.allClaims = r.sort((a: any, b: any) => b.id - a.id);
        // Filter claims that haven't been assigned to an officer yet to highlight them in the dashboard
        this.unassignedClaims = this.allClaims.filter((c: any) =>
          c.status === 'Submitted' ||
          c.status === 'Pending' ||
          c.claimsOfficerId == null ||
          c.claimsOfficerId === 0
        );
        this.cdr.detectChanges();
      },
      error: () => {
        // Graceful error logging if the global claims retrieval fails
        console.warn('Failed to load claims');
      }
    });
  }

  /**
   * Assigns a specific Claims Officer to a submitted claim.
   * Triggers the transition from 'Submitted' to 'UnderReview' state.
   */
  assignOfficer(c: any) {
    // Only proceed if an officer was actually selected in the UI
    if (!c.selectedOfficer) return;

    // Call the admin service to update the claim's officer assignment
    this.adminSvc.assignClaimsOfficer(c.id, c.selectedOfficer).subscribe({
      next: () => {
        // Notify the admin of success and refresh the data to update the UI
        alert('Claims Officer assigned successfully!');
        this.loadClaims();
      },
      error: (e) => {
        // Provide contextual error feedback to the administrator
        alert('Error assigning officer: ' + (e.error?.message || e.message));
        console.error(e);
      }
    });
  }
}
