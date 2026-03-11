import { Component, OnInit, ChangeDetectorRef, signal, AfterViewChecked, ViewChild, ElementRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { HttpClient } from '@angular/common/http';
import { Subject, of } from 'rxjs';
import { debounceTime, distinctUntilChanged, switchMap, catchError } from 'rxjs/operators';
import * as L from 'leaflet';
import { AuthService } from '../../core/services/auth.service';
import { InsuranceRequestService } from '../../core/services/insurance-request.service';
import { ActivePolicyService } from '../../core/services/active-policy.service';
import { PaymentService } from '../../core/services/payment.service';
import { ClaimService } from '../../core/services/claim.service';
import { AIService } from '../../core/services/ai.service';


@Component({
  selector: 'app-customer-dashboard',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule],
  templateUrl: './customer-dashboard.component.html',
  styleUrl: './customer-dashboard.component.css'
})
export class CustomerDashboardComponent implements OnInit, AfterViewChecked {
  tab = signal('overview');
  isSidebarOpen = signal(false);
  private custChartsRendered = false;
  submitting = signal(false);

  toggleSidebar() { this.isSidebarOpen.update(v => !v); }
  requestMsg = signal(''); requestErr = signal(false);
  claimMsg = signal(''); claimErr = signal(false);
  payMsg = signal(''); payErr = signal(false);
  minDate = signal('');

  policies: any[] = [];
  claims: any[] = [];
  payments: any[] = [];
  myRequests: any[] = [];
  myApplications: any[] = [];

  locationResults: any[] = [];
  locationSearchSubject = new Subject<string>();

  private map: L.Map | undefined;
  private marker: L.Marker | undefined;

  payingPolicy: any = null;
  paymentAmount = 0;
  paymentRef = '';

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

  request = { eventType: '', eventDate: '', eventDuration: 1, eventLocation: '', estimatedAttendees: 100, eventBudget: 0, coverageRequested: 0, riskFactors: '' };
  claimForm = { policyId: 0, claimAmount: 0, description: '' };

  get userName() {
    const u = this.auth.currentUserValue;
    return u?.['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name'] || u?.email || 'Customer';
  }

  @ViewChild('mapElement', { static: false }) mapElement!: ElementRef;

  constructor(
    private auth: AuthService,
    private requestSvc: InsuranceRequestService,
    private policySvc: ActivePolicyService,
    private paymentSvc: PaymentService,
    private claimSvc: ClaimService,
    private aiSvc: AIService,
    private cdr: ChangeDetectorRef,
    private http: HttpClient
  ) {
    this.locationSearchSubject.pipe(
      debounceTime(500),
      distinctUntilChanged(),
      switchMap(query => {
        if (!query || query.length < 3) {
          this.locationResults = [];
          return of([]);
        }
        const url = `https://nominatim.openstreetmap.org/search?format=json&q=${encodeURIComponent(query)}&limit=5`;
        return this.http.get<any[]>(url).pipe(catchError(() => of([])));
      })
    ).subscribe(results => {
      this.locationResults = results;
      this.cdr.detectChanges();
    });
  }

  onLocationInput(event: any) {
    const val = event.target.value;
    this.locationSearchSubject.next(val);
  }

  selectLocation(loc: any) {
    this.request.eventLocation = loc.display_name;
    this.locationResults = [];
    if (loc.lat && loc.lon) {
      this.placeMarker(parseFloat(loc.lat), parseFloat(loc.lon));
    }
  }

  setTab(t: string) {
    this.tab.set(t);
    this.isSidebarOpen.set(false);
    if (t === 'new-request' && this.map) {
      setTimeout(() => this.map?.invalidateSize(), 200);
    }
  }

