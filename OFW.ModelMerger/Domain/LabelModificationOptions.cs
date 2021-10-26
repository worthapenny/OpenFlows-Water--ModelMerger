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
