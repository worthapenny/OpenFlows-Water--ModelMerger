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
            StringBuilder.AppendLine();
        }
        #endregion

        #region Public Methods
        public void AddBaseModel(IWaterModel waterModel)
        {
            ElementsCountSummary = waterModel.Network.ElementsCountSummary(waterModel);
            ComponentsCountSummary = waterModel.ComponentsCountSummary();

            AddModelSummary("Base/Primary Model Summary", waterModel);
        } 
        public void AddModelSummary(string title, IWaterModel waterModel)
        {
            ElementsCountSummary.AddNetwork(waterModel);
            ComponentsCountSummary.AddModel(waterModel);

            StringBuilder.AppendLine(title);
            StringBuilder.AppendLine(waterModel.Network.ElementsCountSummary(waterModel).ToString());
            StringBuilder.AppendLine();
            StringBuilder.AppendLine(waterModel.Network.PipeDiameterSummary(waterModel).ToString());
            StringBuilder.AppendLine();
            StringBuilder.AppendLine(waterModel.ComponentsCountSummary().ToString());

            AddSeparator();
            StringBuilder.AppendLine();
        }
        public string GetNetworkCountTable() => ElementsCountSummary.ToString();
        public string GetComponentsSummaryTable() => ComponentsCountSummary.ToString();

        public void Add(string message) => StringBuilder.AppendLine(message);
        public void AddSeparator() => StringBuilder.AppendLine(Separator);
        #endregion

        #region Overrides
        public override string ToString()
        {
            return StringBuilder.ToString();
        }
        #endregion

        #region Private Properties
        private IWaterNetworkElementsSummary ElementsCountSummary { get; set; }
        private IWaterComponentsCountSummary ComponentsCountSummary { get; set; }
        private StringBuilder StringBuilder { get; set; } = new StringBuilder();
        #endregion

        #region Private Fields
        private const string Separator = "-----------------------------------------------------------------";
        private const string Stars = "*****************************************************************";

        #endregion
    }
}
