import { Injectable } from '@angular/core';
import { HttpErrorResponse } from '@angular/common/http';
import { throwError } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class ErrorHandlerService {
  
  /**
   * Handle HTTP errors and return a user-friendly error observable
   */
  handleError(error: HttpErrorResponse) {
    let errorMessage = 'An unknown error occurred';
    
    if (error.error instanceof ErrorEvent) {
      // Client-side error
      errorMessage = `Error: ${error.error.message}`;
    } else {
      // Server-side error
      if (error.status === 404) {
        errorMessage = 'The requested resource was not found';
      } else if (error.status === 500) {
        errorMessage = 'Server error occurred. Please try again later';
      } else {
        errorMessage = `Error Code: ${error.status}, Message: ${error.message}`;
      }
    }
    
    console.error(errorMessage);
    return throwError(() => new Error(errorMessage));
  }
}