using Application.Abstractions.FileUploader;
using Application.Abstractions.FileUploader.Models;
using Domain.Entities;
using Domain.Exceptions;
using ImageMagick;
using Microsoft.AspNetCore.Hosting;

namespace Infrastructure.FileUploader;

public sealed class FileService : IFileService
{
    private const int MaxStoredNameLength = 255;
    private const int MaxOriginalFileNameLength = 255;
    private const int MaxExtensionLength = 32;
    private const int MaxContentTypeLength = 255;
    private const int MaxUrlLength = 2_048;
    private const long MaxConvertiblePixels = 50_000_000;
    private const string DefaultContentType = "application/octet-stream";

    private static readonly int[] ResponsiveWidths = [480, 768, 1280];
    private static readonly HashSet<string> ConvertibleImageExtensions = new(StringComparer.OrdinalIgnoreCase)
    {
        ".svg", ".png", ".jpg", ".jpeg", ".bmp", ".tif", ".tiff", ".webp"
    };
    private static readonly char[] InvalidFileNameChars = Path.GetInvalidFileNameChars();

    private readonly string _rootUploadFolder;
    private readonly string _requestPath = "/uploads";

    public FileService(IWebHostEnvironment webHostEnvironment)
    {
        var webRootPath = string.IsNullOrWhiteSpace(webHostEnvironment.WebRootPath)
            ? Path.Combine(webHostEnvironment.ContentRootPath, "wwwroot")
            : webHostEnvironment.WebRootPath;

        _rootUploadFolder = Path.Combine(webRootPath, "uploads");
        Directory.CreateDirectory(_rootUploadFolder);
    }

    public async Task<UploadedFile> UploadAsync(FileUploadRequest file, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(file);

        if (file.Length <= 0)
            throw new ArgumentException("Dosya boş olamaz.", nameof(file));

        var originalFileName = Path.GetFileName(file.FileName).Trim();
        if (string.IsNullOrWhiteSpace(originalFileName))
            throw new ArgumentException("Dosya adı geçersiz.", nameof(file));

        var extension = Path.GetExtension(originalFileName).ToLowerInvariant();
        if (extension.Length > MaxExtensionLength)
            throw new ArgumentException("Dosya uzantısı çok uzun.", nameof(file));

        if (originalFileName.Length > MaxOriginalFileNameLength)
        {
            var originalBaseName = Path.GetFileNameWithoutExtension(originalFileName);
            originalFileName = string.IsNullOrEmpty(extension) || extension.Length >= MaxOriginalFileNameLength
                ? originalFileName[..MaxOriginalFileNameLength]
                : $"{originalBaseName[..Math.Min(originalBaseName.Length, MaxOriginalFileNameLength - extension.Length)]}{extension}";
        }

        var storedBaseName = new string(Path.GetFileNameWithoutExtension(originalFileName)
            .Select(character => InvalidFileNameChars.Contains(character) ? '_' : character)
            .ToArray())
            .Trim()
            .Replace(' ', '_');

        if (string.IsNullOrWhiteSpace(storedBaseName))
            storedBaseName = "file";

        string storedFileName;
        string fullPath;

        do
        {
            var suffix = $"_{Guid.NewGuid():N}"[..13];
            var maxBaseLength = Math.Max(1, MaxStoredNameLength - extension.Length - suffix.Length);
            var shortenedBaseName = storedBaseName.Length <= maxBaseLength
                ? storedBaseName
                : storedBaseName[..maxBaseLength];
            storedFileName = $"{shortenedBaseName}{suffix}{extension}";
            fullPath = Path.Combine(_rootUploadFolder, storedFileName);
        }
        while (File.Exists(fullPath));

        var contentType = string.IsNullOrWhiteSpace(file.ContentType)
            ? DefaultContentType
            : file.ContentType.Trim()[..Math.Min(file.ContentType.Trim().Length, MaxContentTypeLength)];
        var url = $"{_requestPath}/{storedFileName}";

        if (url.Length > MaxUrlLength)
            throw new InvalidOperationException("Oluşturulan dosya adresi desteklenen uzunluğu aşıyor.");

        await using var sourceStream = file.OpenReadStream();
        await using var targetStream = new FileStream(
            fullPath,
            FileMode.CreateNew,
            FileAccess.Write,
            FileShare.None,
            81920,
            FileOptions.Asynchronous);

        await sourceStream.CopyToAsync(targetStream, cancellationToken);

        return new UploadedFile
        {
            Name = Path.GetFileNameWithoutExtension(storedFileName),
            OriginalFileName = originalFileName,
            Extension = extension,
            ContentType = contentType,
            FileSize = file.Length,
            Url = url
        };
    }

