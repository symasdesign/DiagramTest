using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OrgChartControllerExample {
    public partial class AgentTypePropertiesControl : DesignablePropertiesControl {
        public AgentTypePropertiesControl() {
            InitializeComponent();
        }

        public override bool Bind(IShape shape) {
            throw new NotSupportedException();
        }

        protected override void ClearBindings() {
            base.ClearBindings();
            this._agentType = null;
        }

        private AgentType _agentType;
        public bool Bind(AgentType agentType) {
            if (this._agentType == agentType) {
                return false;
            }
            this.ClearBindings();

            this._agentType = agentType;

            var binding = new Binding(nameof(this.layoutControlGroup1.Text), agentType, nameof(agentType.Content), true, DataSourceUpdateMode.Never);
            binding.Format += (sender, args) => {
                var ret = $"{args.Value}";
                if (!string.IsNullOrEmpty(ret)) {
                    ret += " - ";
                }
                args.Value = $"{ret}Agent Type";
            };
            this.layoutControlGroup1.DataBindings.Add(binding);
            this.textEdit1.DataBindings.Add(new Binding(nameof(this.textEdit1.EditValue), agentType, nameof(agentType.OnStartupAction), false, DataSourceUpdateMode.OnPropertyChanged));
            this.textEdit2.DataBindings.Add(new Binding(nameof(this.textEdit2.EditValue), agentType, nameof(agentType.OnDestroyAction), false, DataSourceUpdateMode.OnPropertyChanged));
            this.textEdit3.DataBindings.Add(new Binding(nameof(this.textEdit3.EditValue), agentType, nameof(agentType.OnArrivalAction), false, DataSourceUpdateMode.OnPropertyChanged));
            this.textEdit4.DataBindings.Add(new Binding(nameof(this.textEdit4.EditValue), agentType, nameof(agentType.OnBeforeStepAction), false, DataSourceUpdateMode.OnPropertyChanged));
            this.textEdit5.DataBindings.Add(new Binding(nameof(this.textEdit5.EditValue), agentType, nameof(agentType.OnStepAction), false, DataSourceUpdateMode.OnPropertyChanged));

            return true;
        }

    }


}
