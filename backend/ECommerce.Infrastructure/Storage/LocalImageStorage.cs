using ECommerce.Application.Catalog.Common.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Infrastructure.Storage
{
    public class LocalImageStorage:IImageStorage
    {
        private readonly string _storageDirectory;

        public LocalImageStorage(string storageDirectory)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(storageDirectory);

            _storageDirectory = Path.GetFullPath(storageDirectory);
        }

        public async Task<string> SaveAsync(
            Stream content,
            string extension,
            CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(content);

            // Defense in depth: only server-approved extensions.
            var allowedExtensions = new HashSet<string>(
                StringComparer.OrdinalIgnoreCase)//makes the comparison case-insensitive:
        {
            ".jpg",
            ".png",
            ".webp"
        };

            if (!allowedExtensions.Contains(extension))
            {
                throw new ArgumentException(
                    "Unsupported image extension.",
                    nameof(extension));
            }

            Directory.CreateDirectory(_storageDirectory);

            var storageKey = $"{Guid.NewGuid():N}{extension}";
            var finalPath = Path.Combine(
                _storageDirectory,
                storageKey);

            try
            {
                await using var fileStream = new FileStream(
                    finalPath,
                    FileMode.CreateNew,
                    FileAccess.Write,
                    FileShare.None,
                    bufferSize: 81920,
                    useAsync: true);

                await content.CopyToAsync(
                    fileStream,
                    cancellationToken);

                return storageKey;
            }
            catch
            {
                if (File.Exists(finalPath))
                {
                    File.Delete(finalPath);
                }

                throw;
            }
        }

        public Task DeleteAsync(
            string storageKey,
            CancellationToken cancellationToken = default)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(storageKey);
            cancellationToken.ThrowIfCancellationRequested();

            // Storage keys are single generated filenames,
            // not paths supplied by the HTTP client.
            if (storageKey != Path.GetFileName(storageKey) ||
                storageKey.Contains('/') ||
                storageKey.Contains('\\'))
            {
                throw new ArgumentException(
                    "Invalid storage key.",
                    nameof(storageKey));
            }

            var fullPath = Path.Combine(
                _storageDirectory,
                storageKey);

            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
            }

            return Task.CompletedTask;
        }
    }
}
