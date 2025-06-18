import { Component, Input, OnChanges } from '@angular/core';
import { UserModel } from '../Models/userModel';
import { ChartConfiguration, ChartType } from 'chart.js';
import { NgChartsModule } from 'ng2-charts';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-rolechart',
  standalone: true,
  imports: [NgChartsModule, CommonModule],
  templateUrl: './rolechart.html',
  styleUrls: ['./rolechart.css']
})
export class Rolechart implements OnChanges {
  @Input() users: UserModel[] = [];

  roleChartType: ChartType = 'bar';
  roleLabels: string[] = [];
  roleChartData: ChartConfiguration['data'] = {
    labels: [],
    datasets: [
      {
        label: 'Users per Role',
        data: [],
        backgroundColor: []
      }
    ]
  };

  chartOptions: ChartConfiguration['options'] = {
    responsive: true,
    plugins: {
      legend: {
        display: false
      },
      tooltip: {
        enabled: true
      }
    },
    scales: {
      x: {},
      y: {
        beginAtZero: true,
        ticks: {
          precision: 0
        }
      }
    }
  };

  ngOnChanges(): void {
    const roleMap = new Map<string, number>();

    for (const user of this.users) {
      roleMap.set(user.role, (roleMap.get(user.role) || 0) + 1);
    }

    const labels = Array.from(roleMap.keys());
    const data = Array.from(roleMap.values());

    this.roleChartData = {
      labels,
      datasets: [
        {
          label: 'Users per Role',
          data,
          backgroundColor: this.generateColors(labels.length)
        }
      ]
    };
  }

  private generateColors(count: number): string[] {
    const baseColors = ['#42A5F5', '#66BB6A', '#FFA726', '#FF7043', '#7E57C2', '#26A69A'];
    const colors = [];

    for (let i = 0; i < count; i++) {
      colors.push(baseColors[i % baseColors.length]);
    }

    return colors;
  }
}
