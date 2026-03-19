import { Routes } from '@angular/router';

export const routes: Routes = [
  {
    path: '',
    loadComponent: () =>
      import('./features/home/pages/home-page.component').then((m) => m.HomePageComponent)
  },
  {
    path: 'permissions',
    loadComponent: () =>
      import('./features/permissions/pages/permissions-page.component').then(
        (m) => m.PermissionsPageComponent
      )
  }
];
