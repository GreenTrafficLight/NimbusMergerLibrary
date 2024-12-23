using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NimbusMergerLibrary.Tests.NimbusScript.Constants
{
    public static class GameAircraftShortNames
    {
        public static string GetGameAircraftShortName(string planeStringID)
        {
            return planeStringID switch
            {
                "f04e" => "F-4E",
                "f15c" => "F-15C",
                "f15e" => "F-15E",
                "f18f" => "F/A-18F",
                "f15j" => "F-15J",
                "f35c" => "F-35C",
                "a10a" => "A-10C",
                "f02a" => "F-2A",
                "f14d" => "F-14D",
                "f16c" => "F-16C",
                "f22a" => "F-22A",
                "j39e" => "Gripen E",
                "m21b" => "MiG-21bis",
                "m29a" => "MiG-29A",
                "m31b" => "MiG-31B",
                "mr2k" => "Mirage 2000-5",
                "pkfa" => "Su-57",
                "rflm" => "Rafale M",
                "su33" => "Su-33",
                "su34" => "Su-34",
                "su35" => "Su-35S",
                "su37" => "Su-37",
                "su47" => "Su-47",
                "typn" => "Typhoon",
                "yf23" => "YF-23",
                "su30" => "Su-30M2",
                "adf11f" => "ADF-11F",
                "x02s" => "X-02S",
                "su30sm" => "Su-30SM",
                "f104" => "F-104C",
                "zoef" => "ADF-01",
                "mrgn" => "ADFX-01",
                "dark" => "DarkStar",
                "f18e" => "F/A-18E",
                "f14a" => "F-14A",
                "f18etg" => "F/A-18E | TGM",
                "f14atg" => "F-14A | TGM",
                "su57tg" => "5th Gen Fighter | TGM",
                "asfx" => "ASF-X",
                "fa27" => "XFA-27",
                "fa44" => "CFA-44",
                "f15m" => "F-15 S/MTD",
                "f16x" => "F-16XL",
                "fb22" => "FB-22",
                "f18x" => "F/A-18F Block III",
                "f02x" => "F-2A SK",
                "m35d" => "MiG-35D",
                "f18f_vr" => "F/A-18F",
                "a10a_vr" => "A-10C",
                "f22a_vr" => "F-22A",
                "su30_vr" => "Su-30M2",
                "f104av" => "F-104C -AV-"
            };
        }
    }
}
