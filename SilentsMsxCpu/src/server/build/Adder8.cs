using UnityEngine;

namespace SilentsMsxCpu.Server.Build;

// 8-bit ripple-carry adder: chains 8 FullAdder1 blocks, bit 0's carry-in
// left unwired (reads as 0).
public static class Adder8
{
    public const float BitSpacing = 9f;

    public static (PegRef[] sum, PegRef cout) Build(CircuitBuilder builder, Vector3 origin, PegRef[] a, PegRef[] b)
    {
        var sums = new PegRef[8];
        PegRef? carry = null;
        for (var i = 0; i < 8; i++)
        {
            var bitOrigin = origin + new Vector3(i * BitSpacing, 0, 0);
            var (sum, cout) = FullAdder1.Build(builder, bitOrigin, a[i], b[i], carry);
            sums[i] = sum;
            carry = cout;
        }
        return (sums, carry!.Value);
    }
}
