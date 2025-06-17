import { Component } from '@angular/core';
import { UserModel } from '../Models/User';
import { UserService } from '../Services/userService';

@Component({
  selector: 'app-profile',
  imports: [],
  templateUrl: './profile.html',
  styleUrl: './profile.css'
})
export class Profile {
  profile : UserModel = new UserModel();
  constructor(private userService : UserService){
      this.userService.callGetProfile().subscribe({
        next:(data:any)=>{
          this.profile = UserModel.fromForm(data);
        }
      })
  }

}
