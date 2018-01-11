using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.Utils.Drawing;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.ViewInfo;

namespace OrgChartControllerExample {
    public partial class BranchPropertiesControl : DesignablePropertiesControl {
        public BranchPropertiesControl() {
            InitializeComponent();
            this.colorPickEdit1.Properties.ButtonClick += this.colorPickEdit1_Properties_ButtonClick;
            this.colorPickEdit1.Paint += this.colorPickEdit1_Paint;
            this.colorPickEdit1.Color = Color.White;
        }

        public override bool Bind(IShape shape) {
            if (!base.Bind(shape)) {
                return false;
            }

            var branchShape = (BranchShape)shape;

            var binding = new Binding(nameof(this.layoutControlGroup1.Text), branchShape, nameof(branchShape.Name), true, DataSourceUpdateMode.Never);
            binding.Format += (sender, args) => {
                var ret = $"{args.Value}";
                if (!string.IsNullOrEmpty(ret)) {
                    ret += " - ";
                }

                args.Value = $"{ret}Branch";
            };
            this.layoutControlGroup1.DataBindings.Add(binding);
            this.txtName.DataBindings.Add(new Binding(nameof(this.txtName.EditValue), branchShape, nameof(branchShape.Name), false, DataSourceUpdateMode.OnPropertyChanged));
            this.checkEdit1.DataBindings.Add(new Binding(nameof(this.checkEdit1.EditValue), branchShape, nameof(branchShape.ShowName), false, DataSourceUpdateMode.OnPropertyChanged));
            this.checkEdit2.DataBindings.Add(new Binding(nameof(this.checkEdit2.EditValue), branchShape, nameof(branchShape.Ignore), false, DataSourceUpdateMode.OnPropertyChanged));
            this.colorPickEdit1.DataBindings.Add(new Binding(nameof(this.colorPickEdit1.Color), branchShape, nameof(branchShape.Color), false, DataSourceUpdateMode.OnPropertyChanged));
            this.textEdit2.DataBindings.Add(new Binding(nameof(this.textEdit2.EditValue), branchShape, nameof(branchShape.Action), false, DataSourceUpdateMode.OnPropertyChanged));
            this.memoEdit1.DataBindings.Add(new Binding(nameof(this.memoEdit1.EditValue), branchShape, nameof(branchShape.Description), false, DataSourceUpdateMode.OnPropertyChanged));

            return true;
        }
    }
}
