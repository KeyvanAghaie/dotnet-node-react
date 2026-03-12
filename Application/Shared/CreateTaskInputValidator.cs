using Application.Dto.Users;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Shared
{
    public class CreateTaskInputValidator : AbstractValidator<CreateTaskInput>
    {
        public CreateTaskInputValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Title is required")
                .MinimumLength(3).WithMessage("Title must be at least 3 characters")
                .MaximumLength(200).WithMessage("Title must not exceed 200 characters");

            RuleFor(x => x.Status)
                .NotEmpty().WithMessage("Status is required")
                .Must(status => new[] { "Pending", "InProgress", "Completed" }.Contains(status))
                .WithMessage("Status must be either Pending, InProgress or Completed");

            RuleFor(x => x.UserId)
                .GreaterThan(0).WithMessage("Valid UserId is required");
        }
    }
}
