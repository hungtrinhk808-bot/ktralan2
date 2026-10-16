using System;
using System.Windows.Forms;

namespace QuanLySieuThi
{
    public class frmMain : Form
    {
        private readonly MenuStrip menuStrip = new MenuStrip();
        private readonly StatusStrip statusStrip = new StatusStrip();
        private readonly ToolStripStatusLabel lblUser =
            new ToolStripStatusLabel("Người dùng: Admin");
        private readonly ToolStripStatusLabel lblStatus =
            new ToolStripStatusLabel("Trạng thái: Sẵn sàng");
        private readonly ToolStripProgressBar progressBar =
            new ToolStripProgressBar();

        public frmMain()
        {
            Text = "HỆ THỐNG QUẢN LÝ SIÊU THỊ - MDI";
            Width = 1200;
            Height = 750;
            StartPosition = FormStartPosition.CenterScreen;
            IsMdiContainer = true;
            BuildMenu();
            BuildStatusStrip();
        }

        private void BuildMenu()
        {
            var mnuSystem = new ToolStripMenuItem("Hệ thống");
            var mnuLogout = new ToolStripMenuItem("Đăng xuất");
            var mnuExit = new ToolStripMenuItem("Thoát");

            mnuLogout.Click += (s, e) =>
            {
                MessageBox.Show("Đã đăng xuất khỏi hệ thống.", "Thông báo");
                Close();
            };
            mnuExit.Click += (s, e) => Close();
            mnuSystem.DropDownItems.AddRange(new ToolStripItem[]
            {
                mnuLogout, mnuExit
            });

            var mnuCategory = new ToolStripMenuItem("Danh mục");
            var mnuProduct = new ToolStripMenuItem("Quản lý Sản phẩm");
            var mnuImportExport = new ToolStripMenuItem("Quản lý Nhập xuất");
            mnuProduct.Click += (s, e) => OpenChildForm<frmProductCatalog>();
            mnuImportExport.Click += (s, e) => OpenChildForm<frmAsyncReport>();
            mnuCategory.DropDownItems.AddRange(new ToolStripItem[]
            {
                mnuProduct, mnuImportExport
            });

            var mnuReport = new ToolStripMenuItem("Báo cáo");
            var mnuRevenue = new ToolStripMenuItem("Thống kê Doanh thu");
            mnuRevenue.Click += (s, e) => OpenChildForm<frmAsyncReport>();
            mnuReport.DropDownItems.Add(mnuRevenue);

            var mnuWindow = new ToolStripMenuItem("Cửa sổ");
            var mnuCascade = new ToolStripMenuItem("Cascade");
            var mnuHorizontal = new ToolStripMenuItem("Tile Horizontal");
            var mnuVertical = new ToolStripMenuItem("Tile Vertical");
            mnuCascade.Click += (s, e) => LayoutMdi(MdiLayout.Cascade);
            mnuHorizontal.Click += (s, e) => LayoutMdi(MdiLayout.TileHorizontal);
            mnuVertical.Click += (s, e) => LayoutMdi(MdiLayout.TileVertical);
            mnuWindow.DropDownItems.AddRange(new ToolStripItem[]
            {
                mnuCascade, mnuHorizontal, mnuVertical
            });

            menuStrip.Items.AddRange(new ToolStripItem[]
            {
                mnuSystem, mnuCategory, mnuReport, mnuWindow
            });
            MainMenuStrip = menuStrip;
            Controls.Add(menuStrip);
        }

        private void BuildStatusStrip()
        {
            progressBar.Minimum = 0;
            progressBar.Maximum = 100;
            progressBar.Value = 0;
            progressBar.Visible = false;
            progressBar.Width = 180;

            statusStrip.Items.Add(lblUser);
            statusStrip.Items.Add(new ToolStripStatusLabel(" | "));
            statusStrip.Items.Add(lblStatus);
            statusStrip.Items.Add(new ToolStripStatusLabel { Spring = true });
            statusStrip.Items.Add(progressBar);
            Controls.Add(statusStrip);
        }

        private void OpenChildForm<T>() where T : Form, new()
        {
            foreach (Form child in MdiChildren)
            {
                if (child is T)
                {
                    if (child.WindowState == FormWindowState.Minimized)
                        child.WindowState = FormWindowState.Normal;
                    child.Activate();
                    return;
                }
            }

            var form = new T
            {
                MdiParent = this,
                WindowState = FormWindowState.Maximized
            };
            form.Show();
        }

        public void SetProgress(int value)
        {
            if (IsDisposed || Disposing) return;
            if (InvokeRequired)
            {
                BeginInvoke(new Action<int>(SetProgress), value);
                return;
            }

            value = Math.Max(0, Math.Min(100, value));
            progressBar.Value = value;
            progressBar.Visible = true;
            lblStatus.Text = "Đang tải báo cáo: " + value + "%";
        }

        public void FinishProgress(string message)
        {
            if (IsDisposed || Disposing) return;
            if (InvokeRequired)
            {
                BeginInvoke(new Action<string>(FinishProgress), message);
                return;
            }

            progressBar.Value = 0;
            progressBar.Visible = false;
            lblStatus.Text = message;
        }

        public void SetStatus(string message)
        {
            if (IsDisposed || Disposing) return;
            if (InvokeRequired)
            {
                BeginInvoke(new Action<string>(SetStatus), message);
                return;
            }

            lblStatus.Text = message;
        }
    }
}
