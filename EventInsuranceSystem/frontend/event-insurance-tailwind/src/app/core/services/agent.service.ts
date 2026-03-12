import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../environments/environment';

@Injectable({ providedIn: 'root' })
export class AgentService {
    private api = environment.apiUrl;
    constructor(private http: HttpClient) { }

    // Get assigned insurance requests
    getMyRequests() {
        return this.http.get<any[]>(`${this.api}/insurancerequest/my-requests`);
    }

    // Get all available policy products for suggesting
    getPolicyProducts() {
        return this.http.get<any[]>(`${this.api}/policyproduct`);
    }

    // Suggest a policy to a customer request
    suggestPolicy(dto: { requestId: number; policyProductId: number; suggestedPremium: number; customCoverageAmount?: number }) {
        return this.http.post<any>(`${this.api}/policysuggestion`, dto);
    }

    // Get suggestions for a specific request
    getSuggestionsByRequest(requestId: number) {
        return this.http.get<any[]>(`${this.api}/policysuggestion/request/${requestId}`);
    }

    // Get applications assigned to this agent
    getMyApplications() {
        return this.http.get<any[]>(`${this.api}/policyapplication/agent`);
    }

    // Review an application (approve/reject/escalate)
    reviewApplication(applicationId: number, dto: { isApproved: boolean; reviewNotes: string }) {
        return this.http.post<any>(`${this.api}/agentreview/review/${applicationId}`, dto);
    }

    escalateApplication(applicationId: number, notes: string) {
        return this.http.post<any>(`${this.api}/agentreview/escalate/${applicationId}`, { notes });
    }

    validateDocuments(applicationId: number) {
        return this.http.post<any>(`${this.api}/aivalidation/validate/${applicationId}`, {});
    }

    // Agent's commissions
    getMyCommissions(agentId: number) {
        return this.http.get<any[]>(`${this.api}/commissions/agent/${agentId}`);
    }

    // Agent's policies
    getMyPolicies(agentId: number) {
        return this.http.get<any[]>(`${this.api}/activepolicies/agent/${agentId}`);
    }
}
