using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Plugin.BLE.Abstractions;
using Pollux.Domain.Helper;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Pollux.Domain.Data
{
    public class EddystoneMessage
    {
        public FrameType Type { get; set; }
        public int Rssi { get; set; }
        public string[] Payload { get; set; }
        public Dictionary<string, string> Data { get; set; }

        public EddystoneMessage(int rssi, AdvertisementRecord adv) 
        {
            Payload = BitConverter.ToString(adv.Data).Split('-');
            if (adv.Type != AdvertisementRecordType.ServiceData || Payload[0] + Payload[1] != "AAFE")
            {
                throw new ArgumentException("Not a valid Eddystone Message");
            }

            Rssi = rssi;
            try
            {
                switch (Payload[2])
                {
                    case "00":
                        Type = FrameType.Uid;
                        Data = new Dictionary<string, string>()
                        {
                            { UidDataFields.PowerAt0, Payload[3].HexToDbAt0()},
                            { UidDataFields.Namespace, string.Join(string.Empty, Payload[4..14]) },
                            { UidDataFields.Instance, string.Join(string.Empty, Payload[14..20]) }
                        };
                        break;
                    case "10":
                        Type = FrameType.Url;
                        Data = new Dictionary<string, string>()
                        {
                            { UrlDataFields.PowerAt0, Payload[3].HexToDbAt0()},
                            { UrlDataFields.UrlScheme, Payload[4].HexToUrlPrefix() },
                            { UrlDataFields.UrlData, Payload.ToList().Skip(5).HexToUrlData() }
                        };
                        break;
                    case "20":
                        if (Payload[3] == "00")
                        {
                            Type = FrameType.Tlm;
                            Data = new Dictionary<string, string>()
                            {
                                { TlmDatafields.BatteryVoltage, Convert.ToInt32(string.Join(string.Empty, Payload[4..6]), 16).ToString() },
                                { TlmDatafields.Temperature, Payload[6..8].HexToTemperature() },
                                { TlmDatafields.AdvCount, Convert.ToInt32(string.Join(string.Empty, Payload[8..12]), 16).ToString() },
                                { TlmDatafields.TimeSincePoweron, Convert.ToInt32(string.Join(string.Empty, Payload[12..16]), 16).ToString() }
                            };
                        }
                        else if (Payload[3] == "01")
                        {
                            Type = FrameType.eTlm;
                            //Encrypted Tlm currently not supported
                        }
                        break;
                    case "30":
                        Type = FrameType.Eid;
                        //Ephemeral ID currently not supported
                        break;
                    default:
                        throw new ArgumentException("Not a valid Eddystone Message");
                }
            }catch(Exception)
            {
                throw new ArgumentException("Not a valid Eddystone Message");
            }
        }

        public string getDataField(string key)
        {
            Data.TryGetValue(key, out var value);
            return value;
        }
    }

    public enum FrameType
    {
        Undefined,
        Uid,
        Url,
        Tlm,
        eTlm,
        Eid
    }

    public static class UidDataFields
    {
        public const string PowerAt0 = "PowerAt0"; //1 byte
        public const string Namespace = "Namespace"; // 10 bytes
        public const string Instance = "Instance"; //6 bytes
        public static readonly string[] All = { PowerAt0, Namespace, Instance };
    }

    public static class UrlDataFields
    {
        public const string PowerAt0 = "PowerAt0"; //1 byte
        public const string UrlScheme = "UrlScheme"; //1 byte
        public const string UrlData = "UrlData"; //max 17 bytes
        public static readonly string[] All = { PowerAt0, UrlScheme, UrlData};
    }

    public static class TlmDatafields
    {
        public const string BatteryVoltage = "BatteryVoltage"; //2 bytes
        public const string Temperature = "Temperature"; // 2 bytes
        public const string AdvCount = "AdvCount"; //4 bytes
        public const string TimeSincePoweron = "TimeSincePoweron"; //4 bytes
        public static readonly string[] All = { BatteryVoltage, Temperature, AdvCount, TimeSincePoweron };
    }
}