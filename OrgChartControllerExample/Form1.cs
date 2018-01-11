using DevExpress.Diagram.Core;
using DevExpress.XtraDiagram;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Security.Permissions;
using System.Text;
using System.Windows.Forms;
using DevExpress.Diagram.Core.Native;
using DevExpress.Utils;
using DevExpress.Utils.Serializing;
using DevExpress.XtraBars;
using DevExpress.XtraEditors;
using DevExpress.XtraTreeList;
using OrgChartControllerExample.Annotations;
using OrgChartControllerExample.Extensions;
using OrgChartControllerExample.Properties;

namespace OrgChartControllerExample {
    public partial class Form1 : XtraForm {

        private readonly CustomTreeListNodeData rootNode = new CustomTreeListNodeData(null, null);
        private CustomTreeListNodeData rootFunctionsNode;

        static Form1() {
            // *** Register Shape-Types for deserialization
            //DiagramControl.ItemTypeRegistrator.Register(typeof(AgentType));
            DiagramControl.ItemTypeRegistrator.Register(typeof(EntryPointShape));
            DiagramControl.ItemTypeRegistrator.Register(typeof(StateShape));
            DiagramControl.ItemTypeRegistrator.Register(typeof(TransitionShape));
            DiagramControl.ItemTypeRegistrator.Register(typeof(BranchShape));
            DiagramControl.ItemTypeRegistrator.Register(typeof(HistoryStateShape));
            DiagramControl.ItemTypeRegistrator.Register(typeof(FinalStateShape));
            DiagramControl.ItemTypeRegistrator.Register(typeof(CompositeStateShape));
        }

        public Form1() {
            InitializeComponent();
            this.LoadAgents();
        }

        protected override void OnLoad(EventArgs e) {
            base.OnLoad(e);
            this.EnableDisableDiagramRibbon(false);
        }

        private void EnableDisableDiagramRibbon(bool enable) {
            if (!enable) {
                this.diagramCommandSelectPointerToolBarCheckItem1.Control = null;
                this.diagramCommandSelectPanToolBarCheckItem1.Control = null;
                this.diagramCommandSelectConnectorToolBarCheckItem1.Control = null;
            } else {
                var diagramControl = this.CurrentDiagramControl;
                this.diagramCommandSelectPointerToolBarCheckItem1.Control = diagramControl;
                this.diagramCommandSelectPanToolBarCheckItem1.Control = diagramControl;
                this.diagramCommandSelectConnectorToolBarCheckItem1.Control = diagramControl;
            }

            RibbonHelper.ToggleRibbonPageGroupItems(this.shapePageGroup, enable);
        }

        private IModel model;

        private void LoadAgents() {
            this.model = new DummyModel {Name = "TestModel3"};
            var nodeData = new NodeData(NodeData.NodeType.Model, "TestModel3", model);
            var treeNodeData = new CustomTreeListNodeData(this.rootNode, nodeData);

            var agentTypes = new[] {"EntController", "EntPart", "Visitor1"};
            foreach (var item in agentTypes) {
                var agentType = new AgentType(new DummyIntelligentObject {ObjectName = item});
                agentType.Load();
                nodeData = new NodeData(NodeData.NodeType.Agent, item, agentType);
                nodeData.PropertyChanged += NodeDataOnPropertyChanged;
                new CustomTreeListNodeData(treeNodeData, nodeData);
            }


            //agentType = new AgentType(new DummyIntelligentObject {ObjectName = "EntPart"});
            //agentType.Load();
            //nodeData = new NodeData(NodeData.NodeType.Agent, "EntPart", agentType);
            //nodeData.PropertyChanged += NodeDataOnPropertyChanged;
            //new CustomTreeListNodeData(treeNodeData, nodeData);

            //agentType = new AgentType(new DummyIntelligentObject {ObjectName = "Visitor1"});
            //agentType.Load();
            //nodeData = new NodeData(NodeData.NodeType.Agent, "Visitor1", agentType);
            //nodeData.PropertyChanged += NodeDataOnPropertyChanged;
            //new CustomTreeListNodeData(treeNodeData, nodeData);

            nodeData = new NodeData(NodeData.NodeType.FunctionGroup, "Functions", null);
            this.rootFunctionsNode = new CustomTreeListNodeData(this.rootNode, nodeData);

            //var functions = new[] { "Function1", "Function2", "Function3" };
            var functions = Function.Load();
            foreach (var item in functions) {
                //var function = new Function(item, model);
                //function.Load();
                nodeData = new NodeData(NodeData.NodeType.Function, item.Name, item);
                nodeData.PropertyChanged += NodeDataOnPropertyChanged;
                new CustomTreeListNodeData(this.rootFunctionsNode, nodeData);
            }

            this.objectTreeList.DataSource = this.rootNode;
            this.objectTreeList.ExpandAll();
        }

        private void NodeDataOnPropertyChanged(object sender1, PropertyChangedEventArgs propertyChangedEventArgs) {
            this.objectTreeList.RefreshDataSource();
            this.EnableDisableSaveBarButtonItems();
        }


        private StateChartDocumentForm CurrentStateChartDocumentForm {
            get {
                var page = this.xtraTabbedMdiManager1.SelectedPage;
                return page?.MdiChild as StateChartDocumentForm;
            }
        }

        private FunctionForm CurrentFunctionForm {
            get {
                var page = this.xtraTabbedMdiManager1.SelectedPage;
                return page?.MdiChild as FunctionForm;
            }
        }

        private DiagramControl CurrentDiagramControl => this.CurrentStateChartDocumentForm?.DiagramControl;

        private void bbiState_ItemPress(object sender, DevExpress.XtraBars.ItemClickEventArgs e) {
            //diagramControl1.Commands.Execute(DiagramCommands.StartDragToolCommand, new ShapeTool(BasicShapes.Rectangle));
            var tool = new FactoryItemTool("state", () => "state", (x) => new StateShape(), new System.Windows.Size(110, 50)) as DiagramTool;
            BeginInvoke((Action) (() => this.CurrentDiagramControl.Commands.Execute(DiagramCommandsBase.StartDragToolCommand, tool)));
        }

        private void bbiEntryPoint_ItemPress(object sender, DevExpress.XtraBars.ItemClickEventArgs e) {
            var tool = new FactoryItemTool("entryPoint", () => "entryPoint", diagram => new EntryPointShape() {Type = ConnectorType.Straight, Width = 100, Height = 75});
            BeginInvoke((Action) (() => this.CurrentDiagramControl.Commands.Execute(DiagramCommandsBase.StartDragToolCommand, tool)));
        }

        private void bbiFinalState_ItemPress(object sender, DevExpress.XtraBars.ItemClickEventArgs e) {
            var tool = new FactoryItemTool("finalState", () => "finalState", diagram => new FinalStateShape() {Type = ConnectorType.Straight, Width = 100, Height = 75});
            BeginInvoke((Action) (() => this.CurrentDiagramControl.Commands.Execute(DiagramCommandsBase.StartDragToolCommand, tool)));
        }

        private void bbiBranch_ItemPress(object sender, DevExpress.XtraBars.ItemClickEventArgs e) {
            var tool = new FactoryItemTool("branch", () => "branch", (x) => new BranchShape(), new System.Windows.Size(50, 50)) as DiagramTool;
            BeginInvoke((Action) (() => this.CurrentDiagramControl.Commands.Execute(DiagramCommandsBase.StartDragToolCommand, tool)));
        }

        private void bbiCompositeState_ItemPress(object sender, DevExpress.XtraBars.ItemClickEventArgs e) {
            var tool = new FactoryItemTool("compositeState", () => "compositeState", (x) => new CompositeStateShape(), new System.Windows.Size(50, 50)) as DiagramTool;
            BeginInvoke((Action) (() => this.CurrentDiagramControl.Commands.Execute(DiagramCommandsBase.StartDragToolCommand, tool)));

        }

        private void bbiHistoryState_ItemPress(object sender, DevExpress.XtraBars.ItemClickEventArgs e) {
            var tool = new FactoryItemTool("historyState", () => "historyState", (x) => new HistoryStateShape()) as DiagramTool;
            BeginInvoke((Action) (() => this.CurrentDiagramControl.Commands.Execute(DiagramCommandsBase.StartDragToolCommand, tool)));
        }

        public void ShowAgentTypeProperties(AgentType shape) {
            this.ShowPropertiesControl(this.agentTypePropertiesControl1, shape);
        }

        public void ShowEntryPointProperties(EntryPointShape shape) {
            this.ShowPropertiesControl(this.entryPointPropertiesControl1, shape);
        }

        public void ShowStateProperties(StateShape shape) {
            this.ShowPropertiesControl(this.statePropertiesControl1, shape);
        }

        public void ShowCompositeStateProperties(CompositeStateShape shape) {
            this.ShowPropertiesControl(this.statePropertiesControl1, shape);
        }

        public void ShowTransitionProperties(TransitionShape shape) {
            this.ShowPropertiesControl(this.transitionPropertiesControl1, shape);
        }

        public void ShowBranchProperties(BranchShape shape) {
            this.ShowPropertiesControl(this.branchPropertiesControl1, shape);
        }

        public void ShowHistoryStateProperties(HistoryStateShape shape) {
            this.ShowPropertiesControl(this.historyStatePropertiesControl1, shape);
        }

        public void ShowFinalStateProperties(FinalStateShape shape) {
            this.ShowPropertiesControl(this.finalStatePropertiesControl1, shape);
        }

        public void ShowFunctionProperties(Function function) {
            this.ShowPropertiesControl(this.functionPropertiesControl1, function);
        }

        public void HideAllProperties() {
            try {
                LockWindowUpdate(this.Handle);
                this.UnbindAllPropertiesControl();
                this.ShowPropertiesControl(null);
            } finally {
                LockWindowUpdate(IntPtr.Zero);
            }
        }

        [DllImport("user32.dll")]
        private static extern long LockWindowUpdate(IntPtr Handle);

        private void ShowPropertiesControl(PropertiesControl control, IShape shape) {
            try {
                LockWindowUpdate(this.Handle);
                control.Bind(shape);
                this.ShowPropertiesControl(control);
            } finally {
                LockWindowUpdate(IntPtr.Zero);
            }
        }

        private void ShowPropertiesControl(AgentTypePropertiesControl control, AgentType agentType) {
            try {
                LockWindowUpdate(this.Handle);
                control.Bind(agentType);
                this.ShowPropertiesControl(control);
            } finally {
                LockWindowUpdate(IntPtr.Zero);
            }
        }

        private void ShowPropertiesControl(FunctionPropertiesControl control, Function function) {
            try {
                LockWindowUpdate(this.Handle);
                control.Bind(function);
                this.ShowPropertiesControl(control);                
            } finally {
                LockWindowUpdate(IntPtr.Zero);
            }
        }

        private void ShowPropertiesControl(PropertiesControl control) {
            this.agentTypePropertiesControl1.Visible = control == this.agentTypePropertiesControl1;
            this.entryPointPropertiesControl1.Visible = control == this.entryPointPropertiesControl1;
            this.statePropertiesControl1.Visible = control == this.statePropertiesControl1;
            this.transitionPropertiesControl1.Visible = control == this.transitionPropertiesControl1;
            this.branchPropertiesControl1.Visible = control == this.branchPropertiesControl1;
            this.historyStatePropertiesControl1.Visible = control == this.historyStatePropertiesControl1;
            this.finalStatePropertiesControl1.Visible = control == this.finalStatePropertiesControl1;
            this.functionPropertiesControl1.Visible = control == this.functionPropertiesControl1;
        }

        private void UnbindAllPropertiesControl() {
            this.agentTypePropertiesControl1.Unbind();
            this.entryPointPropertiesControl1.Unbind();
            this.statePropertiesControl1.Unbind();
            this.transitionPropertiesControl1.Unbind();
            this.branchPropertiesControl1.Unbind();
            this.historyStatePropertiesControl1.Unbind();
            this.finalStatePropertiesControl1.Unbind();
            this.functionPropertiesControl1.Unbind();
        }

        public class CustomTreeListNodeData : TreeList.IVirtualTreeListData {
            protected CustomTreeListNodeData Parent;
            public NodeData Data;
            public List<CustomTreeListNodeData> Children = new List<CustomTreeListNodeData>();

            public CustomTreeListNodeData(CustomTreeListNodeData parentNodeData, NodeData cellInfo) {
                this.Parent = parentNodeData;
                this.Data = cellInfo;
                this.Parent?.Children.Add(this);
            }

            public void VirtualTreeGetCellValue(VirtualTreeGetCellValueInfo info) {
                try {
                    info.CellData = this.Data.GetType().InvokeMember(info.Column.FieldName, System.Reflection.BindingFlags.GetProperty, null, this.Data, null);
                } catch (Exception) {
                    info.CellData = null;
                }
            }

            public void VirtualTreeGetChildNodes(VirtualTreeGetChildNodesInfo info) {
                info.Children = this.Children;
            }

            public void VirtualTreeSetCellValue(VirtualTreeSetCellValueInfo info) {
                //no need to implement this.. editing in the treelist is not allowed - as per design
            }
        }

        [Serializable]
        public class NodeData : INotifyPropertyChanged {
            public NodeData(NodeType type, string name, object obj) {
                this.Type = type;
                this.Name = name;
                this.Object = obj;

                if (obj is AgentType at) {
                    at.ModifiedChanged += (sender, args) => {
                        var agentType = ((AgentType) sender);
                        this.modified = agentType.Modified;
                        this.ReloadName(this._name);
                    };
                    this.modified = at.Modified;
                } else if (obj is Function f) {
                    f.PropertyChanged += this.F_PropertyChanged;

                    f.ModifiedChanged += (sender, args) => {
                        var function = (Function) sender;
                        this.modified = function.Modified;
                        this.ReloadName(function.Name);
                    };
                    this.modified = f.Modified;
                }
            }

            private void F_PropertyChanged(object sender, PropertyChangedEventArgs e) {
                if (e.PropertyName == nameof(Function.Name) || e.PropertyName == nameof(Function.Ignore)) {
                    var function = ((Function) sender);
                    this.ReloadName(function.Name);
                }
            }

            public int ID {
                get {
                    if (this.Object != null) {
                        return this.Object.GetHashCode();
                    }

                    return 0;
                }
            }

            public void ReloadName(string name) {
                this.Name = name;
            }

            public object Object { get; }

            private bool modified;
            private string _name;

            public string Name {
                get => this._name + (this.modified ? "*" : "");
                private set {
                    this._name = value;
                    this.OnPropertyChanged(nameof(Name));
                }
            }

            public NodeType Type { get; }

