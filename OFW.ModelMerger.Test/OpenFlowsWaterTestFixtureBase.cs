/**
 * @ Author: Akshaya Niraula
 * @ Create Time: 2021-10-22 18:57:53
 * @ Modified by: Akshaya Niraula
 * @ Modified time: 2021-10-26 17:33:53
 * @ Copyright: Copyright (c) 2021 Akshaya Niraula See LICENSE for details
 */

using Haestad.LicensingFacade;
using Haestad.Support.Support;
using NUnit.Framework;
using OpenFlows.Water;
using OpenFlows.Water.Domain;
using System.IO;
using static OpenFlows.Water.OpenFlowsWater;

namespace OFW.ModelMerger.Test
{
    public abstract class OpenFlowsWaterTestFixtureBase
    {
        #region Constructor
        public OpenFlowsWaterTestFixtureBase()
        {

        }
        #endregion

        #region Setup/Tear-down
        [SetUp]
        public void Setup()
        {
            if (!TestUnitPermission.IsTestUnitRunning())
                TestUnitPermission.Assert();

            Assert.AreEqual(LicenseRunStatusEnum.OK, StartSession(WaterProductLicenseType.WaterGEMS));
            Assert.AreEqual(true, IsValid());

            SetupImpl();
        }
        protected virtual void SetupImpl()
        {
        }
        [TearDown]
        public void Teardown()
        {
            if (WaterModel != null)
                WaterModel.Dispose();
            WaterModel = null;

            TeardownImpl();

            EndSession();

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
            WaterModel = Open(filename);
        }
        protected virtual string BuildTestFilename(string baseFilename)
        {
            return Path.Combine(@"D:\Development\Data\ModelData\Samples", baseFilename);
        }
        #endregion

        #region Protected Properties
        protected IWaterModel WaterModel { get; private set; }
        #endregion
    }
}
