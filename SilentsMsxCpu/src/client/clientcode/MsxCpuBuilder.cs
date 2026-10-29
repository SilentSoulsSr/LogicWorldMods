using System.Collections.Generic;
using JimmysUnityUtilities;
using LogicAPI.Data.BuildingRequests;
using LogicWorld.BuildingManagement;
using LogicWorld.ClientCode;
using LogicWorld.ClientCode.Decorations;
using LogicWorld.Interfaces;
using LogicWorld.References;
using LogicWorld.Rendering.Chunks;
using LogicWorld.Rendering.Components;
using UnityEngine;
using Object = UnityEngine.Object;

namespace SilentsMsxCpu.Client.ClientCode;

// Mirrors ComponentActionExampleMod's SimpleButton (a known-working pressable
// button) since this is the one part of the mod that can't be tested without
// running the game.
public class MsxCpuBuilder : ComponentClientCode, IPressableButton
{
    private static readonly Color24 buttonColor = new Color24(200, 60, 60);

    private bool wasPressed;

    protected override void Initialize()
    {
    }

    protected override IDecoration[] GenerateDecorations(Transform parentToCreateDecorationsUnder)
    {
        var gameObject = new GameObject("MsxCpuBuilderPressable");
        var meshFilter = gameObject.AddComponent<MeshFilter>();
        meshFilter.mesh = Object.Instantiate(Meshes.BetterCube_OpenBottom);
        meshFilter.transform.localScale = new Vector3(0.2f, 0.06f, 0.2f);
        gameObject.AddComponent<MeshRenderer>().material = MaterialsCache.WorldObject(buttonColor);
        var col = gameObject.AddComponent<BoxCollider>();
        col.center = new Vector3(0, 0.5f, 0);
        col.size = new Vector3(1f, 1f, 1f);
        gameObject.AddComponent<ButtonInteractable>().Button = this;
        gameObject.transform.SetParent(parentToCreateDecorationsUnder);

        OutlineWhenInteractableLookedAt = new MeshFilter[] { meshFilter };

        return new IDecoration[]
        {
            new Decoration
            {
                DecorationObject = gameObject,
                LocalPosition = new Vector3(0, 0.3f, 0),
                LocalRotation = Quaternion.identity,
                IncludeInModels = true,
                AutoSetupColliders = true,
            },
        };
    }

    // LW does not debounce mouse-down, so a single click must be debounced manually.
    public void MousePressDown()
    {
        if (wasPressed)
            return;
        wasPressed = true;
        BuildRequestManager.SendBuildRequestWithoutAddingToUndoStack(new BuildRequest_SendComponentAction(Address, new byte[] { 1 }));
    }

    public void MousePressUp()
    {
        wasPressed = false;
    }

    public IReadOnlyList<MeshFilter> OutlineWhenInteractableLookedAt { get; private set; }
}
