import { Component } from '@angular/core';
import { UserService } from '../service/authservice';

@Component({
  selector: 'app-display',
  imports: [],
  templateUrl: './display.html',
  styleUrl: './display.css',
})
export class Display {
  username$: any;
  usrname: string | null = '';

  constructor(private userService: UserService) {
   
    this.userService.username$.subscribe({
      next: (value) => {
        this.usrname = value;
      },
      error: (err) => {
        alert(err);
      },
    });
  }
}
