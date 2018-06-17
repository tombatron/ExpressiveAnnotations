using System.Collections.Generic;
using ExpressiveAnnotations.Analysis;
using ExpressiveAnnotations.Functions;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.ComponentModel.DataAnnotations;

namespace ExpressiveAnnotations.AspNetCore.MvcUnobtrusive.Attributes
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = true)]
    public abstract class ExpressiveAttribute : ValidationAttribute, IClientModelValidator
    {
        public string Expression { get; }

        protected Parser Parser { get; }
        protected Dictionary<Type, Func<object, bool>> CachedValidationFuncs { get; }

        protected ExpressiveAttribute(string expression, string errorMessage) : this(expression, () => errorMessage) { }

        protected ExpressiveAttribute(string expression, Func<string> errorMessageAccessor) : base(errorMessageAccessor)
        {
            Expression = expression ?? throw new ArgumentException(nameof(expression), "Expression not provided.");
            CachedValidationFuncs = new Dictionary<Type, Func<object, bool>>();

            Parser = new Parser();
            Parser.RegisterToolchain();
        }

        public void AddValidation(ClientModelValidationContext context)
        {
            throw new NotImplementedException();
        }
    }
}
