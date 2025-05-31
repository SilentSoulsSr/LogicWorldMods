using System.Collections.Generic;
using System.Linq;
using JimmysUnityUtilities;
using LogicWorld.Server.Circuitry;

namespace SilentsRepeater.Server.LogicCode;

public class Repeater : LogicComponent<Repeater.IData>
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

        int DelayTicks { get; set; }
        byte[] Delays { get; set; }
    }

    private string _previousText = "";
    private bool _isValid = false;
    private List<byte> _delays = null;

    protected override void DoLogicUpdate()
    {
        if (_delays == null)
            _delays = Data.Delays.ToList();

        if (Data.LabelText != _previousText)
        {
            _isValid = false;
            Data.Delays = [];

            var txt = Data.LabelText;
            _previousText = txt;

            txt = txt.Trim();

            if (int.TryParse(txt, out int result))
            {
                Logger.Info("Valid " + _previousText);
                _isValid = true;

                if (result > 256)
                    result = 256;

                if (result < 1)
                    result = 1;

                Data.LabelText = result.ToString();
                Data.DelayTicks = result;
            }
            else
                Logger.Info("!Valid " + _previousText);
        }

        if (!_isValid)
        {
            Outputs[0].On = false;
            return;
        }

        // reset
        if (Inputs[1].On)
            _delays.Clear();

        // Handle previous triggers
        bool trigger = false;

        // Due to the inherit delay of the component, do this bedore the subtracting
        if (Inputs[0].On)
        {
            byte valueToSet = (byte)Data.DelayTicks;
            if (!_delays.Contains(valueToSet))
                _delays.Add((byte)Data.DelayTicks);
        }

        if (_delays.Count > 0)
        {
            _delays = _delays.Select(item => (byte)(item - 1)).ToList();

            byte value = _delays.Min();
            trigger = (value <= 0);
            if (trigger)
                _delays.Remove(value);
        }

        // Set output on trigger value
        Outputs[0].On = trigger;
        Data.Delays = _delays.ToArray();
    }

    protected override void OnCustomDataUpdated()
    {
        QueueLogicUpdate();
    }

    protected override void SetDataDefaultValues()
    {
        Data.LabelText = "10";
        Data.LabelFontSizeMax = 0.8f;
        Data.LabelColor = new Color24(38, 38, 38);
        Data.LabelMonospace = false;
        Data.HorizontalAlignment = 1;
        Data.VerticalAlignment = 1;
        Data.SizeX = 1;
        Data.SizeZ = 1;

        Data.DelayTicks = 10;
        Data.Delays = [];
    }
}
