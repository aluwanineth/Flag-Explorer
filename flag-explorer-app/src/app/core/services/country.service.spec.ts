import { TestBed } from '@angular/core/testing';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { CountryService } from './country.service';
import { ErrorHandlerService } from './error-handler.service';
import { of, throwError } from 'rxjs';

describe('CountryService', () => {
  let service: CountryService;
  let httpMock: HttpTestingController;
  let errorHandler: jasmine.SpyObj<ErrorHandlerService>;

  beforeEach(() => {
    // Create spy for ErrorHandlerService
    const errorHandlerSpy = jasmine.createSpyObj('ErrorHandlerService', ['handleError']);
    errorHandlerSpy.handleError.and.callFake((error: any) => throwError(() => error));

    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
      providers: [
        CountryService,
        { provide: ErrorHandlerService, useValue: errorHandlerSpy }
      ]
    });
    
    service = TestBed.inject(CountryService);
    httpMock = TestBed.inject(HttpTestingController);
    errorHandler = TestBed.inject(ErrorHandlerService) as jasmine.SpyObj<ErrorHandlerService>;
  });

  afterEach(() => {
    httpMock.verify();
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  describe('getAllCountries', () => {
    it('should make a GET request to fetch all countries', () => {
      const mockCountries = [
        { name: 'Germany', flag: 'https://example.com/germany.png' },
        { name: 'Brazil', flag: 'https://example.com/brazil.png' }
      ];

      service.getAllCountries().subscribe(countries => {
        expect(countries).toEqual(mockCountries);
        expect(countries.length).toBe(2);
      });

      // Match any URL ending with /countries
      const req = httpMock.expectOne(req => req.url.endsWith('/countries'));
      expect(req.request.method).toBe('GET');
      req.flush(mockCountries);
    });
  });
});