using LogicAPI.Server;
using SilentsMsxCpu.Server.LogicCode;

namespace SilentsMsxCpu.Server;

public class SilentsMsxCpu_ServerMod : ServerMod
{
    protected override void Initialize()
    {
        MsxCpuBuilderBuildRequestHandler.init();
        MsxCpuBuilderActionHandler.init();
        Logger.Info("SilentsMsxCpu Initialized");
    }
}
