using DataAccess.Services;
using DomainModel.Models;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Repositories
{
    public class MenuVisitoryRepository : IMenuVisitoryRepository
    {
        private readonly DomainModel.Models.NewsDBContext db;
        public MenuVisitoryRepository(DomainModel.Models.NewsDBContext db)
        {
            this.db = db;
        }

        public async Task<List<NewsCategory>> GetAll()
        {
            List<NewsCategory> rsults = await db.NewsCategories.Where(x => x.ParentID == null).Include(x => x.Children).ToListAsync();
            return rsults;
        }
    }
}
