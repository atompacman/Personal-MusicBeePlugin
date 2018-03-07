namespace MusicBeePlugin.RYMExporter.Source
{
   partial class Interface
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

      #region Component Designer generated code

      /// <summary> 
      /// Required method for Designer support - do not modify 
      /// the contents of this method with the code editor.
      /// </summary>
      private void InitializeComponent()
      {
         this.richTextBox1 = new System.Windows.Forms.RichTextBox();
         this.trackBar1 = new System.Windows.Forms.TrackBar();
         this.label1 = new System.Windows.Forms.Label();
         this.button1 = new System.Windows.Forms.Button();
         this.groupBox1 = new System.Windows.Forms.GroupBox();
         this.groupBox2 = new System.Windows.Forms.GroupBox();
         this.groupBox3 = new System.Windows.Forms.GroupBox();
         this.textBox1 = new System.Windows.Forms.TextBox();
         ((System.ComponentModel.ISupportInitialize)(this.trackBar1)).BeginInit();
         this.groupBox1.SuspendLayout();
         this.groupBox2.SuspendLayout();
         this.groupBox3.SuspendLayout();
         this.SuspendLayout();
         // 
         // richTextBox1
         // 
         this.richTextBox1.Location = new System.Drawing.Point(6, 19);
         this.richTextBox1.Name = "richTextBox1";
         this.richTextBox1.Size = new System.Drawing.Size(582, 367);
         this.richTextBox1.TabIndex = 0;
         this.richTextBox1.Text = "";
         // 
         // trackBar1
         // 
         this.trackBar1.Location = new System.Drawing.Point(6, 19);
         this.trackBar1.Maximum = 20;
         this.trackBar1.Minimum = 1;
         this.trackBar1.Name = "trackBar1";
         this.trackBar1.Size = new System.Drawing.Size(484, 45);
         this.trackBar1.TabIndex = 1;
         this.trackBar1.Value = 6;
         this.trackBar1.Scroll += new System.EventHandler(this.TrackBar1_Scroll);
         // 
         // label1
         // 
         this.label1.AutoSize = true;
         this.label1.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
         this.label1.Location = new System.Drawing.Point(496, 25);
         this.label1.Name = "label1";
         this.label1.Size = new System.Drawing.Size(38, 13);
         this.label1.TabIndex = 2;
         this.label1.Text = "label1";
         // 
         // button1
         // 
         this.button1.Location = new System.Drawing.Point(3, 529);
         this.button1.Name = "button1";
         this.button1.Size = new System.Drawing.Size(196, 23);
         this.button1.TabIndex = 3;
         this.button1.Text = "Copy to clipboard and set album rating";
         this.button1.UseVisualStyleBackColor = true;
         this.button1.Click += new System.EventHandler(this.Button1_Click);
         // 
         // groupBox1
         // 
         this.groupBox1.Controls.Add(this.trackBar1);
         this.groupBox1.Controls.Add(this.label1);
         this.groupBox1.ForeColor = System.Drawing.SystemColors.ControlLight;
         this.groupBox1.Location = new System.Drawing.Point(3, 3);
         this.groupBox1.Name = "groupBox1";
         this.groupBox1.Size = new System.Drawing.Size(594, 67);
         this.groupBox1.TabIndex = 6;
         this.groupBox1.TabStop = false;
         this.groupBox1.Text = "Album rating";
         // 
         // groupBox2
         // 
         this.groupBox2.Controls.Add(this.richTextBox1);
         this.groupBox2.ForeColor = System.Drawing.SystemColors.ControlLight;
         this.groupBox2.Location = new System.Drawing.Point(3, 76);
         this.groupBox2.Name = "groupBox2";
         this.groupBox2.Size = new System.Drawing.Size(594, 392);
         this.groupBox2.TabIndex = 7;
         this.groupBox2.TabStop = false;
         this.groupBox2.Text = "Generated track rating text";
         // 
         // groupBox3
         // 
         this.groupBox3.Controls.Add(this.textBox1);
         this.groupBox3.ForeColor = System.Drawing.SystemColors.ControlLight;
         this.groupBox3.Location = new System.Drawing.Point(3, 474);
         this.groupBox3.Name = "groupBox3";
         this.groupBox3.Size = new System.Drawing.Size(594, 49);
         this.groupBox3.TabIndex = 8;
         this.groupBox3.TabStop = false;
         this.groupBox3.Text = "Genre";
         // 
         // textBox1
         // 
         this.textBox1.Location = new System.Drawing.Point(7, 20);
         this.textBox1.Name = "textBox1";
         this.textBox1.Size = new System.Drawing.Size(581, 20);
         this.textBox1.TabIndex = 0;
         // 
         // Interface
         // 
         this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.Controls.Add(this.groupBox3);
         this.Controls.Add(this.groupBox2);
         this.Controls.Add(this.groupBox1);
         this.Controls.Add(this.button1);
         this.Name = "Interface";
         this.Size = new System.Drawing.Size(600, 564);
         ((System.ComponentModel.ISupportInitialize)(this.trackBar1)).EndInit();
         this.groupBox1.ResumeLayout(false);
         this.groupBox1.PerformLayout();
         this.groupBox2.ResumeLayout(false);
         this.groupBox3.ResumeLayout(false);
         this.groupBox3.PerformLayout();
         this.ResumeLayout(false);

      }

      #endregion

      private System.Windows.Forms.RichTextBox richTextBox1;
      private System.Windows.Forms.TrackBar trackBar1;
      private System.Windows.Forms.Label label1;
      private System.Windows.Forms.Button button1;
      private System.Windows.Forms.GroupBox groupBox1;
      private System.Windows.Forms.GroupBox groupBox2;
      private System.Windows.Forms.GroupBox groupBox3;
      private System.Windows.Forms.TextBox textBox1;
   }
}
