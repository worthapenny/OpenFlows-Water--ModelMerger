/**
 * @ Author: Akshaya Niraula
 * @ Create Time: 2021-10-22 19:19:33
 * @ Modified by: Akshaya Niraula
 * @ Modified time: 2021-10-26 17:33:15
 * @ Copyright: Copyright (c) 2021 Akshaya Niraula See LICENSE for details
 */

using DataTablePrettyPrinter;
using OpenFlows.Domain.DataObjects;
using OpenFlows.Water.Domain;
using OpenFlows.Water.Domain.ModelingElements.NetworkElements;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using static OFW.ModelMerger.Extentions.AlternativeExtensions;
using static OFW.ModelMerger.Extentions.CalcOptionsExtensions;

namespace OFW.ModelMerger.Extentions
{

    #region Extensions
    public static class WaterModelExtensions
    {
        public static IWaterModelSummary ModelSummary(this IWaterModel waterModel, string label = "")
        {
            return new WaterModelSummary(waterModel, label);
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

        public static IWaterSnroAltCalcsSelSetSummary SnroAltCalcsSelSetSummary(this IWaterModel waterModel)
        {
            return new WaterSnroAltCalcsSelSetSummary(waterModel);
        }

        public static FileInfo ModelFileInfo(this IModelInfo modelInfo)
        {
            return new FileInfo(modelInfo.Filename);
        }

    }
    #endregion

    #region Scenario Alternative Calc. Options and Selection Set Summary
    public interface IWaterSnroAltCalcsSelSetSummary
    {
        void AddModel(IWaterModel waterModel, string columnName = "");
    }
    public class WaterSnroAltCalcsSelSetSummary : IWaterSnroAltCalcsSelSetSummary
    {
        #region Constructor
        public WaterSnroAltCalcsSelSetSummary(IWaterModel waterModel)
        {
            WaterModel = waterModel;
            Initialize();
        }
        #endregion

        #region Public Methods
        public void AddModel(IWaterModel waterModel, string columnName = "")
        {
            var valueColumn = new DataColumn(
                string.IsNullOrEmpty(columnName)
                    ? $"{waterModel.ModelInfo.ModelFileInfo().Name}"
                    : columnName
                , typeof(string));


            if (DataTable.Columns.Contains(valueColumn.ColumnName))
                return;

            DataTable.Columns.Add(valueColumn);

            // Selection Set
            SelectionSetDataRow[valueColumn] = waterModel.SelectionSets.Count;

            // Scenario
            ScenarioDataRow[valueColumn] = waterModel.Scenarios.Count;

            // Calc Options
            CalcOptionsEpaNetDataRow[valueColumn] = waterModel.CalculationOptions(WaterEngineType.Epanet).Count;
            CalcOptionsHammerDataRow[valueColumn] = waterModel.CalculationOptions(WaterEngineType.Hammer).Count;


            // Alternative Types
            var alternativeTypes = Enum.GetValues(typeof(WaterAlternativeTypeEnum))
                .Cast<WaterAlternativeTypeEnum>()
                .ToArray()
                .OrderBy(e => e.ToString()).ToArray();

            var alternativesMap = waterModel.AlternativeTypes().All;
            for (int i = 0; i < alternativeTypes.Length; i++)
            {
                var alternativeType = alternativeTypes[i];
                AlternativeTypesRowMap[alternativeType.ToString()][valueColumn] = alternativesMap[alternativeType].Count;
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
            var titleColumn = new DataColumn("Name", typeof(string));
            DataTable.Columns.Add(titleColumn);

            // Selection Set
            SelectionSetDataRow = DataTable.NewRow();
            DataTable.Rows.Add(SelectionSetDataRow);
            SelectionSetDataRow[titleColumn] = "Selection Set";

            // Scenario
            ScenarioDataRow = DataTable.NewRow();
            DataTable.Rows.Add(ScenarioDataRow);
            ScenarioDataRow[titleColumn] = "Scenario";

            // Calc Options
            CalcOptionsEpaNetDataRow = DataTable.NewRow();
            DataTable.Rows.Add(CalcOptionsEpaNetDataRow);
            CalcOptionsEpaNetDataRow[titleColumn] = "Calc Options EpaNet";
            CalcOptionsHammerDataRow = DataTable.NewRow();
            DataTable.Rows.Add(CalcOptionsHammerDataRow);
            CalcOptionsHammerDataRow[titleColumn] = "Calc Options Hammer";

            // Separator
            var separatorRow = DataTable.NewRow();
            DataTable.Rows.Add(separatorRow);
            separatorRow[titleColumn] = "-------------------";

            // Alternative Types
            AlternativeTypesRow = DataTable.NewRow();
            DataTable.Rows.Add(AlternativeTypesRow);
            AlternativeTypesRow[titleColumn] = "Alternative Types";

            // Add each Support Element type
            var alternativeTypes = Enum.GetValues(typeof(WaterAlternativeTypeEnum))
                .Cast<WaterAlternativeTypeEnum>()
                .ToArray()
                .OrderBy(e => e.ToString()).ToArray();


            for (int i = 0; i < alternativeTypes.Length; i++)
            {
                var alternativeType = alternativeTypes[i];

                var dataRow = DataTable.NewRow();
                DataTable.Rows.Add(dataRow);
                AlternativeTypesRowMap.Add(alternativeType.ToString(), dataRow);

                dataRow[titleColumn] = $"   {alternativeType}";
            }

        }
        #endregion


        #region Private Properties
        private IWaterModel WaterModel { get; }
        private DataTable DataTable { get; } = new DataTable("Scnro / Alt / Calcs / SelSet Summary");
        private Dictionary<string, DataRow> AlternativeTypesRowMap { get; set; } = new Dictionary<string, DataRow>();

        private DataRow ScenarioDataRow { get; set; }
        private DataRow AlternativeTypesRow { get; set; }
        private DataRow CalcOptionsEpaNetDataRow { get; set; }
        private DataRow CalcOptionsHammerDataRow { get; set; }
        private DataRow SelectionSetDataRow { get; set; }
        #endregion

    }
    #endregion

