import { Routes } from '@angular/router';
import { authGuard } from './core/guards/auth.guard';
import { roleGuard } from './core/guards/role.guard';

export const routes: Routes = [
    { path: '', loadComponent: () => import('./features/home/home.component').then(m => m.HomeComponent) },
    { path: 'login', loadComponent: () => import('./features/login/login.component').then(m => m.LoginComponent) },
    { path: 'register', loadComponent: () => import('./features/register/register.component').then(m => m.RegisterComponent) },

    // Legacy public pages (still working)
    { path: 'products', loadComponent: () => import('./features/products/products.component').then(m => m.ProductsComponent) },
    { path: 'claims', loadComponent: () => import('./features/claims/claims.component').then(m => m.ClaimsComponent) },
    { path: 'agent-portal', loadComponent: () => import('./features/agent-portal/agent-portal.component').then(m => m.AgentPortalComponent) },

    // Role-based Dashboards (SECURED)
    {
        path: 'dashboard/customer',
        canActivate: [authGuard, roleGuard],
        data: { role: 'Customer' },
        loadComponent: () => import('./features/customer-dashboard/customer-dashboard.component').then(m => m.CustomerDashboardComponent)
    },
    {
        path: 'dashboard/agent',
        canActivate: [authGuard, roleGuard],
        data: { role: 'Agent' },
        loadComponent: () => import('./features/agent-dashboard/agent-dashboard.component').then(m => m.AgentDashboardComponent)
    },
    {
        path: 'dashboard/claims-officer',
        canActivate: [authGuard, roleGuard],
        data: { role: 'ClaimsOfficer' },
        loadComponent: () => import('./features/claims-dashboard/claims-dashboard.component').then(m => m.ClaimsDashboardComponent)
    },
    {
        path: 'dashboard/admin',
        canActivate: [authGuard, roleGuard],
        data: { role: 'Admin' },
        loadComponent: () => import('./features/admin-dashboard/admin-dashboard.component').then(m => m.AdminDashboardComponent)
    },

    // Error Pages
    { path: 'error/401', loadComponent: () => import('./core/errors/unauthorized.component').then(m => m.UnauthorizedComponent) },
    { path: 'error/403', loadComponent: () => import('./core/errors/forbidden.component').then(m => m.ForbiddenComponent) },
    { path: 'error/404', loadComponent: () => import('./core/errors/not-found.component').then(m => m.NotFoundComponent) },
    { path: 'error/500', loadComponent: () => import('./core/errors/server-error.component').then(m => m.ServerErrorComponent) },
    { path: 'under-construction', loadComponent: () => import('./core/errors/under-construction.component').then(m => m.UnderConstructionComponent) },

    // Wildcard: Any unknown route redirects to the 404 page
    { path: '**', redirectTo: 'error/404' }
];
