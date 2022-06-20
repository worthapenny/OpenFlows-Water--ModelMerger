/**
 * @ Author: Akshaya Niraula
 * @ Create Time: 2021-10-23 01:12:49
 * @ Modified by: Akshaya Niraula
 * @ Modified time: 2021-10-26 17:33:34
 * @ Copyright: Copyright (c) 2021 Akshaya Niraula See LICENSE for details
 */

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Haestad.Support.User;
using OFW.ModelMerger.Domain;
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
        /// Delete S.A.C except for active ones, Merges Alternatives, and make one root scenario
        /// </summary>
        /// <param name="waterModel"></param>
        public async Task SimplifyAsync(IWaterModel waterModel, LabelModificationOptions options, IProgressIndicator pi)
        {
            pi.AddTask("Removing S.A.C. that are not active...");
            pi.IncrementTask();
            pi.BeginTask(1);

            var newMergedScenario = waterModel.Scenarios.Create(waterModel.ActiveScenario.Id);
            waterModel.SetActiveScenario(newMergedScenario);
            newMergedScenario.Label = options.NewLabelScenarioAltCalcs;

            // make the new scenario as base so that others can be deleted
            newMergedScenario.ParentScenario = null;

            DeleteScenariosExceptActive(waterModel);          
            DeleteAlternativesExceptActive(waterModel);
            DeleteCalcOptionsExceptActive(waterModel);
            await MergeAlternativesToTheRootAsync(waterModel);


            pi.IncrementStep();
            pi.EndTask();
        }


        /// <summary>
        /// Merge All Alternatives to the root Alternative
        /// </summary>
        /// <param name="waterModel"></param>
        public async Task MergeAlternativesToTheRootAsync(IWaterModel waterModel)
        {
            var allAlternatviesMap = waterModel.AlternativeTypes().All;
            var baseAlternativesMap = waterModel.AlternativeTypes().BaseAlternativesMap;


            foreach (var item in allAlternatviesMap)
            {
                var alternativeType = item.Key;
                var alternatives = item.Value;
                var baseAlternativse = baseAlternativesMap[alternativeType];

                if (alternatives.Count > 1)
                {
                    // assuming the larger id are older child
                    // which is not always true
                    var orderedAlternatives = alternatives.OrderBy(a => a.Id).Reverse().ToList();

                    // set the base alternative to the active scenario
                    // and remove the base alternative from collection so that rest can be merged
                    foreach (var baseAlternative in baseAlternativse)
                    {
                        baseAlternative.AssignToActiveScenario(alternativeType, baseAlternative.Id);
                        orderedAlternatives = orderedAlternatives.Where(a => a.Id != baseAlternative.Id).ToList();

                        // Now merge alternative to the parent alternative
                        foreach (var alternative in orderedAlternatives)
                        {
                            await alternative.MergeAllParentsAsync();
                        }
                    }                    
                }
            }
            
        }

        /// <summary>
        /// Delete scenario except for the active one.
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

    }
}
