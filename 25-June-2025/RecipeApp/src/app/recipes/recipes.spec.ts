import {
  ComponentFixture,
  TestBed,
  fakeAsync,
  tick,
} from '@angular/core/testing';
import { Component, CUSTOM_ELEMENTS_SCHEMA } from '@angular/core';
import { of } from 'rxjs';
import { Recipes } from './recipes';
import { RecipeService } from '../services/recipeService';
import { Recipe } from '../recipe/recipe';

@Component({
  selector: 'app-recipe',
  template: '',
})
class MockRecipeComponent {}

describe('Recipes Component', () => {
  let component: Recipes;
  let fixture: ComponentFixture<Recipes>;
  let recipeServiceSpy: jasmine.SpyObj<RecipeService>;

  const dummyRecipeData = {
    recipes: [
      {
        id: 1,
        name: 'Idli',
        image: 'idli.jpg',
        cuisine: 'South Indian',
        prepTimeMinutes: 10,
        cookTimeMinutes: 15,
        servings: 4,
        difficulty: 'Easy',
        ingredients: ['Rice', 'Urad dal'],
        instructions: ['Soak rice', 'Grind and ferment', 'Steam'],
      },
      {
        id: 2,
        name: 'Chole',
        image: 'chole.jpg',
        cuisine: 'North Indian',
        prepTimeMinutes: 20,
        cookTimeMinutes: 30,
        servings: 4,
        difficulty: 'Medium',
        ingredients: ['Chickpeas', 'Tomato', 'Spices'],
        instructions: ['Soak chickpeas', 'Cook with spices'],
      },
    ],
  };

  beforeEach(async () => {
    const spy = jasmine.createSpyObj('RecipeService', ['getRecipes']);
    recipeServiceSpy = spy;

    await TestBed.configureTestingModule({
      imports: [Recipes],
      providers: [{ provide: RecipeService, useValue: recipeServiceSpy }],
      schemas: [CUSTOM_ELEMENTS_SCHEMA],
    }).compileComponents();

    fixture = TestBed.createComponent(Recipes);
    component = fixture.componentInstance;
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should fetch and render recipes on init', fakeAsync(() => {
    recipeServiceSpy.getRecipes.and.returnValue(of(dummyRecipeData));

    component.ngOnInit();
    tick();
    fixture.detectChanges();

    expect(recipeServiceSpy.getRecipes).toHaveBeenCalled();
    expect(component.recipes().length).toBe(2);
    expect(component.recipes()[1].name).toBe('Chole');
  }));
  it('should show no records found', fakeAsync(() => {
    recipeServiceSpy.getRecipes.and.returnValue(of({ recipes: [] }));

    component.ngOnInit();
    tick();
    fixture.detectChanges();

    const compiled = fixture.nativeElement as HTMLElement;
    expect(compiled.textContent).toContain('No Recipes Found');
  }));
});
