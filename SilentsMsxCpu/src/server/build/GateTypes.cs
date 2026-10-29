namespace SilentsMsxCpu.Server.Build;

// Vanilla MHG component type IDs used to build the CPU from primitive gates.
public static class GateTypes
{
    public const string And = "MHG.AndGate";       // dynamic peg count, defaults must be set explicitly
    public const string Xor = "MHG.XorGate";        // fixed: 2 in (0,1), 1 out (0)
    public const string Not = "MHG.Inverter";       // fixed: 1 in (0), 1 out (0)
    public const string DLatch = "MHG.DLatch";      // fixed: 2 in (0=D, 1=Enable), 1 out (0=Q)
    public const string Switch = "MHG.Switch";      // fixed: 1 out (0), toggled by player
    public const string DebugDisplay = "SilentsDebugDisplay.DebugDisplay"; // fixed: 8 in (0=LSB..7=MSB), shows byte value
    public const string CircuitBoard = "MHG.CircuitBoard"; // structural only, no pegs, auto-resizes to its children
}
