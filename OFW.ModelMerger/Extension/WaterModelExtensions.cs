using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using DataTablePrettyPrinter;
using OpenFlows.Domain.DataObjects;
using OpenFlows.Water.Domain;
using OpenFlows.Water.Domain.ModelingElements.Components;
using OpenFlows.Water.Domain.ModelingElements.NetworkElements;

namespace OFW.ModelMerger.Extentions
{

    #region Extensions
    public static class WaterModelExtensions
    {
        public static IWaterModelSummary WaterModelSummary(this IWaterModel waterModel)
        {
            return new WaterModelSummary(waterModel);
        }
        public static IWaterNetworkElementsSummary ElementsCountSummary(this IWaterNetwork waterNetwork, IWaterModel waterModel)
        {
            return new NetworkElementsSummary(waterModel);
        }

        public static IWaterNetworkPipeDiameterSummary PipeDiameterSummary(this IWaterNetwork waterNetwork, IWaterModel waterModel)
        {
            return new PipeDiameterSummary(waterNetwork, waterModel);
        }

        public static IWaterComponentsCountSummary ComponentsCountSummary(this IWaterModel waterModel)
        {
            return new WaterComponentsCountSummary(waterModel);
        }

        public static FileInfo ModelFileInfo(this IModelInfo modelInfo)
        {
            return new FileInfo(modelInfo.Filename);
        }
    }
    #endregion

    #region Components Count Summary
    public interface IWaterComponentsCountSummary
    {
        Dictionary<IdahoSupportElementTypes, int> ElementsCountMap { get; }

        void AddModel(IWaterModel waterModel);
    }

    internal class WaterComponentsCountSummary : IWaterComponentsCountSummary
    {
        #region Constructor
        public WaterComponentsCountSummary(IWaterModel waterModel)
        {
            WaterModel = waterModel;
            Initialize();
        }
        #endregion

        #region Public Methods
        public void AddModel(IWaterModel waterModel)
        {
            var valueColumn = new DataColumn($"{waterModel.ModelInfo.ModelFileInfo().Name}", typeof(string));
            DataTable.Columns.Add(valueColumn);


            // Add each network type
            var elementTypes = Enum.GetValues(typeof(IdahoSupportElementTypes))
                .Cast<IdahoSupportElementTypes>()
                .ToArray()
                .OrderBy(e => e.ToString()).ToArray();


            for (int i = 0; i < elementTypes.Length; i++)
            {
                var elementType = elementTypes[i];

                var manager = waterModel.DomainDataSet.SupportElementManager((int)elementType);
                ElementsCountMap[elementType] = manager.Count;

                DataRowMap[$"{elementType}"][valueColumn] = manager.Count;
            }

        }
        #endregion

        #region Override Methods
        public override string ToString()
        {
            return DataTable.ToPrettyPrintedString();
        }
        #endregion

        #region Private Methods
        private void Initialize()
        {
            var titleColumn = new DataColumn("Component Type", typeof(string));
            DataTable.Columns.Add(titleColumn);

            // Add each Support Element type
            var elementTypes = Enum.GetValues(typeof(IdahoSupportElementTypes))
                .Cast<IdahoSupportElementTypes>()
                .ToArray()
                .OrderBy(e => e.ToString()).ToArray();


            for (int i = 0; i < elementTypes.Length; i++)
            {
                var elementType = elementTypes[i];

                var dataRow = DataTable.NewRow();
                DataTable.Rows.Add(dataRow);
                DataRowMap.Add($"{elementType}", dataRow);

                dataRow[titleColumn] = elementType.ToString().Replace("ElementManager", "").Replace("Idaho", "");
            }

        }
        #endregion

        #region Public Properties
        public Dictionary<IdahoSupportElementTypes, int> ElementsCountMap { get; } = new Dictionary<IdahoSupportElementTypes, int>();
        #endregion

        #region Private Properties
        private IWaterModel WaterModel { get; }
        private DataTable DataTable { get; } = new DataTable("Model Components Summary");
        private Dictionary<string, DataRow> DataRowMap { get; set; } = new Dictionary<string, DataRow>();

        #endregion

    }

    #endregion

    #region Water Model Summary
    public interface IWaterModelSummary
    {
    }
    internal class WaterModelSummary : IWaterModelSummary
    {
        #region Constructor
        public WaterModelSummary(IWaterModel waterModel)
        {
            WaterModel = waterModel;
        }
        #endregion

        #region Public Methods

        public string ToString(
            bool includeDiameterSummary = true,
            bool includeComponentsSummary = true,
            bool includeScenarioAltCalcsSummary = true)
        {
            var retVal = WaterModel.Network.ElementsCountSummary(WaterModel).ToString();

            if (includeDiameterSummary)
                retVal += WaterModel.Network.PipeDiameterSummary(WaterModel).ToString();

            if (includeComponentsSummary)
                retVal += "";

            if (includeScenarioAltCalcsSummary)
                retVal += "";

            return retVal;
        }
        #endregion

        #region Overrides
        public override string ToString()
        {
            return base.ToString();
        }
        #endregion

