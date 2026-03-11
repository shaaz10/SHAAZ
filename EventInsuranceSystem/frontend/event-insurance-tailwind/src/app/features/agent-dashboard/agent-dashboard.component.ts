import { Component, OnInit, ChangeDetectorRef, signal, AfterViewChecked } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { AuthService } from '../../core/services/auth.service';
import { AgentService } from '../../core/services/agent.service';

@Component({
  selector: 'app-agent-dashboard',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './agent-dashboard.component.html',
  styleUrl: './agent-dashboard.component.css'
})
export class AgentDashboardComponent implements OnInit, AfterViewChecked {
  tab = signal('requests');
  isSidebarOpen = signal(false);
  private agentChartsRendered = false;
  loading = signal(false);

  toggleSidebar() { this.isSidebarOpen.update(v => !v); }
  requests: any[] = [];
  applications: any[] = [];
  policies: any[] = [];
  commissions: any[] = [];
  policyProducts: any[] = [];

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

  get userName() {
    const u = this.auth.currentUserValue;
    return u?.email || 'Agent';
  }

  get totalCommission() {
    return this.commissions.reduce((sum: number, c: any) => sum + (c.commissionAmount || 0), 0);
  }

  constructor(private auth: AuthService, private agentSvc: AgentService, private cdr: ChangeDetectorRef) { }

  ngOnInit() {
    this.loadRequests();
    this.loadApplications();
    this.loadPolicies();
    this.loadCommissions();
    this.loadProducts();
  }

  loadProducts() {
    this.agentSvc.getPolicyProducts().subscribe({
      next: r => { this.policyProducts = r; this.cdr.detectChanges(); },
      error: () => { }
    });
  }

  loadRequests() {
    this.agentSvc.getMyRequests().subscribe({
      next: r => {
        // Filter out requests where the customer has already selected a policy
        const activeRequests = r.filter((x: any) => x.status !== 'PolicySelected');
        this.requests = activeRequests.map((x: any) => ({ ...x, _selectedProduct: null, _suggestedPremium: null, _msg: '', _err: false, _suggestions: [] })).sort((a: any, b: any) => b.id - a.id);
        // Load suggestions for each request
        this.requests.forEach(req => {
          this.agentSvc.getSuggestionsByRequest(req.id).subscribe({
            next: s => { req._suggestions = s; this.cdr.detectChanges(); },
            error: () => { }
          });
        });
        this.cdr.detectChanges();
      },
      error: () => { }
    });
  }

  loadApplications() {
    this.agentSvc.getMyApplications().subscribe({
      next: r => {
        // Filter out applications that are no longer actionable by the agent
        const activeApps = r.filter((a: any) =>
          a.status !== 'Approved' &&
          a.status !== 'Escalated' &&
          a.status !== 'Rejected' &&
          a.status !== 'AgentApproved'
        );
        this.applications = activeApps.map((a: any) => ({ ...a, _notes: '', _msg: '', _err: false })).sort((a: any, b: any) => b.id - a.id);
        this.cdr.detectChanges();
      },
      error: () => { }
    });
  }

  loadPolicies() {
    const id = this.auth.currentUserId();
    if (!id) return;
    this.agentSvc.getMyPolicies(id).subscribe({
      next: r => { this.policies = r.sort((a: any, b: any) => b.id - a.id); this.cdr.detectChanges(); },
      error: () => { }
    });
  }

  loadCommissions() {
    const id = this.auth.currentUserId();
    if (!id) return;
    this.agentSvc.getMyCommissions(id).subscribe({
      next: r => { this.commissions = r.sort((a: any, b: any) => b.id - a.id); this.cdr.detectChanges(); },
      error: () => { }
    });
  }

