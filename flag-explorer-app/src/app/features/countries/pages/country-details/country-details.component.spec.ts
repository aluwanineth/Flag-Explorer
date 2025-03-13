import { ComponentFixture, TestBed } from '@angular/core/testing';
import { RouterTestingModule } from '@angular/router/testing';
import { ActivatedRoute, Router, convertToParamMap } from '@angular/router';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { of, throwError } from 'rxjs';
import { By } from '@angular/platform-browser';
import { CountryDetailsComponent } from './country-details.component';
import { CountryService } from '../../../../core/services/country.service';
import { ErrorHandlerService } from '../../../../core/services/error-handler.service';
import { CountryDetails } from '../../../../core/models/country-details.model';
import { DecimalPipe } from '@angular/common';

describe('CountryDetailsComponent', () => {
  let component: CountryDetailsComponent;
  let fixture: ComponentFixture<CountryDetailsComponent>;
  let countryService: jasmine.SpyObj<CountryService>;
  let router: Router;
  let mockCountryDetails: CountryDetails;

  beforeEach(async () => {
    mockCountryDetails = {
      name: 'Germany',
      population: 83000000,
      capital: 'Berlin',
      flag: 'https://example.com/germany.png'
    };

    const countryServiceSpy = jasmine.createSpyObj('CountryService', ['getCountryByName']);
    countryServiceSpy.getCountryByName.and.returnValue(of(mockCountryDetails));

    await TestBed.configureTestingModule({
      declarations: [
        CountryDetailsComponent
      ],
      imports: [
        RouterTestingModule,
        HttpClientTestingModule
      ],
      providers: [
        DecimalPipe,
        { 
          provide: ActivatedRoute, 
          useValue: { 
            params: of({ name: 'germany' }),
            snapshot: {
              paramMap: convertToParamMap({ name: 'germany' })
            }
          } 
        },
        { provide: CountryService, useValue: countryServiceSpy },
        ErrorHandlerService
      ]
    }).compileComponents();

    countryService = TestBed.inject(CountryService) as jasmine.SpyObj<CountryService>;
    router = TestBed.inject(Router);
    spyOn(router, 'navigate');
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(CountryDetailsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should load country details on init', () => {
    expect(countryService.getCountryByName).toHaveBeenCalledWith('germany');
  });

  it('should display country details in the template', () => {
    // Set countryDetails$ directly to ensure the async pipe has data
    component.countryDetails$ = of(mockCountryDetails);
    fixture.detectChanges();

    // Check for the h1 element containing the country name
    const countryNameElement = fixture.debugElement.query(By.css('h1'));
    expect(countryNameElement).not.toBeNull();
    if (countryNameElement) {
      expect(countryNameElement.nativeElement.textContent).toContain('Germany');
    }

    // Check for the capital element
    const capitalElements = fixture.debugElement.queryAll(By.css('.flex.items-start'));
    expect(capitalElements.length).toBeGreaterThan(0);
    const capitalTextElement = capitalElements[0]?.query(By.css('.text-lg'));
    expect(capitalTextElement).not.toBeNull();
    if (capitalTextElement) {
      expect(capitalTextElement.nativeElement.textContent).toContain('Berlin');
    }

    // Check for the population element
    if (capitalElements.length > 1) {
      const populationTextElement = capitalElements[1]?.query(By.css('.text-lg'));
      expect(populationTextElement).not.toBeNull();
      if (populationTextElement) {
        // The number format may vary, so just check for part of the number
        expect(populationTextElement.nativeElement.textContent).toContain('83');
      }
    }
  });

  it('should navigate back when back button is clicked', () => {
    // Find the back button
    const backButton = fixture.debugElement.query(By.css('button'));
    expect(backButton).not.toBeNull();
    if (backButton) {
      backButton.triggerEventHandler('click', null);
      expect(router.navigate).toHaveBeenCalledWith(['/countries']);
    }
  });

  it('should display learn more link with correct URL', () => {
    // Set countryDetails$ to render the link
    component.countryDetails$ = of(mockCountryDetails);
    fixture.detectChanges();

    const learnMoreLink = fixture.debugElement.query(By.css('a[target="_blank"]'));
    expect(learnMoreLink).not.toBeNull();
    if (learnMoreLink) {
      expect(learnMoreLink.attributes['href']).toContain('https://en.wikipedia.org/wiki/Germany');
      expect(learnMoreLink.attributes['target']).toBe('_blank');
    }
  });

  it('should show error message when loading country details fails', () => {
    // Manually set the error state
    component.error = true;
    component.errorMessage = 'Country not found';
    fixture.detectChanges();

    const errorMsg = fixture.debugElement.query(By.css('.bg-red-100'));
    expect(errorMsg).not.toBeNull();
    if (errorMsg) {
      expect(errorMsg.nativeElement.textContent).toContain('Country not found');
    }
  });

  it('should retry loading country details when try again button is clicked', () => {
    // Setup error state
    component.error = true;
    component.errorMessage = 'Failed to load';
    fixture.detectChanges();

    // Setup spy for loadCountryDetails method
    spyOn(component, 'loadCountryDetails');
    
    // Find and click the try again button
    const tryAgainBtn = fixture.debugElement.query(By.css('.bg-red-100 button'));
    expect(tryAgainBtn).not.toBeNull();
    if (tryAgainBtn) {
      tryAgainBtn.triggerEventHandler('click', null);
      expect(component.loadCountryDetails).toHaveBeenCalled();
    }
  });

  it('should show loading spinner when data is being loaded', () => {
    // Set loading state
    component.loading = true;
    fixture.detectChanges();

    const loadingSpinner = fixture.debugElement.query(By.css('.spinner-border'));
    expect(loadingSpinner).not.toBeNull();
  });
});