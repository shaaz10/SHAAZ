import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../environments/environment';

@Injectable({ providedIn: 'root' })
export class ClaimService {
    private api = environment.apiUrl;
    constructor(private http: HttpClient) { }

    raiseClaim(dto: { policyId: number; customerId: number; claimAmount: number; description: string }) {
        return this.http.post<any>(`${this.api}/claims`, dto);
    }

    getByCustomer(customerId: number) {
        return this.http.get<any[]>(`${this.api}/claims/customer/${customerId}`);
    }

    getAll() {
        return this.http.get<any[]>(`${this.api}/claims`);
    }

    getByOfficer(officerId: number) {
        return this.http.get<any[]>(`${this.api}/claims/officer/${officerId}`);
    }

    detectFraud(claimId: number) {
        return this.http.post<any>(`${this.api}/fraud/detect/${claimId}`, {});
    }

    approveClaim(claimId: number, dto: { claimsOfficerId: number; notes: string }) {
        return this.http.post<any>(`${this.api}/claims/${claimId}/approve`, dto);
    }

    rejectClaim(claimId: number, dto: { claimsOfficerId: number; notes: string }) {
        return this.http.post<any>(`${this.api}/claims/${claimId}/reject`, dto);
    }

    settleClaim(claimId: number, dto: { claimsOfficerId: number; settlementNotes: string }) {
        return this.http.post<any>(`${this.api}/claims/${claimId}/settle`, dto);
    }
}
