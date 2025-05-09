using WebApplication1.Data;
using WebApplication1.Models.Entities;

namespace WebApplication1.Repositories
{
    public interface IGoiDichVuRepository
    {
        List<GoiDichVu> GetAll();
        GoiDichVu GetById(int id);
        bool GoiDaTonTai(string tenGoi, int soNgay);
        Task AddAsync(GoiDichVu goi);
        Task UpdateAsync(GoiDichVu goi);
        Task DeleteAsync(int id);
    }
    public class GoiDichVuRepository : IGoiDichVuRepository
    {
        private readonly ApplicationDbContext _context;

        public GoiDichVuRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<GoiDichVu> GetAll()
        {
            return _context.GoiDichVus
                .OrderByDescending(g => g.Id)
                .ToList();
        }

        public GoiDichVu GetById(int id)
        {
            return _context.GoiDichVus.FirstOrDefault(g => g.Id == id);
        }
        public bool GoiDaTonTai(string tenGoi, int soNgay)
        {
            return _context.GoiDichVus.Any(g => g.TenGoi == tenGoi && g.SoNgayHieuLuc == soNgay);
        }
        public async Task AddAsync(GoiDichVu goi)
        {
            _context.GoiDichVus.Add(goi);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(GoiDichVu goi)
        {
            _context.GoiDichVus.Update(goi);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var goi = await _context.GoiDichVus.FindAsync(id);
            if (goi != null)
            {
                _context.GoiDichVus.Remove(goi);
                await _context.SaveChangesAsync();
            }
        }
    }
}
