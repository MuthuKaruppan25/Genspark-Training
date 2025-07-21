import { ComponentFixture, TestBed } from '@angular/core/testing';

import { Videoupload } from './videoupload';

describe('Videoupload', () => {
  let component: Videoupload;
  let fixture: ComponentFixture<Videoupload>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [Videoupload]
    })
    .compileComponents();

    fixture = TestBed.createComponent(Videoupload);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
