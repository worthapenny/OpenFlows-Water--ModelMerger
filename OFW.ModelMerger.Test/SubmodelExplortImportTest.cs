using Haestad.Framework.Application;
using Haestad.Support.User;
using NUnit.Framework;
using OFW.ModelMerger.FormModel;
using System.IO;

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
        public void TestMerge()
        {
            string example1 = Path.GetFullPath(BuildTestFilename($@"Example1.wtg"));
            string example5 = Path.GetFullPath(BuildTestFilename($@"Example5.wtg"));


            var formModel = new ModelMergerFormModel();

            // setup Primary
            formModel.ModelMergeOptionControlModelPrimary = new UserControlModel.ModelMergeOptionControlModel();
            formModel.ModelMergeOptionControlModelPrimary.OpenModel(
                () =>
                {
                    OpenModel(example1);
                    formModel.ModelMergeOptionControlModelPrimary.Options.ProjectPath = example1;
                    formModel.ModelMergeOptionControlModelPrimary.Project = Project;
                    formModel.ModelMergeOptionControlModelPrimary.WaterModel = WaterModel;
                },
                true);
            formModel.ModelMergeOptionControlModelPrimary.Options.ShortName = "Ex1";
            formModel.ModelMergeOptionControlModelPrimary.Options.Scenario =
                formModel.ModelMergeOptionControlModelPrimary.WaterModel.Scenarios.Elements()[1];

            // setup Secondary
            formModel.ModelMergeOptionControlModelSecondary = new UserControlModel.ModelMergeOptionControlModel();
            formModel.ModelMergeOptionControlModelSecondary.OpenModel(
                () => {
                    OpenModel(example5);
                    formModel.ModelMergeOptionControlModelSecondary.Options.ProjectPath = example1;
                    formModel.ModelMergeOptionControlModelSecondary.Project = Project;
                    formModel.ModelMergeOptionControlModelSecondary.WaterModel = WaterModel;
                },
                false);
            formModel.ModelMergeOptionControlModelSecondary.Options.ShortName = "Ex5";
            formModel.ModelMergeOptionControlModelSecondary.Options.Scenario = 
                formModel.ModelMergeOptionControlModelSecondary.WaterModel.ActiveScenario;

            // Perform the merge
            formModel.Merge(new NullProgressIndicator());

            // save as the combined model
            var app = ProjectProperties.Default;
            app.NominalProjectPath = Path.Combine(TempDir, "TestProject.wtg");
            formModel.ModelMergeOptionControlModelPrimary.Project.SaveAs(app);



            Assert.True(true);

        }
        #endregion


        #region Private Properties
        public string TempDir { get; private set; }
        #endregion
    }
}
