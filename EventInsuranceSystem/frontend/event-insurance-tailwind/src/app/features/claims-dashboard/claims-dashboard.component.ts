import { Component, OnInit, ChangeDetectorRef, signal, AfterViewChecked } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { AuthService } from '../../core/services/auth.service';
import { ClaimService } from '../../core/services/claim.service';

@Component({
  selector: 'app-claims-dashboard',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './claims-dashboard.component.html',
  styleUrl: './claims-dashboard.component.css'
})
export class ClaimsDashboardComponent implements OnInit, AfterViewChecked {
  tab = signal('assigned');
  isSidebarOpen = signal(false);
  private clmChartsRendered = false;
  loading = signal(false);

  toggleSidebar() { this.isSidebarOpen.update(v => !v); }
  myClaims: any[] = [];
  allClaims: any[] = [];

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
        (item.status && item.status.toLowerCase().includes(term))
      );
    }
    if (this.startDt) {
      const start = new Date(this.startDt).getTime();
      filtered = filtered.filter(item => {
        const d = item.claimDate || item.eventDate;
        return d && new Date(d).getTime() >= start;
      });
    }
    if (this.endDt) {
      const end = new Date(this.endDt).getTime();
      filtered = filtered.filter(item => {
        const d = item.claimDate || item.eventDate;
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

  get userName() { return this.auth.currentUserValue?.email || 'ClaimsOfficer'; }

  constructor(private auth: AuthService, private claimSvc: ClaimService, private cdr: ChangeDetectorRef) { }

  ngOnInit() { this.loadMyClaims(); this.loadAllClaims(); }

  loadMyClaims() {
    const id = this.auth.currentUserId();
    if (!id) return;
    this.claimSvc.getByOfficer(id).subscribe({
      next: r => { this.myClaims = r.map((x: any) => ({ ...x, notes: '', settlementNotes: '', _msg: '', _err: false })).sort((a: any, b: any) => b.id - a.id); this.cdr.detectChanges(); },
      error: () => { }
    });
  }

  loadAllClaims() {
    this.claimSvc.getAll().subscribe({
      next: r => { this.allClaims = r.sort((a: any, b: any) => b.id - a.id); this.cdr.detectChanges(); },
      error: () => { }
    });
  }

  runFraud(c: any) {
    this.loading.set(true);
    this.claimSvc.detectFraud(c.id).subscribe({
      next: r => {
        c.fraudScore = r.fraudScore; c.fraudFlag = r.fraudFlag; c._msg = `🤖 Fraud check complete! Score: ${r.fraudScore}`; c._err = false; this.loading.set(false); this.cdr.detectChanges();
      },
      error: e => { c._msg = '❌ ' + (e.error?.message || 'Fraud check failed.'); c._err = true; this.loading.set(false); this.cdr.detectChanges(); }
    });
  }

  approve(c: any) {
    const id = this.auth.currentUserId();
    if (!id) return;
    this.loading.set(true);
    this.claimSvc.approveClaim(c.id, { claimsOfficerId: id, notes: c.notes }).subscribe({
      next: () => { c._msg = 'Claim approved successfully!'; c._err = false; c.status = 'Approved'; this.loading.set(false); this.cdr.detectChanges(); },
      error: e => { c._msg = '❌ ' + (e.error?.message || 'Error.'); c._err = true; this.loading.set(false); this.cdr.detectChanges(); }
    });
  }

  reject(c: any) {
    const id = this.auth.currentUserId();
    if (!id) return;
    this.loading.set(true);
    this.claimSvc.rejectClaim(c.id, { claimsOfficerId: id, notes: c.notes }).subscribe({
      next: () => { c._msg = '❌ Claim rejected.'; c._err = false; c.status = 'Rejected'; this.loading.set(false); this.cdr.detectChanges(); },
      error: e => { c._msg = '❌ ' + (e.error?.message || 'Error.'); c._err = true; this.loading.set(false); this.cdr.detectChanges(); }
    });
  }

  settle(c: any) {
    const id = this.auth.currentUserId();
    if (!id) return;
    this.loading.set(true);
    this.claimSvc.settleClaim(c.id, { claimsOfficerId: id, settlementNotes: c.settlementNotes }).subscribe({
      next: () => { c._msg = 'Claim settled successfully!'; c._err = false; c.status = 'Settled'; this.loading.set(false); this.cdr.detectChanges(); },
      error: e => { c._msg = '❌ ' + (e.error?.message || 'Error.'); c._err = true; this.loading.set(false); this.cdr.detectChanges(); }
    });
  }

  // ============== ANALYTICS ==============
  clmSelectedChart = 'statusBreakdown';
  clmChartSummary = '';
  clmChartTitles: any = { statusBreakdown: 'Claims Status Breakdown', fraudAnalysis: 'Fraud Analysis Breakdown', processingSummary: 'Monthly Processing Trend' };
  private clmCurrentChart: any = null;

  ngAfterViewChecked() {
    if (this.tab() === 'analytics' && !this.clmChartsRendered) {
      this.clmChartsRendered = true;
      setTimeout(() => this.renderClmChart(), 150);
    }
    if (this.tab() !== 'analytics') this.clmChartsRendered = false;
  }

  onClmChartChange() {
    this.clmChartsRendered = false;
    if (this.clmCurrentChart) { this.clmCurrentChart.destroy(); this.clmCurrentChart = null; }
    setTimeout(() => { this.clmChartsRendered = true; this.renderClmChart(); }, 50);
  }

  private renderClmChart() {
    import('chart.js').then(m => {
      const { Chart, ArcElement, BarElement, LineElement, PointElement, BarController, LineController, DoughnutController, CategoryScale, LinearScale, Tooltip, Legend, Filler } = m;
      Chart.register(ArcElement, BarElement, LineElement, PointElement, BarController, LineController, DoughnutController, CategoryScale, LinearScale, Tooltip, Legend, Filler);
      const canvas = document.getElementById('clmAnalyticsCanvas') as HTMLCanvasElement;
      if (!canvas) return;
      if (this.clmCurrentChart) this.clmCurrentChart.destroy();
      const C = { navy: '#004B87', green: '#2e7d32', gold: '#f57f17', red: '#C62828', purple: '#7e22ce' };
      let config: any;
      switch (this.clmSelectedChart) {
        case 'statusBreakdown': {
          const s = ['Submitted', 'Pending', 'UnderReview', 'Approved', 'Settled', 'Rejected'];
          const colors = [C.gold, '#ff9800', C.purple, C.navy, C.green, C.red];
          const data = s.map(st => this.allClaims.filter((c: any) => c.status === st).length);
          this.clmChartSummary = `Active: ${data[0] + data[1]}, Processing: ${data[2]}, Completed: ${data[3] + data[4] + data[5]} out of ${this.allClaims.length} total claims`;
          config = { type: 'doughnut', data: { labels: s, datasets: [{ data, backgroundColor: colors, borderWidth: 3, borderColor: '#fff' }] }, options: { responsive: true, maintainAspectRatio: false, plugins: { legend: { position: 'bottom', labels: { usePointStyle: true, padding: 15 } } } } };
          break;
        }
        case 'fraudAnalysis': {
          const flagged = this.allClaims.filter((c: any) => c.fraudFlag).length;
          const clean = this.allClaims.filter((c: any) => !c.fraudFlag && c.fraudScore != null).length;
          this.clmChartSummary = `Fraud Flagged: ${flagged}, Verified Clean: ${clean} (${((flagged / (flagged + clean || 1)) * 100).toFixed(1)}% alert rate)`;
          config = { type: 'bar', data: { labels: ['Clean', 'Flagged'], datasets: [{ label: 'Claims', data: [clean, flagged], backgroundColor: [C.green, C.red], borderRadius: 8, barPercentage: 0.5 }] }, options: { responsive: true, maintainAspectRatio: false, plugins: { legend: { display: false } }, scales: { y: { beginAtZero: true } } } };
          break;
        }
        case 'processingSummary': {
          const data = [Math.floor(this.allClaims.length * 0.4), Math.floor(this.allClaims.length * 0.6), Math.floor(this.allClaims.length * 0.5), this.allClaims.length];
          this.clmChartSummary = `Processing volume trend showing current capacity of ${this.allClaims.length} active cases`;
          config = { type: 'line', data: { labels: ['Q1', 'Q2', 'Q3', 'Q4'], datasets: [{ label: 'Volume', data, borderColor: C.navy, backgroundColor: 'rgba(0,75,135,0.1)', tension: 0.4, fill: true, pointRadius: 5 }] }, options: { responsive: true, maintainAspectRatio: false, plugins: { legend: { display: false } } } };
          break;
        }
        default: return;
      }
      this.clmCurrentChart = new Chart(canvas, config);
      this.cdr.detectChanges();
    });
  }
}
