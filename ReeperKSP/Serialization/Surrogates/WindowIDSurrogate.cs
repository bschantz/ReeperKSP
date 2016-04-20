using System.Globalization;
using ReeperKSP.Gui;

namespace ReeperKSP.Serialization.Surrogates
{
// ReSharper disable once UnusedMember.Global
// ReSharper disable once InconsistentNaming
    public class WindowIDSurrogate : SingleValueSurrogate<WindowID>
    {
        protected override WindowID GetFieldContentsFromString(string value)
        {
            int result;

            if (string.IsNullOrEmpty(value) || !int.TryParse(value, out result))
                return new WindowID();

            return new WindowID(result);
        }


        protected override string GetFieldContentsAsString(WindowID instance)
        {
            return instance.Value.ToString(CultureInfo.InvariantCulture);
        }
    }
}
