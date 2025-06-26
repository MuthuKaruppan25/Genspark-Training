import { HttpClient } from "@angular/common/http";
import { inject, Injectable } from "@angular/core";
import { Observable } from 'rxjs/internal/Observable';
import { RecipeModel } from "../Models/recipe";

@Injectable()
export class RecipeService{
    private http = inject(HttpClient);

getRecipes(): Observable<{ recipes: RecipeModel[] }> {
  return this.http.get<{ recipes: RecipeModel[] }>("https://dummyjson.com/recipes");
}

}