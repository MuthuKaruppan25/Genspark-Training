import { ComponentFixture, TestBed } from '@angular/core/testing';

import { Statechart } from './statechart';

describe('Statechart', () => {
  let component: Statechart;
  let fixture: ComponentFixture<Statechart>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [Statechart]
    })
    .compileComponents();

    fixture = TestBed.createComponent(Statechart);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
