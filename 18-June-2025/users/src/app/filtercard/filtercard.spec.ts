import { ComponentFixture, TestBed } from '@angular/core/testing';

import { Filtercard } from './filtercard';

describe('Filtercard', () => {
  let component: Filtercard;
  let fixture: ComponentFixture<Filtercard>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [Filtercard]
    })
    .compileComponents();

    fixture = TestBed.createComponent(Filtercard);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
