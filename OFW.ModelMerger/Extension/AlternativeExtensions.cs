/**
 * @ Author: Akshaya Niraula
 * @ Create Time: 2021-10-22 19:18:10
 * @ Modified by: Akshaya Niraula
 * @ Modified time: 2021-10-26 17:33:01
 * @ Copyright: Copyright (c) 2021 Akshaya Niraula See LICENSE for details
 */

using System;
using System.Collections.Generic;
using System.Diagnostics;
using Haestad.Domain;
using Haestad.Support.Support;
using OpenFlows.Domain.ModelingElements;
using OpenFlows.Water.Domain;
using OpenFlows.Water.Domain.ModelingElements.NetworkElements;

namespace OFW.ModelMerger.Extentions
{
    public static class AlternativeExtensions
    {
        #region Alternative Types
        public static IWaterAlternativeTypes AlternativeTypes(this IWaterModel waterModel)
        {
            return new WaterAlternativeType(waterModel);
        }
        public interface IWaterAlternativeTypes
        {
            IWaterAlternative ActiveAlternative(WaterAlternativeTypeEnum alternativeType);
            Dictionary<WaterAlternativeTypeEnum, List<IWaterAlternative>> All { get; }
            Dictionary<WaterAlternativeTypeEnum, IWaterAlternative> ActiveAlternatives { get; }
        }

        private class WaterAlternativeType : IWaterAlternativeTypes
        {
            #region Constructor
            public WaterAlternativeType(IWaterModel waterModel)
            {
                WaterModel = waterModel;
            }

            #endregion

            #region Public Methods
            public IWaterAlternative ActiveAlternative(WaterAlternativeTypeEnum alternativeType)
            {
                IScenario scenario = WaterModel
                    .DomainDataSet
                    .ScenarioManager
                    .Element(WaterModel.ActiveScenario.Id) as IScenario;

                return new WaterAlternative(
                    WaterModel
                    .DomainDataSet
                    .AlternativeManager((int)alternativeType)
                    .Element(scenario.AlternativeID((int)alternativeType)) as IAlternative,
                    WaterModel
                    );
            }
            #endregion

            #region Public Properties
            public Dictionary<WaterAlternativeTypeEnum, List<IWaterAlternative>> All
            {
                get
                {
                    var map = new Dictionary<WaterAlternativeTypeEnum, List<IWaterAlternative>>();
                    WaterModel.Alternatives().ForEach(a =>
                    {
                        List<IWaterAlternative> alternatives = null;
                        if (!(map.TryGetValue(a.AlternativeType, out alternatives)))
                            map[a.AlternativeType] = new List<IWaterAlternative>();

                        map[a.AlternativeType].Add(a);
                    });

                    return map;
                }
            }
            public Dictionary<WaterAlternativeTypeEnum, IWaterAlternative> ActiveAlternatives
            {
                get
                {
                    var alternativeTypes = Enum.GetValues(typeof(WaterAlternativeTypeEnum));
                    var activeAlternatives = new Dictionary<WaterAlternativeTypeEnum, IWaterAlternative>();

                    foreach (var alternativeType in alternativeTypes)
                    {
                        var alternativeTypeEnum = (WaterAlternativeTypeEnum)alternativeType;
                        activeAlternatives.Add(alternativeTypeEnum, ActiveAlternative(alternativeTypeEnum));
                    }

                    return activeAlternatives;
                }
            }
            #endregion

            #region IElement Properties
            private IWaterModel WaterModel { get; }
            #endregion
        }
        #region Enums
        public enum WaterAlternativeTypeEnum
        {
            HmiDataSetGeometry = 1,
            HMIDataSetTopology = 2,
            HMIActiveTopology = 3,
            HMIUserDefinedExtensions = 100,
            Physical = 4,
            Demand = 20,
            InitialSettings = 21,
            Operational = 22,
            Age = 23,
            Constituent = 24,
            Trace = 25,
            FireFlow = 26,
            EnergyCost = 28,
            PressureDependentDemand = 29,
            Hammer = 50,
            Flushing = 31,
            PipeBreak = 51,
            Scada = 14
        }
        #endregion

        #endregion

        #region Alternatives
        public static List<IWaterAlternative> Alternatives(this IWaterModel waterModel)
        {
            //var DDS = waterModel.DomainDataSet;
            var alternatives = new List<IWaterAlternative>();
            //IScenario activeScenario = DDS.ScenarioManager.Element(DDS.ScenarioManager.ActiveScenarioID) as IScenario;

            foreach (var alternativeType in waterModel.DomainDataSet.DomainDataSetType().AlternativeTypes())
            {
                //var activeAlternativeId = activeScenario.AlternativeID(alternativeType.Id);
                foreach (IAlternative alternative in waterModel.DomainDataSet.AlternativeManager(alternativeType.Id).Elements())
                    alternatives.Add(new WaterAlternative(alternative, waterModel));
            }

            return alternatives;
        }


