using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.Utils.Extensions;
using DevExpress.Utils.Win;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Popup;
using DevExpress.XtraLayout;
using DevExpress.XtraLayout.Utils;

namespace OrgChartControllerExample {
    public partial class TransitionPropertiesControl : DesignablePropertiesControl {
        public TransitionPropertiesControl() {
            InitializeComponent();

            layoutControl1.SaveLayoutToStream(defaultLayout);
            defaultLayout.Seek(0, SeekOrigin.Begin);

            this.comboBoxEdit1.Properties.Items.AddRange(typeof(TransitionType).GetEnumValues());
            this.comboBoxEdit1.SelectedIndex = 0;


            this.lookUpEdit2.Properties.DataSource = new Dictionary<TimeoutUnit, string> {
                {TimeoutUnit.Milliseconds, "milliseconds"},
                {TimeoutUnit.Seconds, "seconds"},
                {TimeoutUnit.Minutes, "minutes"},
                {TimeoutUnit.Hours, "hours"},
                {TimeoutUnit.Days, "days"},
                {TimeoutUnit.Weeks, "weeks"},
                {TimeoutUnit.Months, "months"},
                {TimeoutUnit.Years, "years"}
            };
            this.lookUpEdit2.ItemIndex = 0; 

            this.lookUpEdit3.Properties.DataSource = new Dictionary<RateUnit, string> {
                {RateUnit.PerMillisecond, "per millisecond"},
                {RateUnit.PerSecond, "per second"},
                {RateUnit.PerMinute, "per minute"},
                {RateUnit.PerHour, "per hour"},
                {RateUnit.PerDay, "per day"},
                {RateUnit.PerWeek, "per week"},
                {RateUnit.PerMonth, "per month"},
                {RateUnit.PerYear, "per year"}
            };
            
            this.lookUpEdit3.ItemIndex = 0; 

            this.comboBoxEdit4.Properties.Items.AddRange(Constants.TypeList.ToArray<object>());
            this.comboBoxEdit4.SelectedItem = "string";
        }

        protected override void OnLoad(EventArgs e) {
            base.OnLoad(e);
            layoutControlItem14.ControlMinSize = radioGroup1.CalcBestSize();
        }

        public override bool Bind(IShape shape) {
            if (!base.Bind(shape)) {
                return false;
            }

            var transitionShape = (TransitionShape)shape;

            var binding = new Binding(nameof(this.layoutControlGroup1.Text), transitionShape, nameof(transitionShape.Name), true, DataSourceUpdateMode.Never);
            binding.Format += (sender, args) => {
                var ret = $"{args.Value}";
                if (!string.IsNullOrEmpty(ret)) {
                    ret += " - ";
                }

                args.Value = $"{ret}Transition";
            };
            this.layoutControlGroup1.DataBindings.Add(binding);
            this.txtName.DataBindings.Add(new Binding(nameof(this.txtName.EditValue), transitionShape, nameof(transitionShape.Name), false, DataSourceUpdateMode.OnPropertyChanged));
            this.checkEdit1.DataBindings.Add(new Binding(nameof(this.checkEdit1.EditValue), transitionShape, nameof(transitionShape.ShowName), false, DataSourceUpdateMode.OnPropertyChanged));
            this.checkEdit2.DataBindings.Add(new Binding(nameof(this.checkEdit2.EditValue), transitionShape, nameof(transitionShape.Ignore), false, DataSourceUpdateMode.OnPropertyChanged));
            this.comboBoxEdit1.DataBindings.Add(new Binding(nameof(this.comboBoxEdit1.EditValue), transitionShape, nameof(transitionShape.TransitionType), false, DataSourceUpdateMode.OnPropertyChanged));

            this.textEdit4.DataBindings.Add(new Binding(nameof(this.textEdit4.EditValue), transitionShape, nameof(transitionShape.Timeout), false, DataSourceUpdateMode.OnPropertyChanged));
            this.lookUpEdit2.DataBindings.Add(new Binding(nameof(this.lookUpEdit2.EditValue), transitionShape, nameof(transitionShape.TimeoutUnit), false, DataSourceUpdateMode.OnPropertyChanged));

            this.textEdit5.DataBindings.Add(new Binding(nameof(this.textEdit5.EditValue), transitionShape, nameof(transitionShape.Rate), false, DataSourceUpdateMode.OnPropertyChanged));
            this.lookUpEdit3.DataBindings.Add(new Binding(nameof(this.lookUpEdit3.EditValue), transitionShape, nameof(transitionShape.RateUnit), false, DataSourceUpdateMode.OnPropertyChanged));

            this.textEdit6.DataBindings.Add(new Binding(nameof(this.textEdit6.EditValue), transitionShape, nameof(transitionShape.Condition), false, DataSourceUpdateMode.OnPropertyChanged));

            this.comboBoxEdit4.DataBindings.Add(new Binding(nameof(this.comboBoxEdit4.EditValue), transitionShape, nameof(transitionShape.MessageType), false, DataSourceUpdateMode.OnPropertyChanged));
            this.radioGroup1.DataBindings.Add(new Binding(nameof(this.radioGroup1.EditValue), transitionShape, nameof(transitionShape.MessageFireTransition), false, DataSourceUpdateMode.OnPropertyChanged));

            this.textEdit2.DataBindings.Add(new Binding(nameof(this.textEdit2.EditValue), transitionShape, nameof(transitionShape.Action), false, DataSourceUpdateMode.OnPropertyChanged));
            this.textEdit3.DataBindings.Add(new Binding(nameof(this.textEdit3.EditValue), transitionShape, nameof(transitionShape.Guard), false, DataSourceUpdateMode.OnPropertyChanged));
            this.memoEdit1.DataBindings.Add(new Binding(nameof(this.memoEdit1.EditValue), transitionShape, nameof(transitionShape.Description), false, DataSourceUpdateMode.OnPropertyChanged));

            return true;
        }

