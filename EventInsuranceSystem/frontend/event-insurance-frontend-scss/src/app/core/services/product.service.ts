import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../environments/environment';

@Injectable({
    providedIn: 'root'
})
export class ProductService {
    private apiUrl = environment.apiUrl;

    constructor(private http: HttpClient) { }

    suggestPolicies(requestText: string) {
        // Calling our Step 2 / Step 3 AI Policy Suggestion endpoint
        return this.http.post<any>(`${this.apiUrl}/insurance-requests/suggest`, {
            customerId: 2, // Hardcoding to customer 2 from local seed for demo purposes
            eventDetails: requestText
        });
    }

    createApplication(suggestionId: number, customerId: number = 2) {
        return this.http.post<any>(`${this.apiUrl}/policy-applications`, {
            policySuggestionId: suggestionId,
            customerId: customerId
        });
    }
}
