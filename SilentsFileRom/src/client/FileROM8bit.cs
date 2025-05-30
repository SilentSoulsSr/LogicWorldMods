﻿using System.Collections.Generic;
using JimmysUnityUtilities;
using LogicWorld.Audio;
using LogicWorld.ClientCode;
using LogicWorld.ClientCode.Decorations;
using LogicWorld.ClientCode.LabelAlignment;
using LogicWorld.Interfaces;
using LogicWorld.References;
using LogicWorld.Rendering.Chunks;
using LogicWorld.Rendering.Components;
using SilentsMod.Shared;
using UnityEngine;

namespace SilentsFileRom.Client.ClientCode;

public class FileROM8bit : ComponentClientCode<FileROM8bit.IData>, IColorableClientCode, IPressableButton, IComponentClientCode
{
    public interface IData : Label.IData
    {
        bool ZButtonDown { get; set; }
        byte[] Zdata { get; set; }
    }

    // Button
    private static readonly Vector3 Down = new Vector3(0f, -0.045f, 0f);
    protected Vector3 UpLocalPosition;
    protected Vector3 DownLocalPosition => UpLocalPosition + Down;
    protected MeshRenderer VisualButton;
    protected BoxCollider flatCollider;
    protected BoxCollider buttonShapeCollider;
    private bool? previousDown;
    private bool firstDataUpdateOver;
    private static Color24 ButtonColorD = new Color24(255, 0, 0);
    private static Color24 ButtonColorC = new Color24(0, 255, 0);
    private bool IsCompiled = false;

    public void MousePressDown()
    {
        if (previousDown == false)
        {
            string message = FileLoader.LoadFromFile(Data.LabelText, Data.Zdata);
            if (!string.IsNullOrEmpty(message))
                Logger.Info(message);
            else
                Logger.Info("Loaded file.");
            Data.ZButtonDown = true;
        }
    }

    public void MousePressUp()
    {
        Data.ZButtonDown = false;
    }

    // Text
    private static Color24 DefaultColor = new Color24(38, 38, 38);

    private LabelTextManager TextManager;
    private RectTransform TextManagerRT;

    public int SizeX
    {
        get
        {
            return base.Data.SizeX;
        }
        set
        {
            base.Data.SizeX = value;
        }
    }

    public int SizeZ
    {
        get
        {
            return base.Data.SizeZ;
        }
        set
        {
            base.Data.SizeZ = value;
        }
    }

    Color24 IColorableClientCode.Color
    {
        get
        {
            return base.Data.LabelColor;
        }
        set
        {
            base.Data.LabelColor = value;
        }
    }

    private float Height => base.CodeInfoFloats[0];

    string IColorableClientCode.ColorsFileKey => "LabelText";

    float IColorableClientCode.MinColorValue => 0f;

    public IReadOnlyList<MeshFilter> OutlineWhenInteractableLookedAt { get; private set; }

    protected override void DataUpdate()
    {
        TextManager.DataUpdate(base.Data);
        if (TextManagerRT != null)
        {
            TextManagerRT.sizeDelta = new Vector2(8, 1) * 0.3f;
        }
        IsCompiled = false;
        UpdateButtonMaterial();
        if (base.PlacedInMainWorld && previousDown != Data.ZButtonDown)
        {
            if (firstDataUpdateOver)
            {
                SoundPlayer.PlaySoundAt(Data.ZButtonDown ? Sounds.ButtonDown : Sounds.ButtonUp, base.Address);
            }
            Vector3 newLocalPosition = (Data.ZButtonDown ? DownLocalPosition : UpLocalPosition);
            TweenDecorationPosition(1, newLocalPosition, 0.04f);
            previousDown = Data.ZButtonDown;
            firstDataUpdateOver = true;
        }
    }

    private void UpdateButtonMaterial()
    {
        if (IsCompiled)
            VisualButton.material = WorldRenderer.MaterialsSource.SolidColor(ButtonColorC);
        else
            VisualButton.material = WorldRenderer.MaterialsSource.SolidColor(ButtonColorD);
    }

    // Button
    protected override void OnComponentReRendered()
    {
        Data.ZButtonDown = false;
    }

    protected override void SetDataDefaultValues()
    {
        base.Data.LabelText = "Filename here";
        base.Data.LabelFontSizeMax = 0.8f;
        base.Data.LabelColor = DefaultColor;
        base.Data.LabelMonospace = false;
        base.Data.HorizontalAlignment = LabelAlignmentHorizontal.Center;
        base.Data.VerticalAlignment = LabelAlignmentVertical.Middle;
        base.Data.SizeX = 8;
        base.Data.SizeZ = 2;
        base.Data.ZButtonDown = false;
        base.Data.Zdata = new byte[65536];
    }

    protected override IDecoration[] GenerateDecorations(Transform parentToCreateDecorationsUnder)
    {
        // Text
        GameObject gameObject = Object.Instantiate(Prefabs.ComponentDecorations.LabelText, parentToCreateDecorationsUnder);
        TextManager = gameObject.GetComponent<LabelTextManager>();
        TextManagerRT = TextManager.GetRectTransform();
        TextManagerRT.sizeDelta = new Vector2(8, 1) * 0.3f;

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

        return new Decoration[3]
        {
        new Decoration
        {
            LocalPosition = new Vector3(-0.5f, Height + 0.01f, 0.5f) * 0.3f,
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
        };
    }
}
