using System;
using System.IO;
using System.Linq;

namespace SilentsFileRom.Shared;

public static class FileLoader
{
    public static string[] GetFiles(string label) => label.Split('\n', System.StringSplitOptions.RemoveEmptyEntries).Select(item => item.Trim()).ToArray();

    public static byte[] LoadFromFile(string file)
    {
        try
        {
            if (string.IsNullOrEmpty(file))
                return null;

            string[] fileData = file.Split("|", StringSplitOptions.RemoveEmptyEntries);
            file = fileData[0];
            if (fileData.Length > 1)
            {
                if (fileData[1].Equals("hex", StringComparison.OrdinalIgnoreCase))
                    return LoadFromHex(file);
                if (fileData[1].Equals("mc", StringComparison.OrdinalIgnoreCase))
                    return LoadFromMc(file);
            }

            if (file.EndsWith(".hex", StringComparison.OrdinalIgnoreCase))
                return LoadFromHex(file);
            if (file.EndsWith(".mc", StringComparison.OrdinalIgnoreCase))
                return LoadFromMc(file);

            return LoadRaw(file);
        }
        catch { }

        return null;
    }

    // Load data from a .hex file.
    private static byte[] LoadFromHex(string file)
    {
        string hex = File.ReadAllText(file);
        if (string.IsNullOrEmpty(hex))
            return null;

        hex = string.Join("",
                hex.Split("\n", StringSplitOptions.RemoveEmptyEntries)
                 .Select(line => line.Split('#', ';')[0].Trim()) // remove all comments
                 .Where(line =>
                     !string.IsNullOrEmpty(line) &&
                     "abcdefgABCDEFG0123456789".Contains(line[0])) // Skip non hex lines
                );

        if ((hex.Length & 1) != 0)
            return null;

        byte[] data = new byte[hex.Length >> 1];
        for (int i = 0; i < hex.Length; i += 2)
        {
            // this will cause any line that was more than one byte to be stored in big endian
            string byteValue = hex.Substring(i, 2);
            data[i >> 1] = Convert.ToByte(hex[i..(i + 2)], 16);
        }

        return data;
    }

    // Load data raw from any other file.
    private static byte[] LoadRaw(string file)
    {
        byte[] data = File.ReadAllBytes(file);
        return data;
    }

    // Load data from a .mc file.
    private static byte[] LoadFromMc(string file)
    {
        string binary = File.ReadAllText(file);
        if (string.IsNullOrEmpty(binary))
            return null;

        binary = string.Join("",
            binary.Split("\n", StringSplitOptions.RemoveEmptyEntries)
                .Select(line => line.Trim())
                .Where(line => !string.IsNullOrEmpty(line))
                .Aggregate(string.Empty, (current, line) => current + line)
        );

        if ((binary.Length & 3) != 0) // binary.Length % 8 != 0
            return null;

        byte[] data = new byte[binary.Length >> 3];
        for (int i = 0; i < binary.Length; i += 8)
        {
            // this will cause any line that was more than one byte to be stored in big endian
            string byteValue = binary.Substring(i, 8);
            data[i >> 3] = Convert.ToByte(byteValue, 2);
        }
        
        return data;
    }

    public static void SaveToFile(string file, byte[] data)
    {
        try
        {
            if (string.IsNullOrEmpty(file))
                return;

            File.WriteAllBytes(file, data);
        }
        catch { }
    }

}

public class TriggerOnChange
{
    private bool _on;
    public void SetOn(bool value)
    {
        Up = _on && !value;
        Down = !_on && value;

        _on = value;
    }

    public bool Down { get; private set; }
    public bool Up { get; private set; }
}
