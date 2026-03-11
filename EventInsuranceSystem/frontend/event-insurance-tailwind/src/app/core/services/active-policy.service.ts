import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../environments/environment';

@Injectable({ providedIn: 'root' })
export class ActivePolicyService {
    private api = environment.apiUrl;
    constructor(private http: HttpClient) { }

    getByCustomer(customerId: number) {
        return this.http.get<any[]>(`${this.api}/activepolicies/customer/${customerId}`);
    }

    getByAgent(agentId: number) {
        return this.http.get<any[]>(`${this.api}/activepolicies/agent/${agentId}`);
    }

    getAll() {
        return this.http.get<any[]>(`${this.api}/activepolicies`);
    }

    create(applicationId: number) {
        return this.http.post<any>(`${this.api}/activepolicies/create/${applicationId}`, {});
    }
}
