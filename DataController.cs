using Microsoft.AspNetCore.Mvc;
using TimeLogs.Implementations;
using TimeLogs.Interfaces;
using TimeLogs.ViewModels;

namespace TimeLogs
{
    [Controller]
    public class DataController : Controller
    {
        private readonly IUserService _userService;
        public DataController(IUserService userService) 
        { 
            _userService = userService;
        }

        [HttpPost]
        public IActionResult CreateDb()
        {
            try
            {
                _userService.CreateDatabase();

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);  
            }
        }

        [HttpPost]
        public IActionResult LoadUserData([FromBody] DataTablesRequest request)
        {
            int totalFiltered = 0;
            var data = _userService.GetGridUsers(request, ref totalFiltered).ToArray();

            var response = new
            {
                data = data,
                recordsFiltered = totalFiltered
            };

            return Json(response);
        }

        [HttpGet]
        public IActionResult GetTopUsers([FromQuery] DateTime? dateFrom, [FromQuery] DateTime? dateTo)
        {
            var data = _userService.GetTopUsersByTime(dateFrom, dateTo);
            var result = data.Select(x => new
            {
                userName = string.Join(' ', x.Name, x.Surname),
                totalTime = x.TimeLogged
            }).ToArray();

            return Json(result);
        }

        [HttpGet]
        public IActionResult GetProgectsByTime([FromQuery] DateTime? dateFrom, [FromQuery] DateTime? dateTo)
        {
            var data = _userService.GetProgectsByTime(dateFrom, dateTo);

            return Json(data);
        }

        [HttpGet]
        public IActionResult GetCompareUser([FromQuery] DateTime? dateFrom, [FromQuery] DateTime? dateTo, [FromQuery] int userId)
        {
            var user = _userService.GetCompareUserByTime(dateFrom, dateTo, userId);

            return Json(new
            {
                userName = string.Join(' ', user.Name, user.Surname),
                totalTime = user.TimeLogged
            });
        }
    }
}
