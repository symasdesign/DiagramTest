using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DevExpress.Diagram.Core;
using DevExpress.XtraDiagram;
using DevExpress.XtraEditors;
using OrgChartControllerExample.Extensions;

namespace OrgChartControllerExample {
    public partial class FunctionForm : XtraForm {

        public FunctionForm(Function  function, Image img) {
            this.Function = function;

            InitializeComponent();
            this.Icon = Icon.FromHandle(new Bitmap(img).GetHicon());

            //this.Function.ModifiedChanged += StateDiagramShapeOnModifiedChanged;
            this.memoEdit1.DataBindings.Add(new Binding(nameof(this.memoEdit1.EditValue), function, nameof(function.Body), false, DataSourceUpdateMode.OnPropertyChanged));

            this.Function.BodyModifiedChanged += StateDiagramShapeOnBodyModifiedChanged;
        }

        public Function Function { get; }

        protected override void OnLoad(EventArgs e) {
            this.UpdateText();
            base.OnLoad(e);
            //this.LoadFunctionBody(this.Function.LoadBody());
        }

        //public void LoadFunctionBody(string s) {
        //    if (string.IsNullOrEmpty(s)) {
        //        return;
        //    }
        //    //this.memoEdit1.e
        //}

        //private void SetAllShapesUnmodified() {
        //    this.Function.Modified = false;
        //}
        private void StateDiagramShapeOnBodyModifiedChanged(object o, EventArgs eventArgs) {
            this.UpdateText();
        }

        private bool hasChanged;
        private void UpdateText() {
            this.Text = Function.Name;
            if (this.HasChanges) {
                this.Text += "*";                
            }
            if (this.hasChanged != this.HasChanges) {
                this.hasChanged = this.HasChanges;
                this.OnHasChangesChanged(EventArgs.Empty);
            }
        }

        public event EventHandler HasChangesChanged;
        public bool HasChanges => this.Function.BodyModified;

        protected virtual void OnHasChangesChanged(EventArgs e) {
            this.HasChangesChanged?.Invoke(this, e);
        }

        public Form1 Mainform => (Form1) this.MdiParent;

        //public void Save() {
        //    if (this.HasChanges) {
        //        using (var stream = new MemoryStream()) {
        //            this.diagramControl1.SaveDocument(stream);
        //            this.Function.SaveBody(this.memoEdit1.EditValue)
        //        }
        //        this.SetAllShapesUnmodified();
        //    }
        //}

        protected override void OnFormClosing(FormClosingEventArgs e) {
            if (e.CloseReason == CloseReason.UserClosing && this.HasChanges) {
                switch (XtraMessageBox.Show($"Do you want to save the changes you made to the body of '{this.Function.Name}'?", "Save Changes", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question)) {
                    case DialogResult.Cancel:
                        e.Cancel = true;
                        return;
                    case DialogResult.No:
                        this.Function.ResetBody();
                        break;
                }
            }


            base.OnFormClosing(e);
        }
    }
}
