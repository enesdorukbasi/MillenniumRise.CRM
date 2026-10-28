import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { CustomerProductDetailPageComponent } from './pages/customer-product-detail-page/customer-product-detail-page.component';
import { ProductDetailPageComponent } from './pages/product-detail-page/product-detail-page.component';
import { ProductsPageComponent } from './pages/products-page/products-page.component';
const routes: Routes = [
  { path: ':productId/customer-products/:customerProductId', component: CustomerProductDetailPageComponent },
  { path: ':id', component: ProductDetailPageComponent },
  { path: '', component: ProductsPageComponent }
];
@NgModule({ imports: [RouterModule.forChild(routes)], exports: [RouterModule] })
export class ProductsRoutingModule {}
