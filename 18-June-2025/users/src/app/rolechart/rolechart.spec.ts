import { ComponentFixture, TestBed } from '@angular/core/testing';

import { Rolechart } from './rolechart';

describe('Rolechart', () => {
  let component: Rolechart;
  let fixture: ComponentFixture<Rolechart>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [Rolechart]
    })
    .compileComponents();

    fixture = TestBed.createComponent(Rolechart);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
