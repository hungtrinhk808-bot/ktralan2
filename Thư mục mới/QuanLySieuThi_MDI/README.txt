HƯỚNG DẪN CHẠY - QUAN LY SIEU THI MDI

Yêu cầu:
- Windows
- Visual Studio 2022 (hoặc bản hỗ trợ .NET 8)
- Workload: .NET desktop development

Cách chạy:
1. Giải nén file ZIP.
2. Mở QuanLySieuThi.csproj bằng Visual Studio.
3. Nếu Visual Studio yêu cầu tải .NET SDK/targeting pack, cài .NET 8 SDK.
4. Chọn Build > Build Solution.
5. Nhấn F5 để chạy.

Chức năng:
- Form cha MDI có menu Hệ thống, Danh mục, Báo cáo, Cửa sổ.
- Không mở trùng Form con; Form đã mở sẽ được Activate.
- Form quản lý sản phẩm dùng DataGridView với AutoGenerateColumns = false.
- Báo cáo doanh thu dùng async/await + Task.Run và tiến trình giả lập 5 giây.
- Đồng bộ dữ liệu chạy nền theo chu kỳ 2 giây.
- Cập nhật giao diện qua InvokeRequired/BeginInvoke để tránh cross-thread operation.

Ghi chú:
- Đây là bản demo độc lập, dùng dữ liệu mẫu trong bộ nhớ; chưa kết nối SQL Server.
- Mục "Quản lý Nhập xuất" hiện mở Form báo cáo/đồng bộ để minh họa điều hướng.
