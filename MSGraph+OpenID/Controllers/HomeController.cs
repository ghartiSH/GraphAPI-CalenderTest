using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Graph;
using MSGraph_OpenID.Models;
using System.Collections;
using System.Diagnostics;

namespace MSGraph_OpenID.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly GraphServiceClient graphServiceClient;

        public HomeController(ILogger<HomeController> logger, GraphServiceClient graph)
        {
            _logger = logger;
            graphServiceClient = graph;
        }

        public async Task<IActionResult> Index()
        {
            string photoData = "";

            var user = await graphServiceClient
                .Me
                .Request()
                .Select(x => new
                {
                    x.DisplayName
                })
                .GetAsync();

            
            using (var photoStream = await graphServiceClient.Me.Photo.Content.Request().GetAsync())
            {
                byte[] photo = ((MemoryStream)photoStream).ToArray();
                photoData = Convert.ToBase64String(photo);
            }

            var queryOptions = new List<QueryOption>()
            {
                new QueryOption("startdatetime", "2022-12-13T03:54:55.028Z"),
                new QueryOption("enddatetime", "2022-12-20T03:54:55.028Z")
            };

            var calendarView = await graphServiceClient.Me.CalendarView
                .Request(queryOptions)
                .GetAsync();

            List<Models.Calendar> calenderList = new();

            foreach(var item in calendarView)
            {
                Models.Calendar calenderObj = new();
                calenderObj.Subject = item.Subject;

                DateTime date = DateTime.Parse(item.Start.DateTime.ToString());

                calenderObj.Date = date.ToShortDateString().ToString();

                calenderList.Add(calenderObj);
            }

            ViewData["UserName"] = user.DisplayName;
            ViewData["ProfileImage"] = photoData;

            return View(calenderList);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [AllowAnonymous]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}