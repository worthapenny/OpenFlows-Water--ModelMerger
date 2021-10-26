/**
 * @ Author: Akshaya Niraula
 * @ Create Time: 2021-10-23 01:12:49
 * @ Modified by: Akshaya Niraula
 * @ Modified time: 2021-10-26 17:33:34
 * @ Copyright: Copyright (c) 2021 Akshaya Niraula See LICENSE for details
 */

using System.Collections.Generic;
using System.Linq;
using Haestad.Support.User;
using OFW.ModelMerger.Extentions;
using OpenFlows.Water.Domain;
using OpenFlows.Water.Domain.ModelingElements;
using static OFW.ModelMerger.Extentions.AlternativeExtensions;
using static OFW.ModelMerger.Extentions.CalcOptionsExtensions;

namespace OFW.ModelMerger.Support
{
    public class SimplifyScenarioAltCalcs
    {
        #region Constructor
        public SimplifyScenarioAltCalcs() { }
        #endregion

        #region Public Methods
        /// <summary>
        /// Performs multiple simplifications.
        /// Delete S.A.C except for active ones, Merges Alternatives, and make one one root scenario
        /// </summary>
        /// <param name="waterModel"></param>
        public void Simplify(IWaterModel waterModel, IProgressIndicator pi)
        {
            pi.AddTask("Removing S.A.C. that are not active...");
            pi.IncrementTask();
            pi.BeginTask(1);

            DeleteScenariosExceptActive(waterModel);
            DeleteAlternativesExceptActive(waterModel);
            DeleteCalcOptionsExceptActive(waterModel);
            MergeAlternativesToTheRoot(waterModel);
            DeleteScenariosToTheRoot(waterModel);

            pi.IncrementStep();
            pi.EndTask();
        }

        /// <summary>
        /// Delete all scenarios and keep just one active scenario
        /// </summary>
        /// <param name="waterModel"></param>
        private void DeleteScenariosToTheRoot(IWaterModel waterModel)
        {
            // get the list of active alternatives
            var activeAlternatives = waterModel.AlternativeTypes().ActiveAlternatives;

            // create a base scenario 
            var baseScenario = waterModel.Scenarios.Create().WoScenario(waterModel);
            baseScenario.Label = waterModel.ActiveScenario.Label;

            // assign the same active alternatives to the base scenario
            foreach (var item in activeAlternatives)
            {
                var alternativeType = item.Key;
                var activeAlternative = item.Value;

                baseScenario.AlternativeID((int)alternativeType, activeAlternative.Id);
            }

            // assign the same calc options
            baseScenario.CalculationOptionsID(WaterEngineType.Epanet, waterModel.ActiveScenario.Options.Id);
            baseScenario.CalculationOptionsID(WaterEngineType.Hammer, waterModel.CalculationOptions(WaterEngineType.Hammer).First().Id);

            // make the base scenario active
            waterModel.Scenarios.Element(baseScenario.Id).MakeCurrent();

            // delete all the scenarios except the base
            foreach (var scenario in waterModel
                .Scenarios
                .Elements()
                .Where(s => s.Id != baseScenario.Id))
                scenario.Delete();

        }

        /// <summary>
        /// Merge All Alternatives to the root Alternative
        /// </summary>
        /// <param name="waterModel"></param>
        public void MergeAlternativesToTheRoot(IWaterModel waterModel)
        {
            var allAlternatviesMap = waterModel.AlternativeTypes().All;
            foreach (var item in allAlternatviesMap)
            {
                var alternativeType = item.Key;
                var alternatives = item.Value;

                if (alternatives.Count > 1)
                    foreach (var alternative in alternatives)
                        alternative.MergeAllParents();
            }
        }

        /// <summary>
        /// Delete scenario except for the active one.
        /// Maintain the tree structure
        /// </summary>
        /// <param name="waterModel"></param>
        public void DeleteScenariosExceptActive(IWaterModel waterModel)
        {
            var activeScenarioId = waterModel.Scenarios.ActiveScenario.Id;
            var deleteScenarios = new List<IWaterScenario>();

            waterModel.Scenarios.Elements().ForEach(s =>
            {
                if (!(s.GetActiveScenarioIdsPath(waterModel).Contains(activeScenarioId)))
                    deleteScenarios.Add(s);
            });

            deleteScenarios.ForEach(s => s?.Delete());
        }

        /// <summary>
        /// Delete Alternatives except for the active one.
        /// Maintain the tree structure
        /// </summary>
        /// <param name="waterModel"></param>
        public void DeleteAlternativesExceptActive(IWaterModel waterModel)
        {
            var deleteAlternatives = new List<IWaterAlternative>();

            var alternativeTypesToAlternativeMap = waterModel.AlternativeTypes().All;
            foreach (var item in alternativeTypesToAlternativeMap)
            {
                var alternativeType = item.Key;
                var alternatives = item.Value;

                // get the active alternative for the given alternative type
                var activeAlternative = waterModel.AlternativeTypes().ActiveAlternative(alternativeType);

                // get the path (based on id) to the active alternative
                var ids = activeAlternative.GetAlternativePathIds(waterModel, activeAlternative);

                // any alternative that doesn't belong to the 'ids' are delete candidates
                alternatives.ForEach(a =>
                {
                    if (!(ids.Contains(a.Id)))
                        deleteAlternatives.Add(a);
                });
            }

            deleteAlternatives.ForEach(a => a.Delete());
        }

        /// <summary>
        /// Delete all the Calc Options except for the active one
        /// </summary>
        /// <param name="waterModel"></param>
        public void DeleteCalcOptionsExceptActive(IWaterModel waterModel)
        {
            var activeCalcOptionId = waterModel.ActiveScenario.Options.Id;
            var deleteCalcOptions = new List<IWaterCalculationOptions>();
            waterModel.CalculationOptions(WaterEngineType.Epanet).ForEach(c =>
            {
                if (c.Id != activeCalcOptionId)
                    deleteCalcOptions.Add(c);
            });

            deleteCalcOptions.ForEach(c => c.Delete());
        }
        #endregion

        #region Private Properties
        //private IWaterModel WaterModel { get; }
        #endregion

    }
}
