/**
 * @ Author: Akshaya Niraula
 * @ Create Time: 2021-10-23 07:07:23
 * @ Modified by: Akshaya Niraula
 * @ Modified time: 2021-10-26 17:32:58
 * @ Copyright: Copyright (c) 2021 Akshaya Niraula See LICENSE for details
 */

using Haestad.Domain;
using Haestad.Domain.DataExchange;
using Haestad.Drawing.Domain;
using Haestad.Framework.Application;
using Haestad.Framework.Windows.Forms.Forms;
using Haestad.Idaho.Importer.Submodels;
using Haestad.Support.Support;
using Haestad.Support.User;
using OpenFlows.Water;
using OpenFlows.Water.Application;
using OpenFlows.Water.Domain;
using Serilog;
using System.IO;
using System.Linq;

namespace OFW.ModelMerger.Domain
{
    public class Submodel
    {
        #region Constructor
        public Submodel(IWaterModel waterModel)
        {
            WaterModel = waterModel;
        }
        #endregion

        #region Public Methods
        public string Export(IProgressIndicator pi)
        {
            Log.Information("About to export to a submodel.");

            pi.AddTask($"Exporting {WaterParentFormModel.CurrentProject.Label} to a submodel...");
            pi.IncrementTask();
            pi.BeginTask(1);

            var subModelDir = Path.Combine(Path.GetTempPath(), "__ModelMerger");
            if (!Directory.Exists(subModelDir)) Directory.CreateDirectory(subModelDir);

            var subModelFileName = WaterParentFormModel.CurrentProject.Label.Replace(".wtg", ".suubmodel");
            string subModelFilePath = Path.Combine(subModelDir, subModelFileName);


            // Make sure directory is created
            if (!Directory.Exists(subModelFilePath)) Directory.CreateDirectory(subModelFilePath);

            // Select all elements in the model
            var elements = WaterModel
                .Network
                .Elements()
                .Select(e => new ElementIdentifier((int)e.WaterElementType, e.Id)).ToArray();

            // Run the Submodel export command
            WaterParentFormModel.ParentFormUIModel.ExecuteCommand(
                CommandType.SubmodelsExport,
                new object[] { subModelFilePath, elements });

            // For some odd reason, a directory with given path is created
            if(Directory.Exists(subModelFilePath)) Directory.Delete(subModelFilePath);  


            // The actual sqlite file path is tad bit different
            subModelFilePath += ".sqlite";

            // Cache the waterModel of exported submodel
            using (var waterModel = OpenFlowsWater.Open(subModelFilePath))
            {
                ExportedSubmodelWaterModel = waterModel;
            }

            pi.IncrementStep();
            pi.EndTask();


            return subModelFilePath;
        }

        public void ImportSubmodel(
            string submodelFilePath,
            string submodelProjectLabel,
            IProject baseProject,
            IProgressIndicator pi)
        {
            Log.Information("About to import the secondary model as a submodel...");
            var waterModel = WaterApplicationManager.GetInstance().WaterModelFor(baseProject);

            pi.AddTask($"Importing {submodelProjectLabel} submodel to {baseProject.Label}...");
            pi.IncrementTask();
            pi.BeginTask(1);

            WaterParentFormModel.CurrentProject = baseProject;

            var piLocal = new ProgressIndicatorForm(true, WaterParentFormModel.OwnerWindow);
            piLocal.CanCancel = true;
            piLocal.Show();

            IImporterDataSource importerBaseDataSource = null;
            try
            {
                GraphicalDomainProjectBase currentProject = (GraphicalDomainProjectBase)baseProject;
                IDomainDataSet dataSet = currentProject.DomainDataSet;

                if (currentProject.Drawing != null)
                    currentProject.Drawing.ClearSelection();

                importerBaseDataSource = new WaterSubmodelToDomainDataSource(dataSet);

                if (importerBaseDataSource is DomainToDomainDataSourceBase)
                {
                    ((DomainToDomainDataSourceBase)importerBaseDataSource).DomainConnectionType
                        = DomainApplicationModel.GetDataSourceConnectionType();
                }

                importerBaseDataSource.Open(submodelFilePath);

                Importer importer = new Importer(importerBaseDataSource, dataSet, baseProject);
                importer.Initialize(piLocal);

                importer.Execute();

                WaterParentFormModel
                    .ParentFormUIModel
                    .LayoutController
                    .SynchronizeWithDatabase((IGraphicalProject)baseProject, new NullProgressIndicator());

            }
            finally
            {
                if (importerBaseDataSource != null) importerBaseDataSource.Close();
                if (piLocal != null) piLocal.Done();
            }

            pi.IncrementStep();
            pi.EndTask();
        }
        #endregion

        #region Public Properties
        public IWaterModel ExportedSubmodelWaterModel { get; private set; }
        #endregion

        #region Private Properties
        private IWaterModel WaterModel { get; set; }
        private WaterParentFormModel WaterParentFormModel => (WaterParentFormModel)WaterApplicationManager.GetInstance().ParentFormModel;
        #endregion

    }
}