    #region Components Count Summary
    public interface IWaterComponentsCountSummary
    {
        Dictionary<IdahoSupportElementTypes, int> ElementsCountMap { get; }

        void AddModel(IWaterModel waterModel, string columnName = "");
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
        public void AddModel(IWaterModel waterModel, string columnName = "")
        {
            var valueColumn = new DataColumn(
                string.IsNullOrEmpty(columnName)
                    ? $"{waterModel.ModelInfo.ModelFileInfo().Name}"
                    : columnName
                , typeof(string));

            if (DataTable.Columns.Contains(valueColumn.ColumnName))
                return;

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
            var titleColumn = new DataColumn("Model Components Type", typeof(string));
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
        private DataTable DataTable { get; } = new DataTable("Components Summary");
        private Dictionary<string, DataRow> DataRowMap { get; set; } = new Dictionary<string, DataRow>();

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
            DataTable = new DataTable($"{waterModel.ModelInfo.ModelFileInfo().Name}: Pipe Diameter Summary Table");
            WaterNetwork = waterNetwork;
            WaterModel = waterModel;
            Build();
        }
        #endregion

        #region Private Methods
        private void Build()
        {
            var diameterUnit = WaterModel.Units.NetworkUnits.Pipe.DiameterUnit.ShortLabel;
            var lengthUnit = WaterModel.Units.NetworkUnits.Pipe.LengthUnit;
            var lengthUnitLabel = lengthUnit.ShortLabel;

            var titleColumn = new DataColumn($"Diameter ({diameterUnit})", typeof(string));
            var countColumn = new DataColumn("Count", typeof(string));
            var lengthColumn = new DataColumn($"Length ({lengthUnitLabel})", typeof(string));

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


            foreach (var diameter in DistinctDiameters.OrderBy(d => d))
            {
                var dataRow = DataTable.NewRow();
                DataTable.Rows.Add(dataRow);

                dataRow[titleColumn] = diameter;
                dataRow[countColumn] = lengthUnit.FormatValue(DiameterToPipeCountMap[diameter]);

                dataRow[lengthColumn] = lengthUnit.FormatValue(DiameterToPipeLengthMap[diameter]);
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
        IWaterNetworkPipeDiameterSummary PipeDiameterSummary { get; }

        void AddModel(IWaterModel waterModell, string columnName = "");

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
        public void AddModel(IWaterModel waterModel, string columnName = "")
        {
            var valueColumn = new DataColumn(
                string.IsNullOrEmpty(columnName)
                    ? $"{waterModel.ModelInfo.ModelFileInfo().Name}"
                    : columnName,
                typeof(string));

            if (DataTable.Columns.Contains(valueColumn.ColumnName))
                return;

            DataTable.Columns.Add(valueColumn);

            // Add each network type
            var elementTypes = Enum.GetValues(typeof(WaterNetworkElementType))
                .Cast<WaterNetworkElementType>()
                .ToArray()
                .OrderBy(e => e.ToString()).ToArray();


            for (int i = 0; i < elementTypes.Length; i++)
            {
                var elementType = elementTypes[i];

                var manager = waterModel.DomainDataSet.DomainElementManager((int)elementType);
                ElementsCountMap[elementType] = manager.Count;

                DataRowMap[$"{elementType}"][valueColumn] = manager.Count;
            }

            try
            {
                // Add pipe length
                var lengthUnit = waterModel.Units.NetworkUnits?.Pipe.LengthUnit.ShortLabel;
                var pipeLengths = (double?)waterModel.Network.Pipes.Input.Lengths()?.Values?.Sum();
                var pipeLengthsFormatted = waterModel.Units.NetworkUnits.Pipe.LengthUnit.FormatValue(pipeLengths.Value);
                DataRowMap[$"{PipeLength}"][valueColumn] = $"{pipeLengthsFormatted} {lengthUnit}";


                // Add laterals
                var lateralLengths = (int?)waterModel.Network.Laterals.Input.Lengths()?.Values?.Sum();
                DataRowMap[$"{LateralLength}"][valueColumn] = $"{lateralLengths} {lengthUnit}";

            }
            catch
            {
                // submodel doesn't have units information so it will run into some exception...
            }
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
        public IWaterNetworkPipeDiameterSummary PipeDiameterSummary => WaterModel.Network.PipeDiameterSummary(WaterModel);
        #endregion

        #region Private Properties
        private IWaterModel WaterModel { get; }
        private DataTable DataTable { get; set; } = new DataTable("Network Elements Count Table");
        private Dictionary<string, DataRow> DataRowMap { get; set; } = new Dictionary<string, DataRow>();
        #endregion

        #region Private Fields
        string PipeLength = "Pipe length";
        string LateralLength = "Lateral length";
        #endregion

    }


    #endregion

    #region Water Model Summary
    public interface IWaterModelSummary
    {
        IWaterNetworkElementsSummary ElementsCountSummary { get; }
        IWaterComponentsCountSummary ComponentsCountSummary { get; }
        IWaterSnroAltCalcsSelSetSummary SnroAltCalcsSelSetSummary { get; }

        List<IWaterNetworkPipeDiameterSummary> PipeDiameterSummaries { get; }

        void AddWaterModel(IWaterModel waterModel, string label = "");

        string ToString(
            bool includeDiameterSummary = true,
            bool includeComponentsSummary = true,
            bool includeScenarioAltCalcsSummary = true);

    }
    internal class WaterModelSummary : IWaterModelSummary
    {
        #region Constructor
        public WaterModelSummary(IWaterModel waterModel, string label = "")
        {
            WaterModel = waterModel;

            ElementsCountSummary = WaterModel.Network.ElementsCountSummary(WaterModel);
            ElementsCountSummary.AddModel(WaterModel, label);

            PipeDiameterSummaries = new List<IWaterNetworkPipeDiameterSummary>();
            PipeDiameterSummaries.Add(WaterModel.Network.PipeDiameterSummary(WaterModel));

            ComponentsCountSummary = WaterModel.ComponentsCountSummary();
            ComponentsCountSummary.AddModel(WaterModel, label);

            SnroAltCalcsSelSetSummary = WaterModel.SnroAltCalcsSelSetSummary();
            SnroAltCalcsSelSetSummary.AddModel(WaterModel, label);
        }
        #endregion

        #region Public Methods
        public void AddWaterModel(IWaterModel waterModel, string label = "")
        {
            ElementsCountSummary.AddModel(waterModel, label);
            ComponentsCountSummary.AddModel(waterModel, label);
            SnroAltCalcsSelSetSummary.AddModel(waterModel, label);
            PipeDiameterSummaries.Add(new PipeDiameterSummary(waterModel.Network, waterModel));
        }

        public string ToString(
            bool includeDiameterSummary = true,
            bool includeComponentsSummary = true,
            bool includeScenarioAltCalcsSummary = true)
        {
            var retVal = ElementsCountSummary.ToString();

            if (includeComponentsSummary)
                retVal += ComponentsCountSummary.ToString();

            if (includeScenarioAltCalcsSummary)
                retVal += SnroAltCalcsSelSetSummary.ToString();

            if (includeDiameterSummary)
                foreach (var pipeDiameeterSummary in PipeDiameterSummaries)
                    retVal += pipeDiameeterSummary.ToString();

            return retVal;
        }
        #endregion

        #region Private Properties

        #endregion

        #region Overrides
        public override string ToString()
        {
            return this.ToString(true, true, true);
        }

        #endregion

        #region Public Properties
        public IWaterNetworkElementsSummary ElementsCountSummary { get; private set; }
        public IWaterComponentsCountSummary ComponentsCountSummary { get; private set; }
        public IWaterSnroAltCalcsSelSetSummary SnroAltCalcsSelSetSummary { get; private set; }
        public List<IWaterNetworkPipeDiameterSummary> PipeDiameterSummaries { get; set; }
        #endregion

        #region Private Properties
        private IWaterModel WaterModel { get; }
        #endregion

    }
    #endregion

}
