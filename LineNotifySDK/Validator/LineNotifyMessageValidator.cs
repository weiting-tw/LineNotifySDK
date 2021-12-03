using System;
using FluentValidation;
using LineNotifySDK.Model;

namespace LineNotifySDK.Validator
{
    public class LineNotifyMessageValidator : AbstractValidator<LineNotifyMessage>
    {
        private const string MaxLengthErrorMessage = "1000 characters max.";
        private const string UrlErrorMessage = "must be http/https url.";
        private const string ImageUrlMissingErrorMessage = "ImageFullSize and ImageThumbnail must have value at the same time.";

        public LineNotifyMessageValidator()
        {
            RuleFor(x => x.Message)
                .NotEmpty()
                .MaximumLength(1000).WithMessage(MaxLengthErrorMessage);
            RuleFor(x => x.ImageThumbnail)
                .Must(IsUri).WithMessage($"{nameof(LineNotifyMessage.ImageThumbnail)} {UrlErrorMessage}")
                .NotNull().When(x => x.ImageFullSize != null).WithMessage(ImageUrlMissingErrorMessage);
            RuleFor(x => x.ImageFullSize)
                .Must(IsUri).WithMessage($"{nameof(LineNotifyMessage.ImageFullSize)} {UrlErrorMessage}")
                .NotNull().When(x => x.ImageThumbnail != null).WithMessage(ImageUrlMissingErrorMessage);
        }

        private bool IsUri(string url)
        {
            return Uri.TryCreate(url, UriKind.Absolute, out _);
        }
    }
}
