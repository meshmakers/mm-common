using System.IO.Compression;

namespace Meshmakers.Common.Shared.Services;

/// <summary>
/// Interface for service for compressing and decompressing data.
/// </summary>
public interface ICompressionService
{
    /// <summary>
    /// Extracts a file from a zip archive.
    /// </summary>
    /// <param name="zipStream">Stream that contains the compressed file.</param>
    /// <param name="contentType">Content type of the compressed file.</param>
    /// <param name="filterFunc">A filter function to select the file to extract.</param>
    /// <param name="targetFile">The file to extract to.</param>
    /// <returns></returns>
    Task ExtractFileFromZipAsync(Stream zipStream, string contentType,
        Func<IEnumerable<CompressedFile>, CompressedFile?> filterFunc, string targetFile);

    /// <summary>
    /// Packs a file to a zip archive.
    /// </summary>
    /// <param name="zipStream">Stream that contains the compressed file.</param>
    /// <param name="entryStream">Stream that contains the content to compress.</param>
    /// <param name="fileNameWithExtension">Name of the file with extension to be used in the zip archive.</param>
    /// <param name="leaveOpen">Whether to leave the zip stream open after packing the file.</param>
    /// <returns></returns>
    Task PackFileToZipAsync(MemoryStream zipStream, Stream entryStream, string fileNameWithExtension,
        bool leaveOpen = false);
}

public class CompressedFile(ZipArchiveEntry zipArchiveEntry)
{
    /// <summary>
    /// Name of the file.
    /// </summary>
    public string Name { get; } = zipArchiveEntry.Name;

    /// <summary>
    /// Length of the file.
    /// </summary>
    public long Length { get; } = zipArchiveEntry.Length;

    /// <summary>
    /// Relative path of the file in compressed file
    /// </summary>
    public string FullName { get; } = zipArchiveEntry.FullName;

    internal ZipArchiveEntry ZipArchiveEntry { get; } = zipArchiveEntry;
}