        readonly MemoryStream defaultLayout = new MemoryStream();

        private void LoadLayout(ICollection<LayoutControlItem> controls) {
            // https://www.devexpress.com/Support/Center/Question/Details/T259044/layout-control-pixel-issue
            layoutControl1.RestoreLayoutFromStream(defaultLayout);
            defaultLayout.Seek(0, SeekOrigin.Begin);
            if (controls == null) {
                return;
            }
            layoutControl1.BeginUpdate();
            foreach (var item in this.layoutControl1.Items.OfType<LayoutControlItem>()) {
                if (!controls.Contains(item) && item != this.layoutControlItem1 && item != this.layoutControlItem2 && item != this.layoutControlItem3 && item != this.layoutControlItem8 && item != this.emptySpaceItem1 && item != this.layoutControlItem5 && item != this.layoutControlItem6 && item != this.layoutControlItem7) {
                    item.HideToCustomization();
                }
            }
            layoutControl1.EndUpdate();
        }

        [DllImport("user32.dll")]
        private static extern long LockWindowUpdate(IntPtr Handle);

        private void comboBoxEdit1_EditValueChanged(object sender, EventArgs e) {
            try {
                LockWindowUpdate(this.Handle);
                var value = (TransitionType) this.comboBoxEdit1.EditValue;
                if (value == TransitionType.Timeout) {
                    this.LoadLayout(new List<LayoutControlItem> {this.layoutControlItem4, this.layoutControlItem9});
                } else if (value == TransitionType.Rate) {
                    this.LoadLayout(new List<LayoutControlItem> {this.layoutControlItem10, this.layoutControlItem11});
                } else if (value == TransitionType.Condition) {
                    this.LoadLayout(new List<LayoutControlItem> {this.layoutControlItem12, this.emptySpaceItem2});
                } else if (value == TransitionType.Message) {
                    this.LoadLayout(new List<LayoutControlItem> {this.layoutControlItem13, this.layoutControlItem14});
                } else if (value == TransitionType.AgentArrival) {
                    this.LoadLayout(new List<LayoutControlItem> { });
                }

                this.comboBoxEdit1.Focus();
            } finally {
                LockWindowUpdate(IntPtr.Zero);
            }
        }


        //private void lookUpEdit2_Popup(object sender, EventArgs e) {
        //    LookUpEdit edit = (LookUpEdit)sender;
        //    PopupLookUpEditForm f = (PopupLookUpEditForm)((IPopupControl)edit).PopupWindow;
        //    f.Width = edit.Width;
        //}
    }


}
