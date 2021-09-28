using System.Diagnostics;
using System.Threading.Tasks;
using LineNotifySample.Models;
using LineNotifySDK;
using LineNotifySDK.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LineNotifySample.Controllers
{
    public class HomeController : Controller
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILineNotifyServices _lineNotifyServices;

        public HomeController(IHttpContextAccessor httpContextAccessor,
            ILineNotifyServices lineNotifyServices)
        {
            _httpContextAccessor = httpContextAccessor;
            _lineNotifyServices = lineNotifyServices;
        }

        public IActionResult Index()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult LineAuthorize()
        {
            return Redirect(_lineNotifyServices.GetAuthorizeUri().AbsoluteUri);
        }

        public async Task<IActionResult> BindCallback(string code, string state)
        {
            var token = await _lineNotifyServices.GetTokenAsync(code).ConfigureAwait(false);
            _httpContextAccessor.HttpContext.Session.SetString("token", token);
            ViewBag.Token = token;
            if (!string.IsNullOrWhiteSpace(ViewBag.Token))
            {
                ViewBag.Message = "綁定成功";
            }
            return View("Index");
        }

        public async Task<IActionResult> LineRevoke()
        {
            await _lineNotifyServices.RevokeAsync(_httpContextAccessor.HttpContext.Session.GetString("token")).ConfigureAwait(false);
            ViewBag.Message = "解除綁定成功";
            return View("Index");
        }

        public async Task<IActionResult> SentMessage(LineNotifyMessage lineNotifyMessage)
        {
            await _lineNotifyServices.SentAsync(_httpContextAccessor.HttpContext.Session.GetString("token"), lineNotifyMessage);
            ViewBag.Message = "傳送成功";
            return View("Index");
        }
    }
}
