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

            byte[] data = File.ReadAllBytes(file);
            return data;
        }
        catch { }

        return null;
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
