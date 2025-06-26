import { ComponentFixture, TestBed } from '@angular/core/testing';
import { Component } from '@angular/core';
import { Recipe } from './recipe';
import { RecipeModel } from '../Models/recipe';
import { RecipeService } from '../services/recipeService';
import { ActivatedRoute } from '@angular/router';
import { provideHttpClient } from '@angular/common/http';
import { provideHttpClientTesting } from '@angular/common/http/testing';



@Component({
  standalone: true,
  imports: [Recipe],
  template: `<app-recipe [recipe]="recipe"></app-recipe>`,
})
class HostComponent {
  recipe: RecipeModel = {
    id: 0,
    name: '',
    image: '',
    cuisine: '',
    prepTimeMinutes: 0,
    cookTimeMinutes: 0,
    servings: 0,
    difficulty: '',
    ingredients: [],
    instructions: [],
  };
}

describe('Recipe Component', () => {
  let fixture: ComponentFixture<HostComponent>;
  let hostComponent: HostComponent;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [HostComponent],
      providers: [
      
      ],
    }).compileComponents();

    fixture = TestBed.createComponent(HostComponent);
    hostComponent = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(hostComponent).toBeTruthy();
  });

  it('should render the recipe title and other data', () => {

    Object.assign(hostComponent.recipe, {
      id: 1,
      name: 'Paneer Butter Masala',
      image: 'https://example.com/paneer.jpg',
      cuisine: 'Indian',
      prepTimeMinutes: 20,
      cookTimeMinutes: 25,
      servings: 4,
      difficulty: 'Medium',
      ingredients: ['Paneer', 'Butter', 'Tomato', 'Cream'],
      instructions: ['Cut paneer', 'Prepare gravy', 'Mix and cook'],
    });


    fixture.detectChanges();


    const compiled = fixture.nativeElement as HTMLElement;
    expect(compiled.textContent).toContain('Paneer Butter Masala');
    expect(compiled.textContent).toContain('Indian');
    expect(compiled.textContent).toContain('Medium');
    expect(compiled.textContent).toContain('Paneer');
    expect(compiled.textContent).toContain('Cut paneer');
  });
});
