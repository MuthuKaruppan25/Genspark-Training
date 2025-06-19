import { AbstractControl, ValidationErrors, ValidatorFn } from '@angular/forms';

const bannedUsernames = ['admin', 'user', 'guest'];
export function UsernameBanValidator(): ValidatorFn {
  return (control: AbstractControl): ValidationErrors | null => {
    const value = control.value?.toLowerCase();
    if (bannedUsernames.includes(value)) {
      return { banned: 'This username is not allowed.' };
    }
    return null;
  };
}
