import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { CustomerProductsPageComponent } from './pages/customer-products-page/customer-products-page.component';
const routes: Routes = [{ path: '', component: CustomerProductsPageComponent }];
@NgModule({ imports: [RouterModule.forChild(routes)], exports: [RouterModule] })
export class CustomerProductsRoutingModule {}
