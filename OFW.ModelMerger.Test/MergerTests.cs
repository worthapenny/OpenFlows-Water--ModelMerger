using Haestad.Support.User;
using NUnit.Framework;
using OFW.ModelMerger.Domain;
using OFW.ModelMerger.Extentions;
using OFW.ModelMerger.Support;
using OpenFlows.Water;
using OpenFlows.Water.Domain.ModelingElements.NetworkElements;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using static OFW.ModelMerger.Extentions.CalcOptionsExtensions;

namespace OFW.ModelMerger.Test
{
    [TestFixture]
    public class WaterModelTests : OpenFlowsWaterTestFixtureBase
    {
        #region Constructor
        public WaterModelTests()
        {
        }
        #endregion

        #region Setup / Teardown
        protected override void SetupImpl()
        {
            string filename = Path.GetFullPath(BuildTestFilename($@"Example10.wtg"));
            OpenModel(filename);

            TempDir = Path.Combine(Path.GetTempPath(), "__ModelMerger");
            if (!Directory.Exists(TempDir)) Directory.CreateDirectory(TempDir);

            Options = new LabelModificationOptions();
            Options.ShortName = "Ex10";
            Options.ScenarioId = WaterModel.ActiveScenario.Id;

        }
        protected override void TeardownImpl()
        {
            if (Directory.Exists(TempDir)) Directory.Delete(TempDir, true);
        }
        #endregion


        #region Tests

        [Test]
        public void SimplyfyAndModifyScenarioAltCalcsExceptActiveTest()
        {
            // Simplification Tests
            var simplifier = new SimplifyScenarioAltCalcs();
            simplifier.Simplify(WaterModel, new NullProgressIndicator());

            // scenario
            Assert.AreEqual(WaterModel.Scenarios.Count, 1);

            // alternatives
            var altDict = WaterModel.AlternativeTypes().All;
            foreach (var altItem in altDict)
            {
                Assert.AreEqual(altItem.Value.Count, 1);
            }

            // calc options
            Assert.AreEqual(WaterModel.CalculationOptions(WaterEngineType.Epanet).Count, 1);
            Assert.AreEqual(WaterModel.CalculationOptions(WaterEngineType.Hammer).Count, 1);


            // Modifition Test
            var modifier = new ModifyLabels(WaterModel);
            modifier.ModifiyLabels(Options, new NullProgressIndicator());

            if (Options.ModificationType == LabelModificationType.Prefix)
            {
                Assert.IsTrue(WaterModel.Network.Pipes.Elements().First().Label.StartsWith(Options.ShortName));
                Assert.IsTrue(WaterModel.Components.Patterns.Elements().First().Label.StartsWith(Options.ShortName));
                Assert.IsTrue(WaterModel.Components.Patterns.Elements().Last().Label.StartsWith(Options.ShortName));

                if (WaterModel.SelectionSets.Count > 0)
                {
                    Assert.IsTrue(WaterModel.SelectionSets.Elements().First().Label.StartsWith(Options.ShortName));
                    Assert.IsTrue(WaterModel.SelectionSets.Elements().Last().Label.StartsWith(Options.ShortName));
                }


                foreach (var altItem in WaterModel.AlternativeTypes().All)
                {
                    Assert.AreEqual(altItem.Value.Count, 1);
                    Assert.AreEqual(altItem.Value.First().Label, Options.NewLabelScenarioAltCalcs);
                }

                Assert.AreEqual(WaterModel.CalculationOptions(WaterEngineType.Epanet).First().Label, Options.NewLabelScenarioAltCalcs);
                Assert.AreEqual(WaterModel.CalculationOptions(WaterEngineType.Hammer).First().Label, Options.NewLabelScenarioAltCalcs);
            }

            WaterModel.SaveAs(Path.Combine(TempDir, $"{Options.ShortName}_Modified_Simplified_SAC.wtg"));
        }


        [Test]
        public void ModelSummaryTest()
        {
            // Network Table
            var elementsCountSummary = WaterModel.Network.ElementsCountSummary(WaterModel);
            Assert.IsNotNull(elementsCountSummary);

            elementsCountSummary.AddNetwork(WaterModel);
            var networkTable = elementsCountSummary.ToString();
            Assert.AreEqual(networkTable.Count(s => s == '\n'), 40);
            Assert.AreEqual(elementsCountSummary.ElementsCountMap.Count, Enum.GetValues(typeof(WaterNetworkElementType)).Length);

            // ComponentsTable
            var componentsCountSummary = WaterModel.ComponentsCountSummary();
            Assert.IsNotNull(componentsCountSummary);

            componentsCountSummary.AddModel(WaterModel);
            var componentsTable = componentsCountSummary.ToString();
            Assert.AreEqual(componentsTable.Count(s => s == '\n'), 18);
            Assert.AreEqual(componentsCountSummary.ElementsCountMap.Count, Enum.GetValues(typeof(IdahoSupportElementTypes)).Length);


            // Add another model
            OpenFlowsWater.SetMaxProjects(5);
            var anotherModelPath = base.BuildTestFilename("Example5.wtg");
            using(var waterModel = OpenFlowsWater.Open(anotherModelPath))
            {
                elementsCountSummary.AddNetwork(waterModel);
                componentsCountSummary.AddModel(waterModel);
            }

            // Netowork Table
            Console.WriteLine("After adding another model");
            networkTable = elementsCountSummary.ToString();
            Assert.AreEqual(networkTable.Count(s => s == '\n'), 40);
            Console.WriteLine(networkTable);

            // Components Table
            componentsTable = componentsCountSummary.ToString();
            Assert.AreEqual(componentsTable.Count(s => s == '\n'), 18);
            Console.WriteLine(componentsTable);


            // Pipe Diameter Table
            Console.WriteLine();

            var diaSummary = WaterModel.Network.PipeDiameterSummary(WaterModel);
            Assert.IsNotNull(diaSummary);

            var diaTable = diaSummary.ToString();
            Assert.AreEqual(diaTable.Count(s => (s == '\n')), diaSummary.DistinctDiameters.Count + 6);

            Console.WriteLine(diaTable);
        }
        #endregion

        #region Private Properties
        private LabelModificationOptions Options { get; set; }
        public string TempDir { get; private set; }
        #endregion
    }
}