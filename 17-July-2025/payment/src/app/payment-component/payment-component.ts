import { CommonModule } from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { ChangeDetectorRef, Component, inject } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import {
  PaymentHistoryService,
  PaymentRecord,
} from '../Services/payment-history';

@Component({
  selector: 'app-payment',
  templateUrl: './payment-component.html',
  styleUrls: ['./payment-component.css'],
  standalone: true,
  imports: [ReactiveFormsModule, CommonModule],
})
export class PaymentComponent {
  paymentForm: any;
  paymentStatus: string | null = null;
  orderId: string = '';
  showHistory = false;
  paymentHistory: PaymentRecord[] = [];

  private http = inject(HttpClient);

  constructor(
    private fb: FormBuilder,
    private cd: ChangeDetectorRef,
    private historyService: PaymentHistoryService
  ) {
    this.paymentForm = this.fb.group({
      amount: [null, [Validators.required, Validators.min(1)]],
      name: ['', Validators.required],
      email: ['', [Validators.required, Validators.email]],
      contact: ['', [Validators.required, Validators.pattern(/^[0-9]{10}$/)]],
    });
  }

  pay() {
    if (this.paymentForm.invalid) {
      this.paymentForm.markAllAsTouched();
      return;
    }

    const amtrequest = {
      Amount: this.paymentForm.value.amount,
    };

    this.http
      .post('http://localhost:5179/api/payment/create-order', amtrequest)
      .subscribe({
        next: (response: any) => {
          this.orderId = response.orderId;

          const options: any = {
            key: 'rzp_test_1u83UZgAJaGm9w',
            amount: this.paymentForm.value.amount * 100,
            currency: 'INR',
            name: 'My Test Company',
            description: 'Test Transaction',
            order_id: this.orderId,
            handler: (res: any) => {
              console.log('Success', res);
              this.paymentStatus = `Payment Successful!`;
              this.historyService.addPayment({
                name: this.paymentForm.value.name,
                email: this.paymentForm.value.email,
                amount: this.paymentForm.value.amount,
                status: 'Success',
                paymentId: res.razorpay_payment_id,
                date: new Date(),
              });
              this.cd.detectChanges();
              setTimeout(() => {
                this.paymentForm.reset();
                this.paymentStatus = null;
                this.cd.detectChanges();
              }, 2000);
            },
            prefill: {
              name: this.paymentForm.value.name,
              email: this.paymentForm.value.email,
              contact: this.paymentForm.value.contact,
            },
            theme: {
              color: '#1976d2',
            },
          };

          const rzp = new (window as any).Razorpay(options);
          rzp.on('payment.failed', (fail: any) => {
            console.log('Fail', fail);
            this.paymentStatus = `Payment Failed. Reason: ${fail.error.description}`;
            this.historyService.addPayment({
              name: this.paymentForm.value.name,
              email: this.paymentForm.value.email,
              amount: this.paymentForm.value.amount,
              status: 'Failed',
              date: new Date(),
            });
            this.cd.detectChanges();
            setTimeout(() => {
              this.paymentForm.reset();
              this.paymentStatus = null;
              this.cd.detectChanges();
            }, 2000);
          });
          rzp.open();
        },
        error: (err) => {
          console.error(err);
        },
      });
  }

  toggleHistory() {
    this.showHistory = !this.showHistory;
    if (this.showHistory) {
      this.paymentHistory = this.historyService.getPayments();
    }
  }
}
