import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';

interface Customer {
  name: string;
  address: string;
  email: string;
  age: number;
  phoneno: string;
  likeCount: number;
  dislikeCount: number;
  liked: boolean;
  disliked: boolean;
}

@Component({
  selector: 'app-first',
  standalone: true,
  imports: [FormsModule, CommonModule],
  templateUrl: './first.html',
  styleUrls: ['./first.css']
})
export class First {
  name = '';
  address = '';
  email = '';
  age: number | null = null;
  phoneno = '';

  customers: Customer[] = [];

  addCustomer() {
    if (this.name && this.email) {
      this.customers.push({
        name: this.name,
        address: this.address,
        email: this.email,
        age: this.age || 0,
        phoneno: this.phoneno,
        likeCount: 0,
        dislikeCount: 0,
        liked: false,
        disliked: false
      });

     
      this.name = '';
      this.address = '';
      this.email = '';
      this.age = null;
      this.phoneno = '';
    }
  }

    like(customer: Customer) {
      customer.likeCount++;
      customer.liked = true; 
    }

    dislike(customer: Customer) {
      customer.dislikeCount++;
      customer.disliked = true; 
    }

}
