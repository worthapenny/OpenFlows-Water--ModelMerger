using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OFW.ModelMerger.Test
{
    [TestFixture]
    public class SubmodelExplortImportTest: OFWAppTestFixtureBase
    {

        #region Constructor
        public SubmodelExplortImportTest()
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
        }

        protected override void TeardownImpl()
        {
            if (Directory.Exists(TempDir)) Directory.Delete(TempDir, true);
        }
        #endregion

        #region Tests
        [Test]
        public void Test1()
        {
            Assert.True(true);
        }
        #endregion


        #region Private Properties
        public string TempDir { get; private set; }
        #endregion
    }
}
