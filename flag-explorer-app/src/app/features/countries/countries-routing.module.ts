import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { CountryListComponent } from './pages/country-list/country-list.component';
import { CountryDetailsComponent } from './pages/country-details/country-details.component';

const routes: Routes = [
  {
    path: '',
    component: CountryListComponent
  },
  {
    path: ':name',
    component: CountryDetailsComponent
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class CountriesRoutingModule { }