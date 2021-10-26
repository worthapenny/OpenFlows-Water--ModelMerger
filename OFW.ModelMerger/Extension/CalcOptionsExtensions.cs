/**
 * @ Author: Akshaya Niraula
 * @ Create Time: 2021-10-22 19:19:33
 * @ Modified by: Akshaya Niraula
 * @ Modified time: 2021-10-26 17:33:04
 * @ Copyright: Copyright (c) 2021 Akshaya Niraula See LICENSE for details
 */

using Haestad.Domain;
using Haestad.Support.Support;
using OpenFlows.Domain.ModelingElements;
using OpenFlows.Water.Domain;
using System.Collections.Generic;

namespace OFW.ModelMerger.Extentions
{
    public static class CalcOptionsExtensions
    {
        public static List<IWaterCalculationOptions> CalculationOptions(this IWaterModel waterModel, string waterEngineType)
        {
            var options = new List<IWaterCalculationOptions>();

            var engineTypes = waterModel.DomainDataSet.DomainDataSetType().NumericalEngineTypes();
            foreach (var engineType in engineTypes)
            {
                if (!(engineType.Name == waterEngineType))
                    continue;

                foreach (var option in waterModel.DomainDataSet.ScenarioManager.CalculationOptionsManager(engineType.Name).Elements())
                    options.Add(new WaterCalculationOption(option as ICalculationOptions));
            }

            return options;
        }

        public struct WaterEngineType {
            public const string Epanet = "EpaNetEngine";
            public const string Hammer = "HammerNumericalEngine";
        }

        public interface IWaterCalculationOptions : IElement
        {
            #region Public Methods
            FieldCollection SupportedFields();
            IField Field(string name);
            void Delete();
            #endregion

            #region Public Properties
            string TypeName { get; }
            #endregion

        }

        //[DebuggerDisplay("{ID}: {Label}")]
        private class WaterCalculationOption : IWaterCalculationOptions
        {
            #region Constructor
            public WaterCalculationOption(ICalculationOptions options)
            {
                CalcOption = options;
            }
            #endregion

            #region Public Methods
            public FieldCollection SupportedFields() => CalcOption.SupportedFields();
            public IField Field(string name) => CalcOption.CalculationOptionsField(name);
            public void Delete()
            {
                CalcOption.Manager.Delete(Id);
            }
            #endregion

            #region Public Properties
            public int Id => CalcOption.Id;
            public string Notes { get => CalcOption.Notes; set => CalcOption.Notes = value; }
            public ModelElementType ModelElementType { get => ModelElementType.Options; }
            public string Label { get => CalcOption.Label; set => CalcOption.Label = value; }
            public string TypeName { get => CalcOption.NumericalEngineTypeName; }
            #endregion

            #region Private Properties
            private ICalculationOptions CalcOption { get; set; }
            #endregion

        }
    }
}
