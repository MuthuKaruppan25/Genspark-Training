import { CommonModule } from '@angular/common';

import { FormsModule } from '@angular/forms';
import { Component, EventEmitter, Input, Output } from '@angular/core';
import { UserModel } from '../Models/userModel';
@Component({
  selector: 'app-filtercard',
  imports: [FormsModule, CommonModule],
  templateUrl: './filtercard.html',
  styleUrl: './filtercard.css',
})
export class Filtercard {
  @Input() users: UserModel[] = [];

  @Output() filtersChanged = new EventEmitter<{
    gender: string;
    role: string;
    state: string;
  }>();

  gender = '';
  role = '';
  state = '';

  get uniqueGenders(): string[] {
    return Array.from(new Set(this.users.map((u) => u.gender))).filter(Boolean);
  }

  get uniqueRoles(): string[] {
    return Array.from(new Set(this.users.map((u) => u.role))).filter(Boolean);
  }

  get uniqueStates(): string[] {
    return Array.from(new Set(this.users.map((u) => u.address.state))).filter(
      Boolean
    );
  }

  emitChanges() {
    this.filtersChanged.emit({
      gender: this.gender,
      role: this.role,
      state: this.state,
    });
  }
  resetFilters() {
    this.gender = '';
    this.role = '';
    this.state = '';
    this.emitChanges();
  }
}
