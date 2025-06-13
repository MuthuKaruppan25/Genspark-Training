import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common'; // <-- Add this import
import { WeatherService } from '../service/weatherservice';
import { Observable } from 'rxjs';
import { WeatherCard } from "../weather-card/weather-card";
import { Search } from "../search/search";

@Component({
  selector: 'app-weather-dashboard',
  imports: [CommonModule, WeatherCard, Search], 
  templateUrl: './weather-dashboard.html',
  styleUrl: './weather-dashboard.css',
})
export class WeatherDashboard implements OnInit {
  weather$: Observable<any> | null = null;

  constructor(private weatherService: WeatherService) {

  }

  ngOnInit() {
    this.weather$ = this.weatherService.getWeatherStream();
  }
}