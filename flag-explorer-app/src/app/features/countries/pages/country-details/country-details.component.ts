import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { Observable, catchError, of, switchMap } from 'rxjs';
import { CountryDetails } from '../../../../core/models/country-details.model';
import { CountryService } from '../../../../core/services/country.service';
import { ErrorHandlerService } from '../../../../core/services/error-handler.service';

@Component({
  selector: 'app-country-details',
  templateUrl: './country-details.component.html',
  styleUrls: ['./country-details.component.scss']
})
export class CountryDetailsComponent implements OnInit {
  countryDetails$: Observable<CountryDetails | null> = of(null);
  loading = true;
  error = false;
  errorMessage = '';

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private countryService: CountryService,
    private errorHandler: ErrorHandlerService
  ) {}

  ngOnInit(): void {
    this.loadCountryDetails();
  }

  loadCountryDetails(): void {
    this.loading = true;
    this.error = false;
    
    this.countryDetails$ = this.route.params.pipe(
      switchMap(params => {
        const countryName = params['name'];
        return this.countryService.getCountryByName(countryName).pipe(
          catchError(error => {
            this.loading = false;
            this.error = true;
            this.errorMessage = error.message;
            return of(null);
          })
        );
      })
    );
    
    // Subscribe to update loading state
    this.countryDetails$.subscribe({
      next: () => this.loading = false,
      error: () => this.loading = false
    });
  }

  goBack(): void {
    this.router.navigate(['/countries']);
  }
}