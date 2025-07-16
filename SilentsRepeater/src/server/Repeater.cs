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

    private bool[] _delays = new bool[10];
    private int _delayTicks = 10;
    private int _tick = 0;

    private string _previousText = "";
    private bool _isValid = false;
    private bool _textChanged = false;

    protected override void DoLogicUpdate()
    {
        if (_textChanged)
        {
            _textChanged = false;
            HandleTextChange();
        }

        if (!_isValid)
        {
            Outputs[0].On = false;
            return;
        }

        // reset
        if (Inputs[1].On)
        {
            for (int i = 0; i < _delayTicks; i++)
                _delays[i] = false;

            _tick = 0;
        }

        // Handle previous triggers
        if (Inputs[0].On)
            _delays[_tick] = true;

        _tick = (_tick + 1) % _delayTicks; // rotate

        // Set output on trigger value
        Outputs[0].On = _delays[_tick];
        _delays[_tick] = false;

        QueueLogicUpdate();
    }

    private void HandleTextChange()
    {
        if (Data.LabelText != _previousText)
        {
            _isValid = false;

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

                _delayTicks = result;
                _delays = new bool[_delayTicks];

                Data.LabelText = result.ToString();
            }
            else
                Logger.Info("!Valid " + _previousText);
        }
    }

    protected override void OnCustomDataUpdated()
    {
        _textChanged = true;
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


        Data.DelayTicks = 0;
        Data.Delays = [0];
        _textChanged = true;
    }
}
