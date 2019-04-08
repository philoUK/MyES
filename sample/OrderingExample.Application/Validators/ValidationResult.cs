namespace OrderingExample.Application.Validators
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Serialization;

    public class ValidationResult
    {
        private readonly List<string> brokenRules;

        private ValidationResult(bool hasPassed, List<string> brokenRules)
        {
            this.brokenRules = brokenRules;
            this.HasPassed = hasPassed;
        }

        public bool HasPassed { get; }

        public IEnumerable<string> Errors => this.brokenRules;

        public static ValidationResult Pass()
        {
            return new ValidationResult(true, new List<string>());
        }

        public static ValidationResult Fails(IEnumerable<string> errors)
        {
            return new ValidationResult(false, new List<string>(errors));
        }

        public ValidationResult Combine(ValidationResult rhs)
        {
            if (this.HasPassed && rhs.HasPassed)
            {
                return ValidationResult.Pass();
            }

            var allRules = new List<string>(this.brokenRules);
            allRules.AddRange(rhs.brokenRules);
            return ValidationResult.Fails(allRules);
        }

        internal string ErrorList()
        {
            return this.Errors.Aggregate("", (prev, next) => prev + " " + next);
        }
    }
}