    public async Task<IReadOnlyList<UploadedFile>> UploadAsync(IEnumerable<FileUploadRequest> files, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(files);

        var uploadedFiles = new List<UploadedFile>();
        try
        {
            foreach (var file in files)
            {
                cancellationToken.ThrowIfCancellationRequested();
                uploadedFiles.Add(await UploadAsync(file, cancellationToken));
            }
        }
        catch
        {
            foreach (var uploadedFile in uploadedFiles)
            {
                try
                {
                    await DeleteAsync(uploadedFile.Url, CancellationToken.None);
                }
                catch
                {
                }
            }

            throw;
        }

        return uploadedFiles;
    }

    public async Task<ConvertedFileResult> CreateWebPAsync(UploadedFile sourceFile, int quality = 82, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(sourceFile);

        if (quality is < 1 or > 100)
            throw new ArgumentOutOfRangeException(nameof(quality), "WebP kalitesi 1 ile 100 arasında olmalıdır.");

        var sourceExtension = sourceFile.Extension.Trim().ToLowerInvariant();
        if (!ConvertibleImageExtensions.Contains(sourceExtension))
            throw new BusinessRuleException("Yalnızca SVG, PNG, JPEG, BMP, TIFF ve WebP görselleri dönüştürülebilir.");

        var relativePath = sourceFile.Url
            .Replace('/', Path.DirectorySeparatorChar)
            .Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar)
            .Trim()
            .TrimStart(Path.DirectorySeparatorChar);
        var uploadsPrefix = $"uploads{Path.DirectorySeparatorChar}";
        if (relativePath.StartsWith(uploadsPrefix, StringComparison.OrdinalIgnoreCase))
            relativePath = relativePath[uploadsPrefix.Length..];

        var rootPath = Path.GetFullPath(_rootUploadFolder);
        var sourcePath = Path.GetFullPath(Path.Combine(rootPath, relativePath));
        var rootPrefix = $"{rootPath}{Path.DirectorySeparatorChar}";
        if (!sourcePath.StartsWith(rootPrefix, StringComparison.OrdinalIgnoreCase) || !File.Exists(sourcePath))
            throw new BusinessRuleException("Kaynak görsel sunucudaki uploads klasöründe bulunamadı.");

        cancellationToken.ThrowIfCancellationRequested();

        var targetBaseName = new string(sourceFile.Name
            .Select(character => InvalidFileNameChars.Contains(character) ? '_' : character)
            .ToArray())
            .Trim()
            .Replace(' ', '_');
        if (string.IsNullOrWhiteSpace(targetBaseName))
            targetBaseName = "image";

        var uniqueSuffix = Guid.NewGuid().ToString("N")[..12];
        var targets = new List<(string FileName, string Path, string Url)>();

        foreach (var width in ResponsiveWidths)
        {
            const string webpExtension = ".webp";
            var suffix = $"_{uniqueSuffix}_{width}w";
            var maxBaseLength = Math.Max(1, MaxStoredNameLength - webpExtension.Length - suffix.Length);
            var shortenedBaseName = targetBaseName.Length <= maxBaseLength
                ? targetBaseName
                : targetBaseName[..maxBaseLength];
            var fileName = $"{shortenedBaseName}{suffix}{webpExtension}";
            targets.Add((fileName, Path.Combine(_rootUploadFolder, fileName), $"{_requestPath}/{fileName}"));
        }

