import { Component} from '@angular/core';
import { UserModel } from '../Models/user';
import { NgModel } from '@angular/forms';
import { AuthenticationService } from '../service/userservice';
import { User } from "../user/user";
import { Display } from "../display/display";
import { UserService } from '../service/authservice';

@Component({
  selector: 'app-authentication',
  imports: [ Display],
  templateUrl: './authentication.html',
  styleUrl: './authentication.css'
})
export class Authentication {

   errText: string="";
   auth : boolean = false;

   constructor(private userService : UserService){

   }

   login(username:string,password:string)
   {
      const user = new UserModel(username,password);
      // if(this.authService.validate(user))
      // {
      //    this.authService.saveToLocalStorage(user,true);
      //    this.auth = true;
      //    alert("Login Successful");
      // }
      // else{
      //    this.errText = "Invalid username or password";
      // }
      this.userService.validateUserLogin(user);
   }

}
