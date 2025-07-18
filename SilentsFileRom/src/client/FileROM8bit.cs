using System.Collections.Generic;
using JimmysUnityUtilities;
using LogicWorld.Audio;
using LogicWorld.ClientCode;
using LogicWorld.ClientCode.Decorations;
using LogicWorld.ClientCode.LabelAlignment;
using LogicWorld.Interfaces;
using LogicWorld.References;
using LogicWorld.Rendering.Chunks;
using LogicWorld.Rendering.Components;
using UnityEngine;

namespace SilentsFileRom.Client.ClientCode;

public class FileROM8bit : ComponentClientCode<FileROM8bit.IData>, IColorableClientCode, IComponentClientCode, IPressableButton
{
    public interface IData : Label.IData
    {
        bool LoadFile { get; set; }
        bool UpdateData { get; set; }
        bool FileSuccess { get; set; }       
        byte[] FileData { get; set; }
    }

    private static readonly Vector3 Down = new Vector3(0f, -0.045f, 0f);

    private bool _isPressedDown = false;
    private bool _isPressedDownDataUpdate = false;
    private LabelTextManager _label;
    private RectTransform _labelTransform;
    private float Height => base.CodeInfoFloats[0];

    protected Vector3 UpLocalPosition;
    protected Vector3 DownLocalPosition => UpLocalPosition + Down;
    protected MeshRenderer VisualButton;
    protected BoxCollider flatCollider;
    protected BoxCollider buttonShapeCollider;

    string IColorableClientCode.ColorsFileKey => "LabelText";
    float IColorableClientCode.MinColorValue => 0f;

    public IReadOnlyList<MeshFilter> OutlineWhenInteractableLookedAt { get; private set; }

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
            _labelTransform.sizeDelta = new Vector2(8, 1) * 0.3f;

        if (Data.FileSuccess)
            VisualButton.material = WorldRenderer.MaterialsSource.SolidColor(new Color24(0, 128, 0));
        else
            VisualButton.material = WorldRenderer.MaterialsSource.SolidColor(new Color24(255, 0, 0));

        if (base.PlacedInMainWorld && _isPressedDown != _isPressedDownDataUpdate)
        {
            SoundPlayer.PlaySoundAt(_isPressedDown ? Sounds.ButtonDown : Sounds.ButtonUp, base.Address);

            Vector3 newLocalPosition = (_isPressedDown ? DownLocalPosition : UpLocalPosition);
            TweenDecorationPosition(1, newLocalPosition, 0.04f);

            _isPressedDownDataUpdate = _isPressedDown;
        }
    }

    public void MousePressDown()
    {
        if (_isPressedDown == false)
        {
            Data.LoadFile = true;
            Data.UpdateData = true;
        }

        _isPressedDown = true;
    }

    public void MousePressUp()
    {
        if (_isPressedDown == true)
            Data.UpdateData = true;

        _isPressedDown = false;
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
        Data.SizeZ = 1;
        Data.LoadFile = false;
        Data.UpdateData = false;
        Data.FileSuccess = false;
        Data.FileData = new byte[0];
    }

    protected override IDecoration[] GenerateDecorations(Transform parentToCreateDecorationsUnder)
    {
        GameObject gameObject = Object.Instantiate(Prefabs.ComponentDecorations.LabelText, parentToCreateDecorationsUnder);
        _label = gameObject.GetComponent<LabelTextManager>();
        _labelTransform = _label.GetRectTransform();
        _labelTransform.sizeDelta = new Vector2(8, 1) * 0.3f;

        // Button
        Vector3 rawBlockScale = GetRawBlockScale();
        UpLocalPosition = new Vector3(rawBlockScale.x / 2f - 0.15f, rawBlockScale.y, rawBlockScale.z / 4f - 0.15f);
        GameObject gameObject2 = Object.Instantiate(Prefabs.ComponentDecorations.ButtonVisuals, parentToCreateDecorationsUnder);
        VisualButton = gameObject2.GetComponentInChildren<MeshRenderer>();
        VisualButton.transform.localScale = new Vector3(rawBlockScale.x - 0.09f, 0.06f, rawBlockScale.z / 2 - 0.09f);
        OutlineWhenInteractableLookedAt = new MeshFilter[1] { VisualButton.GetComponent<MeshFilter>() };

        GameObject gameObject3 = Object.Instantiate(Prefabs.ComponentDecorations.ButtonColliders, parentToCreateDecorationsUnder);
        gameObject3.GetComponent<ButtonInteractable>().Button = this;
        BoxCollider[] colliders = gameObject3.GetComponents<BoxCollider>();
        flatCollider = colliders[0];
        flatCollider.size = new Vector3(rawBlockScale.x, 0.02f, rawBlockScale.z / 2);
        buttonShapeCollider = colliders[1];
        buttonShapeCollider.size = VisualButton.transform.localScale;
        buttonShapeCollider.center = new Vector3(0f, buttonShapeCollider.size.y / 2f, 0f);

        return
        [
            new Decoration
            {
                LocalPosition = new Vector3(-0.5f, Height + 0.001f, 0.5f) * 0.3f,
                LocalRotation = Quaternion.Euler(90f, 0f, 0f),
                DecorationObject = gameObject,
                IncludeInModels = true
            },
            new Decoration
            {
                LocalPosition = UpLocalPosition,
                DecorationObject = gameObject2,
                IncludeInModels = true,
                AutoSetupColliders = true
            },
            new Decoration
            {
                LocalPosition = UpLocalPosition,
                DecorationObject = gameObject3,
                AutoSetupColliders = true
            }
        ];
    }
}
