using JimmysUnityUtilities;
using LogicWorld.Server.Circuitry;

namespace SilentsFileRom.Server.LogicCode
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

        private bool reload = false;

        protected override void DoLogicUpdate()
        {
            if (Inputs[16].On)
            {
                if (reload != true)
                {
                    reload = true;

                    string message = LoadFromFile(Data.LabelText, Data.Zdata);
                    if (!string.IsNullOrEmpty(message))
                        Logger.Info(message);
                    else
                        Logger.Info("Loaded file. " + Data.LabelText);
                }

                for (int i = 0; i < 8; i++)
                    Outputs[i].On = false;

                return;
            }
            reload = false;

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
        private string LoadFromFile(string istr, byte[] idata)
        {
            try
            {
                string file = istr?.Trim()?.Split('\n')?[0]?.Trim();
                if (string.IsNullOrEmpty(file))
                    return "Failed to load file.";

                var data = System.IO.File.ReadAllBytes(file);

                System.Array.Resize(ref data, 65536);
                System.Array.Copy(data, idata, 65536);
            }
            catch (System.Exception ex)
            {
                return "Failed to load file" + ex.Message;
            }

            return null;
        }
    }
}
