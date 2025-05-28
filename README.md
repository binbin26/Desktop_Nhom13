# 📚 Hệ thống Quản lý Học tập (LMS)

Đây là dự án môn học "Desktop" tại Đại học Kinh tế Thành phố Hồ Chí Minh (UEH), phát triển một hệ thống quản lý học tập (LMS) hỗ trợ giảng viên và sinh viên trong việc giảng dạy và học tập trực tuyến.([GitHub][1])

## 🧩 Tính năng chính

* Quản lý người dùng: Đăng ký, đăng nhập, phân quyền (sinh viên, giảng viên, quản trị viên).
* Quản lý khóa học: Tạo, chỉnh sửa, xóa khóa học.
* Quản lý bài học: Thêm, sửa, xóa nội dung bài học.
* Quản lý bài tập: Tạo, giao, chấm điểm bài tập.
* Quản lý điểm số: Nhập, sửa, xem điểm.
* Giao diện người dùng thân thiện, dễ sử dụng.

## 🛠️ Công nghệ sử dụng

* Ngôn ngữ lập trình: C#
* Cơ sở dữ liệu: SQL Server
* Giao diện người dùng: Windows Forms
* Mô hình kiến trúc: 3 lớp (DAL, BLL, GUI)

## 🚀 Hướng dẫn cài đặt và chạy dự án

### Yêu cầu hệ thống

* Hệ điều hành: Windows 10 trở lên
* Visual Studio 2019 hoặc mới hơn
* SQL Server 2017 hoặc mới hơn

### Các bước cài đặt

1. Clone repository về máy:([GitHub][2])

   ```bash
   git clone https://github.com/binbin26/CNPM.git
   ```
2. Mở file `CNPM.sln` bằng Visual Studio.
3. Khôi phục các gói NuGet nếu cần.
4. Cấu hình chuỗi kết nối cơ sở dữ liệu trong `App.config`.
5. Nhấp chuột phải vào Databases và chọn Import Data-tier Application file 'EduMasterDB.bacpac' để nhập cơ sở dữ liệu.
6. Chạy ứng dụng bằng cách nhấn F5 hoặc chọn "Start" trong Visual Studio.([GitHub][3])
7.Chọn 1 trong các tài khoản ở `SQLQuery1.sql` để đăng nhập hoặc đăng ký.
## 📁 Cấu trúc thư mục

* `DAL/`: Lớp truy cập dữ liệu.
* `BLL/`: Lớp xử lý nghiệp vụ.
* `Forms/`: Giao diện người dùng.
* `Models/`: Các lớp mô hình dữ liệu.
* `Utilities/`: Các tiện ích hỗ trợ.
* `Resources/Branding/`: Tài nguyên giao diện.

## 👥 Thành viên nhóm
*Võ Thị Bình
* Nguyễn Gia Bảo 
* Phạm Huỳnh Tố Quyên 

## 📄 Giấy phép

Dự án này được phát triển cho mục đích học tập và không sử dụng cho mục đích thương mại.

---

Nếu bạn cần thêm thông tin hoặc hỗ trợ, vui lòng liên hệ với nhóm phát triển hoặc tạo issue trên GitHub.

