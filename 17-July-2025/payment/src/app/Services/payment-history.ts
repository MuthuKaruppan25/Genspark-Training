import { Injectable } from '@angular/core';

export interface PaymentRecord {
  name: string;
  email: string;
  amount: number;
  status: string;
  paymentId?: string;
  date: Date;
}

@Injectable({
  providedIn: 'root'
})
export class PaymentHistoryService {
  private payments: PaymentRecord[] = [];

  constructor() {

    const saved = localStorage.getItem('paymentHistory');
    if (saved) {
      this.payments = JSON.parse(saved);
    }
  }

  addPayment(payment: PaymentRecord) {
    this.payments.unshift(payment);
    localStorage.setItem('paymentHistory', JSON.stringify(this.payments));
  }

  getPayments() {
    return this.payments;
  }
}
