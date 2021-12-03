using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
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
        private const string TokenKey = "token";

        public HomeController(
            IHttpContextAccessor httpContextAccessor,
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
            if (code == null)
            {
                ViewBag.Message = "Binding Failed";
                return View("Index");
            }

            var token = await _lineNotifyServices.GetTokenAsync(code).ConfigureAwait(false);
            _httpContextAccessor.HttpContext?.Session.SetString(TokenKey, token);
            ViewBag.Token = token;
            if (!string.IsNullOrWhiteSpace(ViewBag.Token))
            {
                ViewBag.Message = "Binding Success";
            }
            return View("Index");
        }

        public async Task<IActionResult> LineRevoke()
        {
            var token = _httpContextAccessor.HttpContext?.Session.GetString(TokenKey);
            if (!string.IsNullOrWhiteSpace(token))
            {
                await _lineNotifyServices.RevokeAsync(token).ConfigureAwait(false);
                _httpContextAccessor.HttpContext.Session.SetString(TokenKey, string.Empty);
                ViewBag.Message = "Unbind Success";
            }
            return View("Index");
        }

        public async Task<IActionResult> SentMessage(NewLineNotifyMessage lineNotifyMessage)
        {
            var token = _httpContextAccessor.HttpContext?.Session.GetString(TokenKey);
            if (!string.IsNullOrWhiteSpace(token))
            {
                if (lineNotifyMessage.ImageFormFile != null
                    && (lineNotifyMessage.ImageFormFile.ContentType.Equals("image/png")
                        || lineNotifyMessage.ImageFormFile.ContentType.Equals("image/jpeg")))
                {
                    lineNotifyMessage.ImageFile = lineNotifyMessage.ImageFormFile.OpenReadStream();
                }

                try
                {
                    await _lineNotifyServices.SentAsync(token, lineNotifyMessage);
                    ViewBag.Message = "Send Message Success";
                }
                catch (ValidationException e)
                {
                    ViewBag.Message = string.Join(',', e.Errors.Select(x => x.ErrorMessage)).ToString();
                }

            }
            else
            {
                ViewBag.Message = "You need to bind first";
            }

            return View("Index");
        }
    }

    public class NewLineNotifyMessage : LineNotifyMessage
    {
        public IFormFile ImageFormFile { get; set; }
    }
}
