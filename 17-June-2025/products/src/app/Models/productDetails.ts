

export class ProductDetails
{
    constructor(public id:number,public title:string,public description:string,public category:string,public price:number,public rating:number,public brand:string,public thumbnail:string)
    {

    }
    static FromForm(data:{id:number, title:string, description:string, category:string, price:number,rating:number, brand:string, thumbnail:string})
    {
        return new ProductDetails(data.id,
            data.title,
            data.description,
            data.category,
            data.price,
            data.rating,
            data.brand,
            data.thumbnail
        );
    }
}