import {
  HttpTestingController,
  provideHttpClientTesting,
} from '@angular/common/http/testing';
import { RecipeService } from './recipeService';
import { provideHttpClient } from '@angular/common/http';
import { TestBed } from '@angular/core/testing';

describe('recipeService', () => {
  let service: RecipeService;
  let httpMock: HttpTestingController;

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [],
      providers: [
        RecipeService,
        provideHttpClient(),
        provideHttpClientTesting(),
      ],
    });
    service = TestBed.inject(RecipeService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpMock.verify();
  });

  it('should retrive recipes', () => {
    const mockRecipes = {
      recipes: [
        {
          id: 1,
          name: 'Classic Margherita Pizza',
          ingredients: [
            'Pizza dough',
            'Tomato sauce',
            'Fresh mozzarella cheese',
            'Fresh basil leaves',
            'Olive oil',
            'Salt and pepper to taste',
          ],
          instructions: [
            'Preheat the oven to 475°F (245°C).',
            'Roll out the pizza dough and spread tomato sauce evenly.',
            'Top with slices of fresh mozzarella and fresh basil leaves.',
            'Drizzle with olive oil and season with salt and pepper.',
            'Bake in the preheated oven for 12-15 minutes or until the crust is golden brown.',
            'Slice and serve hot.',
          ],
          prepTimeMinutes: 20,
          cookTimeMinutes: 15,
          servings: 4,
          difficulty: 'Easy',
          cuisine: 'Italian',
          caloriesPerServing: 300,
          tags: ['Pizza', 'Italian'],
          userId: 166,
          image: 'https://cdn.dummyjson.com/recipe-images/1.webp',
          rating: 4.6,
          reviewCount: 98,
          mealType: ['Dinner'],
        },
        {
          id: 2,
          name: 'Vegetarian Stir-Fry',
          ingredients: [
            'Tofu, cubed',
            'Broccoli florets',
            'Carrots, sliced',
            'Bell peppers, sliced',
            'Soy sauce',
            'Ginger, minced',
            'Garlic, minced',
            'Sesame oil',
            'Cooked rice for serving',
          ],
          instructions: [
            'In a wok, heat sesame oil over medium-high heat.',
            'Add minced ginger and garlic, sauté until fragrant.',
            'Add cubed tofu and stir-fry until golden brown.',
            'Add broccoli, carrots, and bell peppers. Cook until vegetables are tender-crisp.',
            'Pour soy sauce over the stir-fry and toss to combine.',
            'Serve over cooked rice.',
          ],
          prepTimeMinutes: 15,
          cookTimeMinutes: 20,
          servings: 3,
          difficulty: 'Medium',
          cuisine: 'Asian',
          caloriesPerServing: 250,
          tags: ['Vegetarian', 'Stir-fry', 'Asian'],
          userId: 143,
          image: 'https://cdn.dummyjson.com/recipe-images/2.webp',
          rating: 4.7,
          reviewCount: 26,
          mealType: ['Lunch'],
        },
      ],
      total: 50,
      skip: 0,
      limit: 30,
    };
    service.getRecipes().subscribe((items: any) => {
      expect(items.total).toBe(50);
    });

    const req = httpMock.expectOne('https://dummyjson.com/recipes');
    expect(req.request.method).toBe('GET');

    req.flush(mockRecipes);
  });
});
