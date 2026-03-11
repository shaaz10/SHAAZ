import { HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { Router } from '@angular/router';
import { catchError, throwError } from 'rxjs';

/**
 * HTTP Error Interceptor
 * Intercepts HTTP responses and redirects to error pages ONLY for
 * authentication/authorization errors (401, 403).
 * 500 errors are NOT redirected — they are handled by individual components.
 */
export const errorInterceptor: HttpInterceptorFn = (req, next) => {
    const router = inject(Router);

    return next(req).pipe(
        catchError((error) => {
            switch (error.status) {
                case 401:
                    // Unauthorized – user is not logged in or token expired
                    // Only redirect if not already on the login page
                    if (!router.url.includes('/login')) {
                        router.navigate(['/error/401']);
                    }
                    break;
                case 403:
                    // Forbidden – user lacks the required role/permissions
                    router.navigate(['/error/403']);
                    break;
                // 500 errors are NOT redirected — let components handle them with their own error messages
            }
            return throwError(() => error);
        })
    );
};
