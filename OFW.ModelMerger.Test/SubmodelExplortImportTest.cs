using Haestad.Framework.Application;
using Haestad.Support.User;
using NUnit.Framework;
using OFW.ModelMerger.Extentions;
using OFW.ModelMerger.FormModel;
using OpenFlows.Water.Domain;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace OFW.ModelMerger.Test
{
    [TestFixture]
    public class SubmodelExplortImportTest : OFWAppTestFixtureBase
    {

        #region Constructor
        public SubmodelExplortImportTest()
        {
        }
        #endregion

        #region Setup / Teardown
        protected override void SetupImpl()
        {
            TempDir = Path.Combine(Path.GetTempPath(), "__ModelMerger");
            if (!Directory.Exists(TempDir)) Directory.CreateDirectory(TempDir);
        }

        protected override void TeardownImpl()
        {
            if (Directory.Exists(TempDir)) Directory.Delete(TempDir, true);
        }
        #endregion

        #region Tests
        [Test]
        public async Task TestMergeAsync()
        {
            string example1 = Path.GetFullPath(BuildTestFilename($@"Example1.wtg"));
            string example5 = Path.GetFullPath(BuildTestFilename($@"Example5.wtg"));


            var formModel = new ModelMergerFormModel();
            IWaterModel primaryModelBeforeImport = null;
            IWaterModel primaryWaterModel = null;
            IWaterModel secondaryWaterModel = null;

            // Because Primary model will have secondary model imported
            // Let's open up the primary model separately
            var primaryModelBeforeImport_ElementsCount = 0;
            var primaryModelBeforeImport_ComponentsCount = 0; 

            
            // setup Primary
            formModel.ModelMergeOptionControlModelPrimary = new UserControlModel.ModelMergeOptionControlModel();
            formModel.ModelMergeOptionControlModelPrimary.OpenModel(
                () =>
                {
                    OpenModel(example1);
                    formModel.ModelMergeOptionControlModelPrimary.Options.ProjectPath = example1;
                    formModel.ModelMergeOptionControlModelPrimary.Project = Project;
                    formModel.ModelMergeOptionControlModelPrimary.WaterModel = WaterModel;

                    primaryWaterModel = WaterModel;
                    primaryModelBeforeImport_ElementsCount = WaterModel.Network.Elements().Count;
                    primaryModelBeforeImport_ComponentsCount = WaterModel.Components.Elements().Count;
                },
                true);
            formModel.ModelMergeOptionControlModelPrimary.Options.ShortName = "Ex1";
            formModel.ModelMergeOptionControlModelPrimary.Options.Scenario =
                formModel.ModelMergeOptionControlModelPrimary.WaterModel.Scenarios.Elements()[1];

            // setup Secondary
            formModel.ModelMergeOptionControlModelSecondary = new UserControlModel.ModelMergeOptionControlModel();
            formModel.ModelMergeOptionControlModelSecondary.OpenModel(
                () =>
                {
                    OpenModel(example5);
                    formModel.ModelMergeOptionControlModelSecondary.Options.ProjectPath = example1;
                    formModel.ModelMergeOptionControlModelSecondary.Project = Project;
                    formModel.ModelMergeOptionControlModelSecondary.WaterModel = WaterModel;
                    secondaryWaterModel = WaterModel;
                },
                false);
            formModel.ModelMergeOptionControlModelSecondary.Options.ShortName = "Ex5";
            formModel.ModelMergeOptionControlModelSecondary.Options.Scenario =
                formModel.ModelMergeOptionControlModelSecondary.WaterModel.ActiveScenario;

            // Perform the merge
            await formModel.MergeAsync(new NullProgressIndicator());

             
            // Selection Set
            var secondaryModelLabel = secondaryWaterModel.ModelInfo.ModelFileInfo().Name;
            var ss = primaryWaterModel
                .SelectionSets
                .Elements()
                .Where(s => s.Label == $"MM_AllElements_{secondaryModelLabel}");
            Assert.IsNotNull(ss);
            Assert.IsTrue(ss.Any());
            Assert.AreEqual(ss.First().Get().Count, secondaryWaterModel.Network.Elements().Count);

            // Network 
            Assert.AreEqual(
                (primaryModelBeforeImport_ElementsCount + secondaryWaterModel.Network.Elements().Count),
                primaryWaterModel.Network.Elements().Count);

           
            // Support, a.k.a Components
            Assert.AreEqual(
                (primaryModelBeforeImport_ComponentsCount + secondaryWaterModel.Components.Elements().Count),
                primaryWaterModel.Components.Elements().Count);


            // Close models
            primaryWaterModel.Close();
            secondaryWaterModel.Close();    
        }
        #endregion


        #region Private Properties
        public string TempDir { get; private set; }
        #endregion
    }
}
