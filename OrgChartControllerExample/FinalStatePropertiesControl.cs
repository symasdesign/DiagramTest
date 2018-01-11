using System.Windows.Forms;

namespace OrgChartControllerExample {
    public partial class FinalStatePropertiesControl : DesignablePropertiesControl {
        public FinalStatePropertiesControl() {
            InitializeComponent();
        }

        public override bool Bind(IShape shape) {
            if (!base.Bind(shape)) {
                return false;
            }

            var finalStateShape = (FinalStateShape)shape;

            var binding = new Binding(nameof(this.layoutControlGroup1.Text), finalStateShape, nameof(finalStateShape.Name), true, DataSourceUpdateMode.Never);
            binding.Format += (sender, args) => {
                var ret = $"{args.Value}";
                if (!string.IsNullOrEmpty(ret)) {
                    ret += " - ";
                }

                args.Value = $"{ret}Final State";
            };
            this.layoutControlGroup1.DataBindings.Add(binding);
            this.txtName.DataBindings.Add(new Binding(nameof(this.txtName.EditValue), finalStateShape, nameof(finalStateShape.Name), false, DataSourceUpdateMode.OnPropertyChanged));
            this.checkEdit1.DataBindings.Add(new Binding(nameof(this.checkEdit1.EditValue), finalStateShape, nameof(finalStateShape.ShowName), false, DataSourceUpdateMode.OnPropertyChanged));
            this.checkEdit2.DataBindings.Add(new Binding(nameof(this.checkEdit2.EditValue), finalStateShape, nameof(finalStateShape.Ignore), false, DataSourceUpdateMode.OnPropertyChanged));
            this.textEdit2.DataBindings.Add(new Binding(nameof(this.textEdit2.EditValue), finalStateShape, nameof(finalStateShape.Action), false, DataSourceUpdateMode.OnPropertyChanged));
            this.memoEdit1.DataBindings.Add(new Binding(nameof(this.memoEdit1.EditValue), finalStateShape, nameof(finalStateShape.Description), false, DataSourceUpdateMode.OnPropertyChanged));

            return true;
        }
    }
}
