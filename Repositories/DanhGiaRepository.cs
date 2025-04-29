using WebApplication1.Data;
using WebApplication1.Models.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace WebApplication1.Repositories
{
    public interface IDanhGiaRepository
    {
        Task<List<DanhGiaGiaSu>> GetByGiaSuIdAsync(string giaSuId);
        Task<double> GetAverageRatingAsync(string giaSuId);
        Task AddAsync(DanhGiaGiaSu danhGia);
    }
    public class DanhGiaRepository : IDanhGiaRepository
    {
        private readonly ApplicationDbContext _context;
        public DanhGiaRepository(ApplicationDbContext context) => _context = context;

        public async Task<List<DanhGiaGiaSu>> GetByGiaSuIdAsync(string giaSuId)
        {
            return await _context.DanhGiaGiaSus
                .Where(d => d.GiaSuId == giaSuId)
                .Include(d => d.NguoiDanhGia)
                .OrderByDescending(d => d.NgayTao)
                .ToListAsync();
        }

        public async Task<double> GetAverageRatingAsync(string giaSuId)
        {
            var danhGias = await _context.DanhGiaGiaSus
                .Where(d => d.GiaSuId == giaSuId)
                .ToListAsync();

            return danhGias.Any() ? danhGias.Average(d => d.SoSao) : 0;
        }

        public async Task AddAsync(DanhGiaGiaSu danhGia)
        {
            _context.DanhGiaGiaSus.Add(danhGia);
            await _context.SaveChangesAsync();
        }
    }

}
