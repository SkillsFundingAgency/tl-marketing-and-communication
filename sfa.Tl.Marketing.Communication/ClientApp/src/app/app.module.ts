import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';

import { BsDropdownModule } from 'ngx-bootstrap/dropdown';
import { TooltipModule } from 'ngx-bootstrap/tooltip';
import { ModalModule } from 'ngx-bootstrap/modal';

import { AppRoutingModule } from './app-routing.module';
import { AgmCoreModule } from '@agm/core';
import { ProvidersService } from './core/services/providers.service';
import { AgmSnazzyInfoWindowModule } from '@agm/snazzy-info-window';
import { environment } from 'src/environments/environment';
import { HttpClientModule } from '@angular/common/http';
import { FormsModule } from '@angular/forms';
import { AppComponent } from './app.component';
import { HomepageComponent } from './features/homepage/homepage.component';
import { FindAProviderComponent } from './features/find-a-provider/find-a-provider.component';
import { HeaderComponent } from './core/header/header.component';
import { FooterComponent } from './core/footer/footer.component';
import { IntroductionComponent } from './features/introduction/introduction.component';
import { BenefitsComponent } from './features/benefits/benefits.component';
import { SubjectsComponent } from './features/subjects/subjects.component';
import { NavigationComponent } from './core/navigation/navigation.component';

@NgModule({
  imports: [
    BrowserModule,
    BsDropdownModule.forRoot(),
    TooltipModule.forRoot(),
    ModalModule.forRoot(),
    AppRoutingModule,
    HttpClientModule,
    AgmCoreModule.forRoot({
      apiKey: environment.apiKey,
      libraries: ['places']
    }),
    FormsModule,
    AgmSnazzyInfoWindowModule
  ],
  declarations: [
    AppComponent,
    HomepageComponent,
    FindAProviderComponent,
    HeaderComponent,
    FooterComponent,
    IntroductionComponent,
    BenefitsComponent,
    SubjectsComponent,
    NavigationComponent
  ],
  providers: [ProvidersService],
  bootstrap: [AppComponent]
})
export class AppModule { }
