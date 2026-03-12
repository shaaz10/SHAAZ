import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../environments/environment';

@Injectable({ providedIn: 'root' })
export class AdminService {
    private api = environment.apiUrl;
    constructor(private http: HttpClient) { }

    // Assign agent to a request
    assignAgent(requestId: number, agentId: number) {
        return this.http.post(`${this.api}/insurancerequest/assign-agent`, { requestId, agentId });
    }

    getUnassignedRequests() {
        return this.http.get<any[]>(`${this.api}/insurancerequest/unassigned`);
    }

    getAgents() {
        return this.http.get<any[]>(`${this.api}/auth/agents`);
    }

    getClaimsOfficers() {
        return this.http.get<any[]>(`${this.api}/auth/claimsofficers`);
    }

    // Get escalated applications (for admin review)
    getEscalatedApplications() {
        return this.http.get<any[]>(`${this.api}/adminapproval/escalated`);
    }

    // Get approved applications (ready for policy creation)
    getApprovedApplications() {
        return this.http.get<any[]>(`${this.api}/adminapproval/approved`);
    }

    // Approve/Reject an application
    approveApplication(applicationId: number, dto: { approvalNotes: string, suggestedPremium?: number }) {
        return this.http.post<any>(`${this.api}/adminapproval/approve/${applicationId}`, {
            isApproved: true,
            approvalNotes: dto.approvalNotes,
            suggestedPremium: dto.suggestedPremium
        });
    }

    approveAndActivate(applicationId: number, dto: { approvalNotes: string, suggestedPremium?: number }) {
        return this.http.post<any>(`${this.api}/adminapproval/approve-and-activate/${applicationId}`, {
            isApproved: true,
            approvalNotes: dto.approvalNotes,
            suggestedPremium: dto.suggestedPremium
        });
    }

    rejectApplication(applicationId: number, rejectionReason: string) {
        return this.http.post<any>(`${this.api}/adminapproval/reject/${applicationId}`, { rejectionReason });
    }

    // Create active policy from approved application
    createActivePolicy(applicationId: number) {
        return this.http.post<any>(`${this.api}/activepolicies/create/${applicationId}`, {});
    }

    // Get all active policies
    getAllPolicies() {
        return this.http.get<any[]>(`${this.api}/activepolicies`);
    }

    // Get all payments
    getAllPayments() {
        return this.http.get<any[]>(`${this.api}/payments`);
    }

    // Get all commissions
    getAllCommissions() {
        return this.http.get<any[]>(`${this.api}/commissions`);
    }

    // Assign claims officer to a claim
    assignClaimsOfficer(claimId: number, claimsOfficerId: number) {
        return this.http.post<any>(`${this.api}/claims/${claimId}/assign-officer`, claimsOfficerId);
    }

    // Create user
    createUser(dto: { fullName: string; email: string; password: string; phoneNumber: string; roleId: number }) {
        return this.http.post(`${this.api}/auth/create-user`, dto);
    }

    // ==========================================
    // POLICY PRODUCTS MANAGEMENT
    // ==========================================

    getProducts() {
        return this.http.get<any[]>(`${this.api}/policyproduct`);
    }

    createProduct(product: any) {
        return this.http.post<any>(`${this.api}/policyproduct`, product);
    }

    updateProduct(id: number, product: any) {
        return this.http.put<any>(`${this.api}/policyproduct/${id}`, product);
    }

    deleteProduct(id: number) {
        return this.http.delete<any>(`${this.api}/policyproduct/${id}`);
    }
}
