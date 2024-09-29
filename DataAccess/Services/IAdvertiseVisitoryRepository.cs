using DomainModel.Comon;
using DomainModel.Models;
using DomainModel.ViewModels.Advertisment;


namespace DataAccess.Services
{
    public interface IAdvertiseVisitoryRepository
    {
        Task<Advertisement> GetActiveAdvertise();
        Task<List<Advertisement>> GetAll();
        Task<OperationResult> Add(AdvertismentAddEditViewModel advertisement);
        Task<List<Advertisement>> Search(AdvertismentSearchModel sm);
        Task<OperationResult> Delete(int ID);
        Task<OperationResult> Update(AdvertismentAddEditViewModel adv);
        Task<AdvertismentAddEditViewModel> Get(int ID);
    }
}
