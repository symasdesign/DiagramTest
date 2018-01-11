using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraGrid.Views.Grid;
using OrgChartControllerExample.Extensions;

namespace OrgChartControllerExample {
    public partial class FunctionPropertiesControl : DesignablePropertiesControl {
        public FunctionPropertiesControl() {
            InitializeComponent();

            this.comboBoxEdit1.Properties.Items.AddRange(Constants.TypeList.ToArray<object>());
            this.comboBoxEdit1.SelectedItem = "string";

            this.repositoryItemComboBox1.Items.AddRange(Constants.TypeList.ToArray<object>());
        }

        public override bool Bind(IShape shape) {
            throw new NotSupportedException();
        }

        protected override void ClearBindings() {
            base.ClearBindings();
            this._function = null;
        }

        private Function _function;

        public bool Bind(Function function) {
            if (this._function == function) {
                return false;
            }
            this.ClearBindings();

            this._function = function;

            var binding = new Binding(nameof(this.layoutControlGroup1.Text), function, nameof(function.Name), true, DataSourceUpdateMode.Never);
            binding.Format += (sender, args) => {
                var ret = $"{args.Value}";
                if (!string.IsNullOrEmpty(ret)) {
                    ret += " - ";
                }

                args.Value = $"{ret}Function";
            };
            this.layoutControlGroup1.DataBindings.Add(binding);
            this.txtName.DataBindings.Add(new Binding(nameof(this.txtName.EditValue), function, nameof(function.Name), false, DataSourceUpdateMode.OnPropertyChanged));
            this.checkEdit2.DataBindings.Add(new Binding(nameof(this.checkEdit2.EditValue), function, nameof(function.Ignore), false, DataSourceUpdateMode.OnPropertyChanged));
            var dt = new DataTable("arguments");
            dt.Columns.Add("id", typeof(string));
            dt.Columns.Add("name", typeof(string));
            dt.Columns.Add("type", typeof(string));
            dt.Columns.Add("position", typeof(int));
            foreach (var argument in function.Arguments) {
                dt.Rows.Add(argument.Id, argument.Name, argument.Type, argument.Position);
            }
            dt.AcceptChanges();

            dt.TableNewRow += DtOnTableNewRow;
            dt.RowChanged += DtOnRowChanged;
            dt.RowDeleted += DtOnRowDeleted;

            this.gridControl1.DataSource = dt;

            this.comboBoxEdit1.DataBindings.Add(new Binding(nameof(this.comboBoxEdit1.EditValue), function, nameof(function.ReturnType), false, DataSourceUpdateMode.OnPropertyChanged));
            this.memoEdit1.DataBindings.Add(new Binding(nameof(this.memoEdit1.EditValue), function, nameof(function.Description), false, DataSourceUpdateMode.OnPropertyChanged));
            //this.textEdit2.DataBindings.Add(new Binding(nameof(this.textEdit2.EditValue), agentType, nameof(agentType.OnDestroyAction), false, DataSourceUpdateMode.OnPropertyChanged));
            //this.textEdit3.DataBindings.Add(new Binding(nameof(this.textEdit3.EditValue), agentType, nameof(agentType.OnArrivalAction), false, DataSourceUpdateMode.OnPropertyChanged));
            //this.textEdit4.DataBindings.Add(new Binding(nameof(this.textEdit4.EditValue), agentType, nameof(agentType.OnBeforeStepAction), false, DataSourceUpdateMode.OnPropertyChanged));
            //this.textEdit5.DataBindings.Add(new Binding(nameof(this.textEdit5.EditValue), agentType, nameof(agentType.OnStepAction), false, DataSourceUpdateMode.OnPropertyChanged));
            return true;
        }

        private void DtOnTableNewRow(object o, DataTableNewRowEventArgs e) {
            e.Row["id"] = Guid.NewGuid().ToString();
            var rows = e.Row.Table.Rows.OfType<DataRow>().Where(x => x.RowState != DataRowState.Deleted).ToList();
            e.Row["position"] = rows.Any() ? rows.Max(x => x.GetValue<int>("position")) + 1 : 1;
        }

        private bool dtEventsSuspended;

        private void DtOnRowDeleted(object o, DataRowChangeEventArgs e) {
            if (this.dtEventsSuspended) {
                return;
            }

            try {
                this.dtEventsSuspended = true;
                var argument = this._function.Arguments.First(x => x.Id == (string) e.Row["id", DataRowVersion.Original]);
                this._function.Arguments.Remove(argument);
            } finally {
                this.dtEventsSuspended = false;
            }
        }

