using System.IO.Compression;

namespace Meshmakers.Common.Shared.Services;

/// <summary>
/// Implementation of service for compressing and decompressing data.
/// </summary>
public class CompressionService : ICompressionService
{
    public async Task ExtractFileFromZipAsync(Stream zipStream, string contentType,
        Func<IEnumerable<CompressedFile>, CompressedFile?> filterFunc, string targetFile)
    {
        if (contentType.ToLower() != "application/zip" && contentType.ToLower() != "application/x-zip-compressed")
        {
            throw new NotSupportedException($"'{contentType}' not a supported content type.");
        }

        using (var zipArchive = new ZipArchive(zipStream))
        {
            var list = zipArchive.Entries.Select(x=>
                new CompressedFile(x));

            var entry = filterFunc(list);
            if (entry == null)
            {
                throw new NotSupportedException($"No file to extract was found in zip archive.");
            }

            var archiveFileStream = entry.ZipArchiveEntry.Open();

#if NETSTANDARD2_0
            using (var streamWriter = new StreamWriter(targetFile))
#else
            await using (var streamWriter = new StreamWriter(targetFile))
#endif
            {
                await archiveFileStream.CopyToAsync(streamWriter.BaseStream);
            }
        }
    }

    public async Task PackFileToZipAsync(MemoryStream zipStream, Stream entryStream, string fileNameWithExtension,
         bool leaveOpen = false)
    {
        using (var zipArchive = new ZipArchive(zipStream, ZipArchiveMode.Create, leaveOpen))
        {
            var archiveEntry = zipArchive.CreateEntry(fileNameWithExtension);
#if NETSTANDARD2_0
            using (var stream = archiveEntry.Open())
#else
            await using (var stream = archiveEntry.Open())
#endif
            {
                await entryStream.CopyToAsync(stream);
            }
        }
    }
}
