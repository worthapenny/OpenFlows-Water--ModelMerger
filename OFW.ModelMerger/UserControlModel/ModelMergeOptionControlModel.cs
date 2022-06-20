/**
 * @ Author: Akshaya Niraula
 * @ Create Time: 2021-10-22 19:19:33
 * @ Modified by: Akshaya Niraula
 * @ Modified time: 2021-10-26 17:33:18
 * @ Copyright: Copyright (c) 2021 Akshaya Niraula See LICENSE for details
 */


using Haestad.Framework.Application;
using Haestad.Support.User;
using OFW.ModelMerger.Domain;
using OFW.ModelMerger.Support;
using OpenFlows.Water.Domain;
using Serilog;
using System;
using System.Text;
using System.Threading.Tasks;

namespace OFW.ModelMerger.UserControlModel
{
    public class ModelMergeOptionControlModel
    {
        #region Constructors
        public ModelMergeOptionControlModel()
        {
        }
        #endregion

        #region Public Methods
        public void OpenModel(Action modelOpener, bool isPrimary)
        {
            modelOpener();

            if (WaterModel == null) return; // Occurs when model is not opened.

            // Update Scenario
            Options.Scenario = WaterModel.ActiveScenario;

            // Generate Summary
            if (isPrimary)
                SummaryManager.Instance.AddBaseModel(WaterModel);
            else
                SummaryManager.Instance.AddModel(WaterModel);
        }
        public async Task SimplifyScenarioAltCalcs(LabelModificationOptions options, IProgressIndicator pi)
        {
            var simplifier = new SimplifyScenarioAltCalcs();
            await simplifier.SimplifyAsync(WaterModel, options, pi);
        }
        public void ModifyLabels(IProgressIndicator pi)
        {
            new ModifyLabels(WaterModel).Modify(Options, pi);
        }
        public string ExportToSubmodel(IProgressIndicator pi)
        {
            var submodel = new Submodel(WaterModel);
            var path = submodel.Export(pi);

            // cache the WaterModel
            ExportedSubmodelWaterModel = submodel.ExportedSubmodelWaterModel;

            return path;
        }
        public void ImportSubmodel(string submodelFilePath, string submodelProjectLebel, IProject primaryProject, IProgressIndicator pi)
        {

            var submodel = new Submodel(WaterModel);
            submodel.ImportSubmodel(
                submodelFilePath,
                submodelProjectLebel,
                primaryProject,
                pi
                );
        }
        public void LogProjectSummary(string title)
        {
            var sb = new StringBuilder().AppendLine();
            sb.Append(title).AppendLine(":");
            sb.Append("\t").Append("Path:       ").AppendLine(Options.ProjectPath);
            sb.Append("\t").Append("Short Name: ").AppendLine(Options.ShortName);
            sb.Append("\t").Append("Scenario:   ").AppendLine(Options.Scenario.Label);
            sb.AppendLine();

            SummaryManager.Instance.AddMessage(sb.ToString());
            Log.Information(sb.ToString());
        }
        #endregion

        #region Public Properties
        public LabelModificationOptions Options { get; } = new LabelModificationOptions();
        public IProject Project { get; set; }
        public IWaterModel WaterModel { get; set; }
        public IWaterModel ExportedSubmodelWaterModel { get; private set; }

        
        #endregion
    }
}