        private void DtOnRowChanged(object o, DataRowChangeEventArgs e) {
            if (this.dtEventsSuspended) {
                return;
            }

            try {
                this.dtEventsSuspended = true;
                var argument = this._function.Arguments.FirstOrDefault(x => x.Id == (string)e.Row.GetValue<string>("id"));
                if (argument == null) {
                    argument = new Function.Argument() {Id = e.Row.GetValue<string>("Id")};
                    this._function.Arguments.Add(argument);
                }

                argument.Name = e.Row.GetValue<string>("name");
                argument.Type = e.Row.GetValue<string>("type");
                argument.Position = e.Row.GetValue<int>("position");
                
                e.Row.Table.AcceptChanges();
            } finally {
                this.dtEventsSuspended = false;
            }
        }

        private void gridView1_ValidatingEditor(object sender, BaseContainerValidateEditorEventArgs e) {
            var view = (GridView) sender;
            if (view.FocusedColumn == this.gridColumn1) {
                var name = e.Value as string;
                if (string.IsNullOrEmpty(name)) {
                    e.Valid = false;
                    e.ErrorText = "Name is required!";
                    return;
                }

                var row = ((DataRowView)view.GetRow(view.FocusedRowHandle)).Row;
                foreach (DataRow r in row.Table.Rows.OfType<DataRow>().Where(x => x.RowState != DataRowState.Deleted)) {
                    if (r != row && r.GetValue<string>("name") == name) {
                        e.Valid = false;
                        e.ErrorText = "Name must be unique!";
                        return;
                    }
                }
            } else if (view.FocusedColumn == this.gridColumn2) {
                var type = e.Value as string;
                if (string.IsNullOrEmpty(type)) {
                    e.Valid = false;
                    e.ErrorText = "Type is required!";
                    return;
                }
            }
        }

        private void comboBoxEdit1_Properties_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e) {
            if (e.Button.Kind == ButtonPredefines.Close) {
                this.comboBoxEdit1.EditValue = "";
                this.labelControl1.Focus();
            }
        }

        private void gridControl1_EmbeddedNavigator_ButtonClick(object sender, DevExpress.XtraEditors.NavigatorButtonClickEventArgs e) {
            var type = e.Button.Tag as string;
            switch (type) {
                case "first": {
                    var row = this.gridView1.GetDataRow(this.gridView1.FocusedRowHandle);
                    row["Position"] = -1;
                    RenumberArguments(row.Table);
                    break;
                }
                case "last": {
                    var row = this.gridView1.GetDataRow(this.gridView1.FocusedRowHandle);
                    row["Position"] = row.Table.Rows.OfType<DataRow>().Where(x => x.RowState != DataRowState.Deleted).Max(x => x.GetValue<int>("position"))+1;
                    RenumberArguments(row.Table);
                    break;
                }
                case "up": {
                    var row = this.gridView1.GetDataRow(this.gridView1.FocusedRowHandle);
                    var pos = row.GetValue<int>("position");
                    var prev = GetPrevArgumentRow(row);
                    if (prev != null) {
                        row["position"] = prev.GetValue<int>("position");
                        prev["position"] = pos;
                    }
                    break;                    
                }
                case "down": {
                    var row = this.gridView1.GetDataRow(this.gridView1.FocusedRowHandle);
                    var pos = row.GetValue<int>("position");
                    var next = GetNextArgumentRow(row);
                    if (next != null) {
                        row["position"] = next.GetValue<int>("position");
                        next["position"] = pos;
                    }
                    break;
                }
            }
        }

        private static DataRow GetPrevArgumentRow(DataRow row) {
            var pos = row.GetValue<int>("position");
            return row.Table.Rows.OfType<DataRow>().Where(x => x.RowState != DataRowState.Deleted && x.GetValue<int>("position") < pos).OrderBy(x => x.GetValue<int>("position")).LastOrDefault();
        }

        private static DataRow GetNextArgumentRow(DataRow row) {
            var pos = row.GetValue<int>("position");
            return row.Table.Rows.OfType<DataRow>().Where(x => x.RowState != DataRowState.Deleted && x.GetValue<int>("position") > pos).OrderBy(x => x.GetValue<int>("position")).FirstOrDefault();
        }

        private static void RenumberArguments(DataTable dt) {
            var cnt = 1;
            foreach (var r in dt.Rows.OfType<DataRow>().Where(x => x.RowState != DataRowState.Deleted).OrderBy(x => x.GetValue<int>("Position")).ToList()) {
                r["Position"] = cnt;
                cnt++;
            }
        }

        private void gridView1_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e) {
            var row = this.gridView1.GetDataRow(this.gridView1.FocusedRowHandle);
            foreach (NavigatorCustomButton btn in this.gridControl1.EmbeddedNavigator.Buttons.CustomButtons) {
                var type = btn.Tag as string;
                if (type == "first" || type == "up") {
                    btn.Enabled = row != null && GetPrevArgumentRow(row) != null;
                } else if (type == "last" || type == "down") {
                    btn.Enabled = row != null && GetNextArgumentRow(row) != null;
                }
            }
        }
    }
}