        #region Public Properties
        #endregion

        #region Private Properties
        private IWaterModel WaterModel { get; }
        #endregion

    }
    #endregion

    #region Water Network Pipe Diameter Summary
    public interface IWaterNetworkPipeDiameterSummary
    {
        IList<double> DistinctDiameters { get; }
        IDictionary<double, double> DiameterToPipeCountMap { get; }
        IDictionary<double, double> DiameterToPipeLengthMap { get; }

    }

    public class PipeDiameterSummary : IWaterNetworkPipeDiameterSummary
    {
        #region Constructor
        public PipeDiameterSummary(IWaterNetwork waterNetwork, IWaterModel waterModel)
        {
            DataTable = new DataTable($"{waterModel.ModelInfo.ModelFileInfo().Name} | Pipe Diameter Summary Table");
            WaterNetwork = waterNetwork;
            WaterModel = waterModel;
            Build();
        }
        #endregion

        #region Private Methods
        private void Build()
        {
            var diameterUnit = WaterModel.Units.NetworkUnits.Pipe.DiameterUnit.ShortLabel;
            var lengthUnit = WaterModel.Units.NetworkUnits.Pipe.LengthUnit.ShortLabel;

            var titleColumn = new DataColumn($"Diameter ({diameterUnit})", typeof(string));
            var countColumn = new DataColumn("Count", typeof(int));
            var lengthColumn = new DataColumn($"Length ({lengthUnit})", typeof(int));

            DataTable.Columns.Add(titleColumn);
            DataTable.Columns.Add(countColumn);
            DataTable.Columns.Add(lengthColumn);

            foreach (var pipe in WaterNetwork.Pipes.Elements())
            {
                var diameter = pipe.Input.Diameter;
                if (!DistinctDiameters.Contains(diameter)) DistinctDiameters.Add(diameter);

                // Pipe Count
                if (DiameterToPipeCountMap.ContainsKey(diameter))
                    DiameterToPipeCountMap[diameter] += 1;
                else
                    DiameterToPipeCountMap.Add(diameter, 1);

                // Length Sum
                if (DiameterToPipeLengthMap.ContainsKey(diameter))
                    DiameterToPipeLengthMap[diameter] += pipe.Input.Length;
                else
                    DiameterToPipeLengthMap.Add(diameter, pipe.Input.Length);
            }

            foreach (var diameter in DistinctDiameters)
            {
                var dataRow = DataTable.NewRow();
                DataTable.Rows.Add(dataRow);

                dataRow[titleColumn] = diameter;
                dataRow[countColumn] = DiameterToPipeCountMap[diameter];
                dataRow[lengthColumn] = DiameterToPipeLengthMap[diameter];
            }
        }
        #endregion

        #region Overrides
        public override string ToString()
        {
            return DataTable.ToPrettyPrintedString();
        }
        #endregion

        #region Public Properties
        public DataTable DataTable { get; set; }
        public IList<double> DistinctDiameters { get; } = new List<double>();
        public IDictionary<double, double> DiameterToPipeCountMap { get; } = new Dictionary<double, double>();
        public IDictionary<double, double> DiameterToPipeLengthMap { get; } = new Dictionary<double, double>();
        #endregion

        #region Private Properties
        private IWaterNetwork WaterNetwork { get; }
        private IWaterModel WaterModel { get; }
        #endregion

    }
    #endregion


    #region Water Network Element Summary
    public interface IWaterNetworkElementsSummary
    {
        Dictionary<WaterNetworkElementType, int> ElementsCountMap { get; }
        IWaterPipeSummary PipeSummary { get; }

        void AddNetwork(IWaterModel waterModel);

    }

    internal class NetworkElementsSummary : IWaterNetworkElementsSummary
    {
        #region Constructor
        public NetworkElementsSummary(IWaterModel waterModel)
        {
            WaterModel = waterModel;
            Initialize();
        }
        #endregion

        #region Public Methods
        public void AddNetwork(IWaterModel waterModel)
        {
            var valueColumn = new DataColumn($"{waterModel.ModelInfo.ModelFileInfo().Name}", typeof(string));
            DataTable.Columns.Add(valueColumn);

            // Add each network type
            var elementTypes = Enum.GetValues(typeof(WaterNetworkElementType))
                .Cast<WaterNetworkElementType>()
                .ToArray()
                .OrderBy(e => e.ToString()).ToArray();


            for (int i = 0; i < elementTypes.Length; i++)
            {
                var elementType = elementTypes[i];

                var manager = WaterModel.DomainDataSet.DomainElementManager((int)elementType);
                ElementsCountMap[elementType] = manager.Count;

                DataRowMap[$"{elementType}"][valueColumn] = manager.Count;
            }

            // Add pipe length
            var lengthUnit = waterModel.Units.NetworkUnits.Pipe.LengthUnit.ShortLabel;
            var pipeLengths = (int?)waterModel.Network.Pipes.Input.Lengths()?.Values?.Sum();
            DataRowMap[$"{PipeLength}"][valueColumn] = $"{pipeLengths} {lengthUnit}";


            // Add laterals
            var lateralLengths = (int?)waterModel.Network.Laterals.Input.Lengths()?.Values?.Sum();
            DataRowMap[$"{LateralLength}"][valueColumn] = $"{lateralLengths} {lengthUnit}";

        }
        #endregion

