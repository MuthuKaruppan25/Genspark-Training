import { HttpClient } from "@angular/common/http";
import { inject, Injectable } from "@angular/core";
import { Observable } from 'rxjs/internal/Observable';

@Injectable()
export class RecipeService{
    private http = inject(HttpClient);

    getRecipes(): Observable<any[]>{
       return this.http.get<any []>("https://dummyjson.com/recipes");
    }
}