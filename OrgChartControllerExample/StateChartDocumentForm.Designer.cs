namespace OrgChartControllerExample {
    partial class StateChartDocumentForm {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.diagramControl1 = new DiagramControlEx();
            ((System.ComponentModel.ISupportInitialize)(this.diagramControl1)).BeginInit();
            this.SuspendLayout();
            // 
            // diagramControl1
            // 
            this.diagramControl1.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.diagramControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.diagramControl1.Location = new System.Drawing.Point(0, 0);
            this.diagramControl1.Name = "diagramControl1";
            this.diagramControl1.OptionsBehavior.AllowEmptySelection = false;
            this.diagramControl1.OptionsBehavior.SelectedStencils = new DevExpress.Diagram.Core.StencilCollection(new string[0]);
            this.diagramControl1.OptionsView.CanvasSizeMode = DevExpress.Diagram.Core.CanvasSizeMode.Fill;
            this.diagramControl1.OptionsView.Landscape = false;
            this.diagramControl1.OptionsView.MaxZoomFactor = 1F;
            this.diagramControl1.OptionsView.MinZoomFactor = 1F;
            this.diagramControl1.OptionsView.PageMargin = new System.Windows.Forms.Padding(0);
            this.diagramControl1.OptionsView.PageSize = new System.Drawing.SizeF(1587F, 2245F);
            this.diagramControl1.OptionsView.PaperKind = System.Drawing.Printing.PaperKind.A2;
            this.diagramControl1.OptionsView.ShowPageBreaks = false;
            this.diagramControl1.OptionsView.ShowRulers = false;
            this.diagramControl1.Size = new System.Drawing.Size(1264, 693);
            this.diagramControl1.TabIndex = 1;
            // 
            // StateChartDocumentForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1264, 693);
            this.Controls.Add(this.diagramControl1);
            this.Name = "StateChartDocumentForm";
            this.Text = "DiagramForm";
            ((System.ComponentModel.ISupportInitialize)(this.diagramControl1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DiagramControlEx diagramControl1;
    }
}