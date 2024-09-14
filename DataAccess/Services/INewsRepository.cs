using DomainModel.Comon;
using DomainModel.ViewModels.News;


namespace DataAccess.Services
{
    public interface INewsRepository
    {
        Task<OperationResult> Delete(int ID);

        Task<OperationResult> Add(NewsAddEditModel news);
        Task<OperationResult> Update(NewsAddEditModel NewNews);
        Task<NewsAddEditModel> Get(int ID);
        Task<List<NewsListItem>> GetAll();
        Task<ListComplexModel> Search(NewsSearchModel sm);
        Task<bool> ExistNewsTitle(string title);
        Task<bool> ExistNewsSlug(string slug);
        Task RemoveImage(int NewsID);
    }
}
