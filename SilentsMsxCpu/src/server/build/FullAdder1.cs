using UnityEngine;

namespace SilentsMsxCpu.Server.Build;

// Full adder built only from AND + XOR (both vanilla gates), using the
// identity OR(x,y) = XOR(XOR(x,y), AND(x,y)) since there is no native OR gate.
// Sum = A xor B xor Cin. Cout = OR(A&B, Cin&(A xor B)).
public static class FullAdder1
{
    // cin == null means "constant 0" (bit 0 of a chain): the two pegs that would
    // receive it are simply left unwired, which the simulation reads as off.
    public static (PegRef sum, PegRef cout) Build(CircuitBuilder builder, Vector3 origin, PegRef a, PegRef b, PegRef? cin)
    {
        var xorAB = builder.PlaceXor(origin + new Vector3(0, 0, 0), Quaternion.identity);
        builder.Wire(a, new PegRef(xorAB, 0));
        builder.Wire(b, new PegRef(xorAB, 1));

        var sumGate = builder.PlaceXor(origin + new Vector3(2, 0, 0), Quaternion.identity);
        builder.Wire(new PegRef(xorAB, 0), new PegRef(sumGate, 0));
        if (cin is { } cinSum)
            builder.Wire(cinSum, new PegRef(sumGate, 1));

        var andAB = builder.PlaceAnd(origin + new Vector3(0, 0, 2), Quaternion.identity);
        builder.Wire(a, new PegRef(andAB, 0));
        builder.Wire(b, new PegRef(andAB, 1));

        var andCinXor = builder.PlaceAnd(origin + new Vector3(2, 0, 2), Quaternion.identity);
        if (cin is { } cinAnd)
            builder.Wire(cinAnd, new PegRef(andCinXor, 0));
        builder.Wire(new PegRef(xorAB, 0), new PegRef(andCinXor, 1));

        var xorCarryTerms = builder.PlaceXor(origin + new Vector3(4, 0, 0), Quaternion.identity);
        builder.Wire(new PegRef(andAB, 0), new PegRef(xorCarryTerms, 0));
        builder.Wire(new PegRef(andCinXor, 0), new PegRef(xorCarryTerms, 1));

        var andCarryTerms = builder.PlaceAnd(origin + new Vector3(4, 0, 2), Quaternion.identity);
        builder.Wire(new PegRef(andAB, 0), new PegRef(andCarryTerms, 0));
        builder.Wire(new PegRef(andCinXor, 0), new PegRef(andCarryTerms, 1));

        var coutGate = builder.PlaceXor(origin + new Vector3(6, 0, 0), Quaternion.identity);
        builder.Wire(new PegRef(xorCarryTerms, 0), new PegRef(coutGate, 0));
        builder.Wire(new PegRef(andCarryTerms, 0), new PegRef(coutGate, 1));

        return (new PegRef(sumGate, 0), new PegRef(coutGate, 0));
    }
}