            public int NodeImage {
                get {
                    switch (this.Type) {
                        case NodeType.Model:
                            return 0;
                        case NodeType.Agent:
                            return 1;
                        case NodeType.FunctionGroup:
                            return 2;
                        case NodeType.Function:
                            return 3;
                    }

                    return -1;
                }
            }

            public enum NodeType {
                Model,
                Agent,
                FunctionGroup,
                Function
            }

            public event PropertyChangedEventHandler PropertyChanged;

            [NotifyPropertyChangedInvocator]
            protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) {
                this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }


        private void objectTreeList_DoubleClick(object sender, EventArgs e) {
            var viewInfo = this.objectTreeList.CalcHitInfo(((MouseEventArgs) e).Location);
            if (viewInfo.Node != null) {
                var node = this.objectTreeList.GetDataRecordByNode(viewInfo.Node) as CustomTreeListNodeData;
                if (node?.Data.Type == NodeData.NodeType.Agent) {
                    var agentType = (AgentType) node.Data.Object;
                    var pages = this.xtraTabbedMdiManager1.Pages;
                    var existing = pages.FirstOrDefault(x => x.MdiChild is StateChartDocumentForm && ((StateChartDocumentForm) x.MdiChild).AgentType == agentType);
                    if (existing != null) {
                        this.xtraTabbedMdiManager1.SelectedPage = existing;
                        return;
                    }

                    var frm = new StateChartDocumentForm(agentType, this.objectTreeListImages.Images[node.Data.NodeImage]) {MdiParent = this};
                    frm.Show();
                    frm.HasChangesChanged += MdiFormOnHasChangesChanged;
                    frm.FormClosed += (o, args) => {
                        this.UnbindAllPropertiesControl();
                        frm.HasChangesChanged -= this.MdiFormOnHasChangesChanged;
                    };
                    this.ShowAgentTypeProperties(agentType);
                } else if (node?.Data.Type == NodeData.NodeType.Function) {
                    var function = (Function) node.Data.Object;
                    var pages = this.xtraTabbedMdiManager1.Pages;
                    var existing = pages.FirstOrDefault(x => x.MdiChild is FunctionForm && ((FunctionForm) x.MdiChild).Function == function);
                    if (existing != null) {
                        this.xtraTabbedMdiManager1.SelectedPage = existing;
                        return;
                    }

                    var frm = new FunctionForm(function, this.objectTreeListImages.Images[node.Data.NodeImage]) {MdiParent = this};
                    frm.Show();
                    frm.HasChangesChanged += MdiFormOnHasChangesChanged;
                    frm.FormClosed += (o, args) => {
                        this.UnbindAllPropertiesControl();
                        frm.HasChangesChanged -= this.MdiFormOnHasChangesChanged;
                    };
                    this.ShowFunctionProperties(function);
                }

            }
        }

        private void MdiFormOnHasChangesChanged(object sender, EventArgs eventArgs) {
            this.EnableDisableSaveBarButtonItems();
        }

        private AgentType CurrentAgentType {
            get {
                if (this.objectTreeList.FocusedNode != null) {
                    var node = this.objectTreeList.GetDataRecordByNode(this.objectTreeList.FocusedNode) as CustomTreeListNodeData;
                    if (node?.Data.Object is AgentType agentType) {
                        return agentType;
                    }
                }

                return null;
            }
        }

        private Function CurrentFunction {
            get {
                if (this.objectTreeList.FocusedNode != null) {
                    var node = this.objectTreeList.GetDataRecordByNode(this.objectTreeList.FocusedNode) as CustomTreeListNodeData;
                    if (node?.Data.Object is Function function) {
                        return function;
                    }
                }

                return null;
            }
        }

        private void EnableDisableSaveBarButtonItems() {
            var current = this.CurrentStateChartDocumentForm;
            var enable = current != null && current.HasChanges;
            if (!enable) {
                var agentType = this.CurrentAgentType;
                if (agentType != null) {
                    enable = agentType.Modified;
                }

                var function = this.CurrentFunction;
                if (function != null) {
                    enable = function.Modified;
                }
            }

            this.saveBarButtonItem.Enabled = enable;


            enable = this.xtraTabbedMdiManager1.Pages.Select(x => x.MdiChild).OfType<StateChartDocumentForm>().Any(x => x.HasChanges);
            if (!enable) {
                foreach (var treeListNode in this.objectTreeList.NodesIterator.All) {
                    var node = this.objectTreeList.GetDataRecordByNode(treeListNode) as CustomTreeListNodeData;
                    if (node?.Data.Object is AgentType agentType && agentType.Modified) {
                        enable = true;
                        break;
                    }

                    if (node?.Data.Object is Function function && function.Modified) {
                        enable = true;
                        break;
                    }
                }
            }

            this.saveAllBarButtonItem.Enabled = enable;
        }

        private void xtraTabbedMdiManager1_SelectedPageChanged(object sender, EventArgs e) {
            this.EnableDisableDiagramRibbon(this.CurrentDiagramControl != null);
            this.EnableDisableSaveBarButtonItems();
            this.UnbindAllPropertiesControl();

            var diagramControl = this.CurrentDiagramControl;
            this.diagramBarController1.Control = diagramControl;

            foreach (var treeListNode in this.objectTreeList.NodesIterator.All) {
                var node = this.objectTreeList.GetDataRecordByNode(treeListNode) as CustomTreeListNodeData;
                if (node?.Data.Object is AgentType agentType && this.CurrentStateChartDocumentForm?.AgentType == agentType) {
                    this.objectTreeList.FocusedNode = treeListNode;
                    return;
                }

                if (node?.Data.Object is Function function && this.CurrentFunctionForm?.Function == function) {
                    this.objectTreeList.FocusedNode = treeListNode;
                    return;
                }
            }

            if (this.CurrentAgentType != null) {
                this.ShowAgentTypeProperties(this.CurrentAgentType);
            } else if (this.CurrentFunction != null) {
                this.ShowFunctionProperties(this.CurrentFunction);
            } else {
                this.HideAllProperties();
            }


            if (this.CurrentStateChartDocumentForm != null) {
                foreach (var treeListNode in this.objectTreeList.NodesIterator.All) {
                    var node = this.objectTreeList.GetDataRecordByNode(treeListNode) as CustomTreeListNodeData;
                    if (node?.Data.Object is AgentType agentType && this.CurrentStateChartDocumentForm.AgentType == agentType) {
                        this.objectTreeList.FocusedNode = treeListNode;
                        break;
                    }
                }
            } else if (this.CurrentFunctionForm != null) {
                foreach (var treeListNode in this.objectTreeList.NodesIterator.All) {
                    var node = this.objectTreeList.GetDataRecordByNode(treeListNode) as CustomTreeListNodeData;
                    if (node?.Data.Object is Function function && this.CurrentFunctionForm.Function == function) {
                        this.objectTreeList.FocusedNode = treeListNode;
                        break;
                    }
                }
            }
        }

        private void objectTreeList_FocusedNodeChanged(object sender, FocusedNodeChangedEventArgs e) {
            var node = this.objectTreeList.GetDataRecordByNode(e.Node) as CustomTreeListNodeData;
            if (node?.Data.Object is AgentType agentType) {
                this.ShowAgentTypeProperties(agentType);
            } else if (node?.Data.Object is Function function) {
                this.ShowFunctionProperties(function);
            } else {
                this.agentTypePropertiesControl1.Visible = false;
            }

            this.EnableDisableSaveBarButtonItems();
            this.bbiDeleteFunction.Enabled = node?.Data.Object is Function;
        }

