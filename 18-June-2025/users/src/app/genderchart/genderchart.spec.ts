import { ComponentFixture, TestBed } from '@angular/core/testing';

import { Genderchart } from './genderchart';

describe('Genderchart', () => {
  let component: Genderchart;
  let fixture: ComponentFixture<Genderchart>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [Genderchart]
    })
    .compileComponents();

    fixture = TestBed.createComponent(Genderchart);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
