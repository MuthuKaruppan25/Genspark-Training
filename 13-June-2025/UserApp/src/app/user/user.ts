import { Component } from '@angular/core';
import { UserModel } from '../Models/user';
import { AuthenticationService } from '../service/userservice';

@Component({
  selector: 'app-user',
  imports: [],
  templateUrl: './user.html',
  styleUrl: './user.css',
})
export class User {
  user: UserModel | null = null;
  
  constructor(private authService: AuthenticationService) {
    this.user = this.authService.getFromStorage(true);
  }


}
