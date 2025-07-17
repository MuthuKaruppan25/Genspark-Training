
import { ComponentFixture, TestBed, fakeAsync, tick } from '@angular/core/testing';
import { PaymentComponent } from './payment-component';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { PaymentHistoryService } from '../Services/payment-history';

describe('PaymentComponent', () => {
  let component: PaymentComponent;
  let fixture: ComponentFixture<PaymentComponent>;
  let paymentHistoryService: PaymentHistoryService;
  let httpMock: HttpTestingController;

  
  class MockRazorpay {
    options: any;
    constructor(options: any) {
      this.options = options;
    }
    on(event: string, callback: Function) {
      if (event === 'payment.failed') {
        this.failCallback = callback;
      }
    }
    failCallback: any;
    open() {

      this.options.handler({
        razorpay_payment_id: 'pay_test_success'
      });
    }
  }

  beforeEach(async () => {

    (window as any).Razorpay = MockRazorpay;

    await TestBed.configureTestingModule({
      imports: [
        PaymentComponent,
        ReactiveFormsModule,
        FormsModule,
        CommonModule,
        HttpClientTestingModule
      ],
      providers: [PaymentHistoryService]
    }).compileComponents();

    fixture = TestBed.createComponent(PaymentComponent);
    component = fixture.componentInstance;
    paymentHistoryService = TestBed.inject(PaymentHistoryService);
    httpMock = TestBed.inject(HttpTestingController);
    fixture.detectChanges();
  });

  afterEach(() => {
    httpMock.verify();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should initialize form with required fields', () => {
    expect(component.paymentForm).toBeTruthy();
    expect(component.paymentForm.controls.amount).toBeTruthy();
    expect(component.paymentForm.controls.name).toBeTruthy();
    expect(component.paymentForm.controls.email).toBeTruthy();
    expect(component.paymentForm.controls.contact).toBeTruthy();
  });

  it('should mark form invalid when empty', () => {
    expect(component.paymentForm.valid).toBeFalse();
  });

  it('should validate the form with correct data', () => {
    component.paymentForm.setValue({
      amount: 500,
      name: 'John Doe',
      email: 'john@example.com',
      contact: '9876543210'
    });
    expect(component.paymentForm.valid).toBeTrue();
  });

  it('should handle pay() with valid form and store payment history', fakeAsync(() => {
    spyOn(paymentHistoryService, 'addPayment').and.callThrough();

    component.paymentForm.setValue({
      amount: 500,
      name: 'John Doe',
      email: 'john@example.com',
      contact: '9876543210'
    });

    component.pay();


    const req = httpMock.expectOne('http://localhost:5179/api/payment/create-order');
    expect(req.request.method).toBe('POST');
    req.flush({
      orderId: 'order_test_id',
      amount: 500,
      currency: 'INR'
    });

    tick();

    expect(component.paymentStatus).toContain('Payment Successful');
    expect(paymentHistoryService.addPayment).toHaveBeenCalledWith(
      jasmine.objectContaining({
        name: 'John Doe',
        amount: 500,
        status: 'Success'
      })
    );
  }));

  it('should not call pay() if form is invalid', () => {
    component.paymentForm.setValue({
      amount: null,
      name: '',
      email: '',
      contact: ''
    });

    component.pay();

    expect(component.paymentForm.touched).toBeTrue();
    expect(component.paymentStatus).toBeNull();
  });

  it('should handle payment failure and store failed history', fakeAsync(() => {
    
    class FailureRazorpay {
      options: any;
      constructor(options: any) {
        this.options = options;
      }
      on(event: string, callback: Function) {
        if (event === 'payment.failed') {
          callback({
            error: { description: 'Test failure reason' }
          });
        }
      }
      open() {}
    }
    (window as any).Razorpay = FailureRazorpay;

    spyOn(paymentHistoryService, 'addPayment').and.callThrough();

    component.paymentForm.setValue({
      amount: 500,
      name: 'John Doe',
      email: 'john@example.com',
      contact: '9876543210'
    });

    component.pay();

    const req = httpMock.expectOne('http://localhost:5179/api/payment/create-order');
    expect(req.request.method).toBe('POST');
    req.flush({
      orderId: 'order_test_id',
      amount: 500,
      currency: 'INR'
    });

    tick();

    expect(component.paymentStatus).toContain('Payment Failed');
    expect(paymentHistoryService.addPayment).toHaveBeenCalledWith(
      jasmine.objectContaining({
        name: 'John Doe',
        amount: 500,
        status: 'Failed'
      })
    );
  }));

  it('should toggle and load payment history', () => {
    const mockHistory = [
      {
        name: 'John Doe',
        email: 'john@example.com',
        amount: 500,
        status: 'Success',
        date: new Date()
      }
    ];
    spyOn(paymentHistoryService, 'getPayments').and.returnValue(mockHistory);

    component.toggleHistory();

    expect(component.showHistory).toBeTrue();
    expect(component.paymentHistory.length).toBe(1);
    expect(component.paymentHistory[0].name).toBe('John Doe');
  });

});

