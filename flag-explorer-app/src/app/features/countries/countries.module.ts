import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { CountriesRoutingModule } from './countries-routing.module';
import { CountryListComponent } from './pages/country-list/country-list.component';
import { CountryDetailsComponent } from './pages/country-details/country-details.component';
import { SharedModule } from '../../shared/shared.module';

@NgModule({
  declarations: [
    CountryListComponent,
    CountryDetailsComponent
  ],
  imports: [
    CommonModule,
    SharedModule,
    CountriesRoutingModule
  ]
})
export class CountriesModule { }