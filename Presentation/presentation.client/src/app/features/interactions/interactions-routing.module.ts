import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { InteractionsPageComponent } from './pages/interactions-page/interactions-page.component';
const routes: Routes = [{ path: '', component: InteractionsPageComponent }];
@NgModule({ imports: [RouterModule.forChild(routes)], exports: [RouterModule] })
export class InteractionsRoutingModule {}
