﻿using LazZiya.ExpressLocalization.Messages;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace LazZiya.ExpressLocalization.DataAnnotations
{
    /// <summary>
    /// Specifies the maximum length of array or string data allowed in a property. And produces localized error message.
    /// </summary>
    public sealed class ExMaxLengthAttribute : MaxLengthAttribute
    {
        /// <summary>
        /// Initializes a new instance of the LazZiya.ExpressLocalization.DataAnnotations.ExMaxLengthAttribute class
        /// </summary>
        public ExMaxLengthAttribute() : base()
        {
            this.ErrorMessage = ErrorMessage ?? DataAnnotationsErrorMessages.MaxLengthAttribute_ValidationError;
        }

        /// <summary>
        /// Initializes a new instance of the LazZiya.ExpressLocalization.DataAnnotations.ExMaxLengthAttribute class based on the length parameter.
        /// </summary>
        /// <param name="length">The maximum allowable length of array or string data.</param>
        public ExMaxLengthAttribute(int length) : base(length)
        {
            this.ErrorMessage = ErrorMessage ?? DataAnnotationsErrorMessages.MaxLengthAttribute_ValidationError;
        }
    }
}
