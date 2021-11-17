using FluentValidation;
using LineNotifySDK.Model;

namespace LineNotifySDK.Validator
{
    public class LineNotifyMessageValidator : AbstractValidator<LineNotifyMessage>
    {
        public LineNotifyMessageValidator()
        {
            RuleFor(x => x.Message)
                .NotEmpty()
                .MaximumLength(1000).WithMessage("1000 characters max");
            RuleFor(x => x.ImageThumbnail)
                .NotNull().When(x => x.ImageFullSize != null);
            RuleFor(x => x.ImageFullSize)
                .NotNull().When(x => x.ImageThumbnail != null);
        }
    }
}
