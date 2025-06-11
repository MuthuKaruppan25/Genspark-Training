import { Component, signal, OnInit } from '@angular/core';
import { RecipeModel } from '../Models/recipe';
import { RecipeService } from '../services/recipeService';
import { Recipe } from "../recipe/recipe";

@Component({
  selector: 'app-recipes',
  imports: [Recipe],
  templateUrl: './recipes.html',
  styleUrl: './recipes.css',
})
export class Recipes {
  recipes = signal<RecipeModel[]>([]);

  constructor(private recipeService: RecipeService) {}
  ngOnInit(): void {
    this.recipeService.getRecipes().subscribe({
      next :(data:any)=>{
     
         this.recipes.set(data?.recipes as RecipeModel[]);
         console.log(this.recipes());
      }
    })
  }
}