  initMap() {
    if (this.map) {
      this.map.invalidateSize();
      return;
    }

    // Use ViewChild instead of document.getElementById which can fail across renders
    if (!this.mapElement || !this.mapElement.nativeElement) {
      console.warn('Map Core Error: mapElement ViewChild not found in DOM.');
      return;
    }

    const container = this.mapElement.nativeElement;

    if (container.clientHeight === 0) {
      console.warn('Map Core Error: map container has no height. Postponing init.');
      return;
    }

    try {
      // Prevent multiple Leaflet instances on the same element
      if (container._leaflet_id || container.classList.contains('leaflet-container')) {
        console.log('Map Engine: Container already initialized, skipping.');
        return;
      }

      console.log('Map Engine: Starting initialization on standard protocol... ViewChild:', this.mapElement?.nativeElement);

      // Fix for Leaflet default icon paths
      const iconDefault = L.icon({
        iconUrl: 'https://unpkg.com/leaflet@1.9.4/dist/images/marker-icon.png',
        shadowUrl: 'https://unpkg.com/leaflet@1.9.4/dist/images/marker-shadow.png',
        iconSize: [25, 41],
        iconAnchor: [12, 41],
        popupAnchor: [1, -34],
        shadowSize: [41, 41]
      });
      L.Marker.prototype.options.icon = iconDefault;

      // Initialize map with the actual DOM element reference
      this.map = L.map(container, {
        scrollWheelZoom: false,
        zoomControl: true,
        attributionControl: true
      }).setView([20.5937, 78.9629], 4);

      L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
        maxZoom: 19,
        attribution: '© OpenStreetMap contributors'
      }).addTo(this.map);

      this.map.on('click', (e: any) => {
        this.placeMarker(e.latlng.lat, e.latlng.lng);
        this.reverseGeocode(e.latlng.lat, e.latlng.lng);
      });

      // Force a redraw after current execution context finishes
      setTimeout(() => {
        if (this.map) {
          this.map.invalidateSize();
          console.log('Map Engine: Final size validation success.');
        }
      }, 500);

