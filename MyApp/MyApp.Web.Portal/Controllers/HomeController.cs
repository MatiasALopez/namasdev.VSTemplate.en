using System.Web.Mvc;

using AutoMapper;

namespace MyApp.Web.Portal.Controllers
{
    [Authorize]
    public class HomeController : ControllerBase
    {
        public const string NAME = "Home";

        public HomeController(IMapper mapper)
            : base(mapper)
        {
        }

        #region Actions

        public ActionResult Index()
        {
            return View();
        }

        #endregion

        #region Methods

        #endregion
    }
}