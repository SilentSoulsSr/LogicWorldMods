using System.IO;

namespace SilentsFileRom.Shared;

public static class FileLoader
{
    public static byte[] LoadFromFile(string text)
    {
        try
        {
            string file = text?.Trim()?.Split('\n')?[0]?.Trim();
            if (string.IsNullOrEmpty(file))
                return null;

            byte[] data = File.ReadAllBytes(file);
            return data;
        }
        catch { }

        return null;
    }
}
