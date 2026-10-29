using LogicAPI.Data;
using LogicAPI.Interfaces;
using LogicWorld.SharedCode.Components;
using SilentsMsxCpu.Server.Build;
using UnityEngine;

namespace SilentsMsxCpu.Server.LogicCode;

// Runs as part of the client->server component-action network handling, not
// inside LogicManager.DoLogicUpdate() - the safe place to bulk-mutate the world.
public class MsxCpuBuilderActionHandler : IComponentActionMutationHandler
{
    public static void init()
    {
        ComponentActionMutationManager.RegisterHandler(new MsxCpuBuilderActionHandler(), "SilentsMsxCpu.MsxCpuBuilder");
    }

    public void HandleComponentAction(ComponentAddress componentAddress, IComponentInWorld componentInWorld, byte[] actionData)
    {
        var builder = new CircuitBuilder();
        var origin = componentInWorld.Data.LocalPosition + new Vector3(0, 3, 0);
        AdderTestRig.Build(builder, origin);
    }
}
