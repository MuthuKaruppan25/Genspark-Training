import { Component, Input, OnChanges } from '@angular/core';
import { UserModel } from '../Models/userModel';
import { ChartConfiguration, ChartType } from 'chart.js';
import { NgChartsModule } from 'ng2-charts';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-genderchart',
  standalone: true,
  imports: [NgChartsModule, CommonModule],
  templateUrl: './genderchart.html',
  styleUrls: ['./genderchart.css']
})
export class Genderchart implements OnChanges {
  @Input() users: UserModel[] = [];

  genderChartLabels = ['Male', 'Female'];
  genderChartType: ChartType = 'pie';

genderChartData: ChartConfiguration['data'] = {
  labels: this.genderChartLabels,
  datasets: [
    {
      data: [0, 0],
      backgroundColor: ['#42A5F5', '#FF6384'],
    }
  ]
};

chartOptions: ChartConfiguration['options'] = {
  responsive: true,
  plugins: {
    legend: {
      position: 'top'
    },
    tooltip: {
      enabled: true
    }
  }
};


  ngOnChanges(): void {
    const male = this.users.filter(user => user.gender === 'male').length;
    const female = this.users.filter(user => user.gender === 'female').length;

    this.genderChartData = {
      labels: this.genderChartLabels,
      datasets: [
        {
          data: [male, female],
          backgroundColor: ['#42A5F5', '#FF6384'],
        }
      ]
    };
  }
}
