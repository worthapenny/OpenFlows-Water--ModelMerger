/**
 * @ Author: Akshaya Niraula
 * @ Create Time: 2021-10-22 19:12:03
 * @ Modified by: Akshaya Niraula
 * @ Modified time: 2021-10-26 17:32:53
 * @ Copyright: Copyright (c) 2021 Akshaya Niraula See LICENSE for details
 */

namespace OFW.ModelMerger.Domain
{
    public class LabelModificationOptions
    {
        #region Constructor
        public LabelModificationOptions()
        {
        }
        #endregion

        #region Public Properties
        public LabelModificationType ModificationType { get; set; } = LabelModificationType.Prefix;
        public LabelModificationOperationType OperationType { get; set; } = LabelModificationOperationType.Add;
        public string NewLabelScenarioAltCalcs { get; set; } = "Merged";
        public int ScenarioId { get; set; }
        public string ShortName { get;  set; }
        #endregion
    }
}
