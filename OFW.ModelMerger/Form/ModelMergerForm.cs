/**
 * @ Author: Akshaya Niraula
 * @ Create Time: 2021-10-22 19:19:33
 * @ Modified by: Akshaya Niraula
 * @ Modified time: 2021-10-26 17:33:18
 * @ Copyright: Copyright (c) 2021 Akshaya Niraula See LICENSE for details
 */


using Haestad.Framework.Application;
using Haestad.Framework.Windows.Forms.Forms;
using Haestad.Framework.Windows.Forms.Resources;
using Haestad.Support.Support;
using OFW.ModelMerger.Extentions;
using OFW.ModelMerger.FormModel;
using OFW.ModelMerger.Support;
using OFW.ModelMerger.UserControl;
using OpenFlows.Application;
using OpenFlows.Water;
using OpenFlows.Water.Application;
using Serilog;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace OFW.ModelMerger.Form
{
    public class ModelMergerForm : HaestadParentForm, IParentFormSurrogate
    {
        #region Auto Generated Code

        private GroupBox groupBoxPrimary;
        private Panel panelPrimary;
        private GroupBox groupBoxSecondary;
        private Panel panelSecondary;
        private SplitContainer splitContainerVertical;
        private SplitContainer splitContainerHorizontal;
        private UserControl.ModelMergeOptionControl modelMergeOptionControlPrimary;
        private UserControl.ModelMergeOptionControl modelMergeOptionControlSecondary;
        private Serilog.Sinks.WinForms.RichTextBoxLogControl richTextBoxLogControl;

        private void InitializeComponent()
        {
            this.groupBoxPrimary = new System.Windows.Forms.GroupBox();
            this.panelPrimary = new System.Windows.Forms.Panel();
            this.groupBoxSecondary = new System.Windows.Forms.GroupBox();
            this.panelSecondary = new System.Windows.Forms.Panel();
            this.splitContainerVertical = new System.Windows.Forms.SplitContainer();
            this.splitContainerHorizontal = new System.Windows.Forms.SplitContainer();
            this.richTextBoxLogControl = new Serilog.Sinks.WinForms.RichTextBoxLogControl();
            this.modelMergeOptionControlPrimary = new OFW.ModelMerger.UserControl.ModelMergeOptionControl();
            this.modelMergeOptionControlSecondary = new OFW.ModelMerger.UserControl.ModelMergeOptionControl();
            this.groupBoxPrimary.SuspendLayout();
            this.panelPrimary.SuspendLayout();
            this.groupBoxSecondary.SuspendLayout();
            this.panelSecondary.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerVertical)).BeginInit();
            this.splitContainerVertical.Panel1.SuspendLayout();
            this.splitContainerVertical.Panel2.SuspendLayout();
            this.splitContainerVertical.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerHorizontal)).BeginInit();
            this.splitContainerHorizontal.Panel1.SuspendLayout();
            this.splitContainerHorizontal.Panel2.SuspendLayout();
            this.splitContainerHorizontal.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBoxPrimary
            // 
            this.groupBoxPrimary.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBoxPrimary.Controls.Add(this.panelPrimary);
            this.groupBoxPrimary.Location = new System.Drawing.Point(8, 8);
            this.groupBoxPrimary.Name = "groupBoxPrimary";
            this.groupBoxPrimary.Size = new System.Drawing.Size(345, 104);
            this.groupBoxPrimary.TabIndex = 0;
            this.groupBoxPrimary.TabStop = false;
            this.groupBoxPrimary.Text = "Primary / Base Model";
            // 
            // panelPrimary
            // 
            this.panelPrimary.AllowDrop = true;
            this.panelPrimary.Controls.Add(this.modelMergeOptionControlPrimary);
            this.panelPrimary.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelPrimary.Location = new System.Drawing.Point(3, 16);
            this.panelPrimary.Name = "panelPrimary";
            this.panelPrimary.Size = new System.Drawing.Size(339, 85);
            this.panelPrimary.TabIndex = 1;
            // 
            // groupBoxSecondary
            // 
            this.groupBoxSecondary.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBoxSecondary.Controls.Add(this.panelSecondary);
            this.groupBoxSecondary.Location = new System.Drawing.Point(3, 8);
            this.groupBoxSecondary.Name = "groupBoxSecondary";
            this.groupBoxSecondary.Size = new System.Drawing.Size(341, 104);
            this.groupBoxSecondary.TabIndex = 0;
            this.groupBoxSecondary.TabStop = false;
            this.groupBoxSecondary.Text = "Secondary / Other Model";
            // 
            // panelSecondary
            // 
            this.panelSecondary.AllowDrop = true;
            this.panelSecondary.Controls.Add(this.modelMergeOptionControlSecondary);
            this.panelSecondary.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelSecondary.Location = new System.Drawing.Point(3, 16);
            this.panelSecondary.Name = "panelSecondary";
            this.panelSecondary.Size = new System.Drawing.Size(335, 85);
            this.panelSecondary.TabIndex = 1;
            // 
            // splitContainerVertical
            // 
            this.splitContainerVertical.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainerVertical.Location = new System.Drawing.Point(0, 0);
            this.splitContainerVertical.Name = "splitContainerVertical";
            // 
            // splitContainerVertical.Panel1
            // 
            this.splitContainerVertical.Panel1.Controls.Add(this.groupBoxPrimary);
            // 
            // splitContainerVertical.Panel2
            // 
            this.splitContainerVertical.Panel2.Controls.Add(this.groupBoxSecondary);
            this.splitContainerVertical.Size = new System.Drawing.Size(713, 119);
            this.splitContainerVertical.SplitterDistance = 356;
            this.splitContainerVertical.TabIndex = 2;
            // 
            // splitContainerHorizontal
            // 
            this.splitContainerHorizontal.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainerHorizontal.Location = new System.Drawing.Point(0, 0);
            this.splitContainerHorizontal.Name = "splitContainerHorizontal";
            this.splitContainerHorizontal.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainerHorizontal.Panel1
            // 
            this.splitContainerHorizontal.Panel1.Controls.Add(this.splitContainerVertical);
            // 
            // splitContainerHorizontal.Panel2
            // 
            this.splitContainerHorizontal.Panel2.Controls.Add(this.richTextBoxLogControl);
            this.splitContainerHorizontal.Size = new System.Drawing.Size(713, 411);
            this.splitContainerHorizontal.SplitterDistance = 119;
            this.splitContainerHorizontal.TabIndex = 3;
            // 
            // richTextBoxLogControl
            // 
            this.richTextBoxLogControl.BackColor = System.Drawing.SystemColors.Control;
            this.richTextBoxLogControl.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.richTextBoxLogControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.richTextBoxLogControl.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.richTextBoxLogControl.ForContext = "";
            this.richTextBoxLogControl.Location = new System.Drawing.Point(0, 0);
            this.richTextBoxLogControl.Name = "richTextBoxLogControl";
            this.richTextBoxLogControl.Size = new System.Drawing.Size(713, 288);
            this.richTextBoxLogControl.TabIndex = 2;
            this.richTextBoxLogControl.Text = "";
            // 
            // modelMergeOptionControlPrimary
            // 
            this.modelMergeOptionControlPrimary.Dock = System.Windows.Forms.DockStyle.Fill;
            this.modelMergeOptionControlPrimary.IsPrimary = true;
            this.modelMergeOptionControlPrimary.Location = new System.Drawing.Point(0, 0);
            this.modelMergeOptionControlPrimary.Name = "modelMergeOptionControlPrimary";
            this.modelMergeOptionControlPrimary.Size = new System.Drawing.Size(339, 85);
            this.modelMergeOptionControlPrimary.TabIndex = 0;
            // 
            // modelMergeOptionControlSecondary
            // 
            this.modelMergeOptionControlSecondary.Dock = System.Windows.Forms.DockStyle.Fill;
            this.modelMergeOptionControlSecondary.IsPrimary = false;
            this.modelMergeOptionControlSecondary.Location = new System.Drawing.Point(0, 0);
            this.modelMergeOptionControlSecondary.Name = "modelMergeOptionControlSecondary";
            this.modelMergeOptionControlSecondary.Size = new System.Drawing.Size(335, 85);
            this.modelMergeOptionControlSecondary.TabIndex = 0;
            // 
            // ModelMergerForm
            // 
            this.AllowDrop = true;
            this.ClientSize = new System.Drawing.Size(713, 411);
            this.Controls.Add(this.splitContainerHorizontal);
            this.Name = "ModelMergerForm";
            this.helpProviderHaestadForm.SetShowHelp(this, false);
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Model Merger";
            this.groupBoxPrimary.ResumeLayout(false);
            this.panelPrimary.ResumeLayout(false);
            this.groupBoxSecondary.ResumeLayout(false);
            this.panelSecondary.ResumeLayout(false);
            this.splitContainerVertical.Panel1.ResumeLayout(false);
            this.splitContainerVertical.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerVertical)).EndInit();
            this.splitContainerVertical.ResumeLayout(false);
            this.splitContainerHorizontal.Panel1.ResumeLayout(false);
            this.splitContainerHorizontal.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerHorizontal)).EndInit();
            this.splitContainerHorizontal.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        #region Constructor
        public ModelMergerForm(HaestadParentFormModel parentFormModel)
            : base(parentFormModel)
        {
            InitializeComponent();

            OpenFlowsWater.SetMaxProjects(5);

        }
        #endregion

        #region Protected Overrides
        protected override void InitializeBindings()
        {
            ModelMergerFormModel.ModelMergeOptionControlModelPrimary = modelMergeOptionControlPrimary.ModelMergeOptionControlModel;
            ModelMergerFormModel.ModelMergeOptionControlModelSecondary = modelMergeOptionControlSecondary.ModelMergeOptionControlModel;
        }
        public override OpenFileDialog NewOpenFileDialog()
        {
            OpenFileDialog open = new OpenFileDialog();
            open.CheckFileExists = true;
            open.CheckPathExists = true;
            open.DefaultExt = AppManager.ParentFormModel.ApplicationDescription.LeadFileExtension;
            open.Filter = (AppManager.ParentFormModel.ApplicationDescription).MultiExtensionOpenFileFilter;
            open.ShowReadOnly = false;
            return open;
        }
        protected override void InitializeVisually()
        {
            Icon = (Icon)GraphicResourceManager.Current[StandardGraphicResourceNames.DMAMerge];

            richTextBoxLogControl.Text = "" + Environment.NewLine +
                "1. Select [Primary and Secondary] model" + Environment.NewLine +
                "2. Make sure right scenario is active" + Environment.NewLine +
                "3. Enter a short, 3 to 5 characters, name/acronym" + Environment.NewLine +
                "4. Import" + Environment.NewLine +
                "5. Save As." + Environment.NewLine + Environment.NewLine;               
        }      
        protected override void InitializeEvents()
        {
            modelMergeOptionControlPrimary.ProjectOpenExecuted += (s, e) =>
                modelMergeOptionControlPrimary.OpenModel(OpenPrimaryModel);

            modelMergeOptionControlSecondary.ProjectOpenExecuted += (o, e) =>
                modelMergeOptionControlSecondary.OpenModel(OpenSecondaryModel);

            modelMergeOptionControlPrimary.MergeExecuted += (s, e) => MergeModels();

            modelMergeOptionControlPrimary.ShowReportExecuted += (o, e) => ShowReport();

            modelMergeOptionControlPrimary.SaveAsExecuted += (o, e) => SaveProjectAs();



            modelMergeOptionControlPrimary.ShowScenarioSelectionDialogClicked += (s, e) => ShowScenarioSelectionDialog(s);
            modelMergeOptionControlSecondary.ShowScenarioSelectionDialogClicked += (s, e) => ShowScenarioSelectionDialog(s);
        }
        #endregion

        #region Public Methods
        public void SetParentWindowHandle(long handle)
        {
            //no-op
        }
        #endregion

        #region Private Methods
        private void ShowReport()
        {
            var reportRTB = new RichTextBox();
            reportRTB.Dock = DockStyle.Fill;
            reportRTB.Font = richTextBoxLogControl.Font;
            
            reportRTB.Text = SummaryManager.Instance.ToString();

            var form = new CenterParentToolForm("Model Merge Report", this, reportRTB, new Size(600,600));
            form.ShowDialog();
        }
        private void ShowScenarioSelectionDialog(object sender)
        {
            var userControl = (ModelMergeOptionControl)sender;
            WaterApplicationManager.GetInstance().ParentFormModel.CurrentProject = userControl.ModelMergeOptionControlModel.Project;

            new CenterParentToolForm(
              "Scenario",
              FindForm(),
              WaterApplicationManager.GetInstance().ParentFormUIModel.ScenarioManagerProxy,
              new Size(350, 350)
              ).ShowDialog();

        }
        private void SaveProjectAs()
        {
            if (PromptSaveAs(ParentFormModel.CurrentProject) == DialogResult.OK){
                var newProjectFullPath = ParentFormModel.CurrentProject.FullPath;
                Log.Information($"Project is saved as at: {newProjectFullPath}");

                var mbox = MessageBox.Show(this, "Would you like to open the newly saved file in the main application?", "Open in Water[GEMS/CAD/OPS]", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if(mbox == DialogResult.Yes)
                {
                    Process.Start(newProjectFullPath);
                }
            }
        }
        private void MergeModels()
        {
            if (modelMergeOptionControlPrimary.ModelMergeOptionControlModel.WaterModel == null ||
                    modelMergeOptionControlSecondary.ModelMergeOptionControlModel.WaterModel == null)
            {
                var message = "Please make sure to open primary as well as secondary models";
                Log.Warning(message);
                MessageBox.Show(this, message, "Model Open", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var piForm = new ProgressIndicatorForm(true, this);
            bool success = true;
            try
            {
                ModelMergerFormModel.Merge(piForm);

                // Suppress any prompts to save and treat the project as if no changes were made.
                ((ProjectBase)modelMergeOptionControlPrimary.ModelMergeOptionControlModel.Project).MakeClean();
                ((ProjectBase)modelMergeOptionControlSecondary.ModelMergeOptionControlModel.Project).MakeClean();

            }
            catch (Exception ex)
            {
                success = false;
                var message = $"ERROR: \n{ex.Message}\n{ex.StackTrace}";
                Log.Error(ex, message);
                MessageBox.Show(this, message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                piForm.Done();
                piForm?.Close();
            }

            if (success)
            {
                var message = "Merge Completed.\nPlease click on Save As to save the changes.\nYes: To see the summary report.";
                var results = MessageBox.Show(this, message, "Completed", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                if (results == DialogResult.Yes)
                {
                    ShowReport();
                }
            }

        }
        private void UpdateFormText()
        {
            var primaryName = modelMergeOptionControlPrimary.ModelMergeOptionControlModel.WaterModel?.ModelInfo.ModelFileInfo().Name;
            var secondaryName = modelMergeOptionControlSecondary.ModelMergeOptionControlModel.WaterModel?.ModelInfo.ModelFileInfo().Name;

            Text = $"Model Merger | {primaryName} ← {secondaryName}";
        }
        private void OpenPrimaryModel()
        {
            modelMergeOptionControlPrimary.ModelMergeOptionControlModel.WaterModel?.Close();

            var open = NewOpenFileDialog();
            if (open.ShowDialog(this) == DialogResult.OK)
            {
                OpenFile(open.FileName);
                modelMergeOptionControlPrimary.ModelMergeOptionControlModel.Options.ProjectPath = open.FileName;
                modelMergeOptionControlPrimary.ModelMergeOptionControlModel.Project = ParentFormModel.CurrentProject;
                modelMergeOptionControlPrimary.ModelMergeOptionControlModel.WaterModel = AppManager.CurrentWaterModel;

                UpdateFormText();
                
                // Suppress any prompts to save and treat the project as if no changes were made.
                ((ProjectBase)modelMergeOptionControlPrimary.ModelMergeOptionControlModel.Project).MakeClean();

            }

        }

        private void OpenSecondaryModel()
        {
            modelMergeOptionControlSecondary.ModelMergeOptionControlModel.WaterModel?.Close();

            var open = NewOpenFileDialog();
            if (open.ShowDialog(this) == DialogResult.OK)
            {
                OpenFile(open.FileName);
                modelMergeOptionControlSecondary.ModelMergeOptionControlModel.Options.ProjectPath = open.FileName;

                modelMergeOptionControlSecondary.ModelMergeOptionControlModel.Project = ParentFormModel.CurrentProject;
                modelMergeOptionControlSecondary.ModelMergeOptionControlModel.WaterModel = AppManager.CurrentWaterModel;

                UpdateFormText();

                // Suppress any prompts to save and treat the project as if no changes were made.
                ((ProjectBase)modelMergeOptionControlSecondary.ModelMergeOptionControlModel.Project).MakeClean();

            }

        }

        #endregion



        #region Private Properties
        private WaterApplicationManager AppManager => (WaterApplicationManager)WaterApplicationManager.GetInstance();
        private ModelMergerFormModel ModelMergerFormModel => modelMergerFormModel ?? (modelMergerFormModel = new ModelMergerFormModel());
        #endregion

        #region Private Fields
        private ModelMergerFormModel modelMergerFormModel;
        #endregion


    }
}
