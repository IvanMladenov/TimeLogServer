using Microsoft.AspNetCore.Mvc;
using TimeLogs.Implementations;
using TimeLogs.ViewModels;

namespace TimeLogs.Interfaces
{
    public interface IUserService
    {
        List<User> GetGridUsers(DataTablesRequest filters, ref int totalFiltered);
        List<User> GetTopUsersByTime(DateTime? dateFrom, DateTime? dateTo);
        List<Project> GetProgectsByTime(DateTime? dateFrom, DateTime? dateTo);
        User GetCompareUserByTime(DateTime? dateFrom, DateTime? dateTo, int id);
        void CreateDatabase();
    }
}
