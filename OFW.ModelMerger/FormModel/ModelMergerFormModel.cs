/**
 * @ Author: Akshaya Niraula
 * @ Create Time: 2021-10-22 19:19:33
 * @ Modified by: Akshaya Niraula
 * @ Modified time: 2021-10-26 17:33:18
 * @ Copyright: Copyright (c) 2021 Akshaya Niraula See LICENSE for details
 */


using Haestad.Support.User;
using OFW.ModelMerger.Extentions;
using OFW.ModelMerger.Support;
using OFW.ModelMerger.UserControlModel;
using OpenFlows.Water.Application;
using Serilog;
using System;
using System.Diagnostics;
using System.IO;

namespace OFW.ModelMerger.FormModel
{
    public class ModelMergerFormModel
    {
        #region Constructor
        public ModelMergerFormModel()
        {
        }
        #endregion

        #region Public Methods
        public void Merge(IProgressIndicator pi)
        {
            // Log input info
            ModelMergeOptionControlModelPrimary.LogProjectSummary("Primary");
            ModelMergeOptionControlModelSecondary.LogProjectSummary("Secondary");

            // Start the Merge process
            var watch = new Stopwatch();
            watch.Start();

            ModelMergeOptionControlModelPrimary.SimplifyScenarioAltCalcs(
                ModelMergeOptionControlModelPrimary.Options, pi);
            Log.Information($"[1/6] Simplified Primary's Scenario Alternative and Calculation Options");

            ModelMergeOptionControlModelPrimary.ModifyLabels(pi);
            Log.Information($"[2/6] Modified Primary's model labels");


            ModelMergeOptionControlModelSecondary.SimplifyScenarioAltCalcs(
                ModelMergeOptionControlModelSecondary.Options, pi);
            Log.Information($"[3/6] Simplified Secondary's Scenario Alternative and Calculation Options");

            ModelMergeOptionControlModelSecondary.ModifyLabels(pi);
            Log.Information($"[4/6] Modified Secondary's model labels");

            var submodelName = ModelMergeOptionControlModelSecondary.WaterModel.ModelInfo.ModelFileInfo().Name.Replace(".wtg", ".submodel.sqlite");

            var tempDir = Path.Combine(Path.GetTempPath(), "__ModelMerger");
            if (!Directory.Exists(tempDir))
                Directory.CreateDirectory(tempDir);

            var submodelPath = ModelMergeOptionControlModelSecondary.ExportToSubmodel(pi);
            Log.Information($"[5/6] Exported Secondary model to submodel");

            ModelMergeOptionControlModelPrimary.ImportSubmodel(
                submodelPath,
                submodelName,
                ModelMergeOptionControlModelPrimary.Project,
                pi);
            Log.Information($"[6/6] Imported Secondary model's submodel to the primary model");

            watch.Stop();

            Log.Information($"Time taken: {watch.Elapsed} {Environment.NewLine}");

            // Append to the summary
            SummaryManager.Instance.AddModel(ModelMergeOptionControlModelPrimary.WaterModel, "Combined");


            if (File.Exists(submodelPath))
                File.Delete(submodelPath);
        }
        #endregion



        #region Public Properties
        public ModelMergeOptionControlModel ModelMergeOptionControlModelPrimary { get; set; }
        public ModelMergeOptionControlModel ModelMergeOptionControlModelSecondary { get; set; }
        public string SummaryText { get; set; } = String.Empty; // => SummaryBuilder.ToString();
        #endregion

        #region Private Properties
        private WaterApplicationManager AppManager => (WaterApplicationManager)WaterApplicationManager.GetInstance();
        #endregion
    }
}
