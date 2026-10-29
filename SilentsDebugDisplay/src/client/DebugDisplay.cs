using JimmysUnityUtilities;
using LogicWorld.ClientCode;
using LogicWorld.ClientCode.LabelAlignment;
using LogicWorld.Interfaces;
using LogicWorld.References;
using LogicWorld.Rendering.Chunks;
using LogicWorld.Rendering.Components;
using UnityEngine;

namespace SilentsDebugDisplay.Client.ClientCode;

public class DebugDisplay : ComponentClientCode<DebugDisplay.IData>, IColorableClientCode, IComponentClientCode
{
    public interface IData : Label.IData
    {
        byte Value { get; set; }
    }

    private int _value;
    private LabelTextManager _label;
    private RectTransform _labelTransform;

    private float Height => base.CodeInfoFloats[0];

    private Color24 textColor = new Color24(38, 38, 38);

    string IColorableClientCode.ColorsFileKey => "LabelText";
    float IColorableClientCode.MinColorValue => 0f;

    public int SizeX
    {
        get { return Data.SizeX; }
        set { Data.SizeX = value; }
    }

    public int SizeZ
    {
        get { return Data.SizeZ; }
        set { Data.SizeZ = value; }
    }

    Color24 IColorableClientCode.Color
    {
        get { return Data.LabelColor; }
        set { Data.LabelColor = value; }
    }

    protected override void DataUpdate()
    {
        if (_value != Data.Value)
        {
            _value = Data.Value;
            Data.LabelText = $"{Data.Value.ToString("X").PadLeft(2, '0')}\n{Data.Value}\n{(sbyte)Data.Value}";
        }
        _label.DataUpdate(base.Data);
    }

    protected override void SetDataDefaultValues()
    {
        Data.LabelText = "00\n0\n0";
        Data.LabelFontSizeMax = 2.0f;
        Data.LabelColor = textColor;
        Data.LabelMonospace = false;
        Data.HorizontalAlignment = LabelAlignmentHorizontal.Left;
        Data.VerticalAlignment = LabelAlignmentVertical.Top;
        Data.SizeX = 2;
        Data.SizeZ = 3;
    }

    protected override IDecoration[] GenerateDecorations(Transform parentToCreateDecorationsUnder)
    {
        GameObject gameObject = Object.Instantiate(Prefabs.ComponentDecorations.LabelText, parentToCreateDecorationsUnder);
        _label = gameObject.GetComponent<LabelTextManager>();
        _labelTransform = _label.GetRectTransform();
        _labelTransform.sizeDelta = new Vector2(2, 3) * 0.3f;

        return
        [
            new Decoration
            {
                LocalPosition = new Vector3(-0.5f, Height + 0.001f, -0.5f) * 0.3f,
                LocalRotation = Quaternion.Euler(90f, 0f, 0f),
                DecorationObject = gameObject,
                IncludeInModels = true
            }
        ];
    }
}
