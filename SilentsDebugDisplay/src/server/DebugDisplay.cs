using JimmysUnityUtilities;
using LogicWorld.Server.Circuitry;

namespace SilentsDebugDisplay.Server.LogicCode;

public class DebugDisplay : LogicComponent<DebugDisplay.IData>
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

        byte Value { get; set; }
    }

    private Color24 textColor = new Color24(38, 38, 38);

    protected override void DoLogicUpdate()
    {
        int value = 0;
        for (int i = 0; i < 8; i++)
            value += Inputs[i].On ? 1 << i : 0;

        if (Data.Value != (byte)value)
            Data.Value = (byte)value;
    }

    protected override void OnCustomDataUpdated()
    {
        QueueLogicUpdate();
    }

    protected override void SetDataDefaultValues()
    {
        Data.LabelText = "0\n0\n0";
        Data.LabelFontSizeMax = 2.0f;
        Data.LabelColor = textColor;
        Data.LabelMonospace = false;
        Data.HorizontalAlignment = 0;
        Data.VerticalAlignment = 0;
        Data.SizeX = 2;
        Data.SizeZ = 3;
        Data.Value = 0;
    }
}
