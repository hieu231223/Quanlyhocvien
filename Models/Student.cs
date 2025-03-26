using System.ComponentModel.DataAnnotations;
using MyWebApp.Models;

namespace MyWebApp.Models
{
    public class Student
    {
        public int Id { get; set; } //Mã sinh viên

        [Required(ErrorMessage = "Name tối thiểu 4 ký tự, tối đa 100 ký tự")]
        [StringLength(100, MinimumLength = 4)]
        public string Name { get; set; }//Họ tên

        [Required(ErrorMessage = "Email bắt buộc phải được nhập")]
        [RegularExpression(@"[A-Za-z)-9._%+-]+@gmail.com", ErrorMessage = "Nhập định dạng @gmail.com")]
        public string Email { get; set; }//Email

        // [StringLength(100, MinimumLength = 8)]
        [Required(ErrorMessage = "Mật khẩu từ 8 ký tự trở lên,có ký tự viết hoa, viết thường, chữ số và ký tự đặc biệt")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,}$", ErrorMessage = "có ít nhất 8 ký tự, có ký tự viết hoa, viết thường, chữ số, ký tự đặc biệt")]
        public string? Password { get; set; }//Mật khẩu
        [Required]
        public Branch? Branch { get; set; }//Ngành học
        [Required(ErrorMessage = "Thuộc tính bắt buộc")]
        public Gender? Gender { get; set; }//Giới tính
        public bool IsRegular { get; set; }//Hệ: true-chính quy, false-phi cq

        [Required(ErrorMessage = "Thuộc tính bắt buộc")]
        [DataType(DataType.MultilineText)]
        public string? Address { get; set; }//Địa chỉ
        [Range(typeof(DateTime), "1/1/1963", "12/31/2005")]
        [DataType(DataType.Date)]
        [Required(ErrorMessage = "Giá trị không hợp lệ. Hãy nhập lại")]
        public DateTime? DateOfBorth { get; set; }//Ngày sinh


        [Range(0.0, 10.0, ErrorMessage = "Giá trị không hợp lệ. Hãy nhập giá trị từ 0.0-10.0")]
        [Required(ErrorMessage = "Không được để trống")]
        public float? Score { get; set; }
    }
}

