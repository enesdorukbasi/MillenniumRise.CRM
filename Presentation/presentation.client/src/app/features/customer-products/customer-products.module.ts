import { NgModule } from '@angular/core';
import { SharedModule } from '../../shared/shared.module';
import { CustomerProductsRoutingModule } from './customer-products-routing.module';
import { CustomerProductsPageComponent } from './pages/customer-products-page/customer-products-page.component';
@NgModule({ declarations: [CustomerProductsPageComponent], imports: [SharedModule, CustomerProductsRoutingModule] })
export class CustomerProductsModule {}
