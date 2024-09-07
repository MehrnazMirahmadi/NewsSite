using DataAccess.Services;
using DomainModel.Comon;
using DomainModel.Models;
using DomainModel.ViewModels.News;
using Microsoft.EntityFrameworkCore;


namespace DataAccess.Repositories
{
    public class NewsRepository:INewsRepository
    {
        private readonly NewsDBContext db;
        public NewsRepository(NewsDBContext db)
        {
            this.db = db;
        }
        public DomainModel.Models.News ToDBModel(NewsAddEditModel news)
        {
            return new News
            {
                NewsCategoryID = news.NewsCategoryID,
                NewsTitle = news.NewsTitle,
                ImageUrl = news.ImageUrl,
                NewsID = news.NewsID,
                NewsText = news.NewsText,
                RegistrationDate = DateTime.Now,
                Slug = news.Slug,
                SmallDescription = news.SmallDescription,
                SortOrder = news.SortOrder,
                VisitCount = news.VisitCount,
                VoteCount = news.VoteCount,
                VoteSumation = news.VoteSumation,


            };
        }
        public async Task<OperationResult> Add(NewsAddEditModel news)
        {
            var op = new OperationResult();
            if (await ExistNewsSlug(news.Slug))
            {
                return op.ToFailed("Slug Already Exists");
            }
            if (await ExistNewsTitle(news.NewsTitle))
            {
                return op.ToFailed("Title Already Exists");
            }
            try
            {
                var n = ToDBModel(news);
                db.News.Add(n);
                await db.SaveChangesAsync();
                return op.ToSuccess("Added");
            }
            catch (Exception ex)
            {

                return op.ToFailed("Add Error " + ex.Message);

            }


        }

        public async Task<OperationResult> Delete(int ID)
        {
            var op = new OperationResult();
            try
            {

                db.News.Remove(await db.News.FirstOrDefaultAsync(x => x.NewsID == ID));
                await db.SaveChangesAsync();
                return op.ToSuccess("Deleted Successfully");
            }
            catch (Exception ex)
            {
                return op.ToFailed("Failed Delete " + ex.Message);
                throw;
            }
        }

        public async Task<bool> ExistNewsSlug(string slug)
        {
            return await db.News.AnyAsync(x => x.Slug == slug);
        }

        public async Task<bool> ExistNewsTitle(string title)
        {
            return await db.News.AnyAsync(x => x.NewsTitle == title);

        }

        public async Task<NewsAddEditModel> Get(int NewsID)
        {
            var n = await db.News.FirstOrDefaultAsync(x => x.NewsID == NewsID);
            return new NewsAddEditModel
            {
                ImageUrl = n.ImageUrl
                ,
                NewsCategoryID = n.NewsCategoryID
                ,
                NewsID = n.NewsID
                ,
                NewsTitle = n.NewsTitle
                ,
                NewsText = n.NewsText
                ,
                RegistrationDate = n.RegistrationDate
                ,
                Slug = n.Slug
                ,
                SmallDescription = n.SmallDescription
                ,
                SortOrder = n.SortOrder
                ,
                VisitCount = n.VisitCount
                ,
                VoteCount = n.VoteCount
                ,
                VoteSumation = n.VoteSumation

            };
        }

        public async Task<List<NewsListItem>> GetAll()
        {
            return await db.News.Select(news => new NewsListItem
            {
                CategoryName = news.NewsCategory.CategoryName
                      ,
                ImageUrl = news.ImageUrl
                      ,
                NewsTitle = news.NewsTitle
                      ,
                NewsID = news.NewsID
                      ,
                Slug = news.Slug
                      ,
                SortOrder = news.SortOrder
            }).ToListAsync();
        }



        public async Task<OperationResult> Update(NewsAddEditModel NewNews)
        {
            OperationResult op = new OperationResult();
            var n = await db.News.FirstOrDefaultAsync(x => x.NewsTitle == NewNews.NewsTitle);
            if (n != null && n.NewsID != NewNews.NewsID)
            {
                return op.ToFailed("This title belongs to another news");
            }
            n = await db.News.FirstOrDefaultAsync(x => x.Slug == NewNews.Slug);
            if (n != null && n.NewsID != NewNews.NewsID)
            {
                return op.ToFailed("This slug belongs to another news");
            }
            n = await db.News.FirstOrDefaultAsync(x => x.NewsID == NewNews.NewsID);
            try
            {
                n.Slug = NewNews.Slug;
                n.SortOrder = NewNews.SortOrder;
                n.NewsTitle = NewNews.NewsTitle;
                n.NewsText = NewNews.NewsText;
                n.RegistrationDate = NewNews.RegistrationDate;
                n.SmallDescription = NewNews.SmallDescription;
                n.VoteSumation = NewNews.VoteSumation;
                n.NewsCategoryID = NewNews.NewsCategoryID;
                n.VisitCount = NewNews.VisitCount;
                n.VoteCount = NewNews.VoteCount;
                n.ImageUrl = NewNews.ImageUrl;
                await db.SaveChangesAsync();
                return op.ToSuccess("Updated");

            }
            catch (Exception ex)
            {
                return op.ToFailed("Updated failed" + ex.Message);

            }
        }

        public async Task<ListComplexModel> Search(NewsSearchModel sm)
        {
            ListComplexModel result = new ListComplexModel();
            var q = from news in db.News select news;
            if (!string.IsNullOrEmpty(sm.Slug))
            {
                q = q.Where(x => x.Slug.StartsWith(sm.Slug));
            }
            if (!string.IsNullOrEmpty(sm.NewsText))
            {
                q = q.Where(x => x.NewsText.StartsWith(sm.NewsText));
            }
            if (!string.IsNullOrEmpty(sm.NewsTitle))
            {
                q = q.Where(x => x.NewsTitle.StartsWith(sm.NewsTitle));
            }
            if (sm.NewsCategoryID != null && sm.NewsCategoryID > 0)
            {
                q = q.Where(x => x.NewsCategoryID == sm.NewsCategoryID);
            }
            result.RecordCount = await q.CountAsync();
            result.NewsList = await q.OrderByDescending(x => x.SortOrder)
                .Skip(sm.PageIndex * sm.PageSize)
                .Take(sm.PageSize)
                .Select(news => new NewsListItem
                {
                    CategoryName = news.NewsCategory.CategoryName
                    ,
                    ImageUrl = news.ImageUrl
                    ,
                    NewsTitle = news.NewsTitle
                    ,
                    NewsID = news.NewsID
                    ,
                    Slug = news.Slug
                    ,
                    SortOrder = news.SortOrder
                })
                .ToListAsync();
            return result;
        }
    }
}
