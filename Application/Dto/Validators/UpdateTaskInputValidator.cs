using Application.Dto.Users;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Shared
{
    public class UpdateTaskInputValidator : AbstractValidator<UpdateTaskInput>
    {
        private readonly string[] _validStatuses = { "pending", "in-progress", "completed"};

        public UpdateTaskInputValidator()
        {
            // Title validation (optional since it's an update)
            RuleFor(x => x.Title)
                .MinimumLength(3).WithMessage("Title must be at least 3 characters")
                .MaximumLength(200).WithMessage("Title must not exceed 200 characters")
                .Matches(@"^[a-zA-Z0-9\s\-_]+$").WithMessage("Title can only contain letters, numbers, spaces, hyphens, and underscores")
                .When(x => !string.IsNullOrEmpty(x.Title));

            // Status validation (optional but must be valid if provided)
            RuleFor(x => x.Status)
                .Must(BeAValidStatus).WithMessage($"Status must be one of: {string.Join(", ", _validStatuses)}")
                .When(x => !string.IsNullOrEmpty(x.Status));

            // UserId validation (optional but must be valid if provided)
            RuleFor(x => x.UserId)
                .GreaterThan(0).WithMessage("Valid UserId is required")
                .When(x => x.UserId.HasValue);
        }

        private bool BeAValidStatus(string status)
        {
            if (string.IsNullOrEmpty(status))
                return true; // Skip validation for null/empty (handled by When clause)

            return _validStatuses.Contains(status, StringComparer.OrdinalIgnoreCase);
        }
    }
}