  suggestPolicy(req: any) {
    if (!req._selectedProduct || !req._suggestedPremium) return;

    if (req._suggestedPremium <= 0) {
      req._msg = '❌ Premium must be greater than zero.';
      req._err = true;
      return;
    }
    this.loading.set(true);
    this.agentSvc.suggestPolicy({
      requestId: req.id,
      policyProductId: req._selectedProduct,
      suggestedPremium: req._suggestedPremium
    }).subscribe({
      next: () => {
        req._msg = '✅ Policy suggested successfully!';
        req._err = false;
        this.loading.set(false);
        // Reload suggestions
        this.agentSvc.getSuggestionsByRequest(req.id).subscribe({
          next: s => { req._suggestions = s; this.cdr.detectChanges(); },
          error: () => { }
        });
        this.cdr.detectChanges();
      },
      error: e => {
        req._msg = '❌ ' + (e.error?.message || 'Failed to suggest.');
        req._err = true;
        this.loading.set(false);
        this.cdr.detectChanges();
      }
    });
  }

  reviewApp(app: any, isApproved: boolean) {
    this.loading.set(true);
    this.agentSvc.reviewApplication(app.id, { isApproved, reviewNotes: app._notes || '' }).subscribe({
      next: () => {
        app._msg = isApproved ? '✅ Application approved & sent to Admin!' : '❌ Application rejected.';
        app._err = false;
        this.loading.set(false);
        this.loadApplications();
        this.cdr.detectChanges();
      },
      error: e => {
        app._msg = '❌ ' + (e.error?.message || 'Error processing review.');
        app._err = true;
        this.loading.set(false);
        this.cdr.detectChanges();
      }
    });
  }

  /**
   * Escalates a policy application to an administrator for further review.
   * Useful for cases that exceed agent authority or contain complex risk factors.
   */
  escalateApp(app: any) {
    this.loading.set(true);
    // Submit escalation request via the agent service
    this.agentSvc.escalateApplication(app.id, app._notes || 'Escalated to admin for review').subscribe({
      next: () => {
        // Update local status and provide feedback
        app._msg = '⬆️ Application escalated to admin.';
        app._err = false;
        app.status = 'EscalatedToAdmin';
        this.loading.set(false);
        this.cdr.detectChanges();
        // Refresh the application list
        this.loadApplications();
      },
      error: () => {
        app._msg = 'Error escalating.';
        app._err = true;
        this.loading.set(false);
        this.cdr.detectChanges();
      }
    });
  }

  /**
   * Triggers the AI-driven document validation for a specific application.
   * Communicates with the backend which uses Gemini/Machine Learning to analyze uploaded documents.
   */
  validateDocs(app: any) {
    this.loading.set(true);
    app._msg = '🤖 Contacting AI Analysis Engine...';
    app._err = false;
    this.cdr.detectChanges();

    // Call the service to perform AI verification
    this.agentSvc.validateDocuments(app.id).subscribe({
      next: (res) => {
        // Display results of the AI analysis (Risk Score and Category)
        app._msg = `✅ AI Validation complete. Risk Score: ${res.overallRiskScore} (${res.riskCategory})`;
        app._err = false;
        app.status = 'Validated';
        this.loading.set(false);
        this.cdr.detectChanges();
        // Refresh the UI to reflect the 'Validated' status
        this.loadApplications();
      },
      error: (e) => {
        // Handle failures in communication with the AI microservice
        app._msg = '❌ ' + (e.error?.message || 'Error communicating with AI engine.');
        app._err = true;
        this.loading.set(false);
        this.cdr.detectChanges();
      }
    });
  }

  // ============== ANALYTICS ==============
  agentSelectedChart = 'commByPolicy';
  agentChartSummary = '';
  agentChartTitles: any = { commByPolicy: 'Commission by Policy', workBreakdown: 'Work Breakdown', policyCoverage: 'Policy Coverage Amounts' };
  private agentCurrentChart: any = null;

  ngAfterViewChecked() {
    if (this.tab() === 'analytics' && !this.agentChartsRendered) {
      this.agentChartsRendered = true;
      setTimeout(() => this.renderAgentChart(), 150);
    }
    if (this.tab() !== 'analytics') this.agentChartsRendered = false;
  }

