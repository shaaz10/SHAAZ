import { inject } from '@angular/core';
import { Router, CanActivateFn } from '@angular/router';
import { AuthService } from '../services/auth.service';

/**
 * Role Guard
 * Restricts access to specific routes based on user role (Admin, Agent, Customer, etc.)
 */
export const roleGuard: CanActivateFn = (route, state) => {
    const authService = inject(AuthService);
    const router = inject(Router);

    // Requirement: The route definition must include data: { role: 'RoleName' }
    const expectedRole = route.data['role'];

    if (authService.hasRole(expectedRole)) {
        return true;
    }

    // Not authorized, redirect to hidden forbidden page
    router.navigate(['/error/403']);
    return false;
};
