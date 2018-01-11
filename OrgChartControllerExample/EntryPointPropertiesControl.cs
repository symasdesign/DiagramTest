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
    public partial class EntryPointPropertiesControl : DesignablePropertiesControl {
        public EntryPointPropertiesControl() {
            InitializeComponent();
        }

        public override bool Bind(IShape shape) {
            if (!base.Bind(shape)) {
                return false;
            }

            var entryPointShape = (EntryPointShape)shape;

            var binding = new Binding(nameof(this.layoutControlGroup1.Text), entryPointShape, nameof(entryPointShape.Name), true, DataSourceUpdateMode.Never);
            binding.Format += (sender, args) => {
                var ret = $"{args.Value}";
                if (!string.IsNullOrEmpty(ret)) {
                    ret += " - ";
                }

                if (entryPointShape.IsInCompositeState) {
                    ret += "Initial State";
                } else {
                    ret += "Entry Point";
                }

                args.Value = ret;
            };
            this.layoutControlGroup1.DataBindings.Add(binding);
            this.txtName.DataBindings.Add(new Binding(nameof(this.txtName.EditValue), entryPointShape, nameof(entryPointShape.Name), false, DataSourceUpdateMode.OnPropertyChanged));
            this.checkEdit1.DataBindings.Add(new Binding(nameof(this.checkEdit1.EditValue), entryPointShape, nameof(entryPointShape.ShowName), false, DataSourceUpdateMode.OnPropertyChanged));
            this.checkEdit2.DataBindings.Add(new Binding(nameof(this.checkEdit2.EditValue), entryPointShape, nameof(entryPointShape.Ignore), false, DataSourceUpdateMode.OnPropertyChanged));
            this.textEdit2.DataBindings.Add(new Binding(nameof(this.textEdit2.EditValue), entryPointShape, nameof(entryPointShape.Action), false, DataSourceUpdateMode.OnPropertyChanged));
            this.memoEdit1.DataBindings.Add(new Binding(nameof(this.memoEdit1.EditValue), entryPointShape, nameof(entryPointShape.Description), false, DataSourceUpdateMode.OnPropertyChanged));

            return true;
        }
    }
}
