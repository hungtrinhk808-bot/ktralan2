using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace QuanLySieuThi
{
    public class frmProductCatalog : Form
    {
        private readonly BindingList<Product> products = new BindingList<Product>();
        private DataGridView dgvProducts;
        private TextBox txtName;
        private NumericUpDown numPrice;
        private NumericUpDown numQuantity;

        public frmProductCatalog()
        {
            Text = "QUẢN LÝ DANH MỤC SẢN PHẨM";
            Width = 900;
            Height = 550;
            BuildInterface();
            LoadSampleData();
        }

        private void BuildInterface()
        {
            var inputPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 105,
                Padding = new Padding(10)
            };

            var lblName = new Label { Text = "Tên sản phẩm:", Left = 10, Top = 15, AutoSize = true };
            txtName = new TextBox { Left = 105, Top = 10, Width = 180 };

            var lblPrice = new Label { Text = "Đơn giá:", Left = 305, Top = 15, AutoSize = true };
            numPrice = new NumericUpDown
            {
                Left = 365, Top = 10, Width = 130,
                Maximum = 1000000000, DecimalPlaces = 0,
                ThousandsSeparator = true
            };

            var lblQuantity = new Label { Text = "Số lượng:", Left = 515, Top = 15, AutoSize = true };
            numQuantity = new NumericUpDown
            {
                Left = 580, Top = 10, Width = 90,
                Maximum = 1000000, Minimum = 0
            };

            var btnAdd = new Button { Text = "Thêm sản phẩm", Left = 690, Top = 8, Width = 130, Height = 30 };
            btnAdd.Click += BtnAdd_Click;

            var note = new Label
            {
                Text = "Dữ liệu mẫu được lưu trong bộ nhớ, chưa kết nối cơ sở dữ liệu.",
                Left = 10, Top = 55, AutoSize = true, ForeColor = Color.DarkSlateGray
            };

            inputPanel.Controls.AddRange(new Control[]
            {
                lblName, txtName, lblPrice, numPrice, lblQuantity, numQuantity, btnAdd, note
            });

            dgvProducts = new DataGridView
            {
                Dock = DockStyle.Fill,
                AutoGenerateColumns = false,
                AllowUserToAddRows = false,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                DataSource = products
            };

            dgvProducts.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "Id", HeaderText = "Mã sản phẩm", FillWeight = 70
            });
            dgvProducts.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "Name", HeaderText = "Tên sản phẩm", FillWeight = 160
            });
            dgvProducts.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "Price", HeaderText = "Đơn giá (VNĐ)",
                DefaultCellStyle = new DataGridViewCellStyle { Format = "N0" }
            });
            dgvProducts.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "Quantity", HeaderText = "Số lượng"
            });

            Controls.Add(dgvProducts);
            Controls.Add(inputPanel);
        }

        private void LoadSampleData()
        {
            products.Add(new Product { Id = 1, Name = "Tivi Samsung", Price = 8500000, Quantity = 10 });
            products.Add(new Product { Id = 2, Name = "Tủ lạnh LG", Price = 12000000, Quantity = 5 });
            products.Add(new Product { Id = 3, Name = "Quạt điện", Price = 650000, Quantity = 20 });
        }

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            string name = txtName.Text.Trim();
            if (string.IsNullOrWhiteSpace(name))
            {
                MessageBox.Show("Vui lòng nhập tên sản phẩm.", "Thiếu thông tin",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtName.Focus();
                return;
            }

            int nextId = products.Count == 0 ? 1 : products[products.Count - 1].Id + 1;
            products.Add(new Product
            {
                Id = nextId,
                Name = name,
                Price = numPrice.Value,
                Quantity = (int)numQuantity.Value
            });

            txtName.Clear();
            numPrice.Value = 0;
            numQuantity.Value = 0;
            txtName.Focus();
        }
    }
}
