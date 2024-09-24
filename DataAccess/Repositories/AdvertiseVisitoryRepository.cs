using DataAccess.Services;
using DomainModel.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repositories
{
    public class AdvertiseVisitoryRepository : IAdvertiseVisitoryRepository
    {
        private readonly NewsDBContext db;
        public AdvertiseVisitoryRepository(NewsDBContext db)
        {
            this.db = db;
        }

        public async Task<Advertisement> GetActiveAdvertise()
        {
            var adv = await db.Advertisement.OrderByDescending(x => x.AdvertisementID).FirstOrDefaultAsync(x => x.IsDefault == true);
            return adv;
        }
    }
}
