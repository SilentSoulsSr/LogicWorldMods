using JimmysUnityUtilities;
using LogicWorld.Server.Circuitry;
using SilentsFileRom.Shared;

namespace SilentsFileRom.Server.LogicCode;

public class FileROM8bit : LogicComponent<FileROM8bit.IData>
{
    public interface IData
    {
        string LabelText { get; set; }
        Color24 LabelColor { get; set; }
        bool LabelMonospace { get; set; }
        float LabelFontSizeMax { get; set; }
        int HorizontalAlignment { get; set; }
        int VerticalAlignment { get; set; }
        int SizeX { get; set; }
        int SizeZ { get; set; }
        byte[] FileData { get; set; }
    }

    private bool _fileLoaded = false;

    protected override void DoLogicUpdate()
    {
        if (Inputs.Count < 17)
        {
            Logger.Info("Missing input pins (" + Inputs.Count.ToString() + " / 17)");
            return;
        }
        if (Outputs.Count < 9)
        {
            Logger.Info("Missing output pins (" + Outputs.Count.ToString() + " / 9)");
            return;
        }

        if (Inputs[16].On)
        {
            if (_fileLoaded != true)
            {
                _fileLoaded = true;

                byte[] data = FileLoader.LoadFromFile(Data.LabelText);
                if (data == null)
                    Logger.Info("Failed to load file '" + Data.LabelText);
                else
                {
                    Data.FileData = data;
                    Logger.Info("Loaded file. " + Data.LabelText);
                }
            }

            for (int i = 0; i < 8; i++)
                Outputs[i].On = false;

            return;
        }

        _fileLoaded = false;

        // Handle data 
        int address = 0;
        for (int i = 0; i < 16; i++)
            address += Inputs[i].On ? 1 << i : 0;

        byte output = 0;
        bool eof = address >= Data.FileData.Length;
        if (ComponentData.CustomData != null && !eof)
            output = Data.FileData[address];

        Outputs[8].On = eof;
        for (int i = 0; i < 8; i++)
            Outputs[i].On = (output & (1 << i)) > 0;
    }

    protected override void OnCustomDataUpdated()
    {
        QueueLogicUpdate();
    }

    protected override void SetDataDefaultValues()
    {
        Data.LabelText = "Filename here";
        Data.LabelFontSizeMax = 0.8f;
        Data.LabelColor = new Color24(38, 38, 38);
        Data.LabelMonospace = false;
        Data.HorizontalAlignment = 1;
        Data.VerticalAlignment = 1;
        Data.SizeX = 8;
        Data.SizeZ = 2;
        Data.FileData = new byte[0];
    }
}
