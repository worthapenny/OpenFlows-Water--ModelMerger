using Haestad.Framework.Application;
using Haestad.Framework.Windows.Forms.Forms;
using Haestad.LicensingFacade;
using Haestad.Support.Support;
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
            if (!TestUnitPermission.IsTestUnitRunning())
                TestUnitPermission.Assert();

            Assert.AreEqual(LicenseRunStatusEnum.OK, OpenFlowsWater.StartSession(WaterProductLicenseType.WaterGEMS));
            Assert.AreEqual(true, OpenFlowsWater.IsValid());


            ApplicationManagerBase.SetApplicationManager(new WaterApplicationManager());
            
            var pfsd = new ParentFormSurrogateDelegate((fm) =>
            {
                WaterAppParentForm = new WaterAppParentForm(fm);
                return WaterAppParentForm;
            });
            // ApplicationManager.GetInstance().SetParentFormSurrogateDelegate(pfsd)

            
            //ApplicationManager.GetInstance().Start();
            //ApplicationManager.GetInstance().Start(false); // do not show the ParentForm which blocks the automated testing.

            SetupImpl();
        }
        protected virtual void SetupImpl()
        {             
        }
        [TearDown]
        public void Teardown()
        {
            WaterAppParentForm.CloseAllModels();

            if (WaterModel != null)
                WaterModel.Dispose();
            WaterModel = null;

            TeardownImpl();
            OpenFlowsWater.EndSession();

            if (TestUnitPermission.IsTestUnitRunning())
                TestUnitPermission.RevertAssert();
        }
        protected virtual void TeardownImpl()
        {
        }
        #endregion

        #region Protected Methods
        protected void OpenModel(string filename)
        {
            OpenFlowsWater.StartSession(ParentFormModel.LicensedFeatureSet);
            WaterAppParentForm.FileOpen(filename);

            WaterModel = OpenFlowsWater.GetModel(ParentFormModel.CurrentProject);
        }
        protected virtual string BuildTestFilename(string baseFilename)
        {
            return Path.Combine(@"D:\Development\Data\ModelData\Samples", baseFilename);
        }
        #endregion

        #region Protected Properties
        protected IWaterModel WaterModel { get; private set; }
        protected WaterAppParentForm WaterAppParentForm { get; private set; }
        protected HaestadParentFormModel ParentFormModel => WaterAppParentForm.ParentFormModel;
        #endregion
    }


    public class WaterAppParentForm : HaestadParentForm, IParentFormSurrogate
    {
        #region Constructor
        public WaterAppParentForm(HaestadParentFormModel parentFormModel)
            : base(parentFormModel)
        {
            //InitializeComponent();
        }
        #endregion

        #region Public Methods
        public void SetParentWindowHandle(long handle)
        {
            //no-op
        }
        #endregion

        #region Public Properties
        public new HaestadParentFormModel ParentFormModel => base.ParentFormModel;
        #endregion

    }
}