        try
        {
            using var image = new MagickImage(sourcePath);
            image.AutoOrient();

            var pixelCount = checked((long)image.Width * image.Height);
            if (image.Width <= 0 || image.Height <= 0 || pixelCount > MaxConvertiblePixels)
                throw new BusinessRuleException("Görsel boyutları güvenli dönüştürme sınırını aşıyor.");

            image.Strip();
            image.Format = MagickFormat.WebP;
            image.Quality = (uint)quality;
            image.Settings.SetDefine(MagickFormat.WebP, "method", "6");

            for (var index = 0; index < ResponsiveWidths.Length; index++)
            {
                cancellationToken.ThrowIfCancellationRequested();
                using var variant = image.Clone();
                variant.Resize((uint)ResponsiveWidths[index], 0);
                await variant.WriteAsync(targets[index].Path, cancellationToken);
            }

            var convertedOriginalFileName = Path.ChangeExtension(sourceFile.OriginalFileName, ".webp");
            if (convertedOriginalFileName.Length > MaxOriginalFileNameLength)
            {
                var convertedBaseName = Path.GetFileNameWithoutExtension(convertedOriginalFileName);
                const string webpExtension = ".webp";
                convertedOriginalFileName = $"{convertedBaseName[..Math.Min(convertedBaseName.Length, MaxOriginalFileNameLength - webpExtension.Length)]}{webpExtension}";
            }

            var primaryTarget = targets[^1];
            return new ConvertedFileResult
            {
                Name = Path.GetFileNameWithoutExtension(primaryTarget.FileName),
                OriginalFileName = convertedOriginalFileName,
                Extension = ".webp",
                ContentType = "image/webp",
                FileSize = new FileInfo(primaryTarget.Path).Length,
                Url = primaryTarget.Url,
                GeneratedUrls = targets.Select(target => target.Url).ToArray()
            };
        }
        catch (Exception exception)
        {
            foreach (var target in targets)
            {
                if (File.Exists(target.Path))
                    File.Delete(target.Path);
            }

            if (exception is MagickException)
                throw new BusinessRuleException("Seçilen görsel çözümlenemedi veya WebP formatına dönüştürülemedi.");

            var nativeDependencyFailure = false;
            for (var current = exception; current is not null; current = current.InnerException)
            {
                if (current is DllNotFoundException
                    or BadImageFormatException
                    or TypeInitializationException
                    or TypeLoadException
                    or FileLoadException
                    or EntryPointNotFoundException)
                {
                    nativeDependencyFailure = true;
                    break;
                }
            }

            if (nativeDependencyFailure)
                throw new BusinessRuleException("WebP dönüştürücüsünün çalışma zamanı dosyaları bulunamadı.");
            if (exception is UnauthorizedAccessException)
                throw new BusinessRuleException("Uygulamanın uploads klasörüne yazma izni bulunmuyor.");
            if (exception is IOException)
                throw new BusinessRuleException("Görsel dosyası sunucuda okunamadı veya yazılamadı.");

            throw;
        }
    }

    public async Task<SocialShareImageResult> CreateSocialShareImageAsync(UploadedFile sourceFile, int width = 1200, int height = 630, int quality = 88, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(sourceFile);

        if (width <= 0)
            throw new ArgumentOutOfRangeException(nameof(width));
        if (height <= 0)
            throw new ArgumentOutOfRangeException(nameof(height));
        if (quality is < 1 or > 100)
            throw new ArgumentOutOfRangeException(nameof(quality), "WebP kalitesi 1 ile 100 arasında olmalıdır.");

        var sourceExtension = sourceFile.Extension.Trim().ToLowerInvariant();
        if (!ConvertibleImageExtensions.Contains(sourceExtension))
            throw new BusinessRuleException("Yalnızca SVG, PNG, JPEG, BMP, TIFF ve WebP görselleri sosyal paylaşım görseli olarak kullanılabilir.");

        var relativePath = sourceFile.Url
            .Replace('/', Path.DirectorySeparatorChar)
            .Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar)
            .Trim()
            .TrimStart(Path.DirectorySeparatorChar);
        var uploadsPrefix = $"uploads{Path.DirectorySeparatorChar}";
        if (relativePath.StartsWith(uploadsPrefix, StringComparison.OrdinalIgnoreCase))
            relativePath = relativePath[uploadsPrefix.Length..];

        var rootPath = Path.GetFullPath(_rootUploadFolder);
        var sourcePath = Path.GetFullPath(Path.Combine(rootPath, relativePath));
        var rootPrefix = $"{rootPath}{Path.DirectorySeparatorChar}";
        if (!sourcePath.StartsWith(rootPrefix, StringComparison.OrdinalIgnoreCase) || !File.Exists(sourcePath))
            throw new BusinessRuleException("Kaynak görsel sunucudaki uploads klasöründe bulunamadı.");

        cancellationToken.ThrowIfCancellationRequested();

        var baseName = new string(Path.GetFileNameWithoutExtension(sourcePath)
            .Select(character => InvalidFileNameChars.Contains(character) ? '_' : character)
            .ToArray())
            .Trim()
            .Replace(' ', '_');
        if (string.IsNullOrWhiteSpace(baseName))
            baseName = "image";

        const string webpExtension = ".webp";
        var suffix = $"_seo_{width}x{height}";
        var maxBaseLength = Math.Max(1, MaxStoredNameLength - webpExtension.Length - suffix.Length);
        var shortenedBaseName = baseName.Length <= maxBaseLength ? baseName : baseName[..maxBaseLength];
        var targetFileName = $"{shortenedBaseName}{suffix}{webpExtension}";
        var targetPath = Path.Combine(_rootUploadFolder, targetFileName);
        var targetUrl = $"{_requestPath}/{targetFileName}";

        try
        {
            using var image = new MagickImage(sourcePath);
            image.AutoOrient();

            var pixelCount = checked((long)image.Width * image.Height);
            if (image.Width <= 0 || image.Height <= 0 || pixelCount > MaxConvertiblePixels)
                throw new BusinessRuleException("Görsel boyutları güvenli dönüştürme sınırını aşıyor.");

            var geometry = new MagickGeometry((uint)width, (uint)height)
            {
                FillArea = true
            };
            image.Resize(geometry);
            image.Crop((uint)width, (uint)height, Gravity.Center);
            image.Strip();
            image.Format = MagickFormat.WebP;
            image.Quality = (uint)quality;
            image.Settings.SetDefine(MagickFormat.WebP, "method", "6");

            await image.WriteAsync(targetPath, cancellationToken);

            return new SocialShareImageResult
            {
                Url = targetUrl,
                ContentType = "image/webp",
                Extension = webpExtension,
                FileSize = new FileInfo(targetPath).Length,
                Width = width,
                Height = height
            };
        }
        catch (Exception exception)
        {
            if (File.Exists(targetPath))
                File.Delete(targetPath);

            if (exception is MagickException)
                throw new BusinessRuleException("Seçilen görsel çözümlenemedi veya WebP formatına dönüştürülemedi.");

            var nativeDependencyFailure = false;
            for (var current = exception; current is not null; current = current.InnerException)
            {
                if (current is DllNotFoundException
                    or BadImageFormatException
                    or TypeInitializationException
                    or TypeLoadException
                    or FileLoadException
                    or EntryPointNotFoundException)
                {
                    nativeDependencyFailure = true;
                    break;
                }
            }

            if (nativeDependencyFailure)
                throw new BusinessRuleException("WebP dönüştürücüsünün çalışma zamanı dosyaları bulunamadı.");
            if (exception is UnauthorizedAccessException)
                throw new BusinessRuleException("Uygulamanın uploads klasörüne yazma izni bulunmuyor.");
            if (exception is IOException)
                throw new BusinessRuleException("Görsel dosyası sunucuda okunamadı veya yazılamadı.");

            throw;
        }
    }

    public string GetSocialShareImageUrl(UploadedFile sourceFile, int width = 1200, int height = 630)
    {
        ArgumentNullException.ThrowIfNull(sourceFile);

        if (width <= 0)
            throw new ArgumentOutOfRangeException(nameof(width));
        if (height <= 0)
            throw new ArgumentOutOfRangeException(nameof(height));

        var baseName = new string(sourceFile.Name
            .Select(character => InvalidFileNameChars.Contains(character) ? '_' : character)
            .ToArray())
            .Trim()
            .Replace(' ', '_');
        if (string.IsNullOrWhiteSpace(baseName))
            baseName = "image";

        const string webpExtension = ".webp";
        var suffix = $"_seo_{width}x{height}";
        var maxBaseLength = Math.Max(1, MaxStoredNameLength - webpExtension.Length - suffix.Length);
        var shortenedBaseName = baseName.Length <= maxBaseLength ? baseName : baseName[..maxBaseLength];
        var url = $"{_requestPath}/{shortenedBaseName}{suffix}{webpExtension}";

        if (url.Length > MaxUrlLength)
            throw new InvalidOperationException("Oluşturulan dosya adresi desteklenen uzunluğu aşıyor.");

        return url;
    }

    public Task<bool> DeleteAsync(string fileNameOrUrl, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (string.IsNullOrWhiteSpace(fileNameOrUrl))
            return Task.FromResult(false);

        var relativePath = fileNameOrUrl
            .Replace('/', Path.DirectorySeparatorChar)
            .Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar)
            .Trim()
            .TrimStart(Path.DirectorySeparatorChar);
        var uploadsPrefix = $"uploads{Path.DirectorySeparatorChar}";
        if (relativePath.StartsWith(uploadsPrefix, StringComparison.OrdinalIgnoreCase))
            relativePath = relativePath[uploadsPrefix.Length..];

        var rootPath = Path.GetFullPath(_rootUploadFolder);
        var path = Path.GetFullPath(Path.Combine(rootPath, relativePath));
        var rootPrefix = $"{rootPath}{Path.DirectorySeparatorChar}";
        if (!path.StartsWith(rootPrefix, StringComparison.OrdinalIgnoreCase))
            throw new InvalidOperationException("Geçersiz yükleme yolu.");

        var candidatePaths = new List<string>();
        var extension = Path.GetExtension(path);
        var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(path);
        var matchedWidth = ResponsiveWidths
            .Select(width => $"_{width}w")
            .FirstOrDefault(widthSuffix =>
                extension.Equals(".webp", StringComparison.OrdinalIgnoreCase) &&
                fileNameWithoutExtension.EndsWith(widthSuffix, StringComparison.OrdinalIgnoreCase));

        if (matchedWidth is null)
        {
            candidatePaths.Add(path);
        }
        else
        {
            var sharedBaseName = fileNameWithoutExtension[..^matchedWidth.Length];
            candidatePaths.AddRange(ResponsiveWidths.Select(width =>
                Path.Combine(_rootUploadFolder, $"{sharedBaseName}_{width}w.webp")));
        }

        if (!fileNameWithoutExtension.EndsWith("_seo_1200x630", StringComparison.OrdinalIgnoreCase))
        {
            var seoSuffix = "_seo_1200x630";
            var maxBaseLength = Math.Max(1, MaxStoredNameLength - ".webp".Length - seoSuffix.Length);
            var seoBaseName = fileNameWithoutExtension.Length <= maxBaseLength
                ? fileNameWithoutExtension
                : fileNameWithoutExtension[..maxBaseLength];
            candidatePaths.Add(Path.Combine(_rootUploadFolder, $"{seoBaseName}{seoSuffix}.webp"));
        }

        var deleted = false;
        foreach (var candidatePath in candidatePaths.Distinct(StringComparer.OrdinalIgnoreCase))
        {
            if (!File.Exists(candidatePath))
                continue;

            File.Delete(candidatePath);
            deleted = true;
        }

        return Task.FromResult(deleted);
    }

    public bool Exists(string fileNameOrUrl)
    {
        if (string.IsNullOrWhiteSpace(fileNameOrUrl))
            return false;

        var relativePath = fileNameOrUrl
            .Replace('/', Path.DirectorySeparatorChar)
            .Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar)
            .Trim()
            .TrimStart(Path.DirectorySeparatorChar);
        var uploadsPrefix = $"uploads{Path.DirectorySeparatorChar}";
        if (relativePath.StartsWith(uploadsPrefix, StringComparison.OrdinalIgnoreCase))
            relativePath = relativePath[uploadsPrefix.Length..];

        var rootPath = Path.GetFullPath(_rootUploadFolder);
        var path = Path.GetFullPath(Path.Combine(rootPath, relativePath));
        var rootPrefix = $"{rootPath}{Path.DirectorySeparatorChar}";
        if (!path.StartsWith(rootPrefix, StringComparison.OrdinalIgnoreCase))
            throw new InvalidOperationException("Geçersiz yükleme yolu.");

        return File.Exists(path);
    }

    public string GetUrl(string fileNameOrUrl)
    {
        if (string.IsNullOrWhiteSpace(fileNameOrUrl))
            return _requestPath;

        var fileName = Path.GetFileName(fileNameOrUrl.Replace('\\', '/').Trim());
        return string.IsNullOrWhiteSpace(fileName) ? _requestPath : $"{_requestPath}/{fileName}";
    }
}
