using System;
using System.IO;

namespace SilentsMod.Shared;

public static class FileLoader
{
    public static string LoadFromFile(string istr, byte[] idata)
    {
        try
        {
            string file = istr?.Trim()?.Split('\n')?[0]?.Trim();
            if (string.IsNullOrEmpty(file))
                return "Failed to load file.";

            var data = File.ReadAllBytes(file);

            Array.Resize(ref data, 65536);
            Array.Copy(data, idata, 65536);
        }
        catch (Exception ex)
        {
            return "Failed to load file" + ex.Message;
        }

        return null;
    }
}
