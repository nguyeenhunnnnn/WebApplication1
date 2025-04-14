using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace WebApplication1.Models.Entities
{
    public class TaiKhoan : IdentityUser
    {

        [StringLength(500)]
        public string? DiaChi { get; set; }

        public string? FileAvata { get; set; }
        public string? FileCCCD { get; set; }

        [Required]
        [StringLength(30)]
        public string VaiTro { get; set; } 

        [Required]
        [StringLength(30)]
        public string TrangThai { get; set; }

        [Required]
        public string CCCD { get; set; }
        // Quan hệ một-nhiều với BaiDang
        public List<BaiDang>? BaiDangs { get; set; } = new List<BaiDang>();

        // Quan hệ một-nhiều với HoSo
        public List<HoSo>? HoSos { get; set; } = new List<HoSo>();


    }
}
