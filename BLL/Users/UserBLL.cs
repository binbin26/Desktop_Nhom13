using Desktop_Nhom13.DAL;
using Desktop_Nhom13.Models.Users;
using Desktop_Nhom13.Utilities;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Windows.Forms;

namespace Desktop_Nhom13.BLL
{
    public class UserBLL
    {
        public class UserProfile
        {
            public string FullName;
            public string Email;
            public string Phone;
            public string Address;
            public string Role;
            public string AvatarPath;
        }

        private readonly UserDAL _userDAL;

        public UserBLL()
        {
            _userDAL = new UserDAL();

        }

        public UserBLL(UserDAL userDAL)
        {
            _userDAL = userDAL;
        }

        public List<User> GetAllUsers()
        {
            try
            {
                return _userDAL.GetAllUsers();
            }
            catch (Exception ex)
            {
                throw new Exception("Error getting users: " + ex.Message);
            }
        }
        public static UserProfile GetProfile(int userId)
        {
            try
            {
                using (var reader = UserDAL.GetUserProfile(userId))
                {
                    if (reader.Read())
                    {
                        return new UserProfile
                        {
                            FullName = reader.IsDBNull(0) ? "" : reader.GetString(0),
                            Email = reader.IsDBNull(1) ? "" : reader.GetString(1),
                            Phone = reader.IsDBNull(2) ? "" : reader.GetString(2),
                            Address = reader.IsDBNull(3) ? "" : reader.GetString(3),
                            Role = reader.IsDBNull(4) ? "" : reader.GetString(4),
                            AvatarPath = reader.IsDBNull(5) ? null : reader.GetString(5)
                        };
                    }
                    return null;
                }
            }
            catch (Exception ex)
            {
                Logger.LogError($"Lỗi khi lấy thông tin người dùng: {ex.Message}");
                throw new Exception("Lỗi khi lấy thông tin người dùng.", ex);
            }
        }

        public static void ChangePassword(int userId, string oldPassword, string newPassword)
        {
            try
            {
                string currentPassword = UserDAL.GetPasswordHash(userId);
                if (currentPassword != oldPassword)
                    throw new Exception("Mật khẩu cũ không chính xác.");
                if (string.IsNullOrWhiteSpace(newPassword))
                    throw new Exception("Mật khẩu mới không được để trống.");

                UserDAL.UpdatePassword(userId, newPassword);
            }
            catch (Exception ex)
            {
                Logger.LogError($"Lỗi khi đổi mật khẩu người dùng có ID:{userId} + {ex.Message}");
                throw new Exception("Lỗi khi đổi mật khẩu.", ex);
            }
        }

        public static string UpdateAvatar(int userId, string sourcePath)
        {
            try
            {
                string fileName = Path.GetFileName(sourcePath);
                string avatarDir = Path.Combine(Application.StartupPath, "Avatars");

                if (!Directory.Exists(avatarDir))
                    Directory.CreateDirectory(avatarDir);

                string destPath = Path.Combine(avatarDir, fileName);
                File.Copy(sourcePath, destPath, true);

                string relativePath = $"Avatars/{fileName}";
                UserDAL.UpdateAvatarPath(userId, relativePath);
                Logger.LogInfo($"Cập nhật ảnh đại diện cho người dùng ID {userId} thành công: {relativePath}");
                return destPath;
            }
            catch (Exception ex)
            {
                Logger.LogError($"Lỗi khi cập nhật ảnh đại diện cho người dùng ID {userId}: {ex.Message}");
                throw new Exception("Lỗi khi cập nhật ảnh đại diện.", ex);
            }
        }
        public bool Authenticate(string username, string password)
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
                throw new ArgumentException("Tên đăng nhập và mật khẩu không được để trống.");

            User user = _userDAL.GetUserByUsername(username);

            if (user == null)
                return false;
            return user.Password == password;
        }

        public bool RegisterUser(User user)
        {
            if (string.IsNullOrWhiteSpace(user.Username) || string.IsNullOrWhiteSpace(user.Password))
                throw new ArgumentException("Tên đăng nhập và mật khẩu là bắt buộc.");

            if (_userDAL.GetUserByUsername(user.Username) != null)
                throw new InvalidOperationException("Tên đăng nhập đã tồn tại.");
            Logger.LogInfo($"Đăng ký người dùng mới: {user.Username}");
            return _userDAL.AddUser(user);
        }

        public bool AddUser(User user)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(user.Username) ||
                    string.IsNullOrWhiteSpace(user.Password) ||
                    string.IsNullOrWhiteSpace(user.Role) ||
                    string.IsNullOrWhiteSpace(user.SoDienThoai) ||
                    string.IsNullOrWhiteSpace(user.QueQuan) ||
                    string.IsNullOrWhiteSpace(user.Email))
                {
                    throw new Exception("Các trường bắt buộc không được để trống.");
                }
                if (!System.Text.RegularExpressions.Regex.IsMatch(user.Username, @"^[a-zA-Z0-9]{5,20}$"))
                {
                    Logger.LogError("Tên đăng nhập không hợp lệ: chỉ được chứa chữ cái và số, độ dài từ 5 đến 20 ký tự.");
                    throw new Exception("Tên đăng nhập chỉ được chứa chữ cái và số, độ dài từ 5 đến 20 ký tự.");
                }
                if (!System.Text.RegularExpressions.Regex.IsMatch(user.Password, @"^(?=.*[A-Za-z])(?=.*\d).{6,}$"))
                {
                    Logger.LogError("Mật khẩu không hợp lệ: phải có ít nhất 6 ký tự, bao gồm cả chữ cái và số.");
                    throw new Exception("Mật khẩu phải có ít nhất 6 ký tự, bao gồm cả chữ cái và số.");
                }
                if (!System.Text.RegularExpressions.Regex.IsMatch(user.Email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
                {
                    Logger.LogError("Email không hợp lệ: phải có định dạng đúng.");
                    throw new Exception("Email không hợp lệ.");
                }
                if (!System.Text.RegularExpressions.Regex.IsMatch(user.SoDienThoai, @"^\d{10}$"))
                {
                    Logger.LogError("Số điện thoại không hợp lệ: phải chứa 10 chữ số.");
                    throw new Exception("Số điện thoại phải chứa 10 chữ số.");
                }
                var existingUser = _userDAL.GetUserByUsername(user.Username);
                if (existingUser != null)
                {
                    throw new Exception("Tên đăng nhập đã tồn tại.");
                }
                return _userDAL.AddUser(user);
            }
            catch (Exception ex)
            {
                Logger.LogError($"Lỗi khi thêm người dùng: {ex.Message}");
                throw new Exception("Lỗi khi thêm người dùng: " + ex.Message);
            }
        }

        public User GetUserByUsername(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
            {
                throw new ArgumentException("Username không được để trống.");
            }

            return _userDAL.GetUserByUsername(username);
        }


        public bool UpdateUser(User user)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(user.Username) ||
                    string.IsNullOrWhiteSpace(user.Role))
                {
                    throw new Exception("Required fields cannot be empty");
                }
                var existingUser = _userDAL.GetUserByUsername(user.Username);
                if (existingUser == null)
                {
                    throw new Exception("User not found");
                }
                Logger.LogInfo($"Updating user: {user.Username}");
                return _userDAL.UpdateUser(user);
            }
            catch (Exception ex)
            {
                Logger.LogError($"Error updating user: {ex.Message}");
                throw new Exception("Error updating user: " + ex.Message);
            }
        }

        public bool DeleteUser(int userId)
        {
            try
            {
                Logger.LogInfo($"Deleting user with ID: {userId}");
                return _userDAL.DeleteUser(userId);
            }
            catch (Exception ex)
            {
                Logger.LogError($"Error deleting user with ID {userId}: {ex.Message}");
                throw new Exception("Error deleting user: " + ex.Message);
            }
        }


        public bool ValidateLogin(string username, string password)
        {
            try
            {
                User user = _userDAL.GetUserByUsername(username);
                if (user == null || string.IsNullOrEmpty(user.PasswordHash))
                {
                    return false;
                }
                bool isPasswordValid = password == user.PasswordHash;
                return isPasswordValid;
            }
            catch (Exception ex)
            {
                Logger.LogError($"Lỗi đăng nhập: {ex.Message}");
                return false;
            }
        }

        public string GetUserRole(string username)
        {
            try
            {
                User user = _userDAL.GetUserByUsername(username);
                if (user == null)
                {
                    Logger.LogError($"Không tìm thấy người dùng với username: {username}");
                    return null;
                }
                return user.Role;
            }
            catch (Exception ex)
            {
                Logger.LogError($"Lỗi khi lấy vai trò người dùng: {ex.Message}");
                return null;
            }
        }
        public bool UpdateUserAvatar(string username, byte[] avatarImage)
        {
            if (string.IsNullOrWhiteSpace(username) || avatarImage == null || avatarImage.Length == 0)
                throw new ArgumentException("Dữ liệu không hợp lệ");

            UserDAL userDAL = new UserDAL();
            return userDAL.UpdateAvatar(username, avatarImage);
        }

        public bool ChangeUserPassword(string username, string oldPassword, string newPassword)
        {
            string currentPassword = _userDAL.GetPassword(username);

            if (currentPassword == oldPassword)
            {
                if (string.IsNullOrWhiteSpace(newPassword) || newPassword.Length < 6)
                {
                    throw new ArgumentException("Mật khẩu mới phải có ít nhất 6 ký tự.");
                }
                return _userDAL.ChangePassword(username, newPassword);
            }

            return false;
        }

        public DataTable GetAvailableCourses(int userId, string search)
        {
            return _userDAL.GetAvailableCourses(userId, search);
        }
        //report 1
        public List<StudentProgressDTO> GetStudentProgress(string username)
        {
            try
            {
                return _userDAL.GetProgressByUsername(username);
            }
            catch (Exception ex)
            {
                Logger.LogError($"Lỗi khi lấy tiến độ sinh viên ({username}): {ex.Message}");
                throw new ApplicationException("Không thể tải danh sách tiến độ sinh viên.", ex);
            }
        }

        public bool UpdateUserAvatarPath(string username, string imagePath)
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(imagePath))
                throw new ArgumentException("Dữ liệu không hợp lệ");

            UserDAL userDAL = new UserDAL();
            return userDAL.UpdateAvatarPath(username, imagePath);
        }

        public bool ChangeUserAvatar(string username, string selectedImagePath)
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(selectedImagePath))
                throw new ArgumentException("Username hoặc ảnh không hợp lệ");

            try
            {
                string avatarFolderPath = Path.Combine(Application.StartupPath, "Avatars", "Admin");
                if (!Directory.Exists(avatarFolderPath))
                {
                    Directory.CreateDirectory(avatarFolderPath);
                }

                string fileName = $"{username}{Path.GetExtension(selectedImagePath)}";
                string destinationPath = Path.Combine(avatarFolderPath, fileName);

                User user = _userDAL.GetUserByUsername(username);
                if (user == null)
                {
                    Logger.LogError($"Không tìm thấy người dùng với username: {username}");
                    return false;
                }

                string oldAvatarPath = user.AvatarPath;

                if (!string.IsNullOrWhiteSpace(oldAvatarPath) && File.Exists(oldAvatarPath))
                {
                    try
                    {
                        File.Delete(oldAvatarPath);
                    }
                    catch (Exception ex)
                    {
                        Logger.LogError($"Không thể xóa ảnh cũ: {ex.Message}");
                    }
                }
                File.Copy(selectedImagePath, destinationPath, true);

                bool updated = _userDAL.UpdateAvatarPath(username, destinationPath);
                if (!updated)
                {
                    Logger.LogError($"Không thể cập nhật đường dẫn avatar trong database cho user: {username}");
                    if (File.Exists(destinationPath))
                    {
                        try
                        {
                            File.Delete(destinationPath);
                        }
                        catch (Exception ex)
                        {
                            Logger.LogError($"Không thể xóa ảnh mới sau khi cập nhật database thất bại: {ex.Message}");
                        }
                    }
                    return false;
                }
                user.AvatarPath = destinationPath;
                Logger.LogInfo($"Cập nhật avatar thành công cho user: {username}");
                return true;
            }
            catch (Exception ex)
            {
                Logger.LogError($"Lỗi khi cập nhật avatar: {ex.Message}");
                throw new Exception($"Lỗi khi cập nhật avatar: {ex.Message}");
            }
        }

        public int GetUserId(string username)
        {
            try
            {
                User user = _userDAL.GetUserByUsername(username);
                if (user == null)
                {
                    Logger.LogError($"Không tìm thấy người dùng với username: {username}");
                    return -1;
                }
                return user.UserID;
            }
            catch (Exception ex)
            {
                Logger.LogError($"Lỗi khi lấy ID người dùng: {ex.Message}");
                return -1;
            }
        }

        public User GetAdminInfo()
        {
            if (_userDAL == null)
            {
                throw new InvalidOperationException("UserDAL chưa được khởi tạo.");
            }
            return _userDAL.GetAdminInfo();
        }
    }
}