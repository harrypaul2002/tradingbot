using System;
using System.Collections.Generic;
using System.Text;
using Skender.Stock.Indicators;
using System.Linq;
using dto.endpoint.positions.create.otc.v2;
using IGWebApiClient;
using TradingAPI.IG.Queries;

namespace TradingAPI.IG.Algorithms.AlgorithmLibrary
{
    class SimpleCCI : AlgorithmBase
    {

        public SimpleCCI(string epic, ChartScale chartScale) : base (epic, chartScale)
        {

        }

        protected override void AlgorithmStrategy(RestAPI.Models.IgPublicApiData.ChartModel candle)
        {
            Console.WriteLine($"{candle.ChartEpic} time: {candle.UpdateTime}"); // output market name and time every time incoming candle comes through - just to see i can see the stream hasnt died (happens sometimes)


            var inProgressCandle = FileManager.ConvertStreamToPriceSnapshot(candle); // stream candle is a different object to API candle if i recall so do some conversion

            var candles = FileManager.GetCandles(algorithmEpic, algorithmChartScale.ToString()); // get candles currently downloaded 

            candles.Add(inProgressCandle); // add the streamed candle thats just come through to our list of predownloaded candles
            
            //////////////////// BUY //////////////////

            var bidCandles = FileManager.ConvertDataSet(candles, FileManager.TradeType.Bid); // get the BID prices
            var bidCCI = Indicator.GetCci(bidCandles); // get the CCI values

            // Check we have been above 100 CCI recently
            bool HasCCIBeenAbove100Recently_BID = QueryLibrary.HasCCIGoneAboveValueWithinCandleCount(bidCandles, 20, 100, 10); // has the 20 cci been above 100 in the last 10 candles

            // Check we have been just been below 0 CCI
            bool HasCCIBeenBelow0Recently_BID = QueryLibrary.HasCCIGoneBelowValueWithinCandleCount(bidCandles, 20, 0, 5); // has the 20 cci been below 0 in the last 5 candles

            // Check we are now above 0 CCI
            bool IsCCINowAbove0_BID = bidCCI.Last().Cci > 0; // is the cci now above 0
            Console.WriteLine(bidCCI.Last().Cci);

            bool HaveCandlesClosedAbove50EMA_BID = candles.Last().closePrice.bid > Indicator.GetEma(bidCandles, 50).Last().Ema; // is close price of last candle greater than 50EMA

            if (IsCCINowAbove0_BID && HasCCIBeenBelow0Recently_BID && HasCCIBeenAbove100Recently_BID && HaveCandlesClosedAbove50EMA_BID)
            {

                Console.WriteLine("SimpleCCI Algorithm : BUY");
                Alert();
            }


            ////////////////// SELL ///////////////////
            var askCandles = FileManager.ConvertDataSet(candles, FileManager.TradeType.Ask);

            var askCCI = Indicator.GetCci(askCandles);
            var askEMA50 = Indicator.GetEma(askCandles, 50);

            // Check we have been below 100 CCI recently
            bool HasCCIBeenBelow100Recently_Ask = QueryLibrary.HasCCIGoneBelowValueWithinCandleCount(askCandles, 20, -100, 15);

            // Check we have been just been above 0 CCI
            bool HasCCIBeenAbove0Recently_Ask = QueryLibrary.HasCCIGoneAboveValueWithinCandleCount(askCandles, 20, 0, 1);

            // Check we are now below 0 CCI
            bool IsCCINowBelow0_Ask = askCCI.Last().Cci < 0;
            Console.WriteLine(askCCI.Last().Cci);

            bool HaveCandlesClosedAbove50EMA_Ask = candles.Last().closePrice.ask < Indicator.GetEma(bidCandles, 50).Last().Ema;


            if (IsCCINowBelow0_Ask && HasCCIBeenAbove0Recently_Ask && HasCCIBeenBelow100Recently_Ask && HaveCandlesClosedAbove50EMA_Ask)
            {

                Console.WriteLine("SimpleCCI Algorithm : SELL");
                Alert();

               
            }
        }
    }
}
