/**
 * @ Author: Akshaya Niraula
 * @ Create Time: 2021-10-22 19:19:33
 * @ Modified by: Akshaya Niraula
 * @ Modified time: 2021-10-26 17:33:18
 * @ Copyright: Copyright (c) 2021 Akshaya Niraula See LICENSE for details
 */


using System.Drawing;
using System.Windows.Forms;

namespace OFW.ModelMerger.Support
{
    public class CenterParentToolForm
    {
        #region Constructor
        public CenterParentToolForm(string title, Form parentForm, Control control, Size size)
        {
            this.title = title;
            this.parentForm = parentForm;
            this.control = control;
            this.size = size;
        }
        #endregion

        #region Public Methods
        public DialogResult ShowDialog()
        {
            var form = new Form();

            control.Dock = DockStyle.Fill;
            form.Controls.Add(control);
            form.FormBorderStyle = FormBorderStyle.SizableToolWindow;
            form.Text = title;
            form.Size = size;

            form.StartPosition = FormStartPosition.Manual;

            if (parentForm != null)
            {
                form.Location = new Point(
                    parentForm.Left + parentForm.Width / 2 - form.Width / 2,
                    parentForm.Top + parentForm.Height / 2 - form.Height / 2);
            }

            return form.ShowDialog(parentForm);
        }
        #endregion

        #region Fields
        string title;
        Form parentForm;
        Control control;
        Size size;
        #endregion
    }
}
