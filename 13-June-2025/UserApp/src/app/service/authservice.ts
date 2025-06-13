import { BehaviorSubject, Observable } from "rxjs";
import { UserModel } from "../Models/user";
import { Injectable } from "@angular/core";
@Injectable()
export class UserService
{
    private usernameSubject = new BehaviorSubject<string|null>(null);
    username$:Observable<string|null> = this.usernameSubject.asObservable();

    validateUserLogin(user:UserModel)
    {
        if(user.username.length<3)
        {
            this.usernameSubject.next(null);
            
        }
            
        else{
            console.log(user.username);
            this.usernameSubject.next(user.username);
        }
    }

    logout(){
        this.usernameSubject.next(null);
    }
}