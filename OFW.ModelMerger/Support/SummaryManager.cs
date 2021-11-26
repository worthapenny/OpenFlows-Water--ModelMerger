/**
 * @ Author: Akshaya Niraula
 * @ Create Time: 2021-10-25 02:17:45
 * @ Modified by: Akshaya Niraula
 * @ Modified time: 2021-10-26 17:33:37
 * @ Copyright: Copyright (c) 2021 Akshaya Niraula See LICENSE for details
 */

using OFW.ModelMerger.Extentions;
using OpenFlows.Water.Domain;
using System;
using System.Text;

namespace OFW.ModelMerger.Support
{
    public class SummaryManager
    {
        #region Singleton/Constructor
        // Lazy is not needed, just trying it out
        private static readonly Lazy<SummaryManager>
            lazy = new Lazy<SummaryManager>(() => new SummaryManager());

        public static SummaryManager Instance => lazy.Value;
        private SummaryManager()
        {
            StringBuilder.AppendLine();
            StringBuilder.AppendLine(Stars);
            StringBuilder.AppendLine("* Model Merger Summary Report ");
            StringBuilder.AppendLine(Stars);
            StringBuilder.AppendLine();
        }
        #endregion

        #region Public Methods
        public void AddBaseModel(IWaterModel waterModel, string label = "")
        {
            ModelSummary = waterModel.ModelSummary(label);
        }
        public void AddModel(IWaterModel waterModel, string label = "")
        {
            ModelSummary.AddWaterModel(waterModel, label);
        }
        public void AddMessage(string message)
        {
            StringBuilder.AppendLine(message);
        }
        #endregion

        #region Overrides
        public override string ToString()
        {
            if (!built)
            {
                StringBuilder.AppendLine(ModelSummary.ToString());
                built = true;
            }
                return StringBuilder.ToString();
        }
        #endregion

        #region Private Properties
        private bool built { get; set; }    = false;
        public IWaterModelSummary ModelSummary { get; set; }
        private StringBuilder StringBuilder { get; set; } = new StringBuilder();
        #endregion

        #region Private Fields
        private const string Stars = "*****************************************************************";

        #endregion
    }
}
