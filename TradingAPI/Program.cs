using System;
using System.Threading;
using TradingAPI.IG.Algorithms;
using TradingAPI.IG.Algorithms.AlgorithmLibrary;
using TradingAPI.IG;
using IGWebApiClient;
using System.Collections.Generic;
using System.Linq;

namespace TradingAPI
{
    class Program
    {
        static void Main(string[] args)
        {
            // logs you into both the streaming and the API system 
            AccountManager.Login();

            //delete existing candle data if its old or just wanna clean up a bit
            //FileManager.DeleteData("IX.D.SPTRD.DAILY.IP", ChartScale.FiveMinute.ToString());

            // create an algorithm, future algorithms may have more constructor parameters at some point i guess
            var SandP500SimpleCCI = new SimpleCCI("IX.D.SPTRD.DAILY.IP", ChartScale.FiveMinute); 

            // download some prior candles for the indicators to use inside the algorithm
            var PrerequisteCandles = AccountManager.GetMarketCandles(SandP500SimpleCCI.algorithmEpic, "MINUTE_5", 50);

            // serialise the prerequite candles
            FileManager.SerialiseCandles(PrerequisteCandles, "IX.D.SPTRD.DAILY.IP", "MINUTE_5");

            // setup a subscription to US500 one minute candle but DONT serialise the 1 minute candles
            AccountManager.SubscribeToCandleStream(SandP500SimpleCCI.algorithmEpic, ChartScale.OneMinute, false);

            // setup a subscription to US500 one minute candle and DO serialise the 5 minute candles
            AccountManager.SubscribeToCandleStream(SandP500SimpleCCI.algorithmEpic, ChartScale.FiveMinute, true);

            // find our subscription to the US500 one minute candle stream
            var SandP500SubcriptionOneMinute = AccountManager.GetCandleStreamUpdateBinding(SandP500SimpleCCI.algorithmEpic, ChartScale.OneMinute);

            // attach out algorithm's run method to the US500's OneMinute candle stream event
            SandP500SubcriptionOneMinute.CandleUpdate += SandP500SimpleCCI.Run;



            while (true)
            {
                // just to keep process from ending
            }
        }
    }
}
