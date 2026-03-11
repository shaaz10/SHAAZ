import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../environments/environment';
import { Observable } from 'rxjs';

@Injectable({ providedIn: 'root' })
export class AIService {
    private api = environment.apiUrl + '/ai';

    constructor(private http: HttpClient) { }

    enhanceText(text: string): Observable<{ enhancedText: string }> {
        return this.http.post<{ enhancedText: string }>(`${this.api}/enhance-text`, { text });
    }
}
