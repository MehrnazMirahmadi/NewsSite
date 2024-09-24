using DomainModel.Comon;
using DomainModel.ViewModels;


namespace DataAccess.Services
{
    public interface INewsCategoryRepository
    {
        Task<OperationResult> Delete(int ID);
        Task<OperationResult> Add(NewsCategoryAddEditModel cat);
        Task<OperationResult> Update(NewsCategoryAddEditModel cat);
        Task<NewsCategoryAddEditModel> Get(int ID);
        Task<List<NewsCategoryListItem>> GetAll();
        Task<List<NewsCategoryListItem>> GetRoots();
        Task<List<NewsCategoryListItem>> GetSubCategory(int ParentID);
        Task<bool> HasChid(int ID);
        Task<bool> HasNews(int ID);
    }
}
