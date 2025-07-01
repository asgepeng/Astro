using Astro.Winform.Classes;
using Astro.Winform.Forms;
using PointOfSale.Drawing;

namespace PointOfSale
{
    public partial class MainForm : Form
    {
        private NavigationView navigator;
        private TopNavigationView topNavigator;
        private VirtualControlCollection VirtualControls { get; } = new VirtualControlCollection();
        public MainForm()
        {
            InitializeComponent();
            this.DoubleBuffered = true;
            this.SetStyle(ControlStyles.UserPaint |
                          ControlStyles.AllPaintingInWmPaint |
                          ControlStyles.OptimizedDoubleBuffer, true);
            this.UpdateStyles();

            //Top Navigator
            this.topNavigator = new TopNavigationView();

            //Navigator
            this.navigator = new NavigationView();
            this.navigator.Items.Add("Dashboard", global::Astro.Winform.Properties.Resources.db);
            this.navigator.Items.Add("Master Data", global::Astro.Winform.Properties.Resources.data);
            this.navigator.Items.Add("Pengaturan", global::Astro.Winform.Properties.Resources.settings);
            this.navigator.SelectedIndex = 0;

            this.VirtualControls.Add(topNavigator);
            this.VirtualControls.Add(navigator);

            My.Application.ApiUrl = "http://localhost:5002";
            this.Load += new EventHandler(this.FormLoadHandler);
        }
        private void FormLoadHandler(object? sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Maximized;
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            foreach (var item in this.VirtualControls)
            {
                item.Draw(e.Graphics);
            }
        }
        protected override void OnResize(EventArgs e)
        {
            if (navigator != null) navigator.MainFormResize(this.ClientSize);
            if (topNavigator != null) topNavigator.MainFormResize(this.ClientSize);
            base.OnResize(e);
        }
        protected override void OnResizeEnd(EventArgs e)
        {

        }
        protected override void OnMouseMove(MouseEventArgs e)
        {
            var anyHovered = navigator.GetHoveredItem(e.Location);
            if (anyHovered != null)
            {
                if (anyHovered.Hovered)
                {
                    this.toolTipGlobal.ToolTipTitle = anyHovered.Text;
                    this.toolTipGlobal.Show(anyHovered.Text, this, new Point(anyHovered.Bounds.X + anyHovered.Bounds.Width + 20, anyHovered.Bounds.Y + 30));
                }
                this.Invalidate(this.navigator.Bounds);
            }
            if (!navigator.Items.AnyHoveredItem())
            {
                this.toolTipGlobal.Hide(this);
            }
        }
        protected override void OnMouseClick(MouseEventArgs e)
        {
            if (navigator.SelectedChanged(e.Location))
            {
                this.Invalidate(navigator.Bounds);
            }
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            var form = new LoginForm();
            if (form.ShowDialog() == DialogResult.OK)
            {
                this.WindowState = FormWindowState.Maximized;
                if (My.Application.User != null)
                {
                    this.Text = My.Application.User.Name + " - " + My.Application.User.Role.Name;
                }
            }
            else
            {
                this.Close();
            }
        }

        private async void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (My.Application.ApiToken.Trim() != "") await HttpClientSingleton.SignOutAsync();
            HttpClientSingleton.Dispose();
        }
    }
}
