using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LMS_Project.LMS
{
    public class ApiMessage
    {
        public const string GETALL_SUCCESS = "Lấy danh sách dữ liệu thành công";
        public const string GETBYId_SUCCESS = "Lấy dữ liệu thành công";

        public const string CREATE_SUCCESS = "Thêm dữ liệu thành công";
        public const string UPDATE_SUCCESS = "Cập nhật dữ liệu thành công";
        public const string DELETE_SUCCESS = "Xóa dữ liệu thành công";

        public const string CREATE_FAILED = "Thêm dữ liệu thất bại";
        public const string UPDATE_FAILED = "Cập nhật dữ liệu thất bại";
        public const string DELETE_FAILED = "Xóa dữ liệu thất bại";

        public const string IMPORT_WAREHOUSE_SUCCESS = "Nhập kho thành công";
        public const string IMPORT_WAREHOUSE_FAILED = "Nhập kho thất bại";
        public const string EXPORT_WAREHOUSE_SUCCESS = "Xuất kho thành công";
        public const string EXPORT_WAREHOUSE_FAILED = "Xuất kho thất bại";

        public const string SENDMAIL_SUCCESS = "Gửi Email thành công!";
        public const string SENDMAIL_FAILED = "Gửi Email thất bại";

        public const string UNAUTHORIZED = "Không có quyền truy cập tài nguyên này";
        public const string EXPIRE_TOKEN = "Phiên đăng nhập đã hết hiệu lực";
        //public const string AUTHENTICATION_ERROR = "Lỗi xác thực";
        public const string EXIST_CODE = "Mã này đã tồn tại";
        public const string NOT_FOUND = "Không tìm thấy dữ liệu";
        public const string NO_DATA = "Không có dữ liệu";

        public const string LOGIN_SUCCESS = "Đăng nhập thành công";
        public const string LOGIN_FAILED = "Tài khoản hoặc mật khẩu không đúng";
        public const string LOGOUT_SUCCESS = "Đăng xuất thành công";
        public const string SAVE_SUCCESS = "Lưu thành công";
        public const string INVALID_FILE = "File không đúng định dạng";

        //Payment status
        public const string PAYMENT_PAId = "Đã thanh toán";
        public const string PAYMENT_UNPAId = "Chưa thanh toán";
        public const string PAYMENT_SUCCESS = "Thanh toán thành công";
        public const string PAYMENT_FAILED = "Thanh toán thất bại";
        public const string SUCCESS = "Thành công";
    }
}