using LogicAPI.Data;
using UnityEngine;

namespace SilentsMsxCpu.Server.Build;

// Two 8-bit registers (A, B) feeding an 8-bit adder, with switches for data,
// a shared load line, and DebugDisplay readouts for Sum and Cout.
public static class AdderTestRig
{
    public static void Build(CircuitBuilder builder, Vector3 origin)
    {
        var switchesA = PlaceSwitchRow(builder, origin + new Vector3(0, 0, -4));
        var switchesB = PlaceSwitchRow(builder, origin + new Vector3(0, 0, -6));
        var loadSwitch = builder.PlaceComponent(GateTypes.Switch, inputCount: 0, outputCount: 1, origin + new Vector3(-2, 0, -5), Quaternion.identity);

        var regA = Register8.Build(builder, origin + new Vector3(0, 0, -2));
        var regB = Register8.Build(builder, origin + new Vector3(0, 0, 0));

        regA.ConnectDataInputs(builder, switchesA);
        regA.ConnectEnable(builder, loadSwitch);
        regB.ConnectDataInputs(builder, switchesB);
        regB.ConnectEnable(builder, loadSwitch);

        var aOutputs = new PegRef[8];
        var bOutputs = new PegRef[8];
        for (var i = 0; i < 8; i++)
        {
            aOutputs[i] = new PegRef(regA.OutputComponent(i), Register8.OutputPeg);
            bOutputs[i] = new PegRef(regB.OutputComponent(i), Register8.OutputPeg);
        }

        var (sums, cout) = Adder8.Build(builder, origin + new Vector3(0, 0, 4), aOutputs, bOutputs);

        var sumDisplay = builder.PlaceComponent(GateTypes.DebugDisplay, inputCount: 8, outputCount: 0, origin + new Vector3(0, 0, 8), Quaternion.identity);
        for (var i = 0; i < 8; i++)
            builder.Wire(sums[i], new PegRef(sumDisplay, i));

        var coutDisplay = builder.PlaceComponent(GateTypes.DebugDisplay, inputCount: 8, outputCount: 0, origin + new Vector3(4, 0, 8), Quaternion.identity);
        builder.Wire(cout, new PegRef(coutDisplay, 0));
    }

    private static ComponentAddress[] PlaceSwitchRow(CircuitBuilder builder, Vector3 origin)
    {
        var addresses = new ComponentAddress[8];
        for (var i = 0; i < 8; i++)
            addresses[i] = builder.PlaceComponent(GateTypes.Switch, inputCount: 0, outputCount: 1, origin + new Vector3(i * 2f, 0, 0), Quaternion.identity);
        return addresses;
    }
}
