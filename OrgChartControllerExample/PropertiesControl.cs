using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.Utils.Drawing;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.XtraLayout;

namespace OrgChartControllerExample {
    public abstract class PropertiesControl : XtraUserControl {
        public virtual bool Bind(IShape shape) {
            return this.Bind((object)shape);
        }

        private object _model;
        public virtual bool Bind(object model) {
            if (model == this._model) {
                return false;
            }
            this.ClearBindings();
            this._model = model;
            return true;
        }

        public void Unbind() {
            this.ClearBindings();
        }

        protected virtual void ClearBindings() {
            ClearBindings(this);
            this._model = null;
        }
        private static void ClearBindings(Control c) {
            ClearBindings(c.DataBindings);
            foreach (Control cc in c.Controls) {
                ClearBindings(cc);
            }
            if (c is LayoutControl control) {
                ClearBindings(control.Root.DataBindings);
            }
        }

        private static void ClearBindings(ControlBindingsCollection b) {
            var bindings = new Binding[b.Count];
            b.CopyTo(bindings, 0);
            b.Clear();

            foreach (var binding in bindings) {
                TypeDescriptor.Refresh(binding.DataSource);
            }
        }

        protected void colorPickEdit1_Paint(object sender, PaintEventArgs e) {
            var edit = (ColorPickEdit)sender;
            var viewInfo = (ColorEditViewInfo)edit.GetViewInfo();
            var info = new GraphicsInfo();
            info.AddGraphics(e.Graphics);
            var color = edit.Color.IsEmpty ? Color.White : edit.Color;
            using (var brush = new SolidBrush(color)) {
                info.Cache.FillRectangle(brush, viewInfo.ContentRect);
            }
            if (edit.Color.IsEmpty) {
                using (var brush = new SolidBrush(Color.FromArgb(135, 146, 159))) {
                    info.Cache.DrawString("Default", DefaultFont, brush, viewInfo.ContentRect);
                }
            }
            info.ReleaseGraphics();
        }

        protected void colorPickEdit1_Properties_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e) {
            if (e.Button.Kind == ButtonPredefines.Close) {
                var edit = (ColorPickEdit)sender;
                edit.Color = Color.Empty;
            }
        }
    }

    public class DesignablePropertiesControl : PropertiesControl { }
}
