using System.Collections.Generic;
using LogicAPI.Data;
using LogicAPI.Interfaces;
using LogicAPI.Server.Components;
using LogicAPI.WorldDataMutations;
using LogicWorld.Server;
using LogicWorld.Server.Managers;

namespace SilentsMsxCpu.Server.LogicCode;

// Validates the button-click request and re-emits it as a WorldMutation_SendComponentAction.
// MsxCpuBuilderActionHandler then reacts once that mutation is actually applied.
public class MsxCpuBuilderBuildRequestHandler : ComponentActionBuildRequestHandler
{
    public static void init()
    {
        var manager = Program.Get<ComponentActionBuildRequestManager>();
        manager.RegisterHandler(generator, "SilentsMsxCpu.MsxCpuBuilder");
    }

    private static ComponentActionBuildRequestHandler generator(bool prepareUndoRequests, ComponentAddress componentAddress, IComponentInWorld componentInWorld, byte[] actionData) =>
        new MsxCpuBuilderBuildRequestHandler(componentAddress, actionData);

    private readonly ComponentAddress componentAddress;
    private readonly byte[] actionData;

    public MsxCpuBuilderBuildRequestHandler(ComponentAddress componentAddress, byte[] actionData)
    {
        this.componentAddress = componentAddress;
        this.actionData = actionData;
    }

    public override bool CanDo() => actionData.Length == 1;

    public override IEnumerable<WorldDataMutation> EnumerateMutationUpdates()
    {
        yield return new WorldMutation_SendComponentAction
        {
            AddressOfTargetComponent = componentAddress,
            ActionData = actionData,
        };
    }
}