      this.mapInitialized = true;
    } catch (err) {
      console.error('Map Engine Critical Failure:', err);
      this.map = undefined;
      this.mapInitialized = false;
    }
  }

  placeMarker(lat: number, lng: number) {
    if (!this.map) return;
    if (this.marker) {
      this.marker.setLatLng([lat, lng]);
    } else {
      this.marker = L.marker([lat, lng]).addTo(this.map);
    }
    this.map.setView([lat, lng], 13);
  }

  reverseGeocode(lat: number, lng: number) {
    const url = `https://nominatim.openstreetmap.org/reverse?format=json&lat=${lat}&lon=${lng}`;
    this.http.get<any>(url).subscribe({
      next: res => {
        if (res && res.display_name) {
          this.request.eventLocation = res.display_name;
          this.cdr.detectChanges();
        }
      },
      error: () => { }
    });
  }

  ngOnInit() {
    this.minDate.set(new Date().toISOString().split('T')[0]);
    this.loadMyRequests();
    this.loadMyApplications();
    this.loadPolicies();
    this.loadClaims();
  }

  loadMyRequests() {
    this.requestSvc.getMyRequests().subscribe({
      next: r => {
        // Filter out requests that are already 'PolicySelected' (converted to applications)
        const activeRequests = r.filter((x: any) => x.status !== 'PolicySelected');
        this.myRequests = activeRequests.map((x: any) => ({ ...x, _suggestions: [], _msg: '', _err: false })).sort((a: any, b: any) => b.id - a.id);
        // Load suggestions for each request
        this.myRequests.forEach(req => {
          this.requestSvc.getSuggestionsByRequest(req.id).subscribe({
            next: s => { req._suggestions = s; this.cdr.detectChanges(); },
            error: () => { }
          });
        });
        this.cdr.detectChanges();
      },
      error: () => { }
    });
  }

  /**
   * Loads all policy applications associated with the current customer.
   * Maps application status and initializes local state for document management.
   */
  loadMyApplications() {
    const id = this.auth.currentUserId();
    if (!id) return;

    // Fetch applications from the service layer
    this.requestSvc.getMyApplications(id).subscribe({
      next: r => {
        // Initialize extended properties for each application (messages, document upload paths)
        this.myApplications = r.map((a: any) => ({
          ...a,
          _docType: '',
          _docName: '',
          _docPath: '',
          _docs: [],
          _msg: '',
          _err: false
        })).sort((a: any, b: any) => b.id - a.id);

        // Trigger secondary loads for application-specific documentation
        this.myApplications.forEach(app => {
          this.requestSvc.getDocuments(app.id).subscribe({
            next: d => { app._docs = d; this.cdr.detectChanges(); },
            error: () => { }
          });
        });
        this.cdr.detectChanges();
      },
      error: () => { }
    });
  }

  loadPolicies() {
    const id = this.auth.currentUserId();
    if (!id) return;
    this.policySvc.getByCustomer(id).subscribe({
      next: r => { this.policies = r.sort((a: any, b: any) => b.id - a.id); this.cdr.detectChanges(); },
      error: () => { }
    });
  }

  loadClaims() {
    const id = this.auth.currentUserId();
    if (!id) return;
    this.claimSvc.getByCustomer(id).subscribe({
      next: r => { this.claims = r.sort((a: any, b: any) => b.id - a.id); this.cdr.detectChanges(); },
      error: () => { }
    });
  }

  /**
   * Validates and submits a new insurance request for a customer's event.
   * Handles state toggles for loading indicators and status messaging.
   */
  submitRequest() {
    this.requestMsg.set('');
    this.requestErr.set(false);

    // Frontend Validations
    if (!this.request.eventType || !this.request.eventDate || !this.request.eventLocation) {
      this.requestMsg.set('❌ Please fill in all required fields (Event Type, Date, Location).');
      this.requestErr.set(true);
      return;
    }

    if (this.request.eventDate < this.minDate()) {
      this.requestMsg.set('❌ Event date cannot be in the past.');
      this.requestErr.set(true);
      return;
    }

    if (this.request.eventDuration <= 0) {
      this.requestMsg.set('❌ Event duration must be a positive number of days.');
      this.requestErr.set(true);
      return;
    }

    if (this.request.estimatedAttendees <= 0) {
      this.requestMsg.set('❌ Expected attendees must be greater than zero.');
      this.requestErr.set(true);
      return;
    }

    if (this.request.eventBudget <= 0) {
      this.requestMsg.set('❌ Please enter a proper event budget (greater than zero).');
      this.requestErr.set(true);
      return;
    }

    if (this.request.coverageRequested <= 0) {
      this.requestMsg.set('❌ Requested coverage amount must be greater than zero.');
      this.requestErr.set(true);
      return;
    }

    this.submitting.set(true);

    // Send the captured event details to the backend service
    this.requestSvc.create(this.request).subscribe({
      next: () => {
        this.requestMsg.set('✅ Request submitted! An agent will be assigned shortly.');
        this.requestErr.set(false);
        this.submitting.set(false);
        // Reset form
        this.request = { eventType: '', eventDate: '', eventDuration: 1, eventLocation: '', estimatedAttendees: 100, eventBudget: 0, coverageRequested: 0, riskFactors: '' };
        // Refresh the local list to show the newly submitted request
        this.loadMyRequests();
      },
      error: (e) => {
        // Display specific error messages returned by the API
        this.requestMsg.set('❌ ' + (e.error?.message || 'Error submitting request.'));
        this.requestErr.set(true);
        this.submitting.set(false);
      }
    });
  }

  selectSuggestion(suggestion: any, req: any) {
    this.submitting.set(true);
    this.requestSvc.selectPolicy(suggestion.suggestionId).subscribe({
      next: (res) => {
        req._msg = 'Policy selected! Application #' + res.applicationId + ' created. Go to "My Applications" to upload documents.';
        req._err = false;
        this.submitting.set(false);
        this.loadMyApplications();
        this.cdr.detectChanges();
      },
      error: e => {
        req._msg = '❌ ' + (e.error?.message || 'Error selecting policy.');
        req._err = true;
        this.submitting.set(false);
        this.cdr.detectChanges();
      }
    });
  }

  onFileSelected(event: any, app: any) {
    const file = event.target.files[0];
    if (file) {
      app._docFile = file;
    }
  }

  /**
   * Uploads and validates documents for a policy application.
   * Leverages Gemini / AI services on the backend for automated verification.
   */
  uploadDocument(app: any) {
    if (!app._docFile) return;

    this.submitting.set(true);
    app._msg = '🤖 Uploading and Authenticating with AI...';
    app._err = false;

    // Use FormData for multi-part file transfers
    const formData = new FormData();
    formData.append('documentType', app._docType);
    formData.append('file', app._docFile);

    this.requestSvc.uploadDocument(app.id, formData).subscribe({
      next: (res) => {
        // Show AI validation results and reset local upload state
        app._msg = `Document authenticated! AI Confidence Score: ${res.validationResult?.overallRiskScore || 'N/A'}`;
        app._err = false;
        app._docFile = null;
        app._docType = '';
        this.submitting.set(false);

        // Fetch the updated list of documents per application
        this.requestSvc.getDocuments(app.id).subscribe({
          next: d => { app._docs = d; this.cdr.detectChanges(); },
          error: () => { }
        });
        this.cdr.detectChanges();
      },
      error: e => {
        // Feedback loop for invalid documents or server issues
        app._msg = '❌ ' + (e.error?.message || 'Error uploading document.');
        app._err = true;
        this.submitting.set(false);
        this.cdr.detectChanges();
      }
    });
  }

  hasActivePolicy(applicationId: number): boolean {
    return this.policies.some(p => p.applicationId === applicationId);
  }

  openPayment(policy: any) {
    this.payingPolicy = policy;
    this.paymentAmount = policy.premiumAmount;
    this.paymentRef = 'TXN-' + Math.floor(Math.random() * 100000000).toString().padStart(8, '0');
    this.payMsg.set('');
  }

  openClaimForPolicy(policy: any) {
    this.claimForm.policyId = policy.id;
    this.claimForm.claimAmount = 0;
    this.claimForm.description = '';
    this.claimMsg.set('');
    this.tab.set('raise-claim');
  }

  makePayment() {
    this.payMsg.set('');
    this.payErr.set(false);

    // Ensure they are paying exactly the premium amount
    this.paymentAmount = this.payingPolicy.premiumAmount;

    if (this.paymentAmount <= 0) {
      this.payMsg.set('❌ Please enter a proper payment amount (greater than zero).');
      this.payErr.set(true);
      return;
    }

    if (!this.paymentRef) {
      this.payMsg.set('❌ Transaction reference is required.');
      this.payErr.set(true);
      return;
    }

    this.paymentSvc.makePayment({ policyId: this.payingPolicy.id, amount: this.paymentAmount, transactionReference: this.paymentRef }).subscribe({
      next: r => {
        this.payMsg.set('✅ Payment of ₹' + this.paymentAmount + ' successful!');
        this.payErr.set(false);
        this.payments.push(r);
        setTimeout(() => this.payingPolicy = null, 2000);
        this.cdr.detectChanges();
      },
      error: e => {
        this.payMsg.set('❌ ' + (e.error?.message || 'Payment failed.'));
        this.payErr.set(true);
        this.cdr.detectChanges();
      }
    });
  }

  enhancingDescription = signal(false);
  enhanceClaimDescription() {
    if (!this.claimForm.description) {
      alert('Please enter some text first.');
      return;
    }
    this.enhancingDescription.set(true);
    this.aiSvc.enhanceText(this.claimForm.description).subscribe({
      next: (res) => {
        this.claimForm.description = res.enhancedText;
        this.enhancingDescription.set(false);
      },
      error: () => {
        this.enhancingDescription.set(false);
        alert('AI service is currently unavailable. Using manual entry.');
      }
    });
  }

  raiseClaim() {
    this.claimMsg.set('');
    this.claimErr.set(false);

    if (this.claimForm.policyId <= 0) {
      this.claimMsg.set('❌ Please select a valid policy.');
      this.claimErr.set(true);
      return;
    }

    if (this.claimForm.claimAmount <= 0) {
      this.claimMsg.set('❌ Claim amount must be greater than zero. Please enter a proper value.');
      this.claimErr.set(true);
      return;
    }

    if (!this.claimForm.description || this.claimForm.description.length < 10) {
      this.claimMsg.set('❌ Please provide a detailed description (at least 10 characters).');
      this.claimErr.set(true);
      return;
    }

    const customerId = this.auth.currentUserId();
    if (!customerId) return;
    this.submitting.set(true);

    this.claimSvc.raiseClaim({ ...this.claimForm, customerId }).subscribe({
      next: () => {
        this.claimMsg.set('✅ Claim submitted successfully! Our team will review it shortly.');
        this.claimErr.set(false);
        this.submitting.set(false);
        this.claimForm = { policyId: 0, claimAmount: 0, description: '' };
        this.loadClaims();
      },
      error: e => {
        this.claimMsg.set('❌ ' + (e.error?.message || 'Error submitting claim.'));
        this.claimErr.set(true);
        this.submitting.set(false);
      }
    });
  }

  // ============== ANALYTICS ==============
  custSelectedChart = 'premiumVsCoverage';
  custChartSummary = '';
  custChartTitles: any = { premiumVsCoverage: 'Coverage x Premium Correlation', claimsByStatus: 'Claims Status Distribution', monthlyPayments: 'Payment Trends' };
  private custCurrentChart: any = null;

  private mapInitialized = false;

  ngAfterViewChecked() {
    if (this.tab() === 'analytics' && !this.custChartsRendered) {
      this.custChartsRendered = true;
      setTimeout(() => this.renderCustChart(), 150);
    }
    if (this.tab() !== 'analytics') this.custChartsRendered = false;

    // Map Life Cycle Management
    const isNewRequest = this.tab() === 'new-request';
    if (isNewRequest && !this.mapInitialized) {
      if (this.mapElement && this.mapElement.nativeElement && this.mapElement.nativeElement.clientHeight > 0) {
        // Only run init if we are sure the container is ready and not already processed
        if (!this.mapElement.nativeElement._leaflet_id && !this.mapElement.nativeElement.classList.contains('leaflet-container')) {
          console.log('Dashboard Lifecycle: Map container detected, triggering init with deferral...');
          // Defer to next tick to ensure ViewChild and DOM are fully aligned
          setTimeout(() => this.initMap(), 50);
        } else {
          this.mapInitialized = true; // Sync state if container already has classes
        }
      }
    } else if (!isNewRequest && this.mapInitialized) {
      if (this.map) {
        console.log('Dashboard Lifecycle: Cleaning up map resources...');
        try {
          this.map.off();
          this.map.remove();
        } catch (e) {
          console.warn('Map cleanup error:', e);
        }
        this.map = undefined;
        this.marker = undefined;
      }
      this.mapInitialized = false;
    }
  }

  onCustChartChange() {
    this.custChartsRendered = false;
    if (this.custCurrentChart) { this.custCurrentChart.destroy(); this.custCurrentChart = null; }
    setTimeout(() => { this.custChartsRendered = true; this.renderCustChart(); }, 50);
  }

  private renderCustChart() {
    import('chart.js').then(m => {
      const { Chart, ArcElement, BarElement, LineElement, PointElement, BarController, LineController, DoughnutController, CategoryScale, LinearScale, Tooltip, Legend, Filler } = m;
      Chart.register(ArcElement, BarElement, LineElement, PointElement, BarController, LineController, DoughnutController, CategoryScale, LinearScale, Tooltip, Legend, Filler);
      const canvas = document.getElementById('custAnalyticsCanvas') as HTMLCanvasElement;
      if (!canvas) return;
      if (this.custCurrentChart) this.custCurrentChart.destroy();
      const C = { navy: '#004B87', red: '#C62828', green: '#2e7d32', gold: '#f57f17' };
      let config: any;
      switch (this.custSelectedChart) {
        case 'monthlyPayments': {
          const grouped: any = {};
          this.payments.forEach((p: any) => { const k = 'Policy #' + p.policyId; grouped[k] = (grouped[k] || 0) + (p.amount || 0); });
          const labels = Object.keys(grouped); const data = Object.values(grouped) as number[];
          const total = data.reduce((s, v) => s + v, 0);
          this.custChartSummary = `Total Paid: \u20b9${total.toLocaleString()} across ${labels.length} policies (${this.payments.length} transactions)`;
          config = { type: 'bar', data: { labels, datasets: [{ label: 'Amount (\u20b9)', data, backgroundColor: [C.navy, C.green, C.gold, C.red], borderRadius: 8, barPercentage: 0.6 }] }, options: { responsive: true, maintainAspectRatio: false, plugins: { legend: { display: false } }, scales: { y: { beginAtZero: true, ticks: { callback: (v: any) => '\u20b9' + v.toLocaleString() } } } } };
          break;
        }
        case 'claimsByStatus': {
          const statuses = ['Submitted', 'Pending', 'Approved', 'Settled', 'Rejected'];
          const colors = [C.gold, '#ff9800', C.navy, C.green, C.red];
          const data = statuses.map(s => this.claims.filter((c: any) => c.status === s).length);
          this.custChartSummary = `Total Claims: ${this.claims.length} — Active: ${data[0] + data[1]}, Approved: ${data[2]}, Settled: ${data[3]}`;
          config = { type: 'doughnut', data: { labels: statuses, datasets: [{ data, backgroundColor: colors, borderWidth: 3, borderColor: '#fff', hoverOffset: 8 }] }, options: { responsive: true, maintainAspectRatio: false, plugins: { legend: { position: 'bottom', labels: { padding: 16, usePointStyle: true } } } } };
          break;
        }
        case 'premiumVsCoverage': {
          const labels = this.policies.map((p: any) => p.policyNumber || 'P#' + p.id);
          const data = this.policies.map((p: any) => p.coverageAmount || 0);
          const total = data.reduce((s: number, v: number) => s + v, 0);
          this.custChartSummary = `Total Coverage: \u20b9${total.toLocaleString()} across ${this.policies.length} active policies`;
          config = { type: 'line', data: { labels, datasets: [{ label: 'Coverage (\u20b9)', data, borderColor: C.green, backgroundColor: 'rgba(46,125,50,0.12)', tension: 0.4, fill: true, pointRadius: 6, pointBackgroundColor: C.green }] }, options: { responsive: true, maintainAspectRatio: false, plugins: { legend: { display: false } }, scales: { y: { beginAtZero: true, ticks: { callback: (v: any) => '\u20b9' + (v / 1000).toFixed(0) + 'k' } } } } };
          break;
        }
        default: return;
      }
      this.custCurrentChart = new Chart(canvas, config);
      this.cdr.detectChanges();
    });
  }

  // ============================================================
  // ENTERPRISE UI HELPERS (Lifecycle Timeline, Risk Gauge, Docs)
  // ============================================================

  /** Maps application status to the visual lifecycle pipeline */
  getLifecycleSteps(status: string) {
    const progression = [
      'Submitted', 'AgentAssigned', 'PolicySuggested', 'PendingValidation', 'UnderReview', 'AgentApproved', 'Approved', 'Active'
    ];
    const steps = [
      { key: 'Submitted',         label: 'Request\nSubmitted',   icon: 'fa-paper-plane'    },
      { key: 'AgentAssigned',     label: 'Agent\nAssigned',      icon: 'fa-user-tie'       },
      { key: 'PendingValidation', label: 'Docs\nUploaded',       icon: 'fa-file-arrow-up'  },
      { key: 'UnderReview',       label: 'Under\nReview',        icon: 'fa-magnifying-glass'},
      { key: 'AgentApproved',     label: 'Agent\nApproved',      icon: 'fa-circle-check'   },
      { key: 'Approved',          label: 'Admin\nCleared',       icon: 'fa-award'          },
      { key: 'Active',            label: 'Policy\nIssued',       icon: 'fa-shield-halved'  },
    ];
    const currentIdx = progression.indexOf(status);
    return steps.map((s, i) => ({
      ...s,
      state: currentIdx > i ? 'done' : currentIdx === i ? 'active' : 'upcoming'
    }));
  }

  /** Returns the progress bar width percentage for the gradient line */
  getLifecycleProgress(status: string): number {
    const map: any = {
      'Submitted': 4,
      'AgentAssigned': 18,
      'PolicySuggested': 18,
      'PendingValidation': 35,
      'UnderReview': 51,
      'AgentApproved': 68,
      'Approved': 84,
      'Active': 100,
    };
    return map[status] ?? 0;
  }

  /** Derives individual risk factor bars from the aggregate riskScore */
  getRiskFactors(app: any): { label: string; pct: number }[] {
    const base = app.riskScore || 50;
    const attendees = app.estimatedAttendees || 200;
    const budget    = app.eventBudget || 100000;

    const venuePct    = Math.min(100, Math.round(base * 0.9  + (attendees > 1000 ? 15 : 0)));
    const crowdPct    = Math.min(100, Math.round(base * 0.75 + (attendees > 500  ? 10 : 0)));
    const weatherPct  = Math.min(100, Math.round(base * 0.6));
    const financialPct = Math.min(100, Math.round((budget / 500000) * 60 + base * 0.3));

    return [
      { label: 'Venue Risk',       pct: venuePct    },
      { label: 'Crowd Density',    pct: crowdPct    },
      { label: 'Weather Exposure', pct: weatherPct  },
      { label: 'Financial Scale',  pct: financialPct },
    ];
  }

  // ============================================================
  // PREMIUM CALCULATOR (Real-time, pure frontend, no DB changes)
  // ============================================================

  calculatePremium(data: any) {
    const riskScore   = data.riskScore   || 50;
    const eventBudget = data.eventBudget || 100000;
    const coverageReq = data.coverageRequested > 0 ? data.coverageRequested : eventBudget * 1.5;
    const attendees   = data.estimatedAttendees || 200;
    const eventType   = data.eventType || '';

    const baseCoverage = Math.round(coverageReq);
    const basePremium  = baseCoverage * 0.04;

    // Risk add-on (0%–80% of base)
    const riskPct    = Math.round((riskScore / 100) * 80);
    const riskAddon  = Math.round(basePremium * riskPct / 100);

    // Event type add-on
    const eventMap: Record<string, number> = {
      'Wedding': 10, 'Concert': 30, 'Festival': 40,
      'Sports Event': 50, 'Conference': 0, 'Corporate Event': 10
    };
    const eventPct   = eventMap[eventType] ?? 15;
    const eventAddon = Math.round(basePremium * eventPct / 100);

    // Crowd surcharge
    const crowdPct   = attendees > 1000 ? 30 : attendees > 500 ? 15 : 0;
    const crowdAddon = Math.round(basePremium * crowdPct / 100);

    const finalPremium = Math.round(basePremium) + riskAddon + eventAddon + crowdAddon;

    return {
      baseCoverage, basePremium: Math.round(basePremium),
      riskScore, riskPct, riskAddon,
      eventType, eventPct, eventAddon,
      attendees, crowdPct, crowdAddon,
      finalPremium,
      riskLabel: riskScore >= 70 ? 'HIGH RISK' : riskScore >= 40 ? 'MEDIUM RISK' : 'LOW RISK',
      riskColor: riskScore >= 70 ? 'text-red-500' : riskScore >= 40 ? 'text-amber-500' : 'text-emerald-500',
    };
  }
}
