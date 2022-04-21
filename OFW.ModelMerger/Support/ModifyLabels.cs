/**
 * @ Author: Akshaya Niraula
 * @ Create Time: 2021-10-22 19:16:18
 * @ Modified by: Akshaya Niraula
 * @ Modified time: 2021-10-26 17:56:19
 * @ Copyright: Copyright (c) 2021 Akshaya Niraula See LICENSE for details
 */

using Haestad.Support.User;
using OFW.ModelMerger.Domain;
using OFW.ModelMerger.Extentions;
using OpenFlows.Domain.ModelingElements.NetworkElements;
using OpenFlows.Water.Domain;

namespace OFW.ModelMerger.Support
{
    public class ModifyLabels
    {
        #region Constructor
        public ModifyLabels(IWaterModel waterModel)
        {
            WaterModel = waterModel;
        }
        #endregion

        #region Public Methods
        public void Modify(LabelModificationOptions options, IProgressIndicator pi)
        {
            string projectLabel = options.ShortName;
            if (projectLabel == null || projectLabel.Length == 0)
                return;

            pi.AddTask($"Modifying {projectLabel}'s labels for Network elements...");
            pi.AddTask($"Modifying {projectLabel}'s labels for Support elements...");
            pi.AddTask($"Modifying {projectLabel}'s labels for Selection Sets...");
            pi.AddTask($"Modifying {projectLabel}'s labels for Scenarios...");
            pi.AddTask($"Modifying {projectLabel}'s labels for Alternatives");
            pi.AddTask($"Modifying {projectLabel}'s labels for Calc. options...");

            ModifyDomainELementsLabels(options, pi);
            ModifySupportElementTypesLabels(options, pi);
            ModifySelectionSetsLabels(options, pi);
            ModifyScenariosLabels(options, pi);
            ModifyAlternativesLabels(options, pi);
            ModifyCalculationOptionsLabels(options, pi);
        }

        //public void ReplaceLabels(LabelModificationOptions options, string projectLabel, IProgressIndicator pi)
        //{
        //    pi.AddTask($"Replacing {projectLabel}'s labels for Scenarios...");
        //    pi.AddTask($"Replacing {projectLabel}'s labels for Alternatives");
        //    pi.AddTask($"Replacing {projectLabel}'s labels for calc. options...");

        //    ReplaceScenariosLabel(options, pi);
        //    ReplaceAlternativesLabel(options, pi);
        //    ReplaceCalculationOptionsLabel(options, pi);

        //}
        #endregion

        #region Private Methods
        private void ModifyDomainELementsLabels(LabelModificationOptions options, IProgressIndicator pi)
        {
            var elements = WaterModel.Network.Elements(ElementStateType.All);
            pi.IncrementTask();
            pi.BeginTask(elements.Count);

            if (pi.IsAborted)
                return;

            elements.ForEach(e =>
            {
                e.Label = GetNewLabel(options, e.Label);
                pi.IncrementStep();
            });

            pi.EndTask();
        }
        private void ModifySupportElementTypesLabels(LabelModificationOptions options, IProgressIndicator pi)
        {
            var elements = WaterModel.Components.Elements();
            pi.IncrementTask();
            pi.BeginTask(elements.Count);

            if (pi.IsAborted)
                return;
            elements.ForEach(e =>
            {
                e.Label = GetNewLabel(options, e.Label);
                pi.IncrementStep();
            });

            pi.EndTask();
        }
        private void ModifySelectionSetsLabels(LabelModificationOptions options, IProgressIndicator pi)
        {
            var sss = WaterModel.SelectionSets.Elements();

            pi.IncrementTask();
            pi.BeginTask(pi.Task.Steps);

            if (pi.IsAborted)
                return;

            sss.ForEach(ss =>
            {
                ss.Label = GetNewLabel(options, ss.Label);
                pi.IncrementStep();
            });

            pi.EndTask();

        }
        private void ModifyScenariosLabels(LabelModificationOptions options, IProgressIndicator pi)
        {
            var scenarios = WaterModel.Scenarios.Elements();
            pi.IncrementTask();
            pi.BeginTask(scenarios.Count);

            if (pi.IsAborted)
                return;

            scenarios.ForEach(s =>
            {
                s.Label = WaterModel.ActiveScenario.Id == s.Id
                    ? options.NewLabelScenarioAltCalcs
                    : s.Label = GetNewLabel(options, s.Label);

                pi.IncrementStep();
            });

            pi.EndTask();
        }
        private void ModifyCalculationOptionsLabels(LabelModificationOptions options, IProgressIndicator pi)
        {

            pi.IncrementTask();
            pi.BeginTask(1);

            if (pi.IsAborted)
                return;

            foreach (var numericalEngineType in WaterModel.DomainDataSet.DomainDataSetType().NumericalEngineTypes())
            {
                if (numericalEngineType.Name == "EpaNetEngine")
                {
                    foreach (var calcOptions in WaterModel.DomainDataSet.ScenarioManager.CalculationOptionsManager(numericalEngineType.Name).Elements())
                    {
                        calcOptions.Label = WaterModel.ActiveScenario.Options.Id == calcOptions.Id
                            ? options.NewLabelScenarioAltCalcs
                            : GetNewLabel(options, calcOptions.Label);
                    }
                }

                else if (numericalEngineType.Name == "HammerNumericalEngine")
                {
                    foreach (var calcOptions in WaterModel.DomainDataSet.ScenarioManager.CalculationOptionsManager(numericalEngineType.Name).Elements())
                    {
                        calcOptions.Label = WaterModel.ActiveScenario.WoScenario(WaterModel).CalculationOptionsID(numericalEngineType.Name) == calcOptions.Id
                            ? options.NewLabelScenarioAltCalcs
                            : GetNewLabel(options, calcOptions.Label);

                    }
                }
            }

            pi.IncrementStep();
            pi.EndTask();
        }
        private void ModifyAlternativesLabels(LabelModificationOptions options, IProgressIndicator pi)
        {
            pi.IncrementTask();

            var alternatives = WaterModel.Alternatives();
            pi.BeginTask(alternatives.Count);

            if (pi.IsAborted)
                return;

            alternatives.ForEach(a =>
            {
                a.Label = a.IsActive()
                    ? options.NewLabelScenarioAltCalcs
                    : GetNewLabel(options, a.Label);
                pi.IncrementStep();
            });

            pi.EndTask();
        }
        private string GetNewLabel(LabelModificationOptions options, string label)
        {
            // Addition
            if (options.ModificationType == LabelModificationType.Prefix
                && options.OperationType == LabelModificationOperationType.Add)
                return options.ShortName + "__" + label;

            if (options.ModificationType == LabelModificationType.Suffix
                && options.OperationType == LabelModificationOperationType.Add)
                return label + "__" + options.ShortName;

            // Removal
            if (options.ModificationType == LabelModificationType.Prefix
                && options.OperationType == LabelModificationOperationType.Remove
                && label.StartsWith(options.ShortName))
                return label.Substring(options.ShortName.Length + 2);

            if (options.ModificationType == LabelModificationType.Suffix
                && options.OperationType == LabelModificationOperationType.Remove
                && label.EndsWith(options.ShortName))
                return label.Substring(0, label.Length - options.ShortName.Length - 2);

            return string.Empty;
        }
        #endregion

        #region Private Properties
        private IWaterModel WaterModel { get; set; }
        #endregion
    }
}
