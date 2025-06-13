import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import {
  BehaviorSubject,
  Observable,
  ReplaySubject,
  catchError,
  interval,
  of,
  switchMap,
  throwError,
} from 'rxjs';
import { startWith } from 'rxjs/operators';
@Injectable({ providedIn: 'root' })
export class WeatherService {
  private apiKey = '968df098cfdf94084c866bf7597cc025';
  private baseUrl = 'https://api.openweathermap.org/data/2.5/weather';

  private citySubject = new BehaviorSubject<string>('Chennai');
  city$ = this.citySubject.asObservable();

  private historySubject = new BehaviorSubject<string[]>(
    this.getStoredHistory()
  );
  history$ = this.historySubject.asObservable();

  constructor(private http: HttpClient) {
    this.loadHistoryFromLocalStorage();
  }

  updateCity(city: string) {
    this.citySubject.next(city);
    this.addToHistory(city);
  }

  getWeather(city: string): Observable<any> {
    return this.http
      .get(`${this.baseUrl}?q=${city}&appid=${this.apiKey}&units=metric`)
      .pipe(
        catchError((err) => {
          if (err.status === 404) {
            return of(null);
          }
          return throwError(() => err);
        })
      );
  }

  getWeatherStream(): Observable<any> {
    return this.city$.pipe(
      switchMap((city) =>
        interval(300000).pipe(
          startWith(0),
          switchMap(() => this.getWeather(city))
        )
      )
    );
  }
  private addToHistory(city: string) {
    const existing = this.getStoredHistory();
    const updated = [
      city,
      ...existing.filter((c) => c.toLowerCase() !== city.toLowerCase()),
    ].slice(0, 5);
    localStorage.setItem('weather-history', JSON.stringify(updated));
    this.historySubject.next(updated); // emit the array
  }

  private loadHistoryFromLocalStorage() {
    const history = this.getStoredHistory();
    this.historySubject.next(history);
  }

  private getStoredHistory(): string[] {
    const raw = localStorage.getItem('weather-history');
    return raw ? JSON.parse(raw) : [];
  }
}
