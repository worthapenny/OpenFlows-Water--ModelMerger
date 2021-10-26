using Haestad.Framework.Application;
using Haestad.Support.User;
using OpenFlows.Water.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenFlows.Application;
using Haestad.Drawing.Domain;
using OpenFlows.Water.Application;
using System.IO;
using Haestad.Support.Support;
using OpenFlows.Water;
using Haestad.Framework.Windows.Forms.Forms;
using Haestad.Domain;
using Haestad.Idaho.Importer.Submodels;
using Haestad.Domain.DataExchange;
using OFW.ModelMerger.Support;
using OFW.ModelMerger.Extentions;

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
        public void Export(string filepath, IProgressIndicator pi)
        {
            pi.AddTask($"Exporting {WaterParentFormModel.CurrentProject.Label} to a submodel...");
            pi.IncrementTask();
            pi.BeginTask(1);

            // Make sure directory is created
            if (!Directory.Exists(filepath)) Directory.CreateDirectory(filepath);

            // Slect all elements in the model
            var elements = WaterModel
                .Network
                .Elements()
                .Select(e => new ElementIdentifier((int)e.WaterElementType, e.Id)).ToArray();

            // Run the Submodel export command
            WaterParentFormModel.ParentFormUIModel.ExecuteCommand(
                CommandType.SubmodelsExport,
                new object[] { filepath, elements });

            // Generate the summary
            using (var waterModel = OpenFlowsWater.Open(filepath))
            {
                SummaryManager.Instance.AddModelSummary($"Exported Submodel", waterModel);
            }

        }

        public void ImportSubmodel(
            string submodelFilePath,
            string submodelProjectLabel,
            IProject baseProject,
            IWaterModel waterModel,
            IProgressIndicator pi)
        {
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


                // Generate the summary
                SummaryManager.Instance.AddModelSummary($"Combined Model", waterModel);

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
        #endregion

        #region Private Properties
        private IWaterModel WaterModel { get; set; }
        private WaterParentFormModel WaterParentFormModel => (WaterParentFormModel)ApplicationManager.GetInstance().ParentFormModel;
        #endregion

    }
}
