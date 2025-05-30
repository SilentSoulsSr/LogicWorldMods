using JimmysUnityUtilities;
using LogicWorld.ClientCode;
using LogicWorld.ClientCode.LabelAlignment;
using LogicWorld.Interfaces;
using LogicWorld.References;
using LogicWorld.Rendering.Chunks;
using LogicWorld.Rendering.Components;
using UnityEngine;

namespace SilentsFileRom.Client.ClientCode;

public class FileROM8bit : ComponentClientCode<FileROM8bit.IData>, IColorableClientCode, IComponentClientCode
{
    public interface IData : Label.IData
    {
        byte[] FileData { get; set; }
    }

    private LabelTextManager _label;
    private RectTransform _labelTransform;
    private float Height => base.CodeInfoFloats[0];
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
        _label.DataUpdate(base.Data);
        if (_labelTransform != null)
            _labelTransform.sizeDelta = new Vector2(8, 2) * 0.3f;
    }

    protected override void SetDataDefaultValues()
    {
        Data.LabelText = "Filename here";
        Data.LabelFontSizeMax = 0.8f;
        Data.LabelColor = new Color24(38, 38, 38);
        Data.LabelMonospace = false;
        Data.HorizontalAlignment = LabelAlignmentHorizontal.Center;
        Data.VerticalAlignment = LabelAlignmentVertical.Middle;
        Data.SizeX = 8;
        Data.SizeZ = 2;
        Data.FileData = new byte[0];
    }

    protected override IDecoration[] GenerateDecorations(Transform parentToCreateDecorationsUnder)
    {
        GameObject gameObject = Object.Instantiate(Prefabs.ComponentDecorations.LabelText, parentToCreateDecorationsUnder);
        _label = gameObject.GetComponent<LabelTextManager>();
        _labelTransform = _label.GetRectTransform();
        _labelTransform.sizeDelta = new Vector2(8, 2) * 0.3f;

        return
        [
            new Decoration
            {
                LocalPosition = new Vector3(-0.5f, Height + 0.01f, -0.5f) * 0.3f,
                LocalRotation = Quaternion.Euler(90f, 0f, 0f),
                DecorationObject = gameObject,
                IncludeInModels = true
            }
        ];
    }
}
