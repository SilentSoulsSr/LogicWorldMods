using System;
using System.Collections.Generic;
using System.Linq;
using JimmysUnityUtilities;
using LogicWorld.Server.Circuitry;
using SilentsFileRom.Shared;

namespace SilentsFileRom.Server.LogicCode;

public class FileRAM8bit : LogicComponent<FileRAM8bit.IData>
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

    private byte[] _fileData = new byte[65536];

    const int PegInAddress = 0;
    const int PegInData = 16;
    const int PegInWriteMemory = 24;
    const int PegInReset = 25;
    const int PegInFileLoad = 26;
    const int PegInFileSave = 27;

    const int PegOutValue = 0;
    const int PegOutLoaded = 8;
    const int PegOutSaved = 9;

    private Dictionary<int, TriggerOnChange> _pressed = new(
    [
        new(PegInWriteMemory, new()),
        new(PegInReset, new()) ,
        new(PegInFileLoad, new()),
        new(PegInFileSave, new()),
    ]);

    protected override void DoLogicUpdate()
    {
        bool issaved = false;
        bool isloaded = false;
        int address = 0;
        byte output = 0;

        // Handle

        foreach (var peg in _pressed)
            _pressed[peg.Key].SetOn(Inputs[peg.Key].On);

        bool isValid = _pressed.Where(item => Inputs[item.Key].On).Count() <= 1;
        if (isValid)
        {
            if (_pressed[PegInReset].Down)
                _fileData = new byte[65536];

            if (_pressed[PegInFileLoad].Down)
            {
                string[] files = FileLoader.GetFiles(Data.LabelText);
                byte[] data = FileLoader.LoadFromFile(files.First());
                if (data == null)
                    Logger.Info("Failed to load file '" + files.First());
                else if (data.Length > 65536)
                    Logger.Info("File to large " + files.First());
                else
                {
                    _fileData = new byte[65536];
                    Array.Copy(data, _fileData, data.Length);
                    Logger.Info("Loaded file. " + files.First());
                }

                isloaded = true;
            }

            if (_pressed[PegInFileSave].Down)
            {
                string[] files = FileLoader.GetFiles(Data.LabelText);
                FileLoader.SaveToFile(files.Last(), _fileData);
                Logger.Info("Saved to file. " + files.Last());

                issaved = true;
            }
        }

        for (int i = 0; i < 16; i++)
            address += Inputs[i + PegInAddress].On ? 1 << i : 0;

        if (ComponentData.CustomData != null)
            output = _fileData[address];

        if (isValid && Inputs[PegInWriteMemory].On && ComponentData.CustomData != null)
        {
            int value = 0;
            for (int i = 0; i < 8; i++)
                value += Inputs[i + PegInData].On ? 1 << i : 0;

            output = (byte)value;
            _fileData[address] = output;
        }


        // Write outputs
        for (int i = 0; i < 8; i++)
            Outputs[i + PegOutValue].On = (output & (1 << i)) > 0;

        Outputs[PegOutSaved].On = issaved;
        Outputs[PegOutLoaded].On = isloaded;
    }

    protected override void OnCustomDataUpdated()
    {
        QueueLogicUpdate();
    }

    protected override void SetDataDefaultValues()
    {
        Data.LabelText = "Input File\nOutput File";
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
