import { TestBed } from '@angular/core/testing';
import { HttpErrorResponse } from '@angular/common/http';
import { ErrorHandlerService } from './error-handler.service';

describe('ErrorHandlerService', () => {
  let service: ErrorHandlerService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(ErrorHandlerService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  describe('handleError', () => {
    it('should handle client-side error', () => {
      const errorEvent = new ErrorEvent('Client Error', {
        message: 'Network error occurred'
      });
      const error = new HttpErrorResponse({
        error: errorEvent,
        statusText: 'Client Error',
        status: 0
      });

      service.handleError(error).subscribe({
        next: () => fail('Should have returned an error observable'),
        error: (err) => {
          expect(err).toBeTruthy();
          expect(err.message).toContain('Network error occurred');
        }
      });
    });

    it('should handle 404 error with user-friendly message', () => {
      const error = new HttpErrorResponse({
        error: 'Not Found',
        status: 404,
        statusText: 'Not Found'
      });

      service.handleError(error).subscribe({
        next: () => fail('Should have returned an error observable'),
        error: (err) => {
          expect(err).toBeTruthy();
          expect(err.message).toContain('resource was not found');
        }
      });
    });

    it('should handle 500 error with user-friendly message', () => {
      const error = new HttpErrorResponse({
        error: 'Server Error',
        status: 500,
        statusText: 'Server Error'
      });

      service.handleError(error).subscribe({
        next: () => fail('Should have returned an error observable'),
        error: (err) => {
          expect(err).toBeTruthy();
          expect(err.message).toContain('Server error occurred');
        }
      });
    });

    it('should handle other server errors with status code', () => {
      const error = new HttpErrorResponse({
        error: 'Bad Request',
        status: 400,
        statusText: 'Bad Request'
      });

      service.handleError(error).subscribe({
        next: () => fail('Should have returned an error observable'),
        error: (err) => {
          expect(err).toBeTruthy();
          expect(err.message).toContain('Error Code: 400');
        }
      });
    });
  });
});