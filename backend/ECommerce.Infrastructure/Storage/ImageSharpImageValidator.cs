using ECommerce.Application.Catalog.Common.Abstractions;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Formats.Webp;
using static System.Net.Mime.MediaTypeNames;
using Image = SixLabors.ImageSharp.Image;
namespace ECommerce.Infrastructure.Storage
{
    public class ImageSharpImageValidator: IImageValidator
    {
        private const int MaxFileBytes = 5 * 1024 * 1024;
        private const int MaxDimension = 6000;
        private const long MaxPixels = 20_000_000;

        public async Task<ValidatedImage> ValidateAsync(
            Stream content,
            CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(content);

            using var input = new MemoryStream();

            var buffer = new byte[81920];
            long totalBytes = 0;

            while (true)
            {
                var bytesRead = await content.ReadAsync(
                    buffer.AsMemory(),
                    cancellationToken);

                if (bytesRead == 0)
                    break;

                totalBytes += bytesRead;

                if (totalBytes > MaxFileBytes)
                {
                    throw new InvalidImageException(
                        "Image cannot exceed 5 MB.");
                }

                await input.WriteAsync(
                    buffer.AsMemory(0, bytesRead),
                    cancellationToken);
            }

            if (totalBytes == 0)
            {
                throw new InvalidImageException(
                    "Image file is empty.");
            }

            input.Position = 0;

            try
            {
                // Inspect dimensions before fully decoding.
                var info = await Image.IdentifyAsync(
                    input,
                    cancellationToken);

                if (info is null ||
                    info.Width > MaxDimension ||
                    info.Height > MaxDimension ||
                    (long)info.Width * info.Height > MaxPixels)
                {
                    throw new InvalidImageException(
                        "Invalid image dimensions.");
                }

                input.Position = 0;

                using var image = await Image.LoadAsync(
                    input,
                    cancellationToken);

                // Re-encode into a server-controlled format.
                var output = new MemoryStream();

                try
                {
                    await image.SaveAsWebpAsync(
                        output,
                        new WebpEncoder { Quality = 85 },
                        cancellationToken);

                    output.Position = 0;

                    return new ValidatedImage(
                        output,
                        ".webp");
                }
                catch
                {
                    output.Dispose();
                    throw;
                }
            }
            catch (UnknownImageFormatException)
            {
                throw new InvalidImageException(
                    "Unsupported image format.");
            }
            catch (InvalidImageContentException)
            {
                throw new InvalidImageException(
                    "Image content is invalid.");
            }
        }
    }
}
