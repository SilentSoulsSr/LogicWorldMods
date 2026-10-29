using LogicAPI.Server.Components;

namespace SilentsMsxCpu.Server.LogicCode;

// No pegs, no custom data - exists only so MsxCpuBuilderActionHandler has a
// component to look up when the button is clicked.
public class MsxCpuBuilder : LogicComponent
{
    protected override void DoLogicUpdate()
    {
    }
}
