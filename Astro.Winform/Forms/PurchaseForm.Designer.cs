namespace Astro.Winform.Forms
{
    partial class PurchaseForm
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
            Models.Transactions.Purchase purchase1 = new Models.Transactions.Purchase();
            purchaseControl1 = new UserControls.PurchaseControl();
            SuspendLayout();
            // 
            // purchaseControl1
            // 
            purchaseControl1.BackColor = Color.White;
            purchaseControl1.Dock = DockStyle.Fill;
            purchaseControl1.Font = new Font("Segoe UI", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            purchaseControl1.Location = new Point(0, 0);
            purchaseControl1.Name = "purchaseControl1";
            purchase1.Cost = 0;
            purchase1.Date = new DateTime(2025, 8, 15, 14, 58, 43, 240);
            purchase1.Discount = 0;
            purchase1.Id = new Guid("00000000-0000-0000-0000-000000000000");
            purchase1.InvoiceNumber = "";
            purchase1.Location = 0;
            purchase1.SubTotal = 0L;
            purchase1.SupplierId = 0;
            purchase1.Tax = 0;
            purchase1.TotalPaid = 0L;
            purchaseControl1.Purchase = purchase1;
            purchaseControl1.Size = new Size(1236, 520);
            purchaseControl1.TabIndex = 0;
            // 
            // PurchaseForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1236, 520);
            Controls.Add(purchaseControl1);
            Name = "PurchaseForm";
            Text = "PurchaseForm";
            ResumeLayout(false);
        }

        #endregion

        private UserControls.PurchaseControl purchaseControl1;
    }
}