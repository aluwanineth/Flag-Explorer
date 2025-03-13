import { Component, OnInit } from '@angular/core';
import { catchError, Observable, of } from 'rxjs';
import { Country } from '../../../../core/models/country.model';
import { CountryService } from '../../../../core/services/country.service';
import { Router } from '@angular/router';
import { ErrorHandlerService } from '../../../../core/services/error-handler.service';

@Component({
  selector: 'app-country-list',
  templateUrl: './country-list.component.html',
  styleUrls: ['./country-list.component.scss']
})
export class CountryListComponent implements OnInit {
  countries$: Observable<Country[]> = of([]);
  allCountries: Country[] = [];
  filteredCountries: Country[] = [];
  searchTerm: string = '';
  loading = true;
  error = false;
  errorMessage = '';

  constructor(
    private countryService: CountryService,
    private errorHandler: ErrorHandlerService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.loadCountries();
  }

  loadCountries(): void {
    this.loading = true;
    this.error = false;
    this.countries$ = this.countryService.getAllCountries().pipe(
      catchError(error => {
        this.loading = false;
        this.error = true;
        this.errorMessage = error.message;
        return of([]);
      })
    );
    
    this.countries$.subscribe({
      next: (countries) => {
        this.loading = false;
        this.allCountries = countries;
        this.filteredCountries = [...this.allCountries];
      },
      error: () => this.loading = false
    });
  }

  filterCountries(): void {
    if (!this.searchTerm.trim()) {
      this.filteredCountries = [...this.allCountries];
      return;
    }
    
    const search = this.searchTerm.toLowerCase().trim();
    this.filteredCountries = this.allCountries.filter(country => 
      country.name.toLowerCase().includes(search)
    );
  }

  clearSearch(): void {
    this.searchTerm = '';
    this.filteredCountries = [...this.allCountries];
  }

  goToDetails(countryName: string): void {
    this.router.navigate(['/countries', countryName]);
  }
}