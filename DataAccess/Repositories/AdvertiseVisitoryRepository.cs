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

        public async Task<List<Advertisement>> Search(AdvertismentSearchModel sm)
        {
            var query = db.Advertisement.AsQueryable();

            if (!string.IsNullOrEmpty(sm.Title))
            {
                query = query.Where(x => x.Title.Contains(sm.Title));
            }
            if (sm.IsDefault)
            {
                query = query.Where(x => x.IsDefault == sm.IsDefault);
            }

            return await query.ToListAsync();
        }

        public Task<OperationResult> Delete(int ID)
        {
            throw new NotImplementedException();
        }

        public async Task<OperationResult> Update(AdvertismentAddEditViewModel adv)
        {
            OperationResult op = new OperationResult();
            var existingAdv = await db.Advertisement.FirstOrDefaultAsync(x => x.AdvertisementID == adv.AdvertisementID);

            if (existingAdv == null)
            {
                return op.ToFailed("تبلیغ وجود ندارد");
            }
            try
            {
                using (var transaction = await db.Database.BeginTransactionAsync())
                {
                    existingAdv.Title = adv.Title ?? existingAdv.Title; 
                    existingAdv.ImageUrl = adv.ImageUrl ?? existingAdv.ImageUrl;
                    existingAdv.Url = adv.Url ?? existingAdv.Url;
                    existingAdv.Alt = adv.Alt ?? existingAdv.Alt;
                    existingAdv.IsDefault = adv.IsDefault;
                    db.Advertisement.Update(existingAdv);
                    await db.SaveChangesAsync();
                    await transaction.CommitAsync();
                }

                return op.ToSuccess("Advertisement updated successfully");
            }
            catch (Exception ex)
            {

                return op.ToFailed("Update failed: " + ex.Message);
            }
        }
        public async Task<AdvertismentAddEditViewModel> Get(int ID)
        {
            var adv = await db.Advertisement.FirstOrDefaultAsync(x => x.AdvertisementID == ID);
            if (adv == null)
            {
                return null;
            }
            return new AdvertismentAddEditViewModel
            {
                Title = adv.Title,
                ImageUrl = adv.ImageUrl,
                Url = adv.Url,
                Alt = adv.Alt,
                IsDefault = adv.IsDefault
            };
        }

    }
}
