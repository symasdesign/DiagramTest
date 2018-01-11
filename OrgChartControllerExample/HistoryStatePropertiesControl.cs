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
    public partial class HistoryStatePropertiesControl : DesignablePropertiesControl {
        public HistoryStatePropertiesControl() {
            InitializeComponent();
        }

        public override bool Bind(IShape shape) {
            if (!base.Bind(shape)) {
                return false;
            }

            var historyStateShape = (HistoryStateShape)shape;

            var binding = new Binding(nameof(this.layoutControlGroup1.Text), historyStateShape, nameof(historyStateShape.Name), true, DataSourceUpdateMode.Never);
            binding.Format += (sender, args) => {
                var ret = $"{args.Value}";
                if (!string.IsNullOrEmpty(ret)) {
                    ret += " - ";
                }

                args.Value = $"{ret}History State";
            };
            this.layoutControlGroup1.DataBindings.Add(binding);
            this.txtName.DataBindings.Add(new Binding(nameof(this.txtName.EditValue), historyStateShape, nameof(historyStateShape.Name), false, DataSourceUpdateMode.OnPropertyChanged));
            this.checkEdit1.DataBindings.Add(new Binding(nameof(this.checkEdit1.EditValue), historyStateShape, nameof(historyStateShape.ShowName), false, DataSourceUpdateMode.OnPropertyChanged));
            this.checkEdit2.DataBindings.Add(new Binding(nameof(this.checkEdit2.EditValue), historyStateShape, nameof(historyStateShape.Ignore), false, DataSourceUpdateMode.OnPropertyChanged));
            this.radioGroup1.DataBindings.Add(new Binding(nameof(this.radioGroup1.EditValue), historyStateShape, nameof(historyStateShape.IsDeep), false, DataSourceUpdateMode.OnPropertyChanged));
            this.textEdit2.DataBindings.Add(new Binding(nameof(this.textEdit2.EditValue), historyStateShape, nameof(historyStateShape.Action), false, DataSourceUpdateMode.OnPropertyChanged));
            this.memoEdit1.DataBindings.Add(new Binding(nameof(this.memoEdit1.EditValue), historyStateShape, nameof(historyStateShape.Description), false, DataSourceUpdateMode.OnPropertyChanged));

            return true;
        }
    }
}
