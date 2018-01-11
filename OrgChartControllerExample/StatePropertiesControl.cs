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
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.ViewInfo;

namespace OrgChartControllerExample {
    public partial class StatePropertiesControl : DesignablePropertiesControl {
        public StatePropertiesControl() {
            InitializeComponent();
            this.colorPickEdit1.Properties.ButtonClick += this.colorPickEdit1_Properties_ButtonClick;
            this.colorPickEdit1.Paint += this.colorPickEdit1_Paint;
            this.colorPickEdit1.Color = Color.White;
        }

        private void StatePropertiesControl_Load(object sender, EventArgs e) {

        }

        public override bool Bind(IShape shape) {
            if (!base.Bind(shape)) {
                return false;
            }

            if (shape is StateShape) {
                this.BindState((StateShape)shape);
            } else if (shape is CompositeStateShape) {
                this.BindCompositeState((CompositeStateShape) shape);
            }

            return true;
        }

        private void BindState(StateShape stateShape) {
            var binding = new Binding(nameof(this.layoutControlGroup1.Text), stateShape, nameof(stateShape.Name), true, DataSourceUpdateMode.Never);
            binding.Format += (sender, args) => {
                var ret = $"{args.Value}";
                if (!string.IsNullOrEmpty(ret)) {
                    ret += " - ";
                }
                args.Value = $"{ret}State";
            };
            this.layoutControlGroup1.DataBindings.Add(binding);
            this.txtName.DataBindings.Add(new Binding(nameof(this.txtName.EditValue), stateShape, nameof(stateShape.Name), false, DataSourceUpdateMode.OnPropertyChanged));
            this.checkEdit1.DataBindings.Add(new Binding(nameof(this.checkEdit1.EditValue), stateShape, nameof(stateShape.ShowName), false, DataSourceUpdateMode.OnPropertyChanged));
            this.checkEdit2.DataBindings.Add(new Binding(nameof(this.checkEdit2.EditValue), stateShape, nameof(stateShape.Ignore), false, DataSourceUpdateMode.OnPropertyChanged));
            this.colorPickEdit1.DataBindings.Add(new Binding(nameof(this.colorPickEdit1.Color), stateShape, nameof(stateShape.Color), false, DataSourceUpdateMode.OnPropertyChanged));
            this.textEdit2.DataBindings.Add(new Binding(nameof(this.textEdit2.EditValue), stateShape, nameof(stateShape.EntryAction), false, DataSourceUpdateMode.OnPropertyChanged));
            this.textEdit3.DataBindings.Add(new Binding(nameof(this.textEdit3.EditValue), stateShape, nameof(stateShape.ExitAction), false, DataSourceUpdateMode.OnPropertyChanged));
            this.memoEdit1.DataBindings.Add(new Binding(nameof(this.memoEdit1.EditValue), stateShape, nameof(stateShape.Description), false, DataSourceUpdateMode.OnPropertyChanged));
        }

        private void BindCompositeState(CompositeStateShape stateShape) {

            var binding = new Binding(nameof(this.layoutControlGroup1.Text), stateShape, nameof(stateShape.Name), true, DataSourceUpdateMode.Never);
            binding.Format += (sender, args) => {
                var ret = $"{args.Value}";
                if (!string.IsNullOrEmpty(ret)) {
                    ret += " - ";
                }
                args.Value = $"{ret}Composite State";
            };
            this.layoutControlGroup1.DataBindings.Add(binding);
            this.txtName.DataBindings.Add(new Binding(nameof(this.txtName.EditValue), stateShape, nameof(stateShape.Name), false, DataSourceUpdateMode.OnPropertyChanged));
            this.checkEdit1.DataBindings.Add(new Binding(nameof(this.checkEdit1.EditValue), stateShape, nameof(stateShape.ShowName), false, DataSourceUpdateMode.OnPropertyChanged));
            this.checkEdit2.DataBindings.Add(new Binding(nameof(this.checkEdit2.EditValue), stateShape, nameof(stateShape.Ignore), false, DataSourceUpdateMode.OnPropertyChanged));
            this.colorPickEdit1.DataBindings.Add(new Binding(nameof(this.colorPickEdit1.Color), stateShape, nameof(stateShape.Color), false, DataSourceUpdateMode.OnPropertyChanged));
            this.textEdit2.DataBindings.Add(new Binding(nameof(this.textEdit2.EditValue), stateShape, nameof(stateShape.EntryAction), false, DataSourceUpdateMode.OnPropertyChanged));
            this.textEdit3.DataBindings.Add(new Binding(nameof(this.textEdit3.EditValue), stateShape, nameof(stateShape.ExitAction), false, DataSourceUpdateMode.OnPropertyChanged));
            this.memoEdit1.DataBindings.Add(new Binding(nameof(this.memoEdit1.EditValue), stateShape, nameof(stateShape.Description), false, DataSourceUpdateMode.OnPropertyChanged));
        }
    }
}
