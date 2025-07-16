using System.Collections.Generic;
using System.Linq;
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
        bool LoadFile { get; set; }
        bool UpdateData { get; set; }
        bool FileSuccess { get; set; }
        byte[] FileData { get; set; }
    }

    private byte[] _fileData = null;

    const int PegInAddress = 0;
    const int PegInFileLoad = 16;

    const int PegOutValue = 0;
    const int PegOutEOF = 8;

    private Dictionary<int, TriggerOnChange> _pressed = new(
    [
        new(PegInFileLoad, new()),
    ]);

    protected override void DoLogicUpdate()
    {
        if (Data.UpdateData == true)
            Data.UpdateData = false;

        foreach (var peg in _pressed)
            _pressed[peg.Key].SetOn(Inputs[peg.Key].On);

        if (_fileData == null || _pressed[PegInFileLoad].Down || Data.LoadFile)
        {
            _fileData = new byte[0];
            Data.LoadFile = false;

            string[] files = FileLoader.GetFiles(Data.LabelText);
            byte[] data = FileLoader.LoadFromFile(files.First());
            if (data == null)
            {
                Data.FileSuccess = false;
                Logger.Info("Failed to load file " + Data.LabelText);
            }
            else if (data.Length > 65536)
            {
                Data.FileSuccess = false;
                Logger.Info("File to large " + files.First());
            }
            else
            {
                Data.FileSuccess = true;
                _fileData = data;
                Logger.Info("Loaded file. " + Data.LabelText);
            }

            for (int i = 0; i < 9; i++)
                Outputs[i].On = false;

            return;
        }

        // Handle data 
        int address = 0;
        for (int i = 0; i < 16; i++)
            address += Inputs[i + PegInAddress].On ? 1 << i : 0;

        byte output = 0;
        bool eof = address >= _fileData.Length;
        if (ComponentData.CustomData != null && !eof)
            output = _fileData[address];

        Outputs[PegOutEOF].On = eof;
        for (int i = 0; i < 8; i++)
            Outputs[i + PegOutValue].On = (output & (1 << i)) > 0;
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
        Data.LoadFile = false;
        Data.UpdateData = false;
        Data.FileSuccess = false;
        Data.FileData = new byte[0];
    }
}
