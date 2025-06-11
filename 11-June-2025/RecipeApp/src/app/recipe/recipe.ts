import { Component, Input } from '@angular/core';
import { RecipeModel } from '../Models/recipe';

@Component({
  selector: 'app-recipe',
  imports: [],
  templateUrl: './recipe.html',
  styleUrl: './recipe.css'
})
export class Recipe {
   @Input() recipe:RecipeModel | undefined = undefined;
   showModal = false;
}
