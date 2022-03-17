using System;
using System.Linq;
using System.Web.Http.Filters;
using System.Web.Http.Controllers;
using Chubb.Tracker.TrackerReportingService.Data.Database;
using System.Net;
using System.Net.Http;
using System.IO;
using System.Web;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace Chubb.Tracker.TrackerReportingService
{
    /// <summary>
    /// Used to validate the UserID parameter before the api method call
    /// </summary>
    public class UserValidateAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            string content;
            string paramModelUserID = string.Empty;
            string paramUserID = string.Empty;
            bool isValidUser;
            using (var reader = new StreamReader(HttpContext.Current.Request.InputStream))
                content = reader.ReadToEnd();

            if (!string.IsNullOrWhiteSpace(content) && content.Length > 0)
            {
                var value = content.Trim();
                if ((value.StartsWith("{") && value.EndsWith("}")) || //For object
                    (value.StartsWith("[") && value.EndsWith("]"))) //For array
                {
                    try
                    {
                        JObject obj = JObject.Parse(content);
                        paramModelUserID = (string)obj["UserID"];
                    }
                    catch (JsonReaderException)
                    {
                        paramModelUserID = string.Empty;
                    }
                }
            }

            if (!string.IsNullOrEmpty(paramModelUserID))
            {
                using (var context = new TrackingReportingServiceEntitiesContainer())
                {

                    if (IsValidEmail(paramModelUserID))
                    {
                        isValidUser = context.Forecaster_logon.Where(x => x.Email.Equals(paramModelUserID) && x.Disabled.Equals("N")).Count() > 0 ? true : false;
                    }
                    else
                    {
                        isValidUser = context.Forecaster_logon.Where(x => x.LAN_ID.Equals(paramModelUserID) && x.Disabled.Equals("N")).Count() > 0 ? true : false;
                    }
                    if (!isValidUser)
                    {
                        actionContext.Response = actionContext.Request.CreateErrorResponse(HttpStatusCode.NotFound, String.Format("Invalid Tracker UserID: {0}", paramModelUserID));
                        //(HttpStatusCode.BadRequest, "Invalid UserID");
                    }

                }
            }

            else
            {
                if (actionContext.ActionArguments.Keys.Contains("UserID"))
                {
                    paramUserID = Convert.ToString(actionContext.ActionArguments["UserID"]);
                }
                if (!string.IsNullOrEmpty(paramUserID)) //&& actionContext.ControllerContext.ControllerDescriptor.ControllerName == "Dashboard"
                {
                    using (var context = new TrackingReportingServiceEntitiesContainer())
                    {
                        if (IsValidEmail(paramUserID))
                        {
                            isValidUser = context.Forecaster_logon.Where(x => x.Email.Equals(paramUserID) && x.Disabled.Equals("N")).Count() > 0 ? true : false;
                        }
                        else
                        {
                            isValidUser = context.Forecaster_logon.Where(x => x.LAN_ID.Equals(paramUserID) && x.Disabled.Equals("N")).Count() > 0 ? true : false;
                        }

                        if (!isValidUser)
                        {
                            actionContext.Response = actionContext.Request.CreateErrorResponse(HttpStatusCode.NotFound, String.Format("Invalid Tracker UserID: {0}", paramUserID));
                        }

                    }

                }
            }
        }

        public bool IsValidEmail(string email)
        {
            try
            {
                email = email.Trim();
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

    }

}
