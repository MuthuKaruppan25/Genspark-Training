import { Component } from '@angular/core';
import { UserService } from '../Services/UserService';
import { UserModel } from '../Models/userModel';
import { Genderchart } from '../genderchart/genderchart';
import { Statechart } from '../statechart/statechart';
import { Rolechart } from '../rolechart/rolechart';
import { NgChartsModule } from 'ng2-charts';
import { Filtercard } from '../filtercard/filtercard';

@Component({
  selector: 'app-dashboard',
  imports: [Genderchart, Statechart, Rolechart, NgChartsModule, Filtercard],
  templateUrl: './dashboard.html',
  styleUrl: './dashboard.css',
})
export class Dashboard {
  users: UserModel[] = [];
  filteredUsers: UserModel[] = [];

  constructor(private userService: UserService) {}

  ngOnInit() {
    this.userService.getUsers().subscribe({
      next: (data: any) => {
        this.users = data;
        this.filteredUsers = [...this.users];
      },
    });
  }

  handleFilterChange(filters: { gender: string; role: string; state: string }) {
    this.filteredUsers = this.users.filter((user) => {
      const matchGender = filters.gender
        ? user.gender === filters.gender
        : true;
      const matchRole = filters.role ? user.role === filters.role : true;
      const matchState = filters.state
        ? user.address.state === filters.state
        : true;
      return matchGender && matchRole && matchState;
    });
  }
}
