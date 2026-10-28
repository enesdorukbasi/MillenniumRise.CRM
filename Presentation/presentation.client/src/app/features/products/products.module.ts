import { NgModule } from '@angular/core';
import { SharedModule } from '../../shared/shared.module';
import { ProductsRoutingModule } from './products-routing.module';
import { CustomerProductDetailPageComponent } from './pages/customer-product-detail-page/customer-product-detail-page.component';
import { ProductDetailPageComponent } from './pages/product-detail-page/product-detail-page.component';
import { ProductsPageComponent } from './pages/products-page/products-page.component';
@NgModule({ declarations: [ProductsPageComponent, ProductDetailPageComponent, CustomerProductDetailPageComponent], imports: [SharedModule, ProductsRoutingModule] })
export class ProductsModule {}
