using LogicAPI.Data;
using UnityEngine;

namespace SilentsMsxCpu.Server.Build;

// 8-bit register: one DLatch per bit, sharing a single Enable line.
// DLatch pegs: input 0 = D, input 1 = Enable, output 0 = Q.
// BitSpacing is an unverified guess at the DLatch footprint (grid 1x3) -
// tell me if latches overlap or look too spread out in-game.
public class Register8
{
    public const float BitSpacing = 2f;

    public ComponentAddress[] Latches { get; } = new ComponentAddress[8];

    public static Register8 Build(CircuitBuilder builder, Vector3 origin, ComponentAddress parent = default)
    {
        var register = new Register8();
        for (var i = 0; i < 8; i++)
        {
            var position = origin + new Vector3(i * BitSpacing, 0, 0);
            register.Latches[i] = builder.PlaceComponent(GateTypes.DLatch, inputCount: 2, outputCount: 1, position, Quaternion.identity, parent);
        }
        return register;
    }

    public void ConnectDataInputs(CircuitBuilder builder, ComponentAddress[] dataBitSources, int sourceOutputPeg = 0)
    {
        for (var i = 0; i < 8; i++)
            builder.Wire(dataBitSources[i], sourceOutputPeg, Latches[i], 0);
    }

    public void ConnectEnable(CircuitBuilder builder, ComponentAddress enableSource, int enableOutputPeg = 0)
    {
        for (var i = 0; i < 8; i++)
            builder.Wire(enableSource, enableOutputPeg, Latches[i], 1);
    }

    public ComponentAddress OutputComponent(int bitIndex) => Latches[bitIndex];
    public const int OutputPeg = 0;
}
