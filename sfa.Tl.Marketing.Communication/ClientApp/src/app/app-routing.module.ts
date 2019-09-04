import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';

import { HomepageComponent } from '@features/homepage/homepage.component';
import { IntroductionComponent } from '@features/introduction/introduction.component';
import { BenefitsComponent } from '@features/benefits/benefits.component';
import { SubjectsComponent } from '@features/subjects/subjects.component';
import { FindAProviderComponent } from '@features/find-a-provider/find-a-provider.component';


const routes: Routes = [
  {
    path: '',
    component: HomepageComponent,
    pathMatch: 'full',
  },
  {
    path: 'introduction',
    component: IntroductionComponent,
  },
  {
    path: 'benefits',
    component: BenefitsComponent,
  },
  {
    path: 'subjects',
    component: SubjectsComponent,
  },
  {
    path: 'map',
    component: FindAProviderComponent,
  },
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
