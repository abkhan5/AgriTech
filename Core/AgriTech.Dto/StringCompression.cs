using System.IO.Compression;
using System.Text;
using System.Text.Json.Serialization;
using System.Text.Json;

namespace AgriTech;

public static class StringCompression
{
    /// <summary>
    ///     Compresses a string and returns a deflate compressed, Base64 encoded string.
    /// </summary>
    /// <param name="uncompressedString">String to compress</param>
    public static async Task<string> Compress(this string uncompressedString, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(uncompressedString))
            return uncompressedString;
        var level = CompressionLevel.SmallestSize;
        var bytes = Encoding.UTF8.GetBytes(uncompressedString);
        await using var input = new MemoryStream(bytes);
        await using var output = new MemoryStream();
        await using var stream = new BrotliStream(output, level, true);
        await input.CopyToAsync(stream, cancellationToken);
        await stream.FlushAsync(cancellationToken);
        var result = output.ToArray();
        return Convert.ToHexString(result);
        //var compResult = new CompressionResult(
        //    new CompressionValue(uncompressedString, bytes.Length),
        //    new CompressionValue(compressedString, result.Length),
        //    level,"Brotli");
        //return compResult.Result.Value;
    }


    /// <summary>
    ///     Decompresses a deflate compressed, Base64 encoded string and returns an uncompressed string.
    /// </summary>
    /// <param name="compressedString">String to decompress.</param>
    public static async Task<string> Decompress(this string compressedString, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(compressedString))
            return compressedString;
        var bytes = Convert.FromHexString(compressedString);
        await using var input = new MemoryStream(bytes);
        await using var output = new MemoryStream();
        await using var stream = new BrotliStream(input, CompressionMode.Decompress);

        await stream.CopyToAsync(output, cancellationToken);

        return Encoding.UTF8.GetString(output.ToArray());
    }

    public static async Task<Stream> Compress(this byte[] uncompressedBytes, CancellationToken cancellationToken)
    {
        if (uncompressedBytes == null || uncompressedBytes.Length == 0)
            return null;
        var level = CompressionLevel.SmallestSize;
        await using var input = new MemoryStream(uncompressedBytes);
        await using var output = new MemoryStream();
        await using var stream = new BrotliStream(output, level, true);
        await input.CopyToAsync(stream, cancellationToken);
        await stream.FlushAsync(cancellationToken);
        return output;
    }

    public static async Task<Stream> Compress(this Stream input, CancellationToken cancellationToken)
    {
        if (input == null)
            return null;
        var level = CompressionLevel.SmallestSize;
        await using var output = new MemoryStream();
        await using var stream = new BrotliStream(output, level, true);
        await input.CopyToAsync(stream, cancellationToken);
        await stream.FlushAsync(cancellationToken);
        return output;
    }

    public static async Task<byte[]> Decompress(this Stream input, CancellationToken cancellationToken)
    {
        if (input == null)
            return null;

        await using var output = new MemoryStream();
        await using var stream = new BrotliStream(input, CompressionMode.Decompress);

        await stream.CopyToAsync(output, cancellationToken);

        return output.ToArray();
    }

    public record CompressionResult(CompressionValue Original, CompressionValue Result, CompressionLevel Level,
        string Kind)
    {
        public int Difference => Original.Size - Result.Size;

        public decimal Percent => Math.Abs(Difference / (decimal)Original.Size);
    }

    public record CompressionValue(string Value, int Size);

    public static string ToJson<T>(this T item) =>
      JsonSerializer.Serialize(item, new JsonSerializerOptions
      {
          ReferenceHandler = ReferenceHandler.IgnoreCycles,
          PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
          DictionaryKeyPolicy = JsonNamingPolicy.CamelCase,
          PropertyNameCaseInsensitive = true,
      });


    public static T FromJson<T>(this string json) =>
    JsonSerializer.Deserialize<T>(json ?? "", new JsonSerializerOptions
    {
        ReferenceHandler = ReferenceHandler.IgnoreCycles,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DictionaryKeyPolicy = JsonNamingPolicy.CamelCase,
        PropertyNameCaseInsensitive = true
    });
}