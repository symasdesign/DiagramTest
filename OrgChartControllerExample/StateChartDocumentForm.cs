using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DevExpress.Diagram.Core;
using DevExpress.XtraDiagram;
using DevExpress.XtraDiagram.Adorners;
using DevExpress.XtraEditors;
using OrgChartControllerExample.Extensions;
using DevExpress.Diagram.Core.Native;

namespace OrgChartControllerExample {
    public partial class StateChartDocumentForm : DevExpress.XtraEditors.XtraForm {

        public StateChartDocumentForm(AgentType agentType, Image img) {
            this.AgentType = agentType;
            InitializeComponent();
            this.Icon = Icon.FromHandle(new Bitmap(img).GetHicon());

            this.diagramControl1.SelectionChanged += this.diagramControl1_SelectionChanged;
            this.diagramControl1.ItemCreating += this.diagramControl1_ItemCreating;
            this.diagramControl1.ShowingEditor += DiagramControl1OnShowingEditor;
            diagramControl1.CustomDrawItem += DiagramControl1_CustomDrawItem;
            this.diagramControl1.ItemInitializing += (sender, args) => {
                if (args.Item is TransitionShape connector) {
                    connector.Type = ConnectorType.Curved;
                    //connector.Appearance.ForeColor = Color.Black;
                //} else if (args.Item is EntryPointShape entryPointShape) {
                    //entryPointShape.Appearance.ForeColor = Color.Black;
                } else if (args.Item is StateShape shape) {
                    shape.ShowName = true;
                }
                if (args.Item is StateDiagramShape sds) {
                    sds.ModifiedChanged += StateDiagramShapeOnModifiedChanged;
                } else if (args.Item is StateDiagramConnector sdc) {
                    sdc.ModifiedChanged += StateDiagramShapeOnModifiedChanged;
                } else if (args.Item is CompositeStateShape css) {
                    css.ModifiedChanged += StateDiagramShapeOnModifiedChanged;
                }

            };
            this.diagramControl1.HasChangesChanged += (sender, args) => {
                this.UpdateText();
            };
            this.AgentType.ModifiedChanged += StateDiagramShapeOnModifiedChanged;
        }

        protected override void Dispose(bool disposing) {
            if (disposing) {
                this.components?.Dispose();
                this.AgentType.ModifiedChanged -= StateDiagramShapeOnModifiedChanged;
            }
            base.Dispose(disposing);
        }

        public AgentType AgentType { get; }
        private void DiagramControl1OnShowingEditor(object o, DiagramShowingEditorEventArgs e) {
            if (e.Item is StateDiagramShape stateDiagramShape && !stateDiagramShape.ShowName) {
                e.Cancel = true;
            } else if (e.Item is StateDiagramConnector stateDiagramConnector && !stateDiagramConnector.ShowName) {
                e.Cancel = true;
            }
        }


        private void DiagramControl1_CustomDrawItem(object sender, CustomDrawItemEventArgs e) {
            if (!(e.Item is IDrawingShape drawingShape)) {
                return;
            }

            drawingShape.Draw(e);
            e.Handled = true;
        }

        private void diagramControl1_ItemCreating(object sender, DiagramItemCreatingEventArgs e) {
            if (e.ItemType == typeof(DiagramConnector)) {
                e.Item = new TransitionShape();
            }
        }

        protected override void OnLoad(EventArgs e) {
            this.UpdateText();
            base.OnLoad(e);
            this.LoadDocument(this.AgentType.LoadStateChart());
        }

        public void LoadDocument(string s) {
            if (string.IsNullOrEmpty(s)) {
                return;
            }
            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(s))) {
                this.diagramControl1.LoadDocument(stream);
            }
            this.SetAllShapesUnmodified();
            foreach (var item in this.diagramControl1.Items.OfType<StateDiagramShape>()) {
                item.ModifiedChanged += StateDiagramShapeOnModifiedChanged;
            }
            foreach (var item in this.diagramControl1.Items.OfType<StateDiagramConnector>()) {
                item.ModifiedChanged += StateDiagramShapeOnModifiedChanged;
            }
            foreach (var item in this.diagramControl1.Items.OfType<CompositeStateShape>()) {
                item.ModifiedChanged += StateDiagramShapeOnModifiedChanged;
            }
        }

        private void SetAllShapesUnmodified() {
            foreach (var item in this.diagramControl1.Items.OfType<StateDiagramShape>()) {
                item.Modified = false;
            }
            foreach (var item in this.diagramControl1.Items.OfType<StateDiagramConnector>()) {
                item.Modified = false;
            }
            foreach (var item in this.diagramControl1.Items.OfType<CompositeStateShape>()) {
                item.Modified = false;
            }

            this.AgentType.Modified = false;
        }
        private void StateDiagramShapeOnModifiedChanged(object o, EventArgs eventArgs) {
            this.UpdateText();
        }

        private bool hasChanged;
        private void UpdateText() {
            this.Text = AgentType.Content;
            if (this.HasChanges) {
                this.Text += "*";                
            }
            if (this.hasChanged != this.HasChanges) {
                this.hasChanged = this.HasChanges;
                this.OnHasChangesChanged(EventArgs.Empty);
            }
        }

        public event EventHandler HasChangesChanged;
        public bool HasChanges => this.diagramControl1.HasChanges || this.diagramControl1.Items.OfType<StateDiagramShape>().Any(x => x.Modified) || this.diagramControl1.Items.OfType<StateDiagramConnector>().Any(x => x.Modified) || this.diagramControl1.Items.OfType<CompositeStateShape>().Any(x => x.Modified) || this.AgentType.Modified;

        protected virtual void OnHasChangesChanged(EventArgs e) {
            this.HasChangesChanged?.Invoke(this, e);
        }
        public DiagramControl DiagramControl => this.diagramControl1;

        public Form1 Mainform => (Form1) this.MdiParent;

        private void diagramControl1_SelectionChanged(object sender, DiagramSelectionChangedEventArgs e) {
            if (this.Disposing) {
                return;
            }
            var shape = this.DiagramControl.SelectedItems.LastOrDefault() as IShape;

            switch (shape) {
                case null:
                    this.Mainform.ShowAgentTypeProperties(this.AgentType);
                    break;
                case EntryPointShape _:
                    this.Mainform.ShowEntryPointProperties((EntryPointShape)shape);
                    break;
                case StateShape _:
                    this.Mainform.ShowStateProperties((StateShape)shape);
                    break;
                case CompositeStateShape _:
                    this.Mainform.ShowCompositeStateProperties((CompositeStateShape)shape);
                    break;
                case TransitionShape _:
                    this.Mainform.ShowTransitionProperties((TransitionShape)shape);
                    break;
                case BranchShape _:
                    this.Mainform.ShowBranchProperties((BranchShape)shape);
                    break;
                case HistoryStateShape _:
                    this.Mainform.ShowHistoryStateProperties((HistoryStateShape)shape);
                    break;
                case FinalStateShape _:
                    this.Mainform.ShowFinalStateProperties((FinalStateShape)shape);
                    break;
            }
        }

        public void Save() {
            if (this.HasChanges) {
                using (var stream = new MemoryStream()) {
                    this.diagramControl1.SaveDocument(stream);
                    this.AgentType.SaveStateChart(stream.GetString());
                }
                this.SetAllShapesUnmodified();
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e) {
            if (e.CloseReason == CloseReason.UserClosing && this.HasChanges) {
                switch (XtraMessageBox.Show($"Do you want to save the changes you made to {this.AgentType.Content}?", "Save Changes", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question)) {
                    case DialogResult.Cancel:
                        e.Cancel = true;
                        return;
                    case DialogResult.Yes:
                        this.Save();
                        break;
                }
            }


            base.OnFormClosing(e);
        }
    }

    public class DiagramControlEx : DiagramControl, IDiagramControl {
        IAdornerFactory IDiagramControl.CreateAdornerFactory() {
            return new DiagramAdornerFactoryEx(this);
        }
    }

    public class DiagramInplaceEditorAdornerEx : DiagramInplaceEditorAdorner {
        public DiagramInplaceEditorAdornerEx(DiagramItem item, IAdornerController controller) : base(item, controller) { }

        protected override RectangleF CalcAdornerDisplayBounds() {
            if (!(this.Item is TransitionShape connector)) {
                return base.CalcAdornerDisplayBounds();
            }

            PointF connectorRelatedTextPosition = StateDiagramConnector.GetTextPosition(connector);
            System.Windows.Point controlConnectorPoint = Diagram.PointToControl(connector.DiagramPosition());
            return new RectangleF(connectorRelatedTextPosition.X + (float)controlConnectorPoint.X, connectorRelatedTextPosition.Y + (float)controlConnectorPoint.Y, 50, 20);
        }
    }
    public class DiagramAdornerFactoryEx : DiagramAdornerFactory, IAdornerFactory {
        public DiagramAdornerFactoryEx(DiagramControl diagram) : base(diagram) { }
        IAdorner<IInplaceEditorAdorner> IAdornerFactory.CreateInplaceEditor(IDiagramItem item) {
            // https://www.devexpress.com/Support/Center/Question/Details/T593109/position-contenteditor-of-connector#answer-fc560ba7-c01b-4880-bd5c-df15cf6c96bb
            // Please note that the DiagramInplaceEditorAdorner class is not a part of our public API and may be modified in future releases.

            return GetController().Create(x => new DiagramInplaceEditorAdornerEx((DiagramItem)item, x));
        }
    }
}
