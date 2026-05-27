import { Routes } from '@angular/router';
import { authGuard } from './core/guards/auth.guard';

export const routes: Routes = [
    {
        path: 'login',
        loadComponent: () => import('./features/auth/pages/login-page/login-page.component').then(m => m.LoginPageComponent)
    },
    {
        path: '',
        canActivate: [authGuard],
        children: [
            {
                path: '',
                redirectTo: '/dashboard',
                pathMatch: 'full'
            },
            {
                path: 'dashboard',
                loadComponent: () => import('./features/home/pages/dashboard-page/dashboard-page.component')
                    .then(m => m.DashboardPageComponent)
            },
            {
                path: 'notes',
                loadComponent: () => import('./features/notes/pages/notes-page/notes-page.component')
                    .then(m => m.NotesPageComponent)
            },
            {
                path: 'goals',
                loadComponent: () => import('./features/goals/pages/goals-page/goals-page.component')
                    .then(m => m.GoalsPageComponent)
            },
            {
                path: 'goals/:id',
                loadComponent: () => import('./features/goals/pages/goal-editor-page/goal-editor-page.component')
                    .then(m => m.GoalEditorPageComponent)
            },
            {
                path: 'pomodoro',
                loadComponent: () => import('./features/pomodoro/pages/pomodoro-page/pomodoro-page.component')
                    .then(m => m.PomodoroPageComponent)
            },

            {
                path: 'settings',
                loadComponent: () => import('./features/settings/pages/settings-page/settings-page.component')
                    .then(m => m.SettingsPageComponent)
            },
        ]
    },
    { path: '**', redirectTo: 'login' }
];
