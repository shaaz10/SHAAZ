import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../environments/environment';

@Injectable({ providedIn: 'root' })
export class InsuranceRequestService {
    private api = environment.apiUrl;
    constructor(private http: HttpClient) { }

    // Customer: submit a new insurance request
    create(dto: any) {
        return this.http.post(`${this.api}/insurancerequest`, dto);
    }

    // Customer: get my submitted requests
    getMyRequests() {
        return this.http.get<any[]>(`${this.api}/insurancerequest/customer-requests`);
    }

    // Get suggestions for a specific request
    getSuggestionsByRequest(requestId: number) {
        return this.http.get<any[]>(`${this.api}/policysuggestion/request/${requestId}`);
    }

    // Select a policy suggestion (creates application)
    selectPolicy(suggestionId: number) {
        return this.http.post<any>(`${this.api}/policyapplication/select`, { suggestionId });
    }

    // Upload document for an application
    uploadDocument(applicationId: number, formData: FormData) {
        return this.http.post<any>(`${this.api}/applicationdocument/upload/${applicationId}`, formData);
    }

    // Get documents for an application
    getDocuments(applicationId: number) {
        return this.http.get<any[]>(`${this.api}/applicationdocument/${applicationId}`);
    }

    // Get customer's applications
    getMyApplications(customerId: number) {
        return this.http.get<any[]>(`${this.api}/policyapplication/customer/${customerId}`);
    }

    // Agent: view my assigned requests
    getAgentRequests() {
        return this.http.get<any[]>(`${this.api}/insurancerequest/my-requests`);
    }

    // Admin: assign agent to a request
    assignAgent(requestId: number, agentId: number) {
        return this.http.post(`${this.api}/insurancerequest/assign-agent`, { requestId, agentId });
    }
}

