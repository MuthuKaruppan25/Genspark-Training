import { HttpClient } from "@angular/common/http";
import { inject, Injectable } from "@angular/core";
import { Observable } from "rxjs";

@Injectable()
export class ProductService
{
    private http = inject(HttpClient);

    getProductsSearchResult(searchData:string,limit:number = 10,skip:number =10) : Observable<any []>
    {
        return this.http.get<any []>(`https://dummyjson.com/products/search?q=${searchData}&limit=${limit}&skip=${skip}`);
    }
}