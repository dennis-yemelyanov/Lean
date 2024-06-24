/*
 * QUANTCONNECT.COM - Democratizing Finance, Empowering Individuals.
 * Lean Algorithmic Trading Engine v2.0. Copyright 2014 QuantConnect Corporation.
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
*/

using System;
using System.Linq;
using QuantConnect.Data;
using QuantConnect.Interfaces;
using System.Collections.Generic;
using QuantConnect.Data.UniverseSelection;

namespace QuantConnect.Algorithm.CSharp
{
    /// <summary>
    /// </summary>
    public class FileBasedOptionUniverseRegressionAlgorithm : QCAlgorithm, IRegressionAlgorithmDefinition
    {
        private List<DateTime> _tradableDates;
        private HashSet<DateTime> _optionChainsDates = new HashSet<DateTime>();

        private Symbol _option;

        public override void Initialize()
        {
            SetStartDate(2014, 06, 06);
            SetEndDate(2014, 06, 10);

            var aapl = AddEquity("AAPL");
            var option = AddOption(aapl.Symbol, Resolution.Minute);
            option.SetFilter(-10, +10, 0, 365);
            _option = option.Symbol;

            _tradableDates = QuantConnect.Time.EachTradeableDay(aapl, StartDate, EndDate).ToList();

            SetBenchmark(x => 0);

            // -------------------------------------------------------

            //var sid = contracts.First().ID.ToString();
            //var sid1 = contracts[1].ID.ToString();
            //var sid2 = contracts[2].ID.ToString();
            //var sid3 = contracts[3].ID.ToString();
            //var sid4 = contracts[4].ID.ToString();

            //var underlying = QuantConnect.Symbol.Create("AAPL", SecurityType.Equity, Market.USA);
            //var underlyingSid = underlying.ID.ToString();
            //var sidddd = SecurityIdentifier.Parse($"AAPL VXBK4Q9ZIFD2|{underlyingSid}");
            //var symbol = new QuantConnect.Symbol(sidddd, sidddd.Symbol);
        }

        public override void OnData(Slice slice)
        {
            if (slice.OptionChains.TryGetValue(_option, out var chain))
            {
                _optionChainsDates.Add(Time.Date);

                Log($"[{Time}] Received option chain for {_option} containing {chain.Contracts.Count} contracts");
            }
        }

        public override void OnEndOfAlgorithm()
        {
            foreach (var date in _tradableDates)
            {
                if (!_optionChainsDates.Contains(date))
                {
                    throw new Exception($"No option chain received on {date}");
                }
            }
        }

        /// <summary>
        /// This is used by the regression test system to indicate if the open source Lean repository has the required data to run this algorithm.
        /// </summary>
        public bool CanRunLocally { get; } = true;

        /// <summary>
        /// This is used by the regression test system to indicate which languages this algorithm is written in.
        /// </summary>
        public Language[] Languages { get; } = { Language.CSharp };

        /// <summary>
        /// Data Points count of all timeslices of algorithm
        /// </summary>
        public long DataPoints => 24;

        /// <summary>
        /// Data Points count of the algorithm history
        /// </summary>
        public int AlgorithmHistoryDataPoints => 0;

        /// <summary>
        /// This is used by the regression test system to indicate what the expected statistics are from running the algorithm
        /// </summary>
        public Dictionary<string, string> ExpectedStatistics => new Dictionary<string, string>
        {
            {"Total Orders", "0"},
            {"Average Win", "0%"},
            {"Average Loss", "0%"},
            {"Compounding Annual Return", "0%"},
            {"Drawdown", "0%"},
            {"Expectancy", "0"},
            {"Start Equity", "100000"},
            {"End Equity", "100000"},
            {"Net Profit", "0%"},
            {"Sharpe Ratio", "0"},
            {"Sortino Ratio", "0"},
            {"Probabilistic Sharpe Ratio", "0%"},
            {"Loss Rate", "0%"},
            {"Win Rate", "0%"},
            {"Profit-Loss Ratio", "0"},
            {"Alpha", "0"},
            {"Beta", "0"},
            {"Annual Standard Deviation", "0"},
            {"Annual Variance", "0"},
            {"Information Ratio", "-9.486"},
            {"Tracking Error", "0.008"},
            {"Treynor Ratio", "0"},
            {"Total Fees", "$0.00"},
            {"Estimated Strategy Capacity", "$0"},
            {"Lowest Capacity Asset", ""},
            {"Portfolio Turnover", "0%"},
            {"OrderListHash", "d41d8cd98f00b204e9800998ecf8427e"}
        };
    }
}