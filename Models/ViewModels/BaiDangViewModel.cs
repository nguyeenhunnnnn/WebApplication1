﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
namespace WebApplication1.Models.ViewModels
{
    public class BaiDangViewModel
    {
        public int PK_iMaBaiDang { get; set; }
        public string sTieuDe { get; set; } = null!;

        [Display(Name = "Mô tả")]
        public string sMoTa { get; set; }

        public string sDiaDiem { get; set; }
        public decimal fMucLuong { get; set; }
        public string sTrangThai { get; set; } = null!;

        public DateTime dNgayTao { get; set; }
        public DateTime dThoiGianHetHan { get; set; } // Tùy chọn

        public string Nguoitao { get; set; } 
        //cột mới 
        public string sMonday { get; set; }
        public string sYCau { get; set; }
        public string sGioiTinh { get; set; }
        public string sTuoi { get; set; }
        public string sKinhNghiem { get; set; }
        public string sBangCap { get; set; }
    }
}
