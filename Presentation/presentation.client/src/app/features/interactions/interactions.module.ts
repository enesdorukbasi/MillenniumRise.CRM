import { NgModule } from '@angular/core';
import { SharedModule } from '../../shared/shared.module';
import { InteractionsRoutingModule } from './interactions-routing.module';
import { InteractionsPageComponent } from './pages/interactions-page/interactions-page.component';
@NgModule({ declarations: [InteractionsPageComponent], imports: [SharedModule, InteractionsRoutingModule] })
export class InteractionsModule {}