        private void saveBarButtonItem_ItemClick(object sender, ItemClickEventArgs e) {
            this.CurrentStateChartDocumentForm?.Save();
            this.CurrentAgentType?.Save();

            this.CurrentFunction?.Save();
        }

        private void saveAllBarButtonItem_ItemClick(object sender, ItemClickEventArgs e) {
            this.SaveAllChanges();
        }

        private void SaveAllChanges() {
            foreach (var f in this.xtraTabbedMdiManager1.Pages.Select(x => x.MdiChild).OfType<StateChartDocumentForm>()) {
                f.Save();
            }

            foreach (var treeListNode in this.objectTreeList.NodesIterator.All.ToList()) {
                var node = this.objectTreeList.GetDataRecordByNode(treeListNode) as CustomTreeListNodeData;
                if (node?.Data.Object is AgentType agentType) {
                    agentType.Save();
                } else if (node?.Data.Object is Function function) {
                    function.Save();
                }
            }

        }

        protected override void OnFormClosing(FormClosingEventArgs e) {
            var hasChanges = false;
            foreach (var f in this.xtraTabbedMdiManager1.Pages.Select(x => x.MdiChild).OfType<StateChartDocumentForm>()) {
                hasChanges = f.HasChanges;
                if (hasChanges) {
                    break;
                }
            }

            if (!hasChanges) {
                foreach (var f in this.xtraTabbedMdiManager1.Pages.Select(x => x.MdiChild).OfType<FunctionForm>()) {
                    hasChanges = f.HasChanges;
                    if (hasChanges) {
                        break;
                    }
                }
            }

            if (!hasChanges) {
                foreach (var treeListNode in this.objectTreeList.NodesIterator.All.ToList()) {
                    var node = this.objectTreeList.GetDataRecordByNode(treeListNode) as CustomTreeListNodeData;
                    if (node?.Data.Object is AgentType agentType) {
                        hasChanges = agentType.Modified;
                        if (hasChanges) {
                            break;
                        }
                    } else if (node?.Data.Object is Function function) {
                        hasChanges = function.Modified;
                        if (hasChanges) {
                            break;
                        }
                    }
                }
            }

            if (hasChanges) {
                switch (XtraMessageBox.Show("Do you want to save the current changes?", "Save Changes", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question)) {
                    case DialogResult.Cancel:
                        e.Cancel = true;
                        return;
                    case DialogResult.Yes:
                        this.SaveAllChanges();
                        break;
                }
            }

            base.OnFormClosing(e);
        }

        private void bbiAddFunction_ItemClick(object sender, ItemClickEventArgs e) {
            var function = new Function(Guid.NewGuid().ToString(), this.model) {Name = "New function"};
            var cnt = 0;
            while (true) {
                var found = false;
                foreach (var treeListNode in this.objectTreeList.NodesIterator.All) {
                    var node = this.objectTreeList.GetDataRecordByNode(treeListNode) as CustomTreeListNodeData;
                    if (node?.Data.Object is Function f && f.Name == function.Name) {
                        cnt++;
                        found = true;
                        break;
                    }
                }

                if (!found) {
                    break;
                }

                function.Name = $"New function ({cnt})";
            }

            var nodeData = new NodeData(NodeData.NodeType.Function, function.Name, function);
            function.Modified = true;
            nodeData.PropertyChanged += NodeDataOnPropertyChanged;
            new CustomTreeListNodeData(this.rootFunctionsNode, nodeData);
            this.EnableDisableSaveBarButtonItems();
            this.objectTreeList.RefreshDataSource();
            if (this.rootFunctionsNode.Children.Count == 1) {
                this.objectTreeList.ExpandAll();
            }

            foreach (var treeListNode in this.objectTreeList.NodesIterator.All) {
                var node = this.objectTreeList.GetDataRecordByNode(treeListNode) as CustomTreeListNodeData;
                if (node?.Data.Object is Function f && f == function) {
                    this.objectTreeList.FocusedNode = treeListNode;
                    break;
                }
            }
        }

        private void bbiDeleteFunction_ItemClick(object sender, ItemClickEventArgs e) {
            var func = this.CurrentFunction;
            if (XtraMessageBox.Show($"Are you sure you want to delete the function '{func.Name}'?", "Delete function", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No) {
                return;
            }
            func.Delete();
            this.objectTreeList.DeleteNode(this.objectTreeList.FocusedNode);
        }

        private void objectTreeList_NodeCellStyle(object sender, GetCustomNodeCellStyleEventArgs e) {
            var node = this.objectTreeList.GetDataRecordByNode(e.Node) as CustomTreeListNodeData;
            if (node?.Data.Object is Function function && function.Ignore) {
                e.Appearance.ForeColor = Color.FromArgb(147, 147, 147);
            }
        }
    }

    public class DummyModel : IModel {
        public string Name { get; set; }
    }
    public interface IModel {
        string Name { get; set; }

    }

    public class DummyIntelligentObject : IIntelligentObject {
        public string ObjectName { get; set; }
    }
    public interface IIntelligentObject {
        string ObjectName { get; set; }
    }

    public class AgentType {
        private readonly IIntelligentObject agent;
        public AgentType(IIntelligentObject agent) {
            this.agent = agent;
        }

        public string Content => this.agent.ObjectName;

        public void Save() {
            if (this.Modified) {
                using (var stream = new MemoryStream()) {
                    var ser = new XmlXtraSerializer();
                    ser.SerializeObject(this, stream, "Simio");
                    File.WriteAllText($"..\\..\\StateCharts\\{this.agent.ObjectName}_AgentType.xml", stream.GetString());
                }

                this.Modified = false;
            }
        }
        public void SaveStateChart(string stateChart) {
            // *** TODO: stateChart in ein String-Property von agent speichern
            //var stateChartProperty = agent.Properties["StateChart"];
            //stateChartProperty.Value = stateChart;
            File.WriteAllText($"..\\..\\StateCharts\\{this.agent.ObjectName}.xml", stateChart);
        }

        public void Load() {
            var path = $"..\\..\\StateCharts\\{this.agent.ObjectName}_AgentType.xml";
            if (File.Exists(path)) {
                using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(File.ReadAllText(path)))) {
                    var ser = new XmlXtraSerializer();
                    ser.DeserializeObject(this, stream, "Simio");
                }
            }
            this.Modified = false;
        }

        public string LoadStateChart() {
            // *** TODO: stateChart vom StringProperty StateChart des agent laden
            //var stateChartProperty = agent.Properties["StateChart"];
            // return stateChartProperty.Value;
            var ret = "";
            var path = $"..\\..\\StateCharts\\{this.agent.ObjectName}.xml";
            if (File.Exists(path)) {
                ret = File.ReadAllText(path);
            }
            return ret;
        }

        private string onStartupAction;

        [XtraSerializableProperty]
        public string OnStartupAction {
            get => this.onStartupAction;
            set {
                var mod = this.Modified || this.onStartupAction != value;
                this.onStartupAction = value;
                this.Modified = mod;
            }
        }

        private string onDestroyAction;

        [XtraSerializableProperty]
        public string OnDestroyAction {
            get => this.onDestroyAction;
            set {
                var mod = this.Modified || this.onDestroyAction != value;
                this.onDestroyAction = value;
                this.Modified = mod;
            }
        }

        private string onArrivalAction;

