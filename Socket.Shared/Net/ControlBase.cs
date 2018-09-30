using System.Diagnostics;

namespace Socket.Shared.Net
{
    public class ControlBase
    {
        public byte STX { get { return 0x02; } }
        public byte ETX { get { return 0x03; } }

        public ControlBase() { }

        /// <summary>
        /// Calculate the CRC of the data stream
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public byte GetCheckDigit(byte[] data)
        {
            byte cd = 0x00;
            for (int i = 0; i < data.Length - 1; i++) { cd += data[i]; }
            Debug.Print($"GetCheckDigit {cd.ToString()} , {(0 - cd)}, {((0 - cd) & 0x7f)}");
            return (byte)((0 - cd) & 0x7f);
        }
        /// <summary>
        /// Calc CRC and also add STX and ETX to the calculation
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public byte CalcCRC(byte[] data)
        {
            byte cd = 0x00;
            cd += STX;
            for (int i = 0; i < data.Length ; i++) { cd += data[i]; }
            cd += ETX;
            Debug.Print($"CalcCRC {cd.ToString()} , {(0 - cd)}, {((0 - cd) & 0x7f)}");
            return (byte)((0 - cd) & 0x7f);
        }
    }
}
