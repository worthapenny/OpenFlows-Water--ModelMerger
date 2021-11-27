using Haestad.Framework.Application;
using NUnit.Framework;
using OpenFlows.Application;
using OpenFlows.Water;
using OpenFlows.Water.Application;
using OpenFlows.Water.Domain;
using System.IO;

namespace OFW.ModelMerger.Test
{

    public abstract class OFWAppTestFixtureBase
    {
        #region Constructor
        public OFWAppTestFixtureBase()
        {

        }
        #endregion

        #region Setup/Tear-down
        [SetUp]
        public void Setup()
        {
            ApplicationManagerBase.SetApplicationManager(new WaterApplicationManager());            
            OpenFlowsWater.SetMaxProjects(5);

            // By passing in false, this will suppress the primary user interface.
            // Make sure you are logged into CONNECTION client.
            WaterApplicationManager.GetInstance().Start(false);

            SetupImpl();
        }
        protected virtual void SetupImpl()
        {             
        }
        [TearDown]
        public void Teardown()
        {
            TeardownImpl();

            WaterApplicationManager.GetInstance().Stop();
        }
        protected virtual void TeardownImpl()
        {
        }
        #endregion

        #region Protected Methods
        protected void OpenModel(string filename)
        {
            ProjectProperties pp = ProjectProperties.Default;
            pp.NominalProjectPath = filename;

            WaterApplicationManager.GetInstance().ParentFormModel.OpenProject(pp);
        }
        protected virtual string BuildTestFilename(string baseFilename)
        {
            return Path.Combine(@"D:\Development\Data\ModelData\Samples", baseFilename);
        }
        #endregion

        #region Protected Properties
        protected IWaterModel WaterModel => WaterApplicationManager.GetInstance().CurrentWaterModel;
        protected IProject Project => WaterApplicationManager.GetInstance().ParentFormModel.CurrentProject;
        #endregion
    }
}
