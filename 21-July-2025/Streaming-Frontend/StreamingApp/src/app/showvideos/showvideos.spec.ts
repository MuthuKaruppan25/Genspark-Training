import { ComponentFixture, TestBed } from '@angular/core/testing';

import { Showvideos } from './showvideos';

describe('Showvideos', () => {
  let component: Showvideos;
  let fixture: ComponentFixture<Showvideos>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [Showvideos]
    })
    .compileComponents();

    fixture = TestBed.createComponent(Showvideos);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
