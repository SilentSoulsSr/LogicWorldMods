using LogicAPI.Data;
using LogicAPI.Services;
using LogicAPI.WorldDataMutations;
using LogicWorld.Server;
using LogicWorld.Server.Managers;
using LogicWorld.SharedCode.Components;
using UnityEngine;

namespace SilentsMsxCpu.Server.Build;

// Places components and wires directly into the running world via the same
// WorldDataMutation API the game itself uses, so builds show up immediately
// for every connected client.
public class CircuitBuilder
{
    private readonly IWorldData worldData;
    private readonly IWorldUpdates worldUpdates;
    private readonly IWorldDataMutator worldDataMutator;
    private readonly ComponentTypesManager componentTypes;

    public CircuitBuilder()
    {
        worldData = Program.Get<IWorldData>();
        worldUpdates = Program.Get<IWorldUpdates>();
        worldDataMutator = Program.Get<IWorldDataMutator>();
        componentTypes = Program.Get<ComponentTypesManager>();
    }

    private void Apply(WorldDataMutation mutation)
    {
        worldUpdates.QueueMutationToBeSentToClient(mutation);
        mutation.ApplyMutation(worldDataMutator);
    }

    // ComponentData's constructor leaves InputInfos/OutputInfos null (InputCount/
    // OutputCount = 0), so without this, CircuitryManager never registers any pegs
    // for the component at all - it silently exists with zero working pegs. Only
    // the array LENGTH matters here: real StateIDs get allocated and broadcast
    // back separately once pegs are initialized.
    public ComponentAddress PlaceComponent(string typeId, int inputCount, int outputCount, Vector3 position, Quaternion rotation, ComponentAddress parent = default)
    {
        var type = componentTypes.GetComponentType(typeId);
        var address = new ComponentAddress(worldDataMutator.HighestComponentAddressAddedSoFar + 1);
        var data = new ComponentData(type);
        var editable = (IEditableComponentData)data;
        editable.InputInfos = new InputInfo[inputCount];
        editable.OutputInfos = new OutputInfo[outputCount];
        editable.LocalPosition = position;
        editable.LocalRotation = rotation;
        editable.Parent = parent;
        Apply(new WorldMutation_AddSingleNewComponent
        {
            AddressOfNewComponent = address,
            NewComponent = data,
        });
        return address;
    }

    // AndGate has no static prefab peg list (dynamic), so it's created with zero
    // pegs and immediately resized to the real count via the same mutation
    // Ecconia's WireSetMutation uses for resizing.
    public ComponentAddress PlaceDynamicGate(string typeId, Vector3 position, Quaternion rotation, int inputCount, int outputCount, ComponentAddress parent = default)
    {
        var address = PlaceComponent(typeId, inputCount: 0, outputCount: 0, position, rotation, parent);
        Apply(new WorldMutation_ChangeDynamicComponentPegCounts
        {
            AddressOfTargetComponent = address,
            NewInputCount = inputCount,
            NewOutputCount = outputCount,
        });
        return address;
    }

    public ComponentAddress PlaceAnd(Vector3 position, Quaternion rotation, ComponentAddress parent = default) =>
        PlaceDynamicGate(GateTypes.And, position, rotation, inputCount: 2, outputCount: 1, parent);

    public ComponentAddress PlaceXor(Vector3 position, Quaternion rotation, ComponentAddress parent = default) =>
        PlaceComponent(GateTypes.Xor, inputCount: 2, outputCount: 1, position, rotation);

    public void Wire(PegRef fromOutput, PegRef toInput, float rotation = 0f) =>
        Wire(fromOutput.Component, fromOutput.Peg, toInput.Component, toInput.Peg, rotation);

    public void Wire(ComponentAddress fromComponent, int fromOutputPeg, ComponentAddress toComponent, int toInputPeg, float rotation = 0f)
    {
        var wireAddress = new WireAddress(worldDataMutator.HighestWireAddressAddedSoFar + 1);
        var outputPeg = new PegAddress(fromComponent, fromOutputPeg, PegType.Output);
        var inputPeg = new PegAddress(toComponent, toInputPeg, PegType.Input);
        Apply(new WorldMutation_AddNewWire
        {
            AddressOfNewWire = wireAddress,
            DataOfNewWire = new WireData(outputPeg, inputPeg, rotation),
        });
    }
}
