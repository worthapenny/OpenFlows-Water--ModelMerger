/**
 * @ Author: Akshaya Niraula
 * @ Create Time: 2021-10-22 19:19:33
 * @ Modified by: Akshaya Niraula
 * @ Modified time: 2021-10-26 17:33:18
 * @ Copyright: Copyright (c) 2021 Akshaya Niraula See LICENSE for details
 */

using Haestad.Domain;
using OpenFlows.Water.Domain;
using OpenFlows.Water.Domain.ModelingElements;
using System.Collections.Generic;
using static OFW.ModelMerger.Extentions.AlternativeExtensions;

namespace OFW.ModelMerger.Extentions
{
    public static class WaterScenarioExtensions
    {
        public static IScenario WoScenario(this IWaterScenario scenario, IWaterModel waterModel) =>
            waterModel.DomainDataSet.ScenarioManager.Element(scenario.Id) as IScenario;

        public static IList<int> GetActiveScenarioIdsPath(this IWaterScenario waterScenario, IWaterModel waterModel)
        {
            var ids = new List<int>();
            GetChildrenScenarioIdChain(waterScenario, ids);

            return ids;
        }
        private static void GetChildrenScenarioIdChain(IWaterScenario waterScenario,  IList<int> ids)
        {
            ids.Add(waterScenario.Id);
            waterScenario
                .Manager
                .ChildrenOfElement(waterScenario.Id)
                .ForEach(s => GetChildrenScenarioIdChain(s,  ids));

        }

        public static Dictionary<WaterAlternativeTypeEnum, IWaterAlternative> ActiveAlternativeMap(this IWaterScenario waterScenario, IWaterModel waterModel)
        {
            // cache active scenario
            var activeScenario = waterModel.ActiveScenario;

            // make given scenario active
            waterModel.SetActiveScenario(waterScenario);

            var retVal= waterModel.AlternativeTypes().ActiveAlternatives;

            // restore active scenario to original
            waterModel.SetActiveScenario(activeScenario); ;

            return retVal;
        }

        public static void AssignAlternatives(this IWaterScenario waterScenario, 
            Dictionary<WaterAlternativeTypeEnum, 
            IWaterAlternative> map, 
            IWaterModel waterModel)
        {
            foreach (var kvp in map)
            {
                waterScenario.WoScenario(waterModel).AlternativeID((int)kvp.Key, kvp.Value.Id);                
            }
        }

    }
}
