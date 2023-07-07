using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;

namespace Meshmakers.Common.Shared;

// ReSharper disable once UnusedType.Global
[SuppressMessage("ReSharper", "UnusedMember.Global")]
public static class Compression
{
    public static async Task ExtractFileFromZipAsync(this Stream zipStream, string contentType,
        string fileExtension, string targetFile)
    {
        if (contentType.ToLower() != "application/zip")
        {
            throw new NotSupportedException($"'{contentType}' not a supported content type.");
        }

        using (var zipArchive = new ZipArchive(zipStream))
        {
            var entry = zipArchive.Entries.FirstOrDefault(x =>
                Path.GetExtension(x.Name).ToLower() == fileExtension.ToLower());
            if (entry == null)
            {
                throw new NotSupportedException($"No file extension '{fileExtension}' was found in zip archive.");
            }

            var archiveFileStream = entry.Open();

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

    public static async Task PackFileToZipAsync(this Stream zipStream, string fileNameWithExtension,
        MemoryStream zipArchiveStream)
    {
        using (var zipArchive = new ZipArchive(zipArchiveStream, ZipArchiveMode.Create))
        {
            var archiveEntry = zipArchive.CreateEntry(fileNameWithExtension);
#if NETSTANDARD2_0  
            using (var stream = archiveEntry.Open())
#else
             await using (var stream = archiveEntry.Open())
#endif
            {
                await zipStream.CopyToAsync(stream);
            }
        }
    }
}
