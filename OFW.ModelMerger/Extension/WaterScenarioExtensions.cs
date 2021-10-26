/**
 * @ Author: Akshaya Niraula
 * @ Create Time: 2021-10-22 19:19:33
 * @ Modified by: Akshaya Niraula
 * @ Modified time: 2021-10-26 17:33:18
 * @ Copyright: Copyright (c) 2021 Akshaya Niraula See LICENSE for details
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Haestad.Domain;
using OpenFlows.Water.Domain;
using OpenFlows.Water.Domain.ModelingElements;

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
    }
}
