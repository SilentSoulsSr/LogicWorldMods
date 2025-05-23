using JimmysUnityUtilities;
using LogicAPI.Server.Components;
using LogicWorld.Server.Circuitry;
using System;

namespace SilentMemory.Server.LogicCode
{
    public class FileROM8bit : LogicComponent<FileROM8bit.IData>
    {
        public interface IData
        {
            string LabelText { get; set; }
            Color24 LabelColor { get; set; }
            bool LabelMonospace { get; set; }
            float LabelFontSizeMax { get; set; }
            int HorizontalAlignment { get; set; }
            int VerticalAlignment { get; set; }
            int SizeX { get; set; }
            int SizeZ { get; set; }
            bool ZButtonDown { get; set; }
            byte[] Zdata { get; set; }
        }
        private static Color24 DefaultColor = new Color24(38, 38, 38);

        protected override void DoLogicUpdate()
        {
            int address = 0;
            for (int i = 0; i < 16; i++)
                address += Inputs[i].On ? 1 << i : 0;

            byte output = 0;
            if (ComponentData.CustomData != null)
                output = Data.Zdata[address];

            for (int i = 0; i < 8; i++)
                Outputs[i].On = (output & (1 << i)) > 0;
        }

        protected override void OnCustomDataUpdated()
        {
            QueueLogicUpdate();
        }

        protected override void SetDataDefaultValues()
        {
            base.Data.LabelText = "Filename here";
            base.Data.LabelFontSizeMax = 0.8f;
            base.Data.LabelColor = DefaultColor;
            base.Data.LabelMonospace = false;
            base.Data.HorizontalAlignment = 1;
            base.Data.VerticalAlignment = 1;
            base.Data.SizeX = 8;
            base.Data.SizeZ = 2;
            base.Data.ZButtonDown = false;
            base.Data.Zdata = new byte[65536];
        }
    }
}