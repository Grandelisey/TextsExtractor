using System.Text;

namespace TextsExtractor.Services;

internal static class FileService
{
    public static byte[] GetFileBinaryContent(string filePath)
    {
        byte[] fileBinaryContent;
        using (var fileStream = new FileStream(filePath, FileMode.Open))
        {
            using (var reader = new BinaryReader(fileStream))
            {
                fileBinaryContent = reader.ReadBytes((int)fileStream.Length);
            }
        }
        return fileBinaryContent;
    }

    public static DirectoryInfo CreateOutputDirectory(FileInfo inputFile, string destinationPath)
    {
        var path = Path.Combine(destinationPath, inputFile.Name);
        return Directory.CreateDirectory(path);
    }
    
    public static async Task WriteTextToFileAsync (string text, string filePath)
    {
        await using var writer = new StreamWriter(filePath, false, Encoding.UTF8);
        await writer.WriteAsync(text);
    }

    public static string AddNewExtension(string fileName, string newExtension)
    {
        return $"{fileName}.{newExtension}";
    }
    
    
}