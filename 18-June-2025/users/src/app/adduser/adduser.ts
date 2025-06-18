import { Component } from '@angular/core';
import { UserModel } from '../Models/userModel';
import { AddressModel } from '../Models/AddressModel';
import { UserService } from '../Services/UserService';
import {
  FormControl,
  FormGroup,
  FormsModule,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { CommonModule } from '@angular/common';
import { TextValidator } from '../Misc/TextValidator';

@Component({
  selector: 'app-adduser',
  imports: [FormsModule, CommonModule, ReactiveFormsModule],
  templateUrl: './adduser.html',
  styleUrl: './adduser.css',
})
export class Adduser {
  userForm: FormGroup;

  constructor(private userService: UserService) {
    this.userForm = new FormGroup({
      firstName: new FormControl('', [Validators.required,TextValidator()]),
      lastName: new FormControl('', [Validators.required,TextValidator()]),
      age: new FormControl(0, [Validators.required, Validators.min(1)]),
      gender: new FormControl('', Validators.required),
      role: new FormControl('', [Validators.required,TextValidator()]),
      city: new FormControl('', [Validators.required,TextValidator()]),
      state: new FormControl('', [Validators.required,TextValidator()]),
      country: new FormControl('', [Validators.required,TextValidator()]),
    });
  }

  get firstName() {
    return this.userForm.get('firstName')!;
  }
  get lastName() {
    return this.userForm.get('lastName')!;
  }
  get age() {
    return this.userForm.get('age')!;
  }
  get gender() {
    return this.userForm.get('gender')!;
  }
  get role() {
    return this.userForm.get('role')!;
  }
  get city() {
    return this.userForm.get('city')!;
  }
  get state() {
    return this.userForm.get('state')!;
  }
  get country() {
    return this.userForm.get('country')!;
  }

  handleSubmit(): void {
    if (this.userForm.invalid) {
      alert('Please fill all required fields correctly.');
      return;
    }

    const address = new AddressModel(
      this.userForm.value.city,
      this.userForm.value.state,
      this.userForm.value.country
    );

    const user = new UserModel(
      this.userForm.value.firstName,
      this.userForm.value.lastName,
      this.userForm.value.age,
      this.userForm.value.gender,
      this.userForm.value.role,
      address
    );

    this.userService.addUser(user).subscribe({
      next: (response) => {
        console.log('Success response:', response);
        alert(' User added successfully!');
        this.userForm.reset();
      },
      error: (error) => {
        console.error('Error:', error);
        alert('User could not be added. Please try again.');
      },
    });
  }
}
