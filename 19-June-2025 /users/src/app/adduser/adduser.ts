import { Component } from '@angular/core';
import {
  FormControl,
  FormGroup,
  FormsModule,
  ReactiveFormsModule,
  Validators,
  ValidatorFn,
  AbstractControl,
} from '@angular/forms';
import { CommonModule } from '@angular/common';


import { Store } from '@ngrx/store';
import { addUSer } from '../rxjs/user.actions';
import { UserList } from "../user-list/user-list";
import { User } from '../Models/user';
import { UsernameBanValidator } from '../Misc/usernameValidator';
import { PasswordStrengthValidator } from '../Misc/passwordValidator';
import { Snackbar } from "../snackbar/snackbar";

@Component({
  selector: 'app-adduser',
  standalone: true,
  imports: [FormsModule, CommonModule, ReactiveFormsModule, Snackbar],
  templateUrl: './adduser.html',
  styleUrl: './adduser.css',
})
export class Adduser {
  userForm: FormGroup;
  showToast: boolean = false;
  toastMessage : string = "";
  color : string = "";

  constructor( private store: Store) {
    this.userForm = new FormGroup(
      {
        username: new FormControl('', [Validators.required, Validators.minLength(3), UsernameBanValidator()]),
        email: new FormControl('', [Validators.required, Validators.email]),
        password: new FormControl('', [Validators.required, Validators.minLength(6), PasswordStrengthValidator()]),
        confirmPassword: new FormControl('', Validators.required),
        role: new FormControl('', [Validators.required, Validators.minLength(2)]),
      },
      { validators: this.passwordMatchValidator }
    );
  }

  get username() {
    return this.userForm.get('username')!;
  }
  get email() {
    return this.userForm.get('email')!;
  }
  get password() {
    return this.userForm.get('password')!;
  }
  get confirmPassword() {
    return this.userForm.get('confirmPassword')!;
  }
  get role() {
    return this.userForm.get('role')!;
  }

  passwordMatchValidator: ValidatorFn = (formGroup: AbstractControl): { [key: string]: any } | null => {
    const pass = formGroup.get('password')?.value;
    const confirm = formGroup.get('confirmPassword')?.value;
    return pass === confirm ? null : { mismatch: true };
  };

  handleSubmit(): void {
    if (this.userForm.invalid) {
      this.toastMessage = "User cannot be added, please try again";
      this.color = "#ff0000";
      this.showToast = true;


      setTimeout(()=>{this.showToast = false},2000);
      return;
    }

    const newUser = new User(
      this.userForm.value.username,
      this.userForm.value.email,
      this.userForm.value.password,
      this.userForm.value.confirmPassword,
      this.userForm.value.role
    );

    this.store.dispatch(addUSer({ user: newUser }));
          this.toastMessage = "User added successfully";
        this.color = "#008000";
      this.showToast = true;

      setTimeout(()=>{this.showToast = false},2000);
    this.userForm.reset();
  }
}
