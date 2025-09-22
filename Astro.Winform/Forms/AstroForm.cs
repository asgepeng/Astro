using Astro.Data;
using Astro.Forms.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Astro.Winform.Forms
{
    public partial class AstroForm : Form
    {
        private readonly IDBClient db = My.Application.CreateDBAccess();
        private SideBar sidebar;
        public AstroForm()
        {
            InitializeComponent();
            this.SetStyle(ControlStyles.AllPaintingInWmPaint
            | ControlStyles.UserPaint
            | ControlStyles.ResizeRedraw
            | ControlStyles.OptimizedDoubleBuffer, true);
            this.UpdateStyles();

            this.sidebar = new SideBar();

            this.sidebar.Location = new Point(0, 40);
            this.sidebar.Size = new Size(200, this.Height - 40);
            this.VirtualControls.Add(this.sidebar);
        }
        private VirtualControlCollection VirtualControls { get; } = new VirtualControlCollection();
        protected override void OnPaintBackground(PaintEventArgs e)
        {
            base.OnPaintBackground(e);
            this.sidebar.DrawBackground(e.Graphics);
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            this.VirtualControls.Draw(e.Graphics);
        }
        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            if (this.sidebar.Bounds.Contains(e.Location))
            {
                if (this.sidebar.MouseDown(e))
                {
                    this.Invalidate(sidebar.Bounds);
                }
                return;
            }
        }
        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if (this.sidebar.IsDragging)
            {
                if (this.sidebar.MouseMove(e))
                {
                    this.Invalidate(this.sidebar.Bounds);
                }
                return;
            }
            if (this.sidebar.Pressed && !this.sidebar.Bounds.Contains(e.Location)) return;
            if (this.sidebar.Bounds.Contains(e.Location))
            {
                if (this.sidebar.MouseMove(e))
                {
                    this.Invalidate(this.sidebar.Bounds);
                }
                return;
            }
            this.sidebar.MouseLeave();
            this.Invalidate(this.sidebar.Bounds);
        }
        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            if (this.sidebar.IsDragging)
            {
                if (this.sidebar.MouseMove(e))
                {
                    this.sidebar.MouseUp(e);
                    this.Invalidate(this.sidebar.Bounds);
                }
                return;
            }
            if (this.sidebar.Pressed && !this.sidebar.Bounds.Contains(e.Location))
            {
                this.sidebar.MouseUp(e);
                this.sidebar.MouseLeave();
                this.Invalidate(this.sidebar.Bounds);
            }
            if (this.sidebar.Bounds.Contains(e.Location))
            {
                var item = this.sidebar.ItemClicked(e);
                if (item != null)
                {
                    this.Invalidate(this.sidebar.Bounds);
                    return;
                }
            }
        }
        protected override void OnMouseWheel(MouseEventArgs e)
        {
            base.OnMouseWheel(e);
            if (this.sidebar.Bounds.Contains(e.Location))
            {
                this.sidebar.MouseWheel(e);
                this.Invalidate(sidebar.Bounds);
            }
        }
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            if (this.sidebar != null) this.sidebar.Resize(this.ClientSize);
        }
        private void AstroForm_Click(object sender, EventArgs e)
        {

        }

        private async void AstroForm_Load(object sender, EventArgs e)
        {
            await this.sidebar.LoadMenuAsync(db);
            this.sidebar.ArrangeLayout();
            this.Invalidate();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
