using DomainModel.Comon;
using DomainModel.Models;
using DomainModel.ViewModels.Advertisment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Services
{
    public interface IAdvertiseVisitoryRepository
    {
        Task<Advertisement> GetActiveAdvertise();
        Task<List<Advertisement>> GetAll();
        Task<OperationResult> Add(AdvertismentAddEditViewModel advertisement);

    }
}
