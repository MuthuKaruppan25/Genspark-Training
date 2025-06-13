import { Component } from '@angular/core';
import { CommonModule } from '@angular/common'; // <-- Import this
import { WeatherService } from '../service/weatherservice';
import { Observable } from 'rxjs';

@Component({
  selector: 'app-search',
  standalone: true, // <-- If using standalone components
  imports: [CommonModule], // <-- Add CommonModule here
  templateUrl: './search.html',
  styleUrls: ['./search.css']
})
export class Search {
  city: string = '';
  history$: Observable<string[]> | null = null;

  constructor(private weatherService: WeatherService) {
    this.history$ = this.weatherService.history$;
  }

  search(city:string) {
    if (city.trim()) {
      this.weatherService.updateCity(city.trim());
    }
  }

  searchFromHistory(city: string) {
    this.weatherService.updateCity(city);
  }
}