import { AbstractControl, ValidationErrors, ValidatorFn } from "@angular/forms";


export function TextValidator() :ValidatorFn{
    return (control : AbstractControl):ValidationErrors|null => {
        const value = control?.value;
        if(value?.length < 4){
            return {minlength:"Password should be atleast 5 characters long"};
        }
        else if(value?.length > 15)
        {
            return {maxlength:"Password cannot be more than 15 characters"};
        }
        return null;

    }
}