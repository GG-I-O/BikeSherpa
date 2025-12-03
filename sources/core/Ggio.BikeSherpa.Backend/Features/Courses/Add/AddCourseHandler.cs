using Ardalis.Result;
using FluentValidation;
using Ggio.BikeSherpa.Backend.Domain.CourseAggregate;
using Mediator;

namespace Ggio.BikeSherpa.Backend.Features.Courses.Add;

public record AddCourseCommand(DateTimeOffset StartDate) : ICommand<Result<Guid>>;

public class AddCourseCommandValidator : AbstractValidator<AddCourseCommand>
{
     public AddCourseCommandValidator()
     {
          RuleFor(x => x.StartDate).NotEmpty().Must(x => x > DateTimeOffset.Now);
     }
}

public class AddCourseHandler(ICourseFactory factory, IValidator<AddCourseCommand> validator) : ICommandHandler<AddCourseCommand, Result<Guid>>
{
     public async ValueTask<Result<Guid>> Handle(AddCourseCommand command, CancellationToken cancellationToken)
     {
          await validator.ValidateAndThrowAsync(command, cancellationToken);
          var course = await factory.CreateCourseAsync(command.StartDate);
          return Result<Guid>.Success(course.Id);
     }
}
