

import { AbstractControl, ValidationErrors, ValidatorFn } from '@angular/forms';

export function PasswordStrengthValidator(): ValidatorFn {
  return (control: AbstractControl): ValidationErrors | null => {
    const value = control.value;
    if (!value) return null;

    const hasNumber = /[0-9]/.test(value);
    const hasSymbol = /[!@#$%^&*(),.?":{}|<>]/.test(value);
    const minLength = value.length >= 6;

    const valid = hasNumber && hasSymbol && minLength;

    if (!valid) {
      return {
        strength: 'Password must be at least 6 characters and include a number and a symbol.',
      };
    }

    return null;
  };
}