        public interface IWaterAlternative : IElement
        {
            List<IWaterAlternative> Children();
            FieldCollection SupportedField();
            IField AlternativeField(string name, WaterNetworkElementType elementType);
            IAlternativeType WoAlternativeType { get; }
            WaterAlternativeTypeEnum AlternativeType { get; }
            IList<int> GetAlternativePathIds(IWaterModel waterModel, IWaterAlternative alternative);
            int ParentId { get; set; }
            IAlternative WoAlternative { get; }


            bool IsActive();
            void Delete();
            void MergeAllParents();
            void AssignToActiveScenario(WaterAlternativeTypeEnum alternativeType, int alternativeId);
        }

        [DebuggerDisplay("Alternative: {Id}: {Label} [{AlternativeType}]")]
        private class WaterAlternative : IWaterAlternative, IElement
        {
            #region Constructor
            public WaterAlternative(IAlternative alternative, IWaterModel waterModel)
            {
                WoAlternative = alternative;
                WaterModel = waterModel;
            }
            #endregion

            #region Public Methods
            public List<IWaterAlternative> Children()
            {
                var alternatives = new List<IWaterAlternative>();
                foreach (var alternative in WoAlternative.Children())
                    alternatives.Add(new WaterAlternative(alternative as IAlternative, WaterModel));

                return alternatives;
            }
            public void Delete() => WoAlternative.Manager.Delete(WoAlternative.Id);
            public FieldCollection SupportedField() => WoAlternative.SupportedFields();
            public IField AlternativeField(string name, WaterNetworkElementType elementType)
                => WoAlternative.AlternativeField(name, (int)elementType);
            public bool IsActive()
            {
                var activeAlternativeId = WaterModel
                    .ActiveScenario
                    .WoScenario(WaterModel)
                    .AlternativeID((int)AlternativeType);
                return activeAlternativeId == Id;
            }
            public IList<int> GetAlternativePathIds(IWaterModel waterModel, IWaterAlternative alternative)
            {
                var ids = new List<int>();
                GetParentAlternativeIdChain(alternative.WoAlternative, ids);
                return ids;
            }
            public void MergeAllParents()
            {
                var parentAlternativeId = WoAlternative.ParentID;

                while (parentAlternativeId > 0)
                {
                    // if current alternative is active, make the parent alternative active
                    // as you can not start the merge from the active alternative
                    if (IsActive())
                        AssignToActiveScenario(AlternativeType, ParentId);

                    (WoAlternative.Manager as IAlternativeManager).Merge(Id);

                    var parentAlternative = WaterModel.DomainDataSet.AlternativeManager((int)AlternativeType).Element(ParentId) as IAlternative;
                    parentAlternativeId = parentAlternative.ParentID;
                }
            }
            public void AssignToActiveScenario(WaterAlternativeTypeEnum alternativeType, int alternativeId)
            {
                WaterModel
                    .ActiveScenario
                    .WoScenario(WaterModel)
                    .AlternativeID((int)alternativeType, alternativeId);

            }
            #endregion

            #region Private Properties
            private void GetParentAlternativeIdChain(IAlternative alternative, IList<int> ids)
            {
                ids.Add(alternative.Id);
                if (alternative.ParentID > 0)
                    GetParentAlternativeIdChain(
                        alternative.Manager.Element(alternative.ParentID) as IAlternative,
                        ids);

            }

            private IWaterModel WaterModel { get; }
            #endregion

            #region Public Properties
            public int Id => WoAlternative.Id;
            public string Label { get => WoAlternative.Label; set => WoAlternative.Label = value; }
            public string Notes { get => WoAlternative.Notes; set => WoAlternative.Notes = value; }
            public int ParentId { get => WoAlternative.ParentID; set => WoAlternative.ParentID = value; }
            public WaterAlternativeTypeEnum AlternativeType => (WaterAlternativeTypeEnum)WoAlternative.AlternativeTypeID;
            public ModelElementType ModelElementType => ModelElementType.All; // not sure how to set to specific type 
            public IAlternativeType WoAlternativeType => WoAlternative.AlternativeType();
            public IAlternative WoAlternative { get; }
            #endregion

        }
        #endregion

    }
}
