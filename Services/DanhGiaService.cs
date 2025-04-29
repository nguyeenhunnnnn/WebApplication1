using WebApplication1.Models.Entities;
using WebApplication1.Repositories;

namespace WebApplication1.Services
{
    public interface IDanhGiaService
    {
        Task<List<DanhGiaGiaSu>> GetDanhGiaByGiaSuIdAsync(string giaSuId);
        Task<double> GetAverageRatingAsync(string giaSuId);
        Task AddDanhGiaAsync(string nguoiDanhGiaId, string giaSuId, int soSao, string? noiDung);
    }
    public class DanhGiaService : IDanhGiaService
    {
        private readonly IDanhGiaRepository _repository;

        public DanhGiaService(IDanhGiaRepository repository) => _repository = repository;

        public Task<List<DanhGiaGiaSu>> GetDanhGiaByGiaSuIdAsync(string giaSuId)
            => _repository.GetByGiaSuIdAsync(giaSuId);

        public Task<double> GetAverageRatingAsync(string giaSuId)
            => _repository.GetAverageRatingAsync(giaSuId);

        public async Task AddDanhGiaAsync(string nguoiDanhGiaId, string giaSuId, int soSao, string? noiDung)
        {
            var danhGia = new DanhGiaGiaSu
            {
                NguoiDanhGiaId = nguoiDanhGiaId,
                GiaSuId = giaSuId,
                SoSao = soSao,
                NoiDung = noiDung,
                NgayTao = DateTime.Now
            };
            await _repository.AddAsync(danhGia);
        }
    }

}
