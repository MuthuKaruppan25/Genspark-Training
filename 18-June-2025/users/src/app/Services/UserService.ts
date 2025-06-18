import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { UserModel } from '../Models/userModel';
import { map, Observable } from 'rxjs';

@Injectable()
export class UserService {
  private http = inject(HttpClient);
  private apiUrl = 'https://dummyjson.com/users/add';
  private getapiUrl = 'https://dummyjson.com/users';

  addUser(user: UserModel): Observable<any> {
    return this.http.post(this.apiUrl, user);
  }

  getUsers(): Observable<UserModel[]> {
    return this.http.get<any>(this.getapiUrl).pipe(
      map((res) => {
        return res.users.map((u: any) => {
          return new UserModel(
            u.firstName,
            u.lastName,
            u.age,
            u.gender,
            u.role, 
            {
              city: u.address?.city || '',
              state: u.address?.state || '',
              country: u.address?.country || ''
            }
          );
        });
      })
    );
  }
}
