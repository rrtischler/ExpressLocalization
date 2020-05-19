﻿using LazZiya.ExpressLocalization.Messages;
using Microsoft.AspNetCore.Identity;
using System;

namespace LazZiya.ExpressLocalization
{
    /// <summary>
    /// Identity describer errors localizer
    /// </summary>
    public class IdentityErrorsLocalizer : IdentityErrorDescriber 
    {
        private readonly Type ResxResourceType;
        private readonly ISharedCultureLocalizer Localizer;

        /// <summary>
        /// Initialize identity errors localization based on resx files
        /// </summary>
        /// <param name="resType"></param>
        public IdentityErrorsLocalizer(Type resType)
        {
            ResxResourceType = resType;
        }
        
        /// <summary>
        /// Initialize identity erroors localization based on DB locailzer
        /// </summary>
        /// <param name="localizer"></param>
        public IdentityErrorsLocalizer(ISharedCultureLocalizer localizer)
        {
            Localizer = localizer;
        }

        private IdentityError LocalizedIdentityError(string code, params object[] args)
        {
            var msg = Localizer == null
                ? GenericResourceReader.GetString(ResxResourceType, string.Empty, code, args)
                : Localizer[code, args];

            return new IdentityError { Code = code, Description = msg };
        }

        public override IdentityError DuplicateEmail(string email) 
            => LocalizedIdentityError(IdentityErrorMessages.DuplicateEmail, email);

        public override IdentityError DuplicateUserName(string userName) 
            => LocalizedIdentityError(IdentityErrorMessages.DuplicateUserName, userName);

        public override IdentityError InvalidEmail(string email) 
            => LocalizedIdentityError(IdentityErrorMessages.InvalidEmail, email);

        public override IdentityError DuplicateRoleName(string role) 
            => LocalizedIdentityError(IdentityErrorMessages.DuplicateRoleName, role);

        public override IdentityError InvalidRoleName(string role) 
            => LocalizedIdentityError(IdentityErrorMessages.InvalidRoleName, role);

        public override IdentityError InvalidToken() 
            => LocalizedIdentityError(IdentityErrorMessages.InvalidToken);

        public override IdentityError InvalidUserName(string userName) 
            => LocalizedIdentityError(IdentityErrorMessages.InvalidUserName, userName);

        public override IdentityError LoginAlreadyAssociated() 
            => LocalizedIdentityError(IdentityErrorMessages.LoginAlreadyAssociated);

        public override IdentityError PasswordMismatch() 
            => LocalizedIdentityError(IdentityErrorMessages.PasswordMismatch);

        public override IdentityError PasswordRequiresDigit() 
            => LocalizedIdentityError(IdentityErrorMessages.PasswordRequiresDigit);

        public override IdentityError PasswordRequiresLower() 
            => LocalizedIdentityError(IdentityErrorMessages.PasswordRequiresLower);

        public override IdentityError PasswordRequiresNonAlphanumeric() 
            => LocalizedIdentityError(IdentityErrorMessages.PasswordRequiresNonAlphanumeric);

        public override IdentityError PasswordRequiresUniqueChars(int uniqueChars) 
            => LocalizedIdentityError(IdentityErrorMessages.PasswordRequiresUniqueChars, uniqueChars);

        public override IdentityError PasswordRequiresUpper() 
            => LocalizedIdentityError(IdentityErrorMessages.PasswordRequiresUpper);

        public override IdentityError PasswordTooShort(int length) 
            => LocalizedIdentityError(IdentityErrorMessages.PasswordTooShort, length);

        public override IdentityError UserAlreadyHasPassword() 
            => LocalizedIdentityError(IdentityErrorMessages.UserAlreadyHasPassword);

        public override IdentityError UserAlreadyInRole(string role) 
            => LocalizedIdentityError(IdentityErrorMessages.UserAlreadyInRole, role);

        public override IdentityError UserNotInRole(string role) 
            => LocalizedIdentityError(IdentityErrorMessages.UserNotInRole, role);

        public override IdentityError UserLockoutNotEnabled() 
            => LocalizedIdentityError(IdentityErrorMessages.UserLockoutNotEnabled);

        public override IdentityError RecoveryCodeRedemptionFailed() 
            => LocalizedIdentityError(IdentityErrorMessages.RecoveryCodeRedemptionFailed);

        public override IdentityError ConcurrencyFailure() 
            => LocalizedIdentityError(IdentityErrorMessages.ConcurrencyFailure);

        public override IdentityError DefaultError() 
            => LocalizedIdentityError(IdentityErrorMessages.DefaultError);
    }
}
