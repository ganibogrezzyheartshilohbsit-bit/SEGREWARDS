using System.Collections.Generic;
using SEGREWARDS_PROJECT.Models;

namespace SEGREWARDS_PROJECT.Data.Repositories
{
    public interface IWasteTypeRepository
    {
        IReadOnlyList<WasteTypeRecord> GetAll();
        WasteTypeRecord GetByCode(string code);
    }
}
