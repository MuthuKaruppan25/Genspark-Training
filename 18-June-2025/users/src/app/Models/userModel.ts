import { AddressModel } from "./AddressModel";

export class UserModel
{
    constructor(public firstName:string,public lastName:string,public age:number,public gender:string,public role:string,public address: AddressModel)
    {

    }
}