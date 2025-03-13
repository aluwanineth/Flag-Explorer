import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import { Country } from '../models/country.model';
import { CountryDetails } from '../models/country-details.model';


@Injectable({
  providedIn: 'root'
})
export class CountryService {
  private apiUrl = environment.apiUrl;

  constructor(private http: HttpClient) {}

 
  getAllCountries(): Observable<Country[]> {
    return this.http.get<Country[]>(`${this.apiUrl}/countries`);
  }

  getCountryByName(name: string): Observable<CountryDetails> {
    return this.http.get<CountryDetails>(`${this.apiUrl}/countries/${name}`);
  }
}


