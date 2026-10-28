import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { ProgramConfigurationPageComponent } from './pages/program-configuration-page/program-configuration-page.component';

const routes: Routes = [{ path: '', component: ProgramConfigurationPageComponent }];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class ProgramConfigurationRoutingModule {}
