using System.Collections.Generic;
using WebApplication1.Models.Entities;
using WebApplication1.Models.ViewModels;
using WebApplication1.Repositories;

namespace WebApplication1.Services
{
    public interface IGoiDichVuService
    {
        List<ChonGoiViewModel> LayDanhSachGoi();
        ChonGoiViewModel? LayGoiTheoId(int id);
        bool GoiDaTonTai(string tenGoi, int soNgay);
        Task AddAsync(ChonGoiViewModel model);
        Task UpdateAsync(ChonGoiViewModel model);
        Task DeleteAsync(int id);
    }
    public class GoiDichVuService : IGoiDichVuService
    {
        private readonly IGoiDichVuRepository _repository;

        public GoiDichVuService(IGoiDichVuRepository repository)
        {
            _repository = repository;
        }

        public List<ChonGoiViewModel> LayDanhSachGoi()
        {
            var list = _repository.GetAll();
            return list.Select(g => new ChonGoiViewModel
            {
                Id = g.Id,
                TenGoi = g.TenGoi,
                MoTa = g.MoTa,
                Gia = g.Gia,
                SoNgayHieuLuc = g.SoNgayHieuLuc
            }).ToList();
        }

        public ChonGoiViewModel? LayGoiTheoId(int id)
        {
            var g = _repository.GetById(id);
            if (g == null) return null;
            return new ChonGoiViewModel
            {
                Id = g.Id,
                TenGoi = g.TenGoi,
                MoTa = g.MoTa,
                Gia = g.Gia,
                SoNgayHieuLuc = g.SoNgayHieuLuc
            };
        }
        public bool GoiDaTonTai(string tenGoi, int soNgay)
        {
            return _repository.GoiDaTonTai(tenGoi, soNgay);
        }
        public async Task AddAsync(ChonGoiViewModel model)
        {
            var goi = new GoiDichVu
            {
                TenGoi = model.TenGoi,
                MoTa = model.MoTa,
                Gia = model.Gia,
                SoNgayHieuLuc = model.SoNgayHieuLuc
            };
            await _repository.AddAsync(goi);
        }

        public async Task UpdateAsync(ChonGoiViewModel model)
        {
            var goi = new GoiDichVu
            {
                Id = model.Id,
                TenGoi = model.TenGoi,
                MoTa = model.MoTa,
                Gia = model.Gia,
                SoNgayHieuLuc = model.SoNgayHieuLuc
            };
            await _repository.UpdateAsync(goi);
        }

        public async Task DeleteAsync(int id)
        {
            await _repository.DeleteAsync(id);
        }
    }
}
