using DataAccess.Services;
using DomainModel.Comon;
using DomainModel.Models;
using DomainModel.ViewModels;
using DomainModel.ViewModels.News;
using Microsoft.EntityFrameworkCore;


namespace DataAccess.Repositories
{
    public class NewsCategoryRepository : INewsCategoryRepository
    {
        #region Db
        private readonly NewsDBContext db;
        public NewsCategoryRepository(NewsDBContext db)
        {
            this.db = db;
        }
        #endregion

        NewsCategory ToDBModel(NewsCategoryAddEditModel cat)
        {
            return new NewsCategory
            {
                CategoryName = cat.CategoryName,
                ParentID = cat.ParentID,
                Slug = cat.Slug,
                SmallDescription = cat.SmallDescription,
                NewsCategoryID = cat.NewsCategoryID

            };
        }
        NewsCategoryAddEditModel ToViewModel(NewsCategory cat)
        {
            return new NewsCategoryAddEditModel
            {
                CategoryName = cat.CategoryName,
                ParentID = cat.ParentID,
                Slug = cat.Slug,
                SmallDescription = cat.SmallDescription,
                NewsCategoryID = cat.NewsCategoryID,

            };
        }
     
        public async Task<OperationResult> Add(NewsCategoryAddEditModel cat)
        {
            var op = new OperationResult();
            var c = ToDBModel(cat);
            try
            {
                db.NewsCategories.Add(c);
                await db.SaveChangesAsync();
                return op.ToSuccess("Category Added Successfully");
            }
            catch (Exception ex)
            {

                return op.ToFailed("Added Category Failed " + ex.Message);
            }

        }

        public async Task<OperationResult> Delete(int ID)
        {
            OperationResult op = new OperationResult();
            var cat = await db.NewsCategories.FirstOrDefaultAsync(x => x.NewsCategoryID == ID);
            if (cat == null)
            {
                return op.ToFailed("Record Not Found");
            }
            if (await HasNews(ID))
            {
                return op.ToFailed("Has News");
            }
            if (await HasChid(ID))
            {
                return op.ToFailed("Has Sub Category");
            }
            try
            {
                db.NewsCategories.Remove(cat);
                await db.SaveChangesAsync();
                return op.ToSuccess("Deleted Successfully");
            }
            catch (Exception ex)
            {
                return op.ToFailed("Deleted Failed " + ex.Message);

            }


        }

        public async Task<OperationResult> Update(NewsCategoryAddEditModel cat)
        {
            var op = new OperationResult();
            var c = ToDBModel(cat);

            try
            {
                db.Attach(c);
                db.Entry<NewsCategory>(c).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return op.ToSuccess("Saved");
            }
            catch (Exception ex)
            {

                return op.ToFailed("Failed " + ex.Message);
            }

        }

        public async Task<NewsCategoryAddEditModel> Get(int ID)
        {
            var c = await db.NewsCategories.FirstOrDefaultAsync(x => x.NewsCategoryID == ID);
            return ToViewModel(c);
        }

        public async Task<List<NewsCategoryListItem>> GetAll()
        {
            var cats =
                await db.NewsCategories.Select(x => new NewsCategoryListItem
                {
                    CategoryName = x.CategoryName
                ,
                    NewsCategoryID = x.NewsCategoryID
                ,
                    NewsCount = x.News.Count
                ,
                    Slug = x.Slug
                }).ToListAsync();
            return cats;
        }

        public async Task<bool> HasChid(int ID)
        {
            return await db.NewsCategories.AnyAsync(x => x.ParentID == ID);
        }

        public async Task<bool> HasNews(int ID)
        {
            return await db.News.AnyAsync(x => x.NewsCategoryID == ID);
        }

        public async Task<List<NewsCategoryListItem>> GetRoots()
        {
            var cats =
                await db.NewsCategories.Where(x => x.ParentID == null).Select(x => new NewsCategoryListItem
                {
                    CategoryName = x.CategoryName
                ,
                    NewsCategoryID = x.NewsCategoryID
                ,
                    NewsCount = x.News.Count
                ,
                    Slug = x.Slug
                }).ToListAsync();
            return cats;
        }

        public async Task<List<NewsCategoryListItem>> GetSubCategory(int ParentID)
        {
            var cats =
                 await db.NewsCategories.Where(x => x.ParentID == ParentID).Select(x => new NewsCategoryListItem
                 {
                     CategoryName = x.CategoryName
                 ,
                     NewsCategoryID = x.NewsCategoryID
                 ,
                     NewsCount = x.News.Count
                 ,
                     Slug = x.Slug
                 }).ToListAsync();
            return cats;
        }
    }
}
