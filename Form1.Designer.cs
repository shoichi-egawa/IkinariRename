namespace IkinariRename
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            DataGridViewCellStyle dataGridViewCellStyle1 = new DataGridViewCellStyle();
            button1 = new Button();
            dataGridView1 = new DataGridView();
            Column1 = new DataGridViewCheckBoxColumn();
            Column3 = new DataGridViewTextBoxColumn();
            Column4 = new DataGridViewImageColumn();
            Column7 = new DataGridViewTextBoxColumn();
            colType = new DataGridViewComboBoxColumn();
            Column8 = new DataGridViewTextBoxColumn();
            button2 = new Button();
            button3 = new Button();
            btnSelectAll = new Button();
            btnDeselectAll = new Button();
            chkDownsize = new CheckBox();
            rdoSortDate = new RadioButton();
            rdoSortName = new RadioButton();
            btnSizeMedium = new Button();
            btnSizeLarge = new Button();
            btnSizeSmall = new Button();
            label1 = new Label();
            ((System.ComponentModel.ISupportInitialize)dataGridView1).BeginInit();
            SuspendLayout();
            // 
            // button1
            // 
            button1.Location = new Point(12, 12);
            button1.Name = "button1";
            button1.Size = new Size(122, 23);
            button1.TabIndex = 0;
            button1.Text = "写真フォルダ選択";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // dataGridView1
            // 
            dataGridView1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            dataGridView1.BackgroundColor = SystemColors.InactiveCaption;
            dataGridView1.BorderStyle = BorderStyle.None;
            dataGridView1.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridView1.Columns.AddRange(new DataGridViewColumn[] { Column1, Column3, Column4, Column7, colType, Column8 });
            dataGridView1.GridColor = SystemColors.ControlText;
            dataGridView1.Location = new Point(12, 99);
            dataGridView1.Name = "dataGridView1";
            dataGridView1.RowHeadersVisible = false;
            dataGridView1.Size = new Size(635, 607);
            dataGridView1.TabIndex = 9;
            dataGridView1.CellContentClick += dataGridView1_CellContentClick;
            dataGridView1.CellValueChanged += dataGridView1_CellValueChanged;
            // 
            // Column1
            // 
            Column1.HeaderText = "選択";
            Column1.Name = "Column1";
            Column1.Width = 40;
            // 
            // Column3
            // 
            dataGridViewCellStyle1.BackColor = Color.White;
            Column3.DefaultCellStyle = dataGridViewCellStyle1;
            Column3.HeaderText = "写真情報";
            Column3.Name = "Column3";
            // 
            // Column4
            // 
            Column4.HeaderText = "サムネイル";
            Column4.ImageLayout = DataGridViewImageCellLayout.Zoom;
            Column4.Name = "Column4";
            // 
            // Column7
            // 
            Column7.HeaderText = "点名";
            Column7.Name = "Column7";
            // 
            // colType
            // 
            colType.HeaderText = "近景？遠景？";
            colType.Items.AddRange(new object[] { "近景", "遠景", "予備1", "予備2", "予備3" });
            colType.Name = "colType";
            colType.Width = 60;
            // 
            // Column8
            // 
            Column8.HeaderText = "新写真ファイル名";
            Column8.Name = "Column8";
            Column8.ReadOnly = true;
            Column8.Width = 120;
            // 
            // button2
            // 
            button2.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            button2.Location = new Point(572, 66);
            button2.Name = "button2";
            button2.Size = new Size(75, 23);
            button2.TabIndex = 11;
            button2.Text = "リネーム実行";
            button2.UseVisualStyleBackColor = true;
            button2.Click += btnRename_Click;
            // 
            // button3
            // 
            button3.Location = new Point(12, 41);
            button3.Name = "button3";
            button3.Size = new Size(122, 23);
            button3.TabIndex = 1;
            button3.Text = "googleアース出力";
            button3.UseVisualStyleBackColor = true;
            button3.Click += btnExportKml_Click;
            // 
            // btnSelectAll
            // 
            btnSelectAll.Location = new Point(12, 70);
            btnSelectAll.Name = "btnSelectAll";
            btnSelectAll.Size = new Size(58, 23);
            btnSelectAll.TabIndex = 2;
            btnSelectAll.Text = "全選択";
            btnSelectAll.UseVisualStyleBackColor = true;
            btnSelectAll.Click += btnSelectAll_Click;
            // 
            // btnDeselectAll
            // 
            btnDeselectAll.Location = new Point(76, 70);
            btnDeselectAll.Name = "btnDeselectAll";
            btnDeselectAll.Size = new Size(58, 23);
            btnDeselectAll.TabIndex = 3;
            btnDeselectAll.Text = "全解除";
            btnDeselectAll.UseVisualStyleBackColor = true;
            btnDeselectAll.Click += btnDeselectAll_Click;
            // 
            // chkDownsize
            // 
            chkDownsize.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            chkDownsize.AutoSize = true;
            chkDownsize.Location = new Point(487, 41);
            chkDownsize.Name = "chkDownsize";
            chkDownsize.Size = new Size(160, 19);
            chkDownsize.TabIndex = 10;
            chkDownsize.Text = "写真を150KB前後に軽量化";
            chkDownsize.UseVisualStyleBackColor = true;
            // 
            // rdoSortDate
            // 
            rdoSortDate.AutoSize = true;
            rdoSortDate.Location = new Point(157, 17);
            rdoSortDate.Name = "rdoSortDate";
            rdoSortDate.Size = new Size(120, 19);
            rdoSortDate.TabIndex = 4;
            rdoSortDate.TabStop = true;
            rdoSortDate.Text = "撮影日時順でソート";
            rdoSortDate.UseVisualStyleBackColor = true;
            rdoSortDate.CheckedChanged += rdoSortDate_CheckedChanged;
            // 
            // rdoSortName
            // 
            rdoSortName.AutoSize = true;
            rdoSortName.Location = new Point(157, 41);
            rdoSortName.Name = "rdoSortName";
            rdoSortName.Size = new Size(164, 19);
            rdoSortName.TabIndex = 5;
            rdoSortName.TabStop = true;
            rdoSortName.Text = "オリジナルファイル名順でソート";
            rdoSortName.UseVisualStyleBackColor = true;
            rdoSortName.CheckedChanged += rdoSort_CheckedChanged;
            // 
            // btnSizeMedium
            // 
            btnSizeMedium.Location = new Point(283, 70);
            btnSizeMedium.Name = "btnSizeMedium";
            btnSizeMedium.Size = new Size(32, 23);
            btnSizeMedium.TabIndex = 7;
            btnSizeMedium.Text = "中";
            btnSizeMedium.UseVisualStyleBackColor = true;
            btnSizeMedium.Click += btnSizeMedium_Click;
            // 
            // btnSizeLarge
            // 
            btnSizeLarge.Location = new Point(321, 70);
            btnSizeLarge.Name = "btnSizeLarge";
            btnSizeLarge.Size = new Size(32, 23);
            btnSizeLarge.TabIndex = 8;
            btnSizeLarge.Text = "大";
            btnSizeLarge.UseVisualStyleBackColor = true;
            btnSizeLarge.Click += btnSizeLarge_Click;
            // 
            // btnSizeSmall
            // 
            btnSizeSmall.Location = new Point(245, 70);
            btnSizeSmall.Name = "btnSizeSmall";
            btnSizeSmall.Size = new Size(32, 23);
            btnSizeSmall.TabIndex = 6;
            btnSizeSmall.Text = "小";
            btnSizeSmall.UseVisualStyleBackColor = true;
            btnSizeSmall.Click += btnSizeSmall_Click;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(157, 74);
            label1.Name = "label1";
            label1.Size = new Size(82, 15);
            label1.TabIndex = 0;
            label1.Text = "サムネイルサイズ";
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = SystemColors.GradientInactiveCaption;
            ClientSize = new Size(672, 730);
            Controls.Add(label1);
            Controls.Add(btnSizeMedium);
            Controls.Add(btnSizeLarge);
            Controls.Add(btnSizeSmall);
            Controls.Add(rdoSortName);
            Controls.Add(rdoSortDate);
            Controls.Add(chkDownsize);
            Controls.Add(btnDeselectAll);
            Controls.Add(btnSelectAll);
            Controls.Add(button3);
            Controls.Add(button2);
            Controls.Add(dataGridView1);
            Controls.Add(button1);
            MinimumSize = new Size(350, 500);
            Name = "Form1";
            Text = "いきなりリネーム 近景遠景";
            ((System.ComponentModel.ISupportInitialize)dataGridView1).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private Button button1;
        private DataGridView dataGridView1;
        private Button button2;
        private Button button3;
        private Button btnSelectAll;
        private Button btnDeselectAll;
        private DataGridViewCheckBoxColumn Column1;
        private DataGridViewTextBoxColumn Column3;
        private DataGridViewImageColumn Column4;
        private DataGridViewTextBoxColumn Column7;
        private DataGridViewComboBoxColumn colType;
        private DataGridViewTextBoxColumn Column8;
        private CheckBox chkDownsize;
        private RadioButton rdoSortDate;
        private RadioButton rdoSortName;
        private Button btnSizeMedium;
        private Button btnSizeLarge;
        private Button btnSizeSmall;
        private Label label1;
    }
}
