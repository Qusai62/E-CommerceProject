using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.Catalog.Common.Abstractions
{
    public interface IImageValidator
    {
        Task<ValidatedImage> ValidateAsync(
        Stream content,
        CancellationToken cancellationToken = default);
    }
    public sealed class ValidatedImage : IDisposable
    {
        public Stream Content { get; }
        public string Extension { get; }

        public ValidatedImage(Stream content, string extension)
        {
            Content = content;
            Extension = extension;
        }

        public void Dispose() => Content.Dispose();
    }

    public sealed class InvalidImageException : Exception
    {
        public InvalidImageException(string message)
            : base(message)
        {
        }
    }
}
