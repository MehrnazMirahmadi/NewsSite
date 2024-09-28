using DataAccess.Services;
using DomainModel.Comon;
using DomainModel.Models;
using DomainModel.ViewModels.Advertisment;
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
        public Advertisement ToDBModel(AdvertismentAddEditViewModel advertisment)
        {
            return new Advertisement
            {
                ImageUrl=advertisment.ImageUrl,
                Alt = advertisment.Alt,
                Title = advertisment.Title,
                Url = advertisment.Url,
                IsDefault = advertisment.IsDefault


            };
        }
        public async Task<OperationResult> Add(AdvertismentAddEditViewModel advertisement)
        {
            var op = new OperationResult();
            if (advertisement == null)
            {
                return op.ToFailed("مدل تبلیغاتی معتبر نیست."); 
            }
            try
            {
                var advertisementEntity = ToDBModel(advertisement); 
                db.Advertisement.Add(advertisementEntity); 
                await db.SaveChangesAsync();
                return op.ToSuccess("عملیات با موفقیت انجام شد"); 
            }
            catch (DbUpdateException dbEx)
            {
                return op.ToFailed("خطا در پایگاه داده: " + dbEx.Message); 
            }
            catch (Exception ex)
            {
                return op.ToFailed("خطا در ایجاد رخ داد: " + ex.Message); 
            }
        }



        public async Task<Advertisement> GetActiveAdvertise()
        {
            var adv = await db.Advertisement.OrderByDescending(x => x.AdvertisementID).FirstOrDefaultAsync(x => x.IsDefault == true);
            return adv;
        }

        public async Task<List<Advertisement>> GetAll()
        {
            var adv = await db.Advertisement.OrderByDescending(x => x.AdvertisementID).ToListAsync();
            return adv;
        }
    }
}
