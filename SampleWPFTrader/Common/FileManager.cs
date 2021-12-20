using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using dto.endpoint.prices.v2;

namespace SampleWPFTrader.Common
{
    public static class FileManager
    {

        


        public static readonly string CandleRoot = $@"{Directory.GetCurrentDirectory()}\CandleData";

        // Check if the candle folder for the market epic and time exists
        private static void EnsureFolderExists(string epicName, string timeResolution)
        {
            
            try
            {
                // ensure candle data root exists folder name exists
                if (!Directory.Exists($@"{CandleRoot}"))
                {
                    Directory.CreateDirectory($@"{CandleRoot}");
                }

                // ensure epic folder name exists
                if (!Directory.Exists($@"{CandleRoot}\{epicName}"))
                {
                    Directory.CreateDirectory($@"{CandleRoot}\{epicName}");
                }

                // ensure time resolution folder exists
                if (!Directory.Exists($@"{CandleRoot}\{epicName}\{timeResolution}"))
                {
                    Directory.CreateDirectory($@"{CandleRoot}\{epicName}\{timeResolution}");
                }

            }
            catch (Exception e)
            {
                Debug.WriteLine($"Ensure Folder Exists Failed : {e}" );
            }
        }


        /// <summary>
        /// Serialise Candle
        /// timeResolution (MINUTE, MINUTE_2, MINUTE_3, MINUTE_5, MINUTE_10, MINUTE_15, MINUTE_30, HOUR, HOUR_2, HOUR_3, HOUR_4, DAY, WEEK, MONTH)
        /// </summary>
        public static void SerialiseCandle(PriceSnapshot candle, string epicName, string timeResolution)
        {
            try
            {
                var serialiser = new XmlSerializer(typeof(PriceSnapshot));

                if (candle == null)
                {
                    return;
                }

                EnsureFolderExists(epicName, timeResolution);

                var path = $@"{CandleRoot}\{epicName}\{timeResolution}";

                var filePath = Path.Combine(path, candle.snapshotTime.Replace(":", "_").Replace(@"/", "-") + ".xml");

                using (var writer = new StreamWriter(filePath))
                {
                    serialiser.Serialize(writer, candle);
                }
                

            }
            catch (Exception e)
            {
                Debug.WriteLine($"Serialise Candle Failed : {e}");
            }
        }

        public static void SerialiseCandles(List<PriceSnapshot> candles, string epicName, string timeResolution)
        {

            try
            {
                var serialiser = new XmlSerializer(typeof(PriceSnapshot));

                if (candles == null)
                {
                    return;
                }

                EnsureFolderExists(epicName, timeResolution);

                var path = $@"{CandleRoot}\{epicName}\{timeResolution}";

                for (int i = 0; i < candles.Count; i++)
                {

                    var filePath = Path.Combine(path, candles[i].snapshotTime.Replace(":", "_").Replace(@"/", "-") + ".xml");

                    using (var writer = new StreamWriter(filePath))
                    {
                        serialiser.Serialize(writer, candles[i]);
                    }
                }


            }
            catch (Exception e)
            {
                Debug.WriteLine($"Serialise Candles Failed : {e}");
            }
        }

        public static List<PriceSnapshot> GetCandles(string epicName, string timeResolution)
        {
            try
            {
                var serialiser = new XmlSerializer(typeof(PriceSnapshot));
            

                EnsureFolderExists(epicName, timeResolution);

                var path = $@"{CandleRoot}\{epicName}\{timeResolution}";

                DirectoryInfo dir = new DirectoryInfo(path);
                List<PriceSnapshot> candles = new List<PriceSnapshot>();

                var files = dir.GetFiles("*.xml");

                for (int i = 0; i < files.Length; i++)
                {
                    XmlSerializer ser = new XmlSerializer(typeof(PriceSnapshot));
                    using (XmlReader reader = XmlReader.Create(files[i].FullName))
                    {
                        PriceSnapshot MarketData = (PriceSnapshot)ser.Deserialize(reader);
                        candles.Add(MarketData);
                    }
                }
                return candles;


            }
            catch (Exception e)
            {
                Debug.WriteLine($"Get Candles Failed : {e}");
                return null;
            }

            
        }

    }
}
