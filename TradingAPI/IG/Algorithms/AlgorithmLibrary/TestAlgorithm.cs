using IGWebApiClient;
using System;
using System.Collections.Generic;
using System.Text;


namespace TradingAPI.IG.Algorithms.AlgorithmLibrary
{
    class TestAlgorithm :  AlgorithmBase 
    {
        TestAlgorithm(string epic, ChartScale scale) : base (epic, scale)
        {

        }
       

        protected override void AlgorithmStrategy(RestAPI.Models.IgPublicApiData.ChartModel candle)
        {
            Console.WriteLine($"{candle.ChartEpic}: {candle.Bid.Open}");
        }


    }
}
