import { NgModule } from '@angular/core';
import { SharedModule } from '../../shared/shared.module';
import { ProductsRoutingModule } from './products-routing.module';
import { ProductsPageComponent } from './pages/products-page/products-page.component';
@NgModule({ declarations: [ProductsPageComponent], imports: [SharedModule, ProductsRoutingModule] })
export class ProductsModule {}
