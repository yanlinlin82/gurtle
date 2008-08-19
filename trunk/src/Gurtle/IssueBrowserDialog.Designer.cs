namespace Gurtle
{
    public partial class IssueBrowserDialog
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.Button cancelButton;
            System.Windows.Forms.StatusStrip statusStrip;
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(IssueBrowserDialog));
            System.Windows.Forms.ToolTip toolTip;
            this.statusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.workStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.detailButton = new System.Windows.Forms.Button();
            this.refreshButton = new System.Windows.Forms.Button();
            this.updateButton = new System.Windows.Forms.Button();
            this.okButton = new System.Windows.Forms.Button();
            this.issueListView = new System.Windows.Forms.ListView();
            this.idColumn = new System.Windows.Forms.ColumnHeader();
            this.typeColumn = new System.Windows.Forms.ColumnHeader();
            this.statusColumn = new System.Windows.Forms.ColumnHeader();
            this.priorityColumn = new System.Windows.Forms.ColumnHeader();
            this.ownerColumn = new System.Windows.Forms.ColumnHeader();
            this.summaryColumn = new System.Windows.Forms.ColumnHeader();
            cancelButton = new System.Windows.Forms.Button();
            statusStrip = new System.Windows.Forms.StatusStrip();
            toolTip = new System.Windows.Forms.ToolTip(this.components);
            statusStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // cancelButton
            // 
            cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            cancelButton.Location = new System.Drawing.Point(595, 311);
            cancelButton.Name = "cancelButton";
            cancelButton.Size = new System.Drawing.Size(75, 28);
            cancelButton.TabIndex = 5;
            cancelButton.Text = "Cancel";
            cancelButton.UseVisualStyleBackColor = true;
            // 
            // statusStrip
            // 
            statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.statusLabel,
            this.workStatus});
            statusStrip.Location = new System.Drawing.Point(0, 342);
            statusStrip.Name = "statusStrip";
            statusStrip.Size = new System.Drawing.Size(682, 25);
            statusStrip.TabIndex = 4;
            // 
            // statusLabel
            // 
            this.statusLabel.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.statusLabel.Name = "statusLabel";
            this.statusLabel.Size = new System.Drawing.Size(667, 20);
            this.statusLabel.Spring = true;
            this.statusLabel.Text = "Ready";
            this.statusLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // workStatus
            // 
            this.workStatus.AutoSize = false;
            this.workStatus.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.workStatus.Image = ((System.Drawing.Image)(resources.GetObject("workStatus.Image")));
            this.workStatus.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.workStatus.Name = "workStatus";
            this.workStatus.Size = new System.Drawing.Size(45, 20);
            this.workStatus.Visible = false;
            // 
            // detailButton
            // 
            this.detailButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.detailButton.Location = new System.Drawing.Point(12, 311);
            this.detailButton.Name = "detailButton";
            this.detailButton.Size = new System.Drawing.Size(75, 28);
            this.detailButton.TabIndex = 1;
            this.detailButton.Text = "&Details";
            toolTip.SetToolTip(this.detailButton, "Open details of selected issue in the browser");
            this.detailButton.UseVisualStyleBackColor = true;
            this.detailButton.Click += new System.EventHandler(this.DetailButton_Click);
            // 
            // refreshButton
            // 
            this.refreshButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.refreshButton.Enabled = false;
            this.refreshButton.Location = new System.Drawing.Point(93, 311);
            this.refreshButton.Name = "refreshButton";
            this.refreshButton.Size = new System.Drawing.Size(75, 28);
            this.refreshButton.TabIndex = 2;
            this.refreshButton.Text = "&Refresh";
            toolTip.SetToolTip(this.refreshButton, "Reload the issue list");
            this.refreshButton.UseVisualStyleBackColor = true;
            this.refreshButton.Click += new System.EventHandler(this.RefreshButton_Click);
            // 
            // updateButton
            // 
            this.updateButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.updateButton.Location = new System.Drawing.Point(174, 311);
            this.updateButton.Name = "updateButton";
            this.updateButton.Size = new System.Drawing.Size(110, 28);
            this.updateButton.TabIndex = 3;
            this.updateButton.Text = "&Update Client";
            toolTip.SetToolTip(this.updateButton, "Update to a new version of this client");
            this.updateButton.UseVisualStyleBackColor = true;
            this.updateButton.Visible = false;
            this.updateButton.Click += new System.EventHandler(this.UpdateButton_Click);
            // 
            // okButton
            // 
            this.okButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.okButton.Location = new System.Drawing.Point(514, 311);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(75, 28);
            this.okButton.TabIndex = 4;
            this.okButton.Text = "OK";
            this.okButton.UseVisualStyleBackColor = true;
            this.okButton.Click += new System.EventHandler(this.OKButton_Click);
            // 
            // issueListView
            // 
            this.issueListView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.issueListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.idColumn,
            this.typeColumn,
            this.statusColumn,
            this.priorityColumn,
            this.ownerColumn,
            this.summaryColumn});
            this.issueListView.FullRowSelect = true;
            this.issueListView.GridLines = true;
            this.issueListView.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.issueListView.HideSelection = false;
            this.issueListView.Location = new System.Drawing.Point(0, 0);
            this.issueListView.Name = "issueListView";
            this.issueListView.Size = new System.Drawing.Size(682, 305);
            this.issueListView.Sorting = System.Windows.Forms.SortOrder.Ascending;
            this.issueListView.TabIndex = 0;
            this.issueListView.UseCompatibleStateImageBehavior = false;
            this.issueListView.View = System.Windows.Forms.View.Details;
            this.issueListView.SelectedIndexChanged += new System.EventHandler(this.IssueListView_SelectedIndexChanged);
            this.issueListView.DoubleClick += new System.EventHandler(this.IssueListView_DoubleClick);
            // 
            // idColumn
            // 
            this.idColumn.Text = "ID";
            // 
            // typeColumn
            // 
            this.typeColumn.Text = "Type";
            this.typeColumn.Width = 100;
            // 
            // statusColumn
            // 
            this.statusColumn.Text = "Status";
            this.statusColumn.Width = 100;
            // 
            // priorityColumn
            // 
            this.priorityColumn.Text = "Priority";
            this.priorityColumn.Width = 100;
            // 
            // ownerColumn
            // 
            this.ownerColumn.Text = "Owner";
            this.ownerColumn.Width = 100;
            // 
            // summaryColumn
            // 
            this.summaryColumn.Text = "Summary";
            this.summaryColumn.Width = 1000;
            // 
            // IssueBrowserDialog
            // 
            this.AcceptButton = this.okButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = cancelButton;
            this.ClientSize = new System.Drawing.Size(682, 367);
            this.Controls.Add(this.updateButton);
            this.Controls.Add(this.refreshButton);
            this.Controls.Add(statusStrip);
            this.Controls.Add(this.okButton);
            this.Controls.Add(cancelButton);
            this.Controls.Add(this.detailButton);
            this.Controls.Add(this.issueListView);
            this.MinimizeBox = false;
            this.Name = "IssueBrowserDialog";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Issues for {0} ({1})";
            statusStrip.ResumeLayout(false);
            statusStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListView issueListView;
        private System.Windows.Forms.ColumnHeader idColumn;
        private System.Windows.Forms.ColumnHeader summaryColumn;
        private System.Windows.Forms.ColumnHeader statusColumn;
        private System.Windows.Forms.ColumnHeader priorityColumn;
        private System.Windows.Forms.ColumnHeader ownerColumn;
        private System.Windows.Forms.ColumnHeader typeColumn;
        private System.Windows.Forms.Button detailButton;
        private System.Windows.Forms.ToolStripStatusLabel statusLabel;
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.Button refreshButton;
        private System.Windows.Forms.ToolStripStatusLabel workStatus;
        private System.Windows.Forms.Button updateButton;
    }
}

