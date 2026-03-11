import { Component, OnInit, ViewChild, ElementRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../environments/environment';

@Component({
    selector: 'app-home',
    standalone: true,
    imports: [CommonModule, RouterModule],
    templateUrl: './home.component.html',
    styleUrls: ['./home.component.scss']
})
export class HomeComponent implements OnInit {
    products: any[] = [];
    @ViewChild('carousel') carousel!: ElementRef;

    private apiUrl = environment.apiUrl;

    constructor(private http: HttpClient) { }

    ngOnInit() {
        this.http.get<any[]>(`${this.apiUrl}/PolicyProduct`).subscribe({
            next: (data) => {
                this.products = data;
            },
            error: (err) => console.error('Error fetching products', err)
        });
    }

    moveCarousel(direction: number) {
        if (this.carousel) {
            const scrollAmount = 400; // Adjusted for better feel
            this.carousel.nativeElement.scrollBy({
                left: direction * scrollAmount,
                behavior: 'smooth'
            });
        }
    }

    getImageUrl(name: string, index: number): string {
        const lower = name.toLowerCase();
        if (lower.includes('wedding') || lower.includes('personal')) return 'https://images.unsplash.com/photo-1519225421980-715cb0215aed?ixlib=rb-4.0.3&auto=format&fit=crop&w=800&q=80';
        if (lower.includes('corporate') || lower.includes('business') || lower.includes('conference')) return 'https://images.unsplash.com/photo-1540317580384-e5d43616b9aa?ixlib=rb-4.0.3&auto=format&fit=crop&w=800&q=80';
        if (lower.includes('concert') || lower.includes('festival') || lower.includes('public')) return 'https://images.unsplash.com/photo-1459749411175-04bf5292ceea?ixlib=rb-4.0.3&auto=format&fit=crop&w=800&q=80';
        if (lower.includes('sport')) return 'https://images.unsplash.com/photo-1461896836934-ffe607ba8211?ixlib=rb-4.0.3&auto=format&fit=crop&w=800&q=80';

        // Fallback images based on index
        const fallbacks = [
            'https://images.unsplash.com/photo-1511556532299-8f662fc26c06?ixlib=rb-4.0.3&auto=format&fit=crop&w=800&q=80',
            'https://images.unsplash.com/photo-1505236858219-8373dd707528?ixlib=rb-4.0.3&auto=format&fit=crop&w=800&q=80',
            'https://images.unsplash.com/photo-1531058020387-3be344556be6?ixlib=rb-4.0.3&auto=format&fit=crop&w=800&q=80'
        ];
        return fallbacks[index % fallbacks.length];
    }
}
