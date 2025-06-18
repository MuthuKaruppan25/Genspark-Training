import { Component, ElementRef, Input, OnChanges, AfterViewInit, ViewChild } from '@angular/core';
import * as L from 'leaflet';
import usMap from '../../assets/us-states.json';
import { UserModel } from '../Models/userModel';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-statechart',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './statechart.html',
  styleUrls: ['./statechart.css']
})
export class Statechart implements OnChanges, AfterViewInit {
  @Input() users: UserModel[] = [];
  @ViewChild('mapContainer', { static: true }) mapContainer!: ElementRef;

  private map!: L.Map;
  private geoJsonLayer!: L.GeoJSON<any>;

  ngAfterViewInit(): void {
    this.initMap();
    setTimeout(() => this.map.invalidateSize(), 0); // Ensures map renders to full size
  }

  ngOnChanges(): void {
    if (this.map) {
      this.updateMap();
    }
  }

  private initMap(): void {
    this.map = L.map(this.mapContainer.nativeElement, {
      zoomControl: false,
      attributionControl: false,
      dragging: false,
      scrollWheelZoom: false,
      doubleClickZoom: false,
      boxZoom: false,
      keyboard: false
    }).setView([37.8, -96], 4);

    this.addGeoJsonLayer();
  }

  private updateMap(): void {
    if (this.geoJsonLayer) {
      this.geoJsonLayer.clearLayers();
      this.geoJsonLayer.removeFrom(this.map);
    }
    this.addGeoJsonLayer();
  }

  private addGeoJsonLayer(): void {
    const stateCounts = new Map<string, number>();
    this.users.forEach(user => {
      const state = user.address.state;
      stateCounts.set(state, (stateCounts.get(state) || 0) + 1);
    });

    this.geoJsonLayer = L.geoJSON(usMap as any, {
      style: (feature) => ({
        fillColor: this.getColor(stateCounts.get(feature?.properties?.name) || 0),
        weight: 1,
        opacity: 1,
        color: '#999999',
        fillOpacity: 1
      }),
      onEachFeature: (feature, layer) => {
        const stateName = feature?.properties?.name;
        const count = stateCounts.get(stateName) || 0;
        layer.bindPopup(`<strong>${stateName}</strong><br/>Users: ${count}`);
      }
    });

    this.geoJsonLayer.addTo(this.map);
  }

  private getColor(count: number): string {
    return count > 20 ? '#2C2C2C' :
           count > 10 ? '#4B4B4B' :
           count > 5  ? '#7C7C7C' :
           count > 2  ? '#B0B0B0' :
           count > 0  ? '#DADADA' : '#F8F8F8';
  }
}
