import { Component, Input } from '@angular/core';
import { RecipeModel } from '../Models/recipe';

@Component({
  selector: 'app-recipe',
  imports: [],
  templateUrl: './recipe.html',
  styleUrl: './recipe.css',
})
export class Recipe {
  @Input() recipe: RecipeModel = {
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
  showModal = false;
}
