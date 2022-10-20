using System.Runtime.InteropServices;
using System.Text;

namespace Meshmakers.Common.Shared.Ini;

/// <summary>
///     Kapselt das Lesen und Schreiben von Ini-Dateien
/// </summary>
public class IniFile
{
    private readonly string _sFilePath;

    /// <summary>
    ///     Constructor
    /// </summary>
    /// <param name="sFilePath"></param>
    public IniFile(string sFilePath)
    {
        _sFilePath = sFilePath;
    }

    [DllImport("kernel32")]
    private static extern long WritePrivateProfileString(string sSection,
        string sKey, string sValue, string sFilePath);

    [DllImport("kernel32")]
    private static extern int GetPrivateProfileString(string sSection,
        string sKey, string sDefault, StringBuilder sRetVal,
        int nSize, string sFilePath);

    /// <summary>
    ///     Schreibt einen Ini-Wert
    /// </summary>
    /// <param name="sSection">Name der Sektion</param>
    /// <param name="sKey">Name des Schlüssel</param>
    /// <param name="sValue">Der zu schreibende Wert</param>
    public void IniWriteValue(string sSection, string sKey, string sValue)
    {
        WritePrivateProfileString(sSection, sKey, sValue, _sFilePath);
    }

    /// <summary>
    ///     Liest einen Ini-Wert
    /// </summary>
    /// <param name="sSection">Name der Sektion</param>
    /// <param name="sKey">Name des Schlüssel</param>
    /// <returns>Der Wert des Schlüssels</returns>
    public string IniReadValue(string sSection, string sKey)
    {
        var temp = new StringBuilder(255);
        GetPrivateProfileString(sSection, sKey, "", temp, 255, _sFilePath);
        return temp.ToString();
    }
}