using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using WebApplication1.Models.Entities;

namespace WebApplication1.Models.ViewModels
{
    public class ChonGoiViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Tên gói không được để trống")]
        public string TenGoi { get; set; }
        [Required]
        public string MoTa { get; set; }

        [Required]
        [Range(0, double.MaxValue, ErrorMessage = "Giá phải lớn hơn hoặc bằng 0")]
        public decimal Gia { get; set; }

        [Required]
        [Range(0, 30, ErrorMessage = "Số ngày hiệu lực phải từ 0 đến 30")]
        public int SoNgayHieuLuc { get; set; }
    }
}
