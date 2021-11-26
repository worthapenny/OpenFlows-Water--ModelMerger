/**
 * @ Author: Akshaya Niraula
 * @ Create Time: 2021-10-22 18:57:34
 * @ Modified by: Akshaya Niraula
 * @ Modified time: 2021-10-26 17:34:21
 * @ Copyright: Copyright (c) 2021 Akshaya Niraula See LICENSE for details
 */

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
    public class WaterModelTests : OFWTestFixtureBase
    {
        #region Constructor
        public WaterModelTests()
        {
        }
        #endregion

        #region Setup / Teardown
        protected override void SetupImpl()
        {
            string filename = Path.GetFullPath(BuildTestFilename($@"Example5.wtg"));
            OpenModel(filename);

            TempDir = Path.Combine(Path.GetTempPath(), "__ModelMerger");
            if (!Directory.Exists(TempDir)) Directory.CreateDirectory(TempDir);

            Options = new LabelModificationOptions();
            Options.ShortName = "Ex10";
            //Options.Scenario = WaterModel.ActiveScenario;

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
            simplifier.Simplify(WaterModel, new LabelModificationOptions(), new NullProgressIndicator());

            // scenario
            Assert.AreEqual(WaterModel.Scenarios.Count, 1);
            WaterModel.Units.FormatValue(1.2, WaterModel.Units.NetworkUnits.Pipe.LengthUnit);
            // alternatives
            var altDict = WaterModel.AlternativeTypes().All;
            foreach (var altItem in altDict)
            {
                Assert.AreEqual(altItem.Value.Count, 1);
            }

            // calc options
            Assert.AreEqual(WaterModel.CalculationOptions(WaterEngineType.Epanet).Count, 1);
            Assert.AreEqual(WaterModel.CalculationOptions(WaterEngineType.Hammer).Count, 1);


            // Modification Test
            var modifier = new ModifyLabels(WaterModel);
            modifier.Modify(Options, new NullProgressIndicator());

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

            elementsCountSummary.AddModel(WaterModel);
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

            // Scenario / Alternative / Calc Options / SelectionSet
            var sacss = WaterModel.SnroAltCalcsSelSetSummary();
            var sacssTable = sacss.ToString();
            sacss.AddModel(WaterModel);
            Assert.AreEqual(sacssTable.Count(s => s == '\n'), 30);
            Assert.AreEqual(elementsCountSummary.ElementsCountMap.Count, Enum.GetValues(typeof(WaterNetworkElementType)).Length);

            // Add another model
            OpenFlowsWater.SetMaxProjects(5);
            var anotherModelPath = base.BuildTestFilename("Example5.wtg");
            using(var waterModel = OpenFlowsWater.Open(anotherModelPath))
            {
                elementsCountSummary.AddModel(waterModel);
                componentsCountSummary.AddModel(waterModel);
                sacss.AddModel(waterModel);
            }
            
            // Network Table
            Console.WriteLine("After adding another model");
            networkTable = elementsCountSummary.ToString();
            Assert.AreEqual(networkTable.Count(s => s == '\n'), 40);
            Console.WriteLine(networkTable);

            // Components Table
            componentsTable = componentsCountSummary.ToString();
            Assert.AreEqual(componentsTable.Count(s => s == '\n'), 18);
            Console.WriteLine(componentsTable);


            // Scenario / Alternative / Calc Options / SelectionSet
            sacssTable = sacss.ToString();
            Assert.AreEqual(sacssTable.Count(s => s == '\n'), 30);
            Console.WriteLine(sacssTable);

            // Pipe Diameter Table
            Console.WriteLine();

            var diaSummary = WaterModel.Network.PipeDiameterSummary(WaterModel);
            Assert.IsNotNull(diaSummary);

            var diaTable = diaSummary.ToString();
            Assert.AreEqual(diaTable.Count(s => (s == '\n')), diaSummary.DistinctDiameters.Count + 6);
            Console.WriteLine(diaTable);


            // All In one (Model Summary)
            var modelSummaryTable = WaterModel.ModelSummary().ToString();
            Assert.AreEqual(modelSummaryTable.Count(s => s == '\n'), 113);
            Console.WriteLine();
            Console.WriteLine("All Tables at once");
            Console.WriteLine(modelSummaryTable);
        }

        [Test]
        public void SummaryManagerTest()
        {
            SummaryManager.Instance.AddBaseModel(WaterModel);
            //SummaryManager.Instance.AddModelSummary("BASE MODEL", WaterModel);

            var secondarryModelPath = base.BuildTestFilename("Example4.wtg");
            var model1 = OpenFlowsWater.Open(secondarryModelPath);
            SummaryManager.Instance.AddModel(model1);

            var anotherModelPath = base.BuildTestFilename("Example6.wtg");
            var model2 = OpenFlowsWater.Open(anotherModelPath);
            SummaryManager.Instance.AddModel(model2);

            var summary = SummaryManager.Instance.ToString();
            Assert.NotNull(summary);
            Assert.AreEqual(summary.Length, 9016);

            Console.WriteLine(summary);

            model1.Close();
            model2.Close();
        }
        #endregion

        #region Private Properties
        private LabelModificationOptions Options { get; set; }
        public string TempDir { get; private set; }
        #endregion
    }
}