using LogicAPI.Data;

namespace SilentsMsxCpu.Server.Build;

public readonly struct PegRef
{
    public readonly ComponentAddress Component;
    public readonly int Peg;

    public PegRef(ComponentAddress component, int peg)
    {
        Component = component;
        Peg = peg;
    }
}