        #region Private Methods        
        private void Initialize()
        {
            var titleColumn = new DataColumn("Element Type", typeof(string));
            DataTable.Columns.Add(titleColumn);


            // Add each network type
            DataRow dataRow = null;

            var elementTypes = Enum.GetValues(typeof(WaterNetworkElementType))
                .Cast<WaterNetworkElementType>()
                .ToArray()
                .OrderBy(e => e.ToString()).ToArray();


            for (int i = 0; i < elementTypes.Length; i++)
            {
                WaterNetworkElementType elementType = (WaterNetworkElementType)elementTypes[i];

                dataRow = DataTable.NewRow();
                DataTable.Rows.Add(dataRow);
                DataRowMap.Add($"{elementType}", dataRow);

                dataRow[titleColumn] = $"{elementType}";
            }

            // Add Separator
            dataRow = DataTable.NewRow();
            DataTable.Rows.Add(dataRow);
            dataRow[titleColumn] = $"-------------------";


            // Add pipe length
            dataRow = DataTable.NewRow();
            DataTable.Rows.Add(dataRow);
            dataRow[titleColumn] = $"{PipeLength}";
            DataRowMap.Add($"{PipeLength}", dataRow);


            // Add laterals
            dataRow = DataTable.NewRow();
            DataTable.Rows.Add(dataRow);
            dataRow[titleColumn] = $"{LateralLength}";
            DataRowMap.Add($"{LateralLength}", dataRow);

        }

        #endregion

        #region Override Methods
        public override string ToString()
        {
            return DataTable.ToPrettyPrintedString();
        }
        #endregion

        #region Public Properties
        public Dictionary<WaterNetworkElementType, int> ElementsCountMap { get; } = new Dictionary<WaterNetworkElementType, int>();
        public IWaterPipeSummary PipeSummary => pipeSummary
            ?? (pipeSummary = new WaterPipeSummary(WaterModel.Network));
        #endregion

        #region Private Properties
        private IWaterModel WaterModel { get; }
        private DataTable DataTable { get; set; } = new DataTable("Network Elements Count Table");
        private Dictionary<string, DataRow> DataRowMap { get; set; } = new Dictionary<string, DataRow>();
        #endregion

        #region Private Fields
        IWaterPipeSummary pipeSummary;
        string PipeLength = "Pipe length";
        string LateralLength = "Lateral length";
        #endregion

    }


    #endregion

    internal class WaterPipeSummary : IWaterPipeSummary
    {
        #region Constructor
        public WaterPipeSummary(IWaterNetwork waterNetwork)
        {
            WaterNetwork = waterNetwork;
            isSummarized = false;

            DistinctDiameters = new List<double>();
            DiameterToPipeCountMap = new Dictionary<double, double>();
            DiameterToPipeLengthMap = new Dictionary<double, double>();
        }
        #endregion

        #region Public Properties
        public IList<double> DistinctDiameters
        {
            get
            {
                if (!isSummarized) Summarize();
                return DistinctDiameters;
            }
            private set { DistinctDiameters = value; }
        }
        public IDictionary<double, double> DiameterToPipeCountMap
        {
            get
            {
                if (!isSummarized) Summarize();
                return DiameterToPipeCountMap;
            }
            private set { DiameterToPipeCountMap = value; }
        }
        public IDictionary<double, double> DiameterToPipeLengthMap
        {
            get
            {
                if (!isSummarized) Summarize();
                return DiameterToPipeLengthMap;
            }
            private set { DiameterToPipeLengthMap = value; }
        }
        #endregion

        #region Private Methods
        private void Summarize()
        {

            foreach (var pipe in WaterNetwork.Pipes.Elements())
            {
                var diameter = pipe.Input.Diameter;
                if (!DistinctDiameters.Contains(diameter)) DistinctDiameters.Add(diameter);

                // Pipe Count
                if (DiameterToPipeCountMap.ContainsKey(diameter))
                    DiameterToPipeCountMap[diameter] += 1;
                else
                    DiameterToPipeCountMap.Add(diameter, 1);

                // Length Sum
                if (DiameterToPipeLengthMap.ContainsKey(diameter))
                    DiameterToPipeLengthMap[diameter] += pipe.Input.Length;
                else
                    DiameterToPipeLengthMap.Add(diameter, pipe.Input.Length);
            }

            isSummarized = true;
        }
        #endregion

        #region Private Properties
        private IWaterNetwork WaterNetwork { get; }
        #endregion

        #region Private Fields
        bool isSummarized;
        #endregion
    }


    public interface IWaterPipeSummary
    {
        IList<double> DistinctDiameters { get; }
        IDictionary<double, double> DiameterToPipeCountMap { get; }
        IDictionary<double, double> DiameterToPipeLengthMap { get; }
    }
}
