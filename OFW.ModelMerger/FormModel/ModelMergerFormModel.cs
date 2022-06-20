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
using Serilog;
using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;

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
        public async Task MergeAsync(IProgressIndicator pi)
        {
            // Log input info
            ModelMergeOptionControlModelPrimary.LogProjectSummary("Primary");
            ModelMergeOptionControlModelSecondary.LogProjectSummary("Secondary");

            // Start the Merge process
            var watch = new Stopwatch();
            watch.Start();

            await ModelMergeOptionControlModelPrimary.SimplifyScenarioAltCalcs(
                ModelMergeOptionControlModelPrimary.Options, pi);
            Log.Information($"[1/6] Simplified Primary's Scenario Alternative and Calculation Options");

            ModelMergeOptionControlModelPrimary.ModifyLabels(pi);
            Log.Information($"[2/6] Modified Primary's model labels");


            await ModelMergeOptionControlModelSecondary.SimplifyScenarioAltCalcs(
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
        public bool CanMerge(out StringBuilder message, IProgressIndicator pi)
        {
            var canMerge = true;
            message = new StringBuilder();

            pi.AddTask("Validating inputs...");
            pi.IncrementTask();
            pi.BeginTask(1);

            // Make sure the primary models is opened
            if (ModelMergeOptionControlModelPrimary.WaterModel == null)
            {
                canMerge = false;
                message.AppendLine("Please make sure to open primary model");
            }

            // Make sure the secondary models is opened
            if (ModelMergeOptionControlModelSecondary.WaterModel == null)
            {
                canMerge = false;
                message.AppendLine("Please make sure to open secondary model");
            }

            // Make sure either of the short name is provided
            if (string.IsNullOrWhiteSpace(ModelMergeOptionControlModelPrimary.Options.ShortName) &&
                string.IsNullOrWhiteSpace(ModelMergeOptionControlModelSecondary.Options.ShortName))
            {
                canMerge = false;
                message.AppendLine("Please provide the short name for at least one model, primary or secondary.");
            }

            // Make sure short name aren't the same
            if(ModelMergeOptionControlModelPrimary.Options.ShortName ==
                ModelMergeOptionControlModelSecondary.Options.ShortName)
            {
                canMerge = false;
                message.AppendLine("Short name for both models cannot be the same");
            }


            pi.IncrementStep();
            pi.EndTask();

            return canMerge;
        }
        #endregion



        #region Public Properties
        public ModelMergeOptionControlModel ModelMergeOptionControlModelPrimary { get; set; }
        public ModelMergeOptionControlModel ModelMergeOptionControlModelSecondary { get; set; }
        public string SummaryText { get; set; } = String.Empty;
        #endregion
    }
}
