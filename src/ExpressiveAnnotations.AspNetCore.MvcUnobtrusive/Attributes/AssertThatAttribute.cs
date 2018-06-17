using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace ExpressiveAnnotations.AspNetCore.MvcUnobtrusive.Attributes
{
    public sealed class AssertThatAttribute : ExpressiveAttribute
    {
        public AssertThatAttribute(string expression, string errorMessage) : base(expression, errorMessage) { }

        public override void AddValidation(ClientModelValidationContext context)
        {

        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value != null)
            {
                Compile(validationContext.ObjectType);

                if (!CachedValidationFuncs[validationContext.ObjectType](validationContext.ObjectInstance))
                {
                    return new ValidationResult(
                        FormatErrorMessage(validationContext.DisplayName, Expression, validationContext.ObjectInstance),
                        new[] { validationContext.MemberName }
                    );
                }
            }

            return ValidationResult.Success;
        }
    }
}
