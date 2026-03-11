import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../environments/environment';

@Injectable({
    providedIn: 'root'
})
export class AgentPortalService {
    private apiUrl = environment.apiUrl;

    constructor(private http: HttpClient) { }

    getApplicationsByStatus(status: 'Submitted' | 'UnderReview' | 'Approved' | 'Rejected') {
        return this.http.get<any[]>(`${this.apiUrl}/policy-applications/status/${status}`);
    }

    reviewApplication(applicationId: number, agentId: number, dto: any) {
        return this.http.post<any>(`${this.apiUrl}/policy-applications/${applicationId}/agent-review/${agentId}`, dto);
    }
}