  onAgentChartChange() {
    this.agentChartsRendered = false;
    if (this.agentCurrentChart) { this.agentCurrentChart.destroy(); this.agentCurrentChart = null; }
    setTimeout(() => { this.agentChartsRendered = true; this.renderAgentChart(); }, 50);
  }

  private renderAgentChart() {
    import('chart.js').then(m => {
      const { Chart, ArcElement, BarElement, LineElement, PointElement, BarController, LineController, DoughnutController, CategoryScale, LinearScale, Tooltip, Legend, Filler } = m;
      Chart.register(ArcElement, BarElement, LineElement, PointElement, BarController, LineController, DoughnutController, CategoryScale, LinearScale, Tooltip, Legend, Filler);
      const canvas = document.getElementById('agentAnalyticsCanvas') as HTMLCanvasElement;
      if (!canvas) return;
      if (this.agentCurrentChart) this.agentCurrentChart.destroy();
      const C = { navy: '#004B87', green: '#2e7d32', gold: '#f57f17', red: '#C62828' };
      let config: any;
      switch (this.agentSelectedChart) {
        case 'commByPolicy': {
          const grouped: any = {};
          this.commissions.forEach((c: any) => {
            const k = 'Policy #' + c.policyId;
            const amt = Number(c.commissionAmount) || 0;
            grouped[k] = (grouped[k] || 0) + amt;
          });
          const labels = Object.keys(grouped);
          const data = Object.values(grouped).map(v => Number(v));
          this.agentChartSummary = `Total Commission: \u20b9${this.totalCommission.toLocaleString()} from ${this.commissions.length} entries across ${labels.length} policies`;
          config = {
            type: 'bar',
            data: {
              labels,
              datasets: [{
                label: 'Commission (\u20b9)',
                data: data,
                backgroundColor: '#004B87',
                borderRadius: 4,
                barPercentage: 0.6
              }]
            },
            options: {
              responsive: true,
              maintainAspectRatio: false,
              plugins: { legend: { display: false } },
              scales: {
                y: { beginAtZero: true }
              }
            }
          };
          break;
        }
        case 'workBreakdown': {
          const data = [this.requests.length, this.applications.length, this.policies.length];
          this.agentChartSummary = `Open Requests: ${data[0]}, Applications: ${data[1]}, Active Policies: ${data[2]}`;
          config = { type: 'doughnut', data: { labels: ['Open Requests', 'Applications', 'Active Policies'], datasets: [{ data, backgroundColor: [C.gold, C.navy, C.green], borderWidth: 3, borderColor: '#fff', hoverOffset: 8 }] }, options: { responsive: true, maintainAspectRatio: false, plugins: { legend: { position: 'bottom', labels: { padding: 16, usePointStyle: true } } } } };
          break;
        }
        case 'policyCoverage': {
          const labels = this.policies.map((p: any) => p.policyNumber || 'P#' + p.id);
          const data = this.policies.map((p: any) => p.coverageAmount || 0);
          const total = data.reduce((s: number, v: number) => s + v, 0);
          this.agentChartSummary = `Total Coverage Managed: \u20b9${total.toLocaleString()} across ${this.policies.length} policies`;
          config = { type: 'line', data: { labels, datasets: [{ label: 'Coverage (\u20b9)', data, borderColor: C.green, backgroundColor: 'rgba(46,125,50,0.12)', tension: 0.4, fill: true, pointRadius: 6, pointBackgroundColor: C.green }] }, options: { responsive: true, maintainAspectRatio: false, plugins: { legend: { display: false } }, scales: { y: { beginAtZero: true } } } };
          break;
        }
        default: return;
      }
      this.agentCurrentChart = new Chart(canvas, config);
      this.cdr.detectChanges();
    });
  }
}
