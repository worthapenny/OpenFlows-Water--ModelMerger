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
            GetChilderenScenarioIdChain(waterScenario, ids);

            return ids;
        }
        private static void GetChilderenScenarioIdChain(IWaterScenario waterScenario,  IList<int> ids)
        {
            ids.Add(waterScenario.Id);
            waterScenario
                .Manager
                .ChildrenOfElement(waterScenario.Id)
                .ForEach(s => GetChilderenScenarioIdChain(s,  ids));

        }
    }
}
