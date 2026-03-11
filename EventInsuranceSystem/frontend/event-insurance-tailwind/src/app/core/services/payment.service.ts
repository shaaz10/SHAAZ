import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../environments/environment';

@Injectable({ providedIn: 'root' })
export class PaymentService {
    private api = environment.apiUrl;
    constructor(private http: HttpClient) { }

    makePayment(dto: { policyId: number; amount: number; transactionReference?: string }) {
        return this.http.post<any>(`${this.api}/payments`, dto);
    }

    getByPolicy(policyId: number) {
        return this.http.get<any[]>(`${this.api}/payments/policy/${policyId}`);
    }

    getAll() {
        return this.http.get<any[]>(`${this.api}/payments`);
    }
}
