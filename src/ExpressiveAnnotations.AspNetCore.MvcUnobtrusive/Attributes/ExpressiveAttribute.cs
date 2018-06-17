using ExpressiveAnnotations.Analysis;
using ExpressiveAnnotations.Functions;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

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

        public virtual string FormatErrorMessage(string displayName, string expression, object objectInstance)
        {
            try
            {
                IList<FormatItem> items;

                var message = PreformatMessage(displayName, expression, out items);

                message = items.Aggregate(message, (cargo, current) => current.Indicator != null && !current.Constant ? cargo.Replace(current.Uuid.ToString(), Helper.ExtractDisplayName(objectInstance.GetType(), current.FieldPath)) : cargo);
                message = items.Aggregate(message, (cargo, current) => current.Indicator == null && !current.Constant ? cargo.Replace(current.Uuid.ToString(), (Helper.ExtractValue(objectInstance, current.FieldPath) ?? string.Empty).ToString()) : cargo);

                return message;
            }
            catch (Exception ex)
            {
                throw new FormatException($"Problem with error message processing. The message is following: {ErrorMessageString}", ex);
            }
        }

        public void Compile(Type validationContextType, Action<Parser> action = null)
        {
            if (!CachedValidationFuncs.ContainsKey(validationContextType))
            {
                CachedValidationFuncs[validationContextType] = Parser.Parse<bool>(validationContextType, Expression);
            }

            action?.Invoke(Parser);
        }

        public abstract void AddValidation(ClientModelValidationContext context);

        private string PreformatMessage(string displayName, string expression, out IList<FormatItem> items)
        {
            var message = MessageFormatter.FormatString(ErrorMessageString, out items);

            message = string.Format(message, displayName, expression);
            message = items.Aggregate(message, (cargo, current) => cargo.Replace(current.Uuid.ToString(), current.Substitute));

            return message;
        }
    }
}
