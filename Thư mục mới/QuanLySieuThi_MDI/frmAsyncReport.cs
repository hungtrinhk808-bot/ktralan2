using System;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace QuanLySieuThi
{
    public class frmAsyncReport : Form
    {
        private Button btnLoadReport;
        private Button btnStartSync;
        private Button btnStopSync;
        private Label lblReport;
        private Label lblSync;
        private DataGridView dgvUpdates;
        private BindingList<SyncRecord> syncRecords = new BindingList<SyncRecord>();
        private CancellationTokenSource syncCts;
        private bool reportRunning;

        public frmAsyncReport()
        {
            Text = "BÁO CÁO DOANH THU VÀ ĐỒNG BỘ DỮ LIỆU";
            Width = 950;
            Height = 600;
            BuildInterface();
        }

        private void BuildInterface()
        {
            var topPanel = new Panel { Dock = DockStyle.Top, Height = 145, Padding = new Padding(12) };

            btnLoadReport = new Button
            {
                Text = "Tải Báo Cáo Doanh Thu",
                Left = 12, Top = 12, Width = 210, Height = 36
            };
            btnLoadReport.Click += async (s, e) => await LoadRevenueReportAsync();

            lblReport = new Label
            {
                Text = "Báo cáo: Chưa tải dữ liệu.",
                Left = 240, Top = 20, Width = 600, Height = 25
            };

            btnStartSync = new Button
            {
                Text = "Bắt đầu đồng bộ",
                Left = 12, Top = 62, Width = 150, Height = 32
            };
            btnStartSync.Click += BtnStartSync_Click;

            btnStopSync = new Button
            {
                Text = "Dừng đồng bộ",
                Left = 172, Top = 62, Width = 140, Height = 32,
                Enabled = false
            };
            btnStopSync.Click += (s, e) => StopSync();

            lblSync = new Label
            {
                Text = "Đồng bộ: Đang dừng.",
                Left = 330, Top = 70, Width = 500, Height = 25
            };

            var note = new Label
            {
                Text = "Báo cáo chạy nền bằng async/await + Task.Run; dữ liệu đồng bộ cập nhật bằng Invoke.",
                Left = 12, Top = 108, Width = 850, Height = 25
            };

            topPanel.Controls.AddRange(new Control[]
            {
                btnLoadReport, lblReport, btnStartSync, btnStopSync, lblSync, note
            });

            dgvUpdates = new DataGridView
            {
                Dock = DockStyle.Fill,
                AutoGenerateColumns = false,
                AllowUserToAddRows = false,
                ReadOnly = true,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                DataSource = syncRecords
            };
            dgvUpdates.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "Time", HeaderText = "Thời gian"
            });
            dgvUpdates.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "Message", HeaderText = "Dữ liệu mới"
            });

            Controls.Add(dgvUpdates);
            Controls.Add(topPanel);
        }

        private async Task LoadRevenueReportAsync()
        {
            if (reportRunning) return;

            reportRunning = true;
            btnLoadReport.Enabled = false;

            frmMain main = MdiParent as frmMain;
            if (main != null)
            {
                main.SetProgress(0);
                main.SetStatus("Đang tải báo cáo doanh thu...");
            }

            try
            {
                // Progress<T> được tạo trên UI thread nên callback trở về UI thread.
                IProgress<int> progress = new Progress<int>(value =>
                {
                    if (main != null) main.SetProgress(value);
                    lblReport.Text = "Đang xử lý báo cáo: " + value + "%";
                });

                decimal totalRevenue = await Task.Run(async () =>
                {
                    decimal total = 0;
                    const int steps = 5;

                    for (int i = 1; i <= steps; i++)
                    {
                        // Giả lập truy xuất dữ liệu/tác vụ nặng 1 giây mỗi bước.
                        await Task.Delay(1000);
                        total += 1250000m * i;
                        progress.Report(i * 100 / steps);
                    }

                    return total;
                });

                lblReport.Text = "Tổng doanh thu mẫu: " + totalRevenue.ToString("N0") + " VNĐ";
                if (main != null) main.FinishProgress("Tải báo cáo hoàn tất.");
                MessageBox.Show("Đã tải báo cáo doanh thu thành công.",
                    "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                if (main != null) main.FinishProgress("Tải báo cáo thất bại.");
                MessageBox.Show("Không thể tải báo cáo: " + ex.Message,
                    "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                reportRunning = false;
                if (!IsDisposed) btnLoadReport.Enabled = true;
            }
        }

        private void BtnStartSync_Click(object sender, EventArgs e)
        {
            if (syncCts != null) return;

            syncCts = new CancellationTokenSource();
            btnStartSync.Enabled = false;
            btnStopSync.Enabled = true;
            lblSync.Text = "Đồng bộ: Đang chạy...";

            CancellationToken token = syncCts.Token;
            _ = Task.Run(async () =>
            {
                int orderNumber = 1000;

                try
                {
                    while (!token.IsCancellationRequested)
                    {
                        await Task.Delay(2000, token);
                        orderNumber++;

                        string message = "Phát hiện đơn hàng mới #" + orderNumber;
                        // Không truy cập Control trực tiếp từ thread nền.
                        UpdateSyncUI(message);
                    }
                }
                catch (OperationCanceledException)
                {
                    // Dừng đồng bộ theo yêu cầu người dùng.
                }
            }, token);
        }

        private void UpdateSyncUI(string message)
        {
            if (IsDisposed || Disposing) return;

            if (InvokeRequired)
            {
                try
                {
                    BeginInvoke(new Action<string>(UpdateSyncUI), message);
                }
                catch (InvalidOperationException)
                {
                    // Form đã đóng trong lúc thread nền gửi cập nhật.
                }
                return;
            }

            syncRecords.Insert(0, new SyncRecord
            {
                Time = DateTime.Now.ToString("HH:mm:ss"),
                Message = message
            });

            // Giới hạn số dòng để bảng không tăng vô hạn.
            while (syncRecords.Count > 100)
                syncRecords.RemoveAt(syncRecords.Count - 1);

            lblSync.Text = "Đồng bộ: " + message;
            frmMain main = MdiParent as frmMain;
            if (main != null) main.SetStatus(message);
        }

        private void StopSync()
        {
            if (syncCts == null) return;

            syncCts.Cancel();
            syncCts.Dispose();
            syncCts = null;

            btnStartSync.Enabled = true;
            btnStopSync.Enabled = false;
            lblSync.Text = "Đồng bộ: Đã dừng.";
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            StopSync();
            base.OnFormClosing(e);
        }
    }

    public class SyncRecord
    {
        public string Time { get; set; }
        public string Message { get; set; }
    }
}
