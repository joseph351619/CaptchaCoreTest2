using CaptchaCoreTest2.BLL;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Drawing.Imaging;
using System.IO;

namespace MyWebsite.Controllers
{
    [Route("api/[controller]")]
    public class AuthenticationController : Controller
    {
        private const string _captchaHashKey = "CaptchaHash";
        private const string _usernameHashKey = "Username";

        // 我習慣用強型別包裝 Session
        private string CaptchaHash
        {
            get
            {
                return HttpContext.Session.GetString(_captchaHashKey) as string;
            }
            set
            {
                HttpContext.Session.SetString(_captchaHashKey, value);
            }
        }

        // 我習慣用強型別包裝 Session
        private string Username
        {
            get
            {
                return HttpContext.Session.GetString(_usernameHashKey) as string;
            }
            set
            {
                HttpContext.Session.SetString(_usernameHashKey, value);
            }
        }

        private CaptchaBLL captchaBLL = new CaptchaBLL();

        [HttpGet]
        public ResultModel Get()
        {
            var result = new ResultModel();
            if (!string.IsNullOrEmpty(Username))
            {
                result.Data = Username;
            }
            result.IsSuccess = true;
            return result;
        }

        [HttpPost]
        public ResultModel Post([FromBody]dynamic body)
        {
            var result = new ResultModel();
            try
            {
                string username = body.username.Value;
                string password = body.password.Value;
                string code = body.code.Value;

                if (!captchaBLL.ComputeMd5Hash(code).Equals(CaptchaHash))
                {
                    result.Message = "驗證碼輸入錯誤。";
                }
                else if (!username.Equals("john") || !password.Equals("1234"))
                {
                    result.Message = "帳號或密碼錯誤。";
                }
                else
                {
                    Username = username;
                    HttpContext.Session.Remove(_captchaHashKey);
                    result.IsSuccess = true;
                }
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
            }
            return result;
        }

        [HttpDelete]
        public ResultModel Delete()
        {
            var result = new ResultModel();
            HttpContext.Session.Remove(_usernameHashKey);
            result.IsSuccess = true;
            return result;
        }

        [Route("captcha1")]
        [HttpGet]
        public ActionResult GetCaptcha()
        {
            // 隨機產生四個字元
            var randomText = captchaBLL.GenerateRandomText(4);
            // 加密後存在 Session，也可以不用加密，比對時一致就好。
            CaptchaHash = randomText;
            //CaptchaHash = captchaBLL.ComputeMd5Hash(randomText);
            // 回傳 gif 圖檔
            return File(captchaBLL.GenerateCaptchaImage(randomText), "image/gif");
        }



        [Route("captcha2")]
        [HttpGet]
        /// <summary>
        /// 数字验证码
        /// </summary>
        /// <returns></returns>
        public FileContentResult NumberVerifyCode()
        {
            //string code = VerifyCodeHelper.GetSingleObj().CreateVerifyCode(VerifyCodeHelper.VerifyCodeType.NumberVerifyCode);
            string code = CaptchaHash;
            byte[] codeImage = VerifyCodeHelper.GetSingleObj().CreateByteByImgVerifyCode(code, 100, 40);
            return File(codeImage, @"image/jpeg");
        }

        /// <summary>
        /// 字母验证码
        /// </summary>
        /// <returns></returns>
        public FileContentResult AbcVerifyCode()
        {
            //string code = VerifyCodeHelper.GetSingleObj().CreateVerifyCode(VerifyCodeHelper.VerifyCodeType.AbcVerifyCode);
            string code = CaptchaHash;
            var bitmap = VerifyCodeHelper.GetSingleObj().CreateBitmapByImgVerifyCode(code, 100, 40);
            MemoryStream stream = new MemoryStream();
            bitmap.Save(stream, ImageFormat.Png);
            return File(stream.ToArray(), "image/png");
        }

        /// <summary>
        /// 混合验证码
        /// </summary>
        /// <returns></returns>
        public FileContentResult MixVerifyCode()
        {
            //string code = VerifyCodeHelper.GetSingleObj().CreateVerifyCode(VerifyCodeHelper.VerifyCodeType.MixVerifyCode);
            string code = CaptchaHash;
            var bitmap = VerifyCodeHelper.GetSingleObj().CreateBitmapByImgVerifyCode(code, 100, 40);
            MemoryStream stream = new MemoryStream();
            bitmap.Save(stream, ImageFormat.Gif);
            return File(stream.ToArray(), "image/gif");
        }
        
    }
}