using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pollux.Domain.Helper
{
    public static class EddystoneHelper
    {
        public static string HexToUrlPrefix(this string eddystoneHexByte)
        {
            return eddystoneHexByte switch
            {
                "00" => "http://www.",
                "01" => "https://www.",
                "02" => "http://",
                "03" => "https://",
                _ => null
            };
        }

        public static string HexToUrlData(this IEnumerable<string> eddystoneHexBytes)
        {
            var url = string.Empty;

            eddystoneHexBytes.ToList().ForEach(hexValue => {
                url += hexValue switch
                {
                    "00" => ".com/",
                    "01" => ".org/",
                    "02" => ".edu/",
                    "03" => ".net/",
                    "04" => ".info/",
                    "05" => ".biz/",
                    "06" => ".gov/",
                    "07" => ".com",
                    "08" => ".org",
                    "09" => ".edu",
                    "0a" => ".net",
                    "0b" => ".info",
                    "0c" => ".biz",
                    "0d" => ".gov",
                    _ => (char)Convert.ToInt32(hexValue, 16)
                };
            });
            return url;
        }

        public static string HexToDbAt0(this string eddystoneHexByte)
        {
            var converted = (int)(byte)Convert.ToInt32(eddystoneHexByte, 16);
            var db = converted > 127 ? (converted - 256) : converted;
            return db.ToString(); 
        }

        public static string HexToTemperature(this string[] eddystoneHexBytes)
        {
            var integerPart = Convert.ToInt32(eddystoneHexBytes[0], 16);
            var decimalPart = (double) Convert.ToInt32(eddystoneHexBytes[1], 16) / 2.56;
            return integerPart + "," + Math.Round(decimalPart);
        }
    }
}