import { NgModule } from '@angular/core';
import { SharedModule } from '../../shared/shared.module';
import { ProgramConfigurationPageComponent } from './pages/program-configuration-page/program-configuration-page.component';
import { ProgramConfigurationRoutingModule } from './program-configuration-routing.module';

@NgModule({
  declarations: [ProgramConfigurationPageComponent],
  imports: [SharedModule, ProgramConfigurationRoutingModule]
})
export class ProgramConfigurationModule {}
