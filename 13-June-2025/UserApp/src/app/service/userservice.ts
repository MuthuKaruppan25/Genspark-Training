import { Injectable } from '@angular/core';
import { UserModel } from '../Models/user';

@Injectable()
export class AuthenticationService {
  users: UserModel[] = [
    new UserModel('muthu@gmail.com', '123456'),
    new UserModel('gowtham@gmail.com', '123456'),
    new UserModel('mano@gmail.com', '123456'),
  ];

  validate(user: UserModel): boolean {
    return this.users.some(
      (u) => u.username === user.username && u.password === user.password
    );
  }

  saveToLocalStorage(user: UserModel, session: boolean = false) {
    const req = JSON.stringify(user);
    if (!session) localStorage.setItem('user', req);
    else sessionStorage.setItem('user', req);
  }
  getFromStorage(session: boolean = false): UserModel | null {
    const data = session
      ? sessionStorage.getItem('user')
      : localStorage.getItem('user');
    return data ? JSON.parse(data) : null;
  }
}
