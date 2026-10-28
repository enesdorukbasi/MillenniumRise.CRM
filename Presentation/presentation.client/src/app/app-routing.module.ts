import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { authGuard } from './core/auth/auth.guard';
import { guestGuard } from './core/auth/guest.guard';
import { AdminLayoutComponent } from './layout/admin-layout/admin-layout.component';

const routes: Routes = [
  {
    path: 'login',
    canMatch: [guestGuard],
    loadChildren: () => import('./features/auth/auth.module').then(module => module.AuthModule)
  },
  {
    path: '',
    component: AdminLayoutComponent,
    canActivate: [authGuard],
    children: [
      { path: 'dashboard', loadChildren: () => import('./features/dashboard/dashboard.module').then(module => module.DashboardModule) },
      { path: 'customers', loadChildren: () => import('./features/customers/customers.module').then(module => module.CustomersModule) },
      { path: 'products', loadChildren: () => import('./features/products/products.module').then(module => module.ProductsModule) },
      { path: 'files', loadChildren: () => import('./features/files/files.module').then(module => module.FilesModule) },
      { path: 'program-configuration', loadChildren: () => import('./features/program-configuration/program-configuration.module').then(module => module.ProgramConfigurationModule) },
      { path: '', pathMatch: 'full', redirectTo: 'dashboard' }
    ]
  },
  { path: '**', redirectTo: '' }
];

@NgModule({
  imports: [RouterModule.forRoot(routes, { scrollPositionRestoration: 'top' })],
  exports: [RouterModule]
})
export class AppRoutingModule {}
