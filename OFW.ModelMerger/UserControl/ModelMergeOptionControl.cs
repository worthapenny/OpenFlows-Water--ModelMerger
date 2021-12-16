/**
 * @ Author: Akshaya Niraula
 * @ Create Time: 2021-10-22 19:19:33
 * @ Modified by: Akshaya Niraula
 * @ Modified time: 2021-10-26 17:33:18
 * @ Copyright: Copyright (c) 2021 Akshaya Niraula See LICENSE for details
 */


using Haestad.Framework.Windows.Forms.Resources;
using Haestad.Support.Support;
using OFW.ModelMerger.Support;
using OFW.ModelMerger.UserControlModel;
using OpenFlows.Water.Application;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace OFW.ModelMerger.UserControl
{
    public class ModelMergeOptionControl : System.Windows.Forms.UserControl
    {
        #region Constructor
        public ModelMergeOptionControl()
        {
            InitializeComponent();

            toolStripButtonOpen.Image = ((Icon)GraphicResourceManager.Current[StandardGraphicResourceNames.Open]).ToBitmap();
            toolStripButtonSelectScenario.Image = ((Icon)GraphicResourceManager.Current[StandardGraphicResourceNames.Scenario]).ToBitmap();
            toolStripButtonImport.Image = ((Icon)GraphicResourceManager.Current[StandardGraphicResourceNames.Import]).ToBitmap();
            toolStripButtonShowReport.Image = ((Icon)GraphicResourceManager.Current[StandardGraphicResourceNames.Report]).ToBitmap();
            toolStripButtonSaveAs.Image = ((Icon)GraphicResourceManager.Current[StandardGraphicResourceNames.SaveAs]).ToBitmap();
                        
            toolStripButtonOpen.Click += (s, e) => { ProjectOpenExecuted?.Invoke(s, e); };
            toolStripButtonImport.Click += (s, e) => { MergeExecuted?.Invoke(s, e); };
            toolStripButtonSaveAs.Click += (s, e) => { SaveAsExecuted?.Invoke(s, e); };
            toolStripButtonShowReport.Click += (s, e) => { ShowReportExecuted?.Invoke(s, e); };
            toolStripButtonSelectScenario.Click += (s, e) => { ShowScenarioSelectionDialogClicked?.Invoke(this, e); };

            
        }
        #endregion

        #region Public Methods
        public void OpenModel(Action modelOpener)
        {
            ModelMergeOptionControlModel.OpenModel(modelOpener, IsPrimary);

            if (ModelMergeOptionControlModel.WaterModel == null) return;

            UpdateActiveScenario();
            WaterApplicationManager.GetInstance().ParentFormModel.ScenarioEventChannel.ScenarioChanged += (s, e) => UpdateActiveScenario();

            textBoxShortName.DataBindings.Add(
                nameof(textBoxShortName.Text),
                ModelMergeOptionControlModel.Options,
                nameof(ModelMergeOptionControlModel.Options.ShortName),
                false,
                DataSourceUpdateMode.OnPropertyChanged);
        }
        #endregion

        #region Private Methods
        private void UpdateActiveScenario()
        {
            labelScenarioSelected.Text = ModelMergeOptionControlModel.WaterModel.ActiveScenario.Label;
            ModelMergeOptionControlModel.Options.Scenario = ModelMergeOptionControlModel.WaterModel.ActiveScenario;
        }
        #endregion

        #region Auto Generated Codes

        private Label labelScenario;
        private Label labelScenarioSelected;
        private Label labelShortName;
        private TextBox textBoxShortName;
        private ToolStrip toolStrip1;
        private ToolStripButton toolStripButtonSelectScenario;
        private ToolStripSeparator toolStripSeparator1;
        private ToolStripButton toolStripButtonImport;
        private ToolStripButton toolStripButtonSaveAs;
        private ToolStripButton toolStripButtonShowReport;
        private ToolTip toolTip1;
        private System.ComponentModel.IContainer components;
        private ToolStripButton toolStripButtonOpen;


        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ModelMergeOptionControl));
            this.labelScenario = new System.Windows.Forms.Label();
            this.labelScenarioSelected = new System.Windows.Forms.Label();
            this.labelShortName = new System.Windows.Forms.Label();
            this.textBoxShortName = new System.Windows.Forms.TextBox();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolStripButtonOpen = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonSelectScenario = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripButtonImport = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonShowReport = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonSaveAs = new System.Windows.Forms.ToolStripButton();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // labelScenario
            // 
            this.labelScenario.AutoSize = true;
            this.labelScenario.Location = new System.Drawing.Point(3, 38);
            this.labelScenario.Name = "labelScenario";
            this.labelScenario.Size = new System.Drawing.Size(87, 13);
            this.labelScenario.TabIndex = 0;
            this.labelScenario.Text = "Scenario To Use";
            // 
            // labelScenarioSelected
            // 
            this.labelScenarioSelected.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.labelScenarioSelected.Location = new System.Drawing.Point(113, 38);
            this.labelScenarioSelected.Name = "labelScenarioSelected";
            this.labelScenarioSelected.Size = new System.Drawing.Size(119, 18);
            this.labelScenarioSelected.TabIndex = 0;
            this.labelScenarioSelected.Text = "<<Actve Scenario>>";
            this.toolTip1.SetToolTip(this.labelScenarioSelected, "Active Scenario");
            // 
            // labelShortName
            // 
            this.labelShortName.AutoSize = true;
            this.labelShortName.Location = new System.Drawing.Point(3, 62);
            this.labelShortName.Name = "labelShortName";
            this.labelShortName.Size = new System.Drawing.Size(63, 13);
            this.labelShortName.TabIndex = 0;
            this.labelShortName.Text = "Short Name";
            // 
            // textBoxShortName
            // 
            this.textBoxShortName.Location = new System.Drawing.Point(116, 59);
            this.textBoxShortName.Name = "textBoxShortName";
            this.textBoxShortName.Size = new System.Drawing.Size(104, 20);
            this.textBoxShortName.TabIndex = 2;
            this.toolTip1.SetToolTip(this.textBoxShortName, "Short label, 3 to 5 letters, for the project. Acronym.");
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButtonOpen,
            this.toolStripButtonSelectScenario,
            this.toolStripSeparator1,
            this.toolStripButtonImport,
            this.toolStripButtonShowReport,
            this.toolStripButtonSaveAs});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(235, 27);
            this.toolStrip1.TabIndex = 4;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // toolStripButtonOpen
            // 
            this.toolStripButtonOpen.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonOpen.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonOpen.Image")));
            this.toolStripButtonOpen.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonOpen.Name = "toolStripButtonOpen";
            this.toolStripButtonOpen.Size = new System.Drawing.Size(24, 24);
            this.toolStripButtonOpen.Text = "Open";
            // 
            // toolStripButtonSelectScenario
            // 
            this.toolStripButtonSelectScenario.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonSelectScenario.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonSelectScenario.Image")));
            this.toolStripButtonSelectScenario.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonSelectScenario.Name = "toolStripButtonSelectScenario";
            this.toolStripButtonSelectScenario.Size = new System.Drawing.Size(24, 24);
            this.toolStripButtonSelectScenario.Text = "Select Scenario";
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 27);
            // 
            // toolStripButtonImport
            // 
            this.toolStripButtonImport.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonImport.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonImport.Image")));
            this.toolStripButtonImport.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonImport.Name = "toolStripButtonImport";
            this.toolStripButtonImport.Size = new System.Drawing.Size(24, 24);
            this.toolStripButtonImport.Text = "Import";
            // 
            // toolStripButtonShowReport
            // 
            this.toolStripButtonShowReport.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonShowReport.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonShowReport.Image")));
            this.toolStripButtonShowReport.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonShowReport.Name = "toolStripButtonShowReport";
            this.toolStripButtonShowReport.Size = new System.Drawing.Size(24, 24);
            this.toolStripButtonShowReport.Text = "Report";
            this.toolStripButtonShowReport.Visible = false;
            // 
            // toolStripButtonSaveAs
            // 
            this.toolStripButtonSaveAs.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonSaveAs.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonSaveAs.Image")));
            this.toolStripButtonSaveAs.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonSaveAs.Name = "toolStripButtonSaveAs";
            this.toolStripButtonSaveAs.Size = new System.Drawing.Size(24, 24);
            this.toolStripButtonSaveAs.Text = "Save As";
            // 
            // ModelMergeOptionControl
            // 
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.textBoxShortName);
            this.Controls.Add(this.labelScenarioSelected);
            this.Controls.Add(this.labelShortName);
            this.Controls.Add(this.labelScenario);
            this.Name = "ModelMergeOptionControl";
            this.Size = new System.Drawing.Size(235, 88);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        #region Public Events
        public event EventHandler MergeExecuted;
        public event EventHandler SaveAsExecuted;
        public event EventHandler ProjectOpenExecuted;
        public event EventHandler ShowReportExecuted;
        public event EventHandler ShowScenarioSelectionDialogClicked;
        #endregion

        #region Public Methods
        
        public void SelectShortLebel()
        {
            textBoxShortName.Focus();
        }
        #endregion

        #region Public Properties
        public ModelMergeOptionControlModel ModelMergeOptionControlModel => modelMergeOptionControlModel ?? (modelMergeOptionControlModel = new ModelMergeOptionControlModel());
        public bool IsPrimary
        {
            get { return toolStripButtonImport.Visible; }
            set { 
                toolStripButtonImport.Visible = value; 
                toolStripButtonSaveAs.Visible = value; 
                toolStripButtonShowReport.Visible = value;
            }
        }
        #endregion

        #region Private Events
        private ModelMergeOptionControlModel modelMergeOptionControlModel;
        #endregion
    }
}