        [XtraSerializableProperty]
        public string OnArrivalAction {
            get => this.onArrivalAction;
            set {
                var mod = this.Modified || this.onArrivalAction != value;
                this.onArrivalAction = value;
                this.Modified = mod;
            }
        }

        private string onBeforeStepAction;

        [XtraSerializableProperty]
        public string OnBeforeStepAction {
            get => this.onBeforeStepAction;
            set {
                var mod = this.Modified || this.onBeforeStepAction != value;
                this.onBeforeStepAction = value;
                this.Modified = mod;
            }
        }

        private string onStepAction;

        [XtraSerializableProperty]
        public string OnStepAction {
            get => this.onStepAction;
            set {
                var mod = this.Modified || this.onStepAction != value;
                this.onStepAction = value;
                this.Modified = mod;
            }
        }

        public event EventHandler ModifiedChanged;
        private bool modified;
        public bool Modified {
            get => this.modified;
            set {
                if (this.modified != value) {
                    this.modified = value;
                    this.OnModifiedChanged(EventArgs.Empty);
                }
            }
        }

        protected virtual void OnModifiedChanged(EventArgs e) {
            this.ModifiedChanged?.Invoke(this, e);
        }
    }

    public abstract class StateDiagramShape : DiagramShape, INotifyPropertyChanged {
        public override string Content {
            get {
                if (this.ShowName) {
                    return this.Name;
                }
                return "";
            }
            set {
                base.Content = value;
                this.Name = value;
            }
        }

        private string name;
        [XtraSerializableProperty]
        public string Name {
            get => this.name;
            set {
                var mod = this.Modified || this.name != value;
                this.name = value;
                this.OnPropertyChanged(nameof(this.Content));
                this.Modified = mod;
            }
        }

        private bool showName;

        [XtraSerializableProperty]
        public bool ShowName {
            get => this.showName;
            set {
                var mod = this.Modified || this.showName != value;
                this.showName = value;
                this.OnPropertyChanged(nameof(this.Content));
                this.Modified = mod;
            }
        }

        private bool ignore;

        [XtraSerializableProperty]
        public bool Ignore {
            get => this.ignore;
            set {
                var mod = this.Modified || this.ignore != value;
                this.ignore = value;
                if (this.ignore) {
                    this.Appearance.BackColor = Color.FromArgb(231, 231, 231);
                    this.Appearance.ForeColor = Color.FromArgb(147, 147, 147);
                    this.Appearance.BorderColor = Color.FromArgb(165, 165, 165);
                } else {
                    this.ResetColor();
                }
                this.Modified = mod;
            }
        }

        protected virtual void ResetColor() {
            this.Appearance.BackColor = Color.Empty;
            this.Appearance.ForeColor = Color.Empty;
            this.Appearance.BorderColor = Color.Empty;
        }

        private string description;

        [XtraSerializableProperty]
        public string Description {
            get => this.description;
            set {
                var mod = this.Modified || this.description != value;
                this.description = value;
                this.Modified = mod;
            }
        }

        public event EventHandler ModifiedChanged;
        private bool modified;
        public bool Modified {
            get => this.modified;
            set {
                if (this.modified != value) {
                    this.modified = value;
                    this.OnModifiedChanged(EventArgs.Empty);
                }
            }
        }

        protected virtual void OnModifiedChanged(EventArgs e) {
            this.ModifiedChanged?.Invoke(this, e);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            base.OnPropertiesChanged();
        }
    }

    public abstract class StateDiagramConnector : DiagramConnector, INotifyPropertyChanged {
        public static PointF GetTextPosition(DiagramConnector connector) {
            const float offsetX = 20f;
            const float offsetY = 0f;
            var textPosition = connector.Controller().GetTextPosition();
            return new PointF(Convert.ToSingle(textPosition.X + offsetX), Convert.ToSingle(textPosition.Y + offsetY));
        }

        public override string Content {
            get {
                if (this.ShowName) {
                    return this.Name;
                }
                return "";
            }
            set {
                base.Content = value;
                this.Name = value;
            }
        }

        private string name;
        [XtraSerializableProperty]
        public string Name {
            get => this.name;
            set {
                var mod = this.Modified || this.name != value;
                this.name = value;
                this.OnPropertyChanged(nameof(this.Content));
                this.Modified = mod;
            }
        }

        private bool showName;

        [XtraSerializableProperty]
        public bool ShowName {
            get => this.showName;
            set {
                var mod = this.Modified || this.showName != value;
                this.showName = value;
                this.OnPropertyChanged(nameof(this.Content));
                this.Modified = mod;
            }
        }

        private bool ignore;

        [XtraSerializableProperty]
        public bool Ignore {
            get => this.ignore;
            set {
                var mod = this.Modified || this.ignore != value;
                this.ignore = value;
                if (this.ignore) {
                    this.Appearance.BackColor = Color.FromArgb(231, 231, 231);
                    this.Appearance.ForeColor = Color.FromArgb(147, 147, 147);
                    this.Appearance.BorderColor = Color.FromArgb(165, 165, 165);
                } else {
                    this.ResetColor();
                }
                this.Modified = mod;
            }
        }

        

        protected virtual void ResetColor() {
            this.Appearance.BackColor = Color.Empty;
            this.Appearance.ForeColor = Color.Empty;
            this.Appearance.BorderColor = Color.Empty;
        }

        private string description;

        [XtraSerializableProperty]
        public string Description {
            get => this.description;
            set {
                var mod = this.Modified || this.description != value;
                this.description = value;
                this.Modified = mod;
            }
        }

        public event EventHandler ModifiedChanged;
        private bool modified;
        public bool Modified {
            get => this.modified;
            set {
                if (this.modified != value) {
                    this.modified = value;
                    this.OnModifiedChanged(EventArgs.Empty);
                }
            }
        }

        protected virtual void OnModifiedChanged(EventArgs e) {
            this.ModifiedChanged?.Invoke(this, e);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            base.OnPropertiesChanged();
        }
    }

    public class StateShape : StateDiagramShape, IShape {

        public StateShape() {
            this.SetShape(DevExpress.Diagram.Core.BasicFlowchartShapes.Process);
        }

        private Color color;
        [XtraSerializableProperty]
        public Color Color {
            get => this.color;
            set {
                var mod = this.Modified || this.color != value;
                this.color = value;
                if (!this.Ignore) {
                    this.ResetColor();
                }
                this.Modified = mod;
            }
        }

        protected override void ResetColor() {
            this.Appearance.BackColor = this.color;
            this.Appearance.ForeColor = Color.Empty;
            this.Appearance.BorderColor = Color.Empty;
        }

        private string entryAction;

        [XtraSerializableProperty]
        public string EntryAction {
            get => this.entryAction;
            set {
                var mod = this.Modified || this.entryAction != value;
                this.entryAction = value;
                this.Modified = mod;
            }
        }

        private string exitAction;

        [XtraSerializableProperty]
        public string ExitAction {
            get => this.exitAction;
            set {
                var mod = this.Modified || this.exitAction != value;
                this.exitAction = value;
                this.Modified = mod;
            }
        }
    }

    public class BranchShape : StateDiagramShape, IShape {
        public BranchShape() {
            this.SetShape(DevExpress.Diagram.Core.BasicFlowchartShapes.Decision);
        }

        private string action;

        [XtraSerializableProperty]
        public string Action {
            get => this.action;
            set {
                var mod = this.Modified || this.action != value;
                this.action = value;
                this.Modified = mod;
            }
        }

        private Color color;
        [XtraSerializableProperty]
        public Color Color {
            get => this.color;
            set {
                var mod = this.Modified || this.color != value;
                this.color = value;
                if (!this.Ignore) {
                    this.ResetColor();
                }
                this.Modified = mod;
            }
        }

        protected override void ResetColor() {
            this.Appearance.BackColor = this.color;
            this.Appearance.ForeColor = Color.Empty;
            this.Appearance.BorderColor = Color.Empty;
        }

    }

    public class EntryPointShape : StateDiagramConnector, IDrawingShape {
        public override ConnectorPointRestrictions BeginPointRestrictions => ConnectorPointRestrictions.KeepDisconnected;

        private string action;

        [XtraSerializableProperty]
        public string Action {
            get => this.action;
            set {
                var mod = this.Modified || this.action != value;
                this.action = value;
                this.Modified = mod;
            }
        }

        public void Draw(CustomDrawItemEventArgs e) {
            e.DefaultDraw();

            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            var beginPoint = new PointF(this.BeginPoint.X - this.X, this.BeginPoint.Y - this.Y);

            const int radius = 9;
            e.Graphics.FillEllipse(new SolidBrush(e.Appearance.BorderColor), beginPoint.X - radius, beginPoint.Y - radius, radius * 2, radius * 2);
            e.Handled = true;
        }

        public bool IsInCompositeState => (this.EndItem as StateDiagramShape)?.ParentItem is CompositeStateShape;
    }

    public class FinalStateShape : StateDiagramConnector, IDrawingShape {
        public override ConnectorPointRestrictions EndPointRestrictions => ConnectorPointRestrictions.KeepDisconnected;

        public override ArrowDescription EndArrow => null;

        private string action;

        [XtraSerializableProperty]
        public string Action {
            get => this.action;
            set {
                var mod = this.Modified || this.action != value;
                this.action = value;
                this.Modified = mod;
            }
        }
        public void Draw(CustomDrawItemEventArgs e) {
            e.DefaultDraw();

            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            var endPoint = new PointF(this.EndPoint.X - this.X, this.EndPoint.Y - this.Y);

            var radius = 10;
            e.Graphics.FillEllipse(Brushes.White, endPoint.X - radius, endPoint.Y - radius, radius * 2, radius * 2);
            e.Graphics.DrawEllipse(new Pen(e.Appearance.BorderColor, e.Appearance.BorderSize), endPoint.X - radius, endPoint.Y - radius, radius * 2, radius * 2);

            radius = 6;
            e.Graphics.FillEllipse(new SolidBrush(e.Appearance.BorderColor), endPoint.X - radius, endPoint.Y - radius, radius * 2, radius * 2);

        }
    }

    public class TransitionShape : StateDiagramConnector, IDrawingShape {
        public TransitionShape() {
            //this.SetShape(DevExpress.Diagram.Core.BasicFlowchartShapes.Decision);
            this.MessageType = "string";
        }

        public void Draw(CustomDrawItemEventArgs e) {
            e.DefaultDraw(CustomDrawItemMode.Background);
            e.Graphics.DrawString(((DiagramConnector)e.Item).Content, e.Appearance.Font, new SolidBrush(e.Appearance.ForeColor), StateDiagramConnector.GetTextPosition((DiagramConnector)e.Item));

            var center = ConvertHelper.ToWinPointF(this.Controller().GetTextPosition());
            Bitmap image = null;
            switch (this.TransitionType) {
                case TransitionType.Message:
                    image = Resources.TransitionMessage;
                    break;
                case TransitionType.Timeout:
                    image = Resources.TransitionTimeout;
                    break;
                case TransitionType.Rate:
                    image = Resources.TransitionRate;
                    break;
                case TransitionType.AgentArrival:
                    image = Resources.TransitionArrival;
                    break;
                case TransitionType.Condition:
                    image = Resources.TransitionCondition;
                    break;
            }

            if (image != null) {
                e.Graphics.DrawImage(image, new PointF(center.X - 10, center.Y - 10));
            }

            if (this.ShowName) {
                //e.Graphics.DrawString(this.Name, e.Appearance.Font, e.Appearance.GetForeBrush(null), center.X+20, center.Y-10);
            }
            //e.Handled = true;

        }

        private TransitionType transitionType;

        [XtraSerializableProperty]
        public TransitionType TransitionType {
            get => this.transitionType;
            set {
                var mod = this.Modified || this.transitionType != value;
                this.transitionType = value;
                this.OnPropertyChanged();
                this.Modified = mod;
            }
        }
        private string timeout;

        [XtraSerializableProperty]
        public string Timeout {
            get => this.timeout;
            set {
                var mod = this.Modified || this.timeout != value;
                this.timeout = value;
                this.Modified = mod;
            }
        }

        private TimeoutUnit timeoutUnit;

        [XtraSerializableProperty]
        public TimeoutUnit TimeoutUnit {
            get => this.timeoutUnit;
            set {
                var mod = this.Modified || this.timeoutUnit != value;
                this.timeoutUnit = value;
                this.OnPropertyChanged();
                this.Modified = mod;
            }
        }

        private string rate;

        [XtraSerializableProperty]
        public string Rate {
            get => this.rate;
            set {
                var mod = this.Modified || this.rate != value;
                this.rate = value;
                this.Modified = mod;
            }
        }

        private RateUnit rateUnit;

        [XtraSerializableProperty]
        public RateUnit RateUnit {
            get => this.rateUnit;
            set {
                var mod = this.Modified || this.rateUnit != value;
                this.rateUnit = value;
                this.OnPropertyChanged();
                this.Modified = mod;
            }
        }

        private string messageType;

        [XtraSerializableProperty]
        public string MessageType {
            get => this.messageType;
            set {
                var mod = this.Modified || this.messageType != value;
                this.messageType = value;
                this.Modified = mod;
            }
        }

        private MessageFireTransition messageFireTransition;

        [XtraSerializableProperty]
        public MessageFireTransition MessageFireTransition {
            get => this.messageFireTransition;
            set {
                var mod = this.Modified || this.messageFireTransition != value;
                this.messageFireTransition = value;
                this.OnPropertyChanged();
                this.Modified = mod;
            }
        }

        private string action;

        [XtraSerializableProperty]
        public string Action {
            get => this.action;
            set {
                var mod = this.Modified || this.action != value;
                this.action = value;
                this.Modified = mod;
            }
        }

        private string condition;

        [XtraSerializableProperty]
        public string Condition {
            get => this.condition;
            set {
                var mod = this.Modified || this.condition != value;
                this.condition = value;
                this.Modified = mod;
            }
        }

        private string guard;

        [XtraSerializableProperty]
        public string Guard {
            get => this.guard;
            set {
                var mod = this.Modified || this.guard != value;
                this.guard = value;
                this.Modified = mod;
            }
        }
    }

    public enum TransitionType {
        Timeout,
        Rate,
        Condition,
        Message,
        AgentArrival
    }

    public enum TimeoutUnit {
        Milliseconds,
        Seconds,
        Minutes,
        Hours,
        Days,
        Weeks,
        Months,
        Years
    }

    public enum RateUnit {
        PerMillisecond,
        PerSecond,
        PerMinute,
        PerHour,
        PerDay,
        PerWeek,
        PerMonth,
        PerYear
    }

    public enum MessageFireTransition {
        Unconditionally,
        OnParticularMessage,
        TrueExpressioin
    }

    public class CompositeStateShape : DiagramContainer, IShape {

        public override ContainerShapeDescription Shape => StandardContainers.Classic;
        public override bool ShowHeader => true;

        private Color color;
        [XtraSerializableProperty]
        public Color Color {
            get => this.color;
            set {
                var mod = this.Modified || this.color != value;
                this.color = value;
                if (!this.Ignore) {
                    this.ResetColor();
                }
                this.Modified = mod;
            }
        }

        protected void ResetColor() {
            this.Appearance.BackColor = this.color;
            this.Appearance.ForeColor = Color.Empty;
            this.Appearance.BorderColor = Color.Empty;
        }

        private string entryAction;

        [XtraSerializableProperty]
        public string EntryAction {
            get => this.entryAction;
            set {
                var mod = this.Modified || this.entryAction != value;
                this.entryAction = value;
                this.Modified = mod;
            }
        }

        private string exitAction;

        [XtraSerializableProperty]
        public string ExitAction {
            get => this.exitAction;
            set {
                var mod = this.Modified || this.exitAction != value;
                this.exitAction = value;
                this.Modified = mod;
            }
        }

        public override string Header {
            get {
                if (this.ShowName) {
                    return this.Name;
                }
                return "";
            }
            set {
                base.Header = value;
                this.Name = value;
            }
        }

        private string name;
        [XtraSerializableProperty]
        public string Name {
            get => this.name;
            set {
                var mod = this.Modified || this.name != value;
                this.name = value;
                this.OnPropertyChanged(nameof(this.Header));
                this.Modified = mod;
            }
        }

        private bool showName;

        [XtraSerializableProperty]
        public bool ShowName {
            get => this.showName;
            set {
                var mod = this.Modified || this.showName != value;
                this.showName = value;
                this.OnPropertyChanged(nameof(this.Header));
                this.Modified = mod;
            }
        }

        private bool ignore;

        [XtraSerializableProperty]
        public bool Ignore {
            get => this.ignore;
            set {
                var mod = this.Modified || this.ignore != value;
                this.ignore = value;
                if (this.ignore) {
                    this.Appearance.BackColor = Color.FromArgb(231, 231, 231);
                    this.Appearance.ForeColor = Color.FromArgb(147, 147, 147);
                    this.Appearance.BorderColor = Color.FromArgb(165, 165, 165);
                } else {
                    this.ResetColor();
                }
                this.Modified = mod;
            }
        }

        private string description;

        [XtraSerializableProperty]
        public string Description {
            get => this.description;
            set {
                var mod = this.Modified || this.description != value;
                this.description = value;
                this.Modified = mod;
            }
        }

        public event EventHandler ModifiedChanged;
        private bool modified;
        public bool Modified {
            get => this.modified;
            set {
                if (this.modified != value) {
                    this.modified = value;
                    this.OnModifiedChanged(EventArgs.Empty);
                }
            }
        }

        protected virtual void OnModifiedChanged(EventArgs e) {
            this.ModifiedChanged?.Invoke(this, e);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            base.OnPropertiesChanged();
        }

    }

    public class HistoryStateShape : StateDiagramShape, IDrawingShape {
        //private static ShapeDescription historyStateShapeDescription;
        //static HistoryStateShape() {
        //    historyStateShapeDescription = BasicFlowchartShapes.OnPageReference.Update(getEditorBounds: (width, height, parameters) => {
        //        return new System.Windows.Rect(25, 0, 100, height*2);
        //    }, getUseBackgroundAsForeground: () => true);

        //    //ShapeDescription updatedRectangleDescription = basicShapesStencil.GetShape(BasicShapes.Rectangle.Id).Update(getEditorBounds: (width, height, parameters) => new System.Windows.Rect(0, height, width, height));
        //    //basicShapesStencil.RegisterShape(updatedRectangleDescription);
        //}

        public HistoryStateShape() {
            this.SetShape(BasicShapes.Ellipse.Update(getEditorBounds: (width, height, parameters) => new System.Windows.Rect(25, 0, 100, height * 2), getUseBackgroundAsForeground: () => true));
            //this.SetShape(historyStateShapeDescription);
            //this.SetItemSize(new System.Windows.Size(20,20));
            //this.Size = new SizeF(20, 20);
            //this.SetShape(historyStateShapeDescription);
        }

        public override ShapeDescription Shape {
            get { return base.Shape; }
            set { }
        }

        protected override void OnInitializing(DiagramItemInitializingEventArgs e) {
            base.OnInitializing(e);
            this.Appearance.TextOptions.HAlignment = HorzAlignment.Near;
            this.Appearance.TextOptions.VAlignment = VertAlignment.Top;
        }

        public override SizeF Size => new SizeF(20, 20);
        public override bool? CanResize => false;

        public void Draw(CustomDrawItemEventArgs e) {
            e.DefaultDraw();

            e.Graphics.SmoothingMode =System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            e.Graphics.FillEllipse(Brushes.White, 0, 0, this.Width, this.Height);
            e.Graphics.DrawEllipse(new Pen(e.Appearance.BackColor, 2f), 0, 0, this.Width, this.Height);
            var format = new StringFormat {
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Center
            };
            e.Graphics.DrawString("H", e.Appearance.Font, Brushes.Black, new RectangleF(0, 0, this.Width, this.Height), format);
        }


        private string action;

        [XtraSerializableProperty]
        public string Action {
            get => this.action;
            set {
                var mod = this.Modified || this.action != value;
                this.action = value;
                this.Modified = mod;
            }
        }

        private bool isDeep;
        [XtraSerializableProperty]
        public bool IsDeep {
            get => this.isDeep;
            set {
                var mod = this.Modified || this.isDeep != value;
                this.isDeep = value;
                this.Modified = mod;
            }
        }

    }

    public interface IDrawingShape : IShape {
        void Draw(CustomDrawItemEventArgs e);
    }

    public interface IShape {

    }

    [Serializable]
    public class Function : INotifyPropertyChanged, ISerializable {
        //[XmlIgnore]
        //private readonly IModel model;

        protected Function(SerializationInfo info, StreamingContext context) {
            foreach (SerializationEntry entry in info) {
                switch (entry.Name) {
                    case "id":
                        this.Id = info.GetString("id");
                        break;
                    case "arguments":
                        // *** use Property-setter to add the Eventhandlers
                        this.Arguments = (ObservableCollection<Argument>)info.GetValue("arguments", typeof(ObservableCollection<Argument>));
                        break;
                    case "name":
                        this.name = info.GetString("name");
                        break;
                    case "ignore":
                        this.ignore = info.GetBoolean("ignore");
                        break;
                    case "returnType":
                        this.returnType = info.GetString("returnType");
                        break;
                    case "body":
                        this.body = info.GetString("body");
                        break;
                    case "description":
                        this.description = info.GetString("description");
                        break;
                }
            }
        }

        [SecurityPermission(SecurityAction.Demand,
            SerializationFormatter = true)]
        public virtual void GetObjectData(SerializationInfo info, StreamingContext context) {
            //Add data to your serialization process here
            info.AddValue("id", this.Id);
            info.AddValue("arguments", this.arguments);
            info.AddValue("name", this.name);
            info.AddValue("ignore", this.ignore);
            info.AddValue("returnType", this.returnType);
            info.AddValue("body", this.body);
            info.AddValue("description", this.description);
        }

        public Function(string id, IModel model) {
            this.Id = id;
            //this.model = model;
        }

        public string Id { get; }

        public static List<Function> Load() {
            var ret = new List<Function>();
            var functionList = new FunctionList();
            // TODO: aus Model lesen
            var path = $"..\\..\\StateCharts\\Functions.xml";
            if (File.Exists(path)) {
                using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(File.ReadAllText(path)))) {
                    var ser = new XmlXtraSerializer();
                    ser.DeserializeObject(functionList, stream, "Simio");
                }
            }
            
            foreach (var f in functionList.Functions.Values) {
                f.Modified = false;
                ret.Add(f);
            }

            return ret;
        }

        public void Save() {
            if (this.Modified) {
                var functionList = new FunctionList();
                var path = $"..\\..\\StateCharts\\Functions.xml";
                if (File.Exists(path)) {
                    using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(File.ReadAllText(path)))) {
                        var ser = new XmlXtraSerializer();
                        ser.DeserializeObject(functionList, stream, "Simio");
                    }
                }

                functionList.Functions[this.Id] = this;

                var cnt = 1;
                foreach (var argument in this.Arguments.OrderBy(x => x.Position).ToList()) {
                    argument.Position = cnt;
                    cnt++;
                }
                using (var stream = new MemoryStream()) {
                    var ser = new XmlXtraSerializer();
                    ser.SerializeObject(functionList, stream, "Simio");
                    File.WriteAllText(path, stream.GetString());
                }

                this.Modified = false;
            }
        }

        public void Delete() {
            var functionList = new FunctionList();
            var path = $"..\\..\\StateCharts\\Functions.xml";
            if (File.Exists(path)) {
                using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(File.ReadAllText(path)))) {
                    var ser = new XmlXtraSerializer();
                    ser.DeserializeObject(functionList, stream, "Simio");
                }
            }

            if (functionList.Functions.ContainsKey(this.Id)) {
                functionList.Functions.Remove(this.Id);
            }

            using (var stream = new MemoryStream()) {
                var ser = new XmlXtraSerializer();
                ser.SerializeObject(functionList, stream, "Simio");
                File.WriteAllText(path, stream.GetString());
            }
        }

        /// <summary>
        /// Helper-Class for Serialization/Deserialization
        /// </summary>
        private class FunctionList  {
            public FunctionList() {
                this.Functions = new DictionaryEx<string, Function>();
            }
            [XtraSerializableProperty]
            // setter needed for deserialization
            // ReSharper disable once MemberCanBePrivate.Local
            public DictionaryEx<string, Function> Functions { get; set; }
        }

        private ObservableCollection<Argument> arguments;
        [XtraSerializableProperty]
        public ObservableCollection<Argument> Arguments {
            get {
                if (this.arguments == null) {
                    this.Arguments = new ObservableCollection<Argument>();
                }
                return this.arguments;
            }
            private set {
                if (this.arguments != null) {
                    value.CollectionChanged -= OnArgumentsCollectionChanged;
                    foreach (var argument in this.arguments) {
                        argument.PropertyChanged -= this.OnArgmentPropertyChanged;
                    }
                }
                this.arguments = value;
                value.CollectionChanged += OnArgumentsCollectionChanged;
                foreach (var argument in this.arguments) {
                    argument.PropertyChanged += this.OnArgmentPropertyChanged;
                }
            }
        }

        [Serializable]
        public class Argument : INotifyPropertyChanged {
            private string _id;
            private int _position;
            private string _name;
            private string _type;

            public string Id {
                get => this._id;
                set {
                    var changed = this._id != value;
                    this._id = value;
                    if (changed) {
                        this.OnPropertyChanged(nameof(Id));
                    }
                }
            }

            public int Position {
                get => this._position;
                set {
                    var changed = this._position != value;
                    this._position = value;
                    if (changed) {
                        this.OnPropertyChanged(nameof(Position));
                    }
                }
            }

            public string Name {
                get => this._name;
                set {
                    var changed = this._name != value;
                    this._name = value;
                    if (changed) {
                        this.OnPropertyChanged(nameof(Name));
                    }
                }
            }

            public string Type {
                get => this._type;
                set {
                    var changed = this._type != value;
                    this._type = value;
                    if (changed) {
                        this.OnPropertyChanged(nameof(Type));
                    }
                }
            }

            public event PropertyChangedEventHandler PropertyChanged;

            [NotifyPropertyChangedInvocator]
            protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) {
                this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        private void OnArgumentsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e) {
            if (e.OldItems != null) {
                foreach (var item in e.OldItems) {
                    ((Argument) item).PropertyChanged -= OnArgmentPropertyChanged;
                }
            }

            if (e.NewItems != null) {
                foreach (var item in e.NewItems) {
                    ((Argument) item).PropertyChanged += OnArgmentPropertyChanged;
                }
            }

            this.Modified = true;
        }

        private void OnArgmentPropertyChanged(object sender, PropertyChangedEventArgs e) {
            this.Modified = true;
        }

        private string name;
        [XtraSerializableProperty]
        public string Name {
            get => this.name;
            set {
                var changed = this.name != value;
                this.name = value;
                if (changed) {
                    this.OnPropertyChanged(nameof(Name));
                }

                this.Modified = this.Modified || changed;
            }
        }

        private bool ignore;

        [XtraSerializableProperty]
        public bool Ignore {
            get => this.ignore;
            set {
                var changed = this.ignore != value;
                this.ignore = value;
                if (changed) {
                    this.OnPropertyChanged(nameof(Name));
                }

                this.Modified = this.Modified || changed;
            }
        }

        private string returnType;
        [XtraSerializableProperty]
        public string ReturnType {
            get => this.returnType;
            set {
                var mod = this.Modified || this.returnType != value;
                this.returnType = value;
                this.Modified = mod;
            }
        }

        private string description;
        [XtraSerializableProperty]
        public string Description {
            get => this.description;
            set {
                var mod = this.Modified || this.description != value;
                this.description = value;
                this.Modified = mod;
            }
        }

        private string _oriBody;
        public void ResetBody() {
            this.Body = this._oriBody;
            this.BodyModified = false;
        }
        private string body;
        [XtraSerializableProperty]
        public string Body {
            get => this.body;
            set {
                var mod = this.BodyModified || this.body != value;
                this.body = value;
                this.BodyModified = mod;
                this.Modified = mod;
            }
        }

        [field: NonSerialized]
        public event EventHandler ModifiedChanged;
        [field: NonSerialized]
        private bool modified;
        public bool Modified {
            get => this.modified;
            set {
                if (this.modified != value) {
                    this.modified = value;
                    this.OnModifiedChanged(EventArgs.Empty);
                }

                if (!value) {
                    this.BodyModified = false;
                }
            }
        }

        protected virtual void OnModifiedChanged(EventArgs e) {
            this.ModifiedChanged?.Invoke(this, e);
        }

        [field: NonSerialized]
        public event EventHandler BodyModifiedChanged;
        [field: NonSerialized]
        private bool bodyModified;
        public bool BodyModified {
            get => this.bodyModified;
            set {
                if (this.bodyModified != value) {
                    this.bodyModified = value;
                    this.OnBodyModifiedChanged(EventArgs.Empty);
                }

                if (!value) {
                    this._oriBody = this.Body;
                }
            }
        }

        protected virtual void OnBodyModifiedChanged(EventArgs e) {
            this.BodyModifiedChanged?.Invoke(this, e);
        }

        [field: NonSerialized]
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
