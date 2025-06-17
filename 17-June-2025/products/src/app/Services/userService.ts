import { BehaviorSubject, Observable } from 'rxjs';
import { UserLoginModel } from '../Models/userModel';
import { inject } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';

export class UserService {
  private http = inject(HttpClient);
  private usernameSubject = new BehaviorSubject<string | null>(null);
  username$: Observable<string | null> = this.usernameSubject.asObservable();

  validateUserLogin(user: UserLoginModel) {
    if (user.username.length < 3) {
      this.usernameSubject.next(null);
    } else {
      this.callLoginApi(user).subscribe({
        next: (data: any) => {
          this.usernameSubject.next(user.username);
          localStorage.setItem('token', data.accessToken);
        },
      });
    }
  }

  logout() {
    this.usernameSubject.next(null);
  }
  callGetProfile(){
    var token = localStorage.getItem("token");
    var header = new HttpHeaders({
        'Authorization':`Bearer ${token}`
    });
    return this.http.get('https://dummyjson.com/auth/me',{headers:header});
  }

  callLoginApi(user: UserLoginModel): Observable<any> {
    return this.http.post('https://dummyjson.com/auth/login', user);
  }
}
