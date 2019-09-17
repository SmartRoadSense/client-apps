using System;
using System.Globalization;

namespace SmartRoadSense
{
    public interface ILocalize
    {
        CultureInfo GetCurrentCultureInfo();
        void SetLocale(CultureInfo ci);
    }
}