using AmberProject.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;

namespace AmberProject.Controllers
{
  public class HomeController : Controller
  {
    // Connection URL authentication
    private static readonly string clientid = WebConfigurationManager.AppSettings["clientidD365"];
    private static readonly string clientsecret = WebConfigurationManager.AppSettings["seckeyD365"];
    private static readonly string tenant_id = WebConfigurationManager.AppSettings["TenetIdD365"];
    private static readonly string resource = WebConfigurationManager.AppSettings["ResourceD365"];
    private static readonly string D365DataUrl = WebConfigurationManager.AppSettings["D365DataUrl"];

    private static DataTable result; // Declare the result variable as a static member
    private static DataTable companyData;
    private static DataTable acxUserSetupData;

    AmberProject.App_Code.Encryption EncPass = new AmberProject.App_Code.Encryption();

    private async Task FetchEntityData()
    {
      companyData = await GetD365EntityOutput("Companies").ConfigureAwait(false);
      Session["acxUserSetupData"] = await GetD365EntityOutput("AcxUserSetups", "", crossCompany: true).ConfigureAwait(false);
    }


    private IEnumerable<SelectListItem> InitializeCompanyList()
    {
      List<SelectListItem> companyList = companyData.AsEnumerable()
          .Select(row => new SelectListItem { Text = row.Field<string>("Name"), Value = row.Field<string>("DataArea") })
          .ToList();

      return companyList;
    }

    [HttpGet]
    public async Task<ActionResult> Index()
    {
      await FetchEntityData().ConfigureAwait(false);
      var acxUserSetupData = Session["acxUserSetupData"] as DataTable;
      var companyList = InitializeCompanyList();
      var model = new LoginViewModel { CompanyList = companyList/*, Result = result*/ };
      return View(model);
    }

    [HttpPost]
    public async Task<ActionResult> Index(LoginViewModel model)
    {
      if (!ModelState.IsValid)
      {
        model.CompanyList = InitializeCompanyList();
        return View(model);
      }

      

      var pass = EncPass.EncryptPassword(model.Password);
      bool isValidLogin = ValidateLogin(model.UserID, pass, model.CompanyId);
      if (isValidLogin)
      {
        string companyId = model.CompanyId;
        Session["CompanyId"] = companyId;
        Session["User"] = model.UserID;
        Session["pwd"] = pass;

        var acxUserSetupData = Session["acxUserSetupData"] as DataTable;

        if (acxUserSetupData != null)
        {
            string acxDisplayValue = acxUserSetupData.AsEnumerable()
           .Where(row => row.Field<string>("UserID") == model.UserID
                      && row.Field<string>("Password") == pass
                      && row.Field<string>("dataAreaId").Equals(companyId, StringComparison.OrdinalIgnoreCase))
           .Select(row => row.Field<string>("AcxDisplay"))
           .FirstOrDefault();


          // ... Rest of the code ...

          if (acxDisplayValue == "Gate")
          {
            return RedirectToAction("GetDisplayView", "GetDisplayData"); // Redirect to the Gate page
          }
          else if (acxDisplayValue == "Store")
          {
            return RedirectToAction("ViewData", "AmberFetchData"); // Redirect to the Store page
          }
          else
          {
            ModelState.AddModelError("", "Dont have Site to Display.");
            return View(model);
          }
        }
      }
      await FetchEntityData().ConfigureAwait(false);
      model.CompanyList = InitializeCompanyList();
      ModelState.AddModelError("", "Invalid login credentials or company selection.");
      return View(model);
    }


    public async Task<DataTable> GetD365EntityOutput(string apiAction, string filter = "", bool crossCompany = false)
    {
      var token = await GetToken();
      string BaseAddress = $"{D365DataUrl}{apiAction}";
      if (!string.IsNullOrWhiteSpace(filter))
      {
        BaseAddress += filter;
      }

      // Append cross-company filter only for "acxUserSetups" entity
      if (crossCompany && apiAction == "AcxUserSetups")
      {
        BaseAddress += $"?cross-company=true";
      }
      
      CServiceStatus cServiceStatus = new CServiceStatus();
      if (token == null)
      {
        cServiceStatus.ReturnMessage = "Login validate failed. Not able to authenticate. Please try again!!!";
        cServiceStatus.RequestStatus = HttpStatusCode.Unauthorized;
        return null;
      }
      using (var client = new HttpClient())
      {
        try
        {
          ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
          client.BaseAddress = new Uri(BaseAddress);
          client.DefaultRequestHeaders.Authorization
                   = new AuthenticationHeaderValue("Bearer", token.access_token);
          client.Timeout = TimeSpan.FromMinutes(10);

          var responseTask = client.GetAsync("");
          responseTask.Wait();

          var result = responseTask.Result;
          cServiceStatus.RequestStatus = result.StatusCode;
          cServiceStatus.ReturnMessage = result.RequestMessage.ToString();
          if (result.IsSuccessStatusCode)
          {
            var result_data = await result.Content.ReadAsStringAsync();
            cServiceStatus.ReturnJson = result_data;
            var jsonLinq = JObject.Parse(result_data);
            if (jsonLinq != null)
            {
              var srcArray = jsonLinq.Descendants().Where(d => d is JArray).First();
              if (srcArray.Count() > 0)
              {
                var trgArray = new JArray();
                DataTable resultoutput = JsonConvert.DeserializeObject<DataTable>(srcArray.ToString());
                return resultoutput;
              }
            }
          }
        }
        catch (Exception ex)
        {
          cServiceStatus.RequestStatus = HttpStatusCode.ServiceUnavailable;
          cServiceStatus.ReturnMessage = ex.Message;
        }
      }
      return null;
    }

    public async Task<Token> GetToken()
    {
      string result_data = string.Empty;
      using (var client = new HttpClient())
      {
        client.BaseAddress = new Uri($"https://login.microsoftonline.com/{tenant_id}/oauth2/token");
        var httpContent = new StringContent($"grant_type=client_credentials&client_secret=" + clientsecret + "&resource=" + resource + "&tenant_id=" + tenant_id + "&bearerToken=undefined&client_id=" + clientid, Encoding.UTF8, "application/x-www-form-urlencoded");
        var responseTask = client.PostAsync("", httpContent);
        responseTask.Wait();
        var result = responseTask.Result;
        if (result.IsSuccessStatusCode)
        {
          result_data = await result.Content.ReadAsStringAsync();
        }
      }
      return JsonConvert.DeserializeObject<Token>(result_data);
    }

    public class Token
    {
      public string access_token { get; set; }
      public string token_type { get; set; }
      public int expires_in { get; set; }
    }

    public class CServiceStatus
    {
      private HttpStatusCode requestStatus = HttpStatusCode.NotFound;
      private string returnJson = string.Empty;
      private string returnMessage = string.Empty;
      public HttpStatusCode RequestStatus
      {
        get { return requestStatus; }
        set { requestStatus = value; }
      }
      public string ReturnJson
      {
        get { return returnJson; }
        set { returnJson = value; }
      }
      public string ReturnMessage
      {
        get { return returnMessage; }
        set { returnMessage = value; }
      }
    }

    private bool ValidateLogin(string userId, string Password, string companyId)
    {
      var acxUserSetupData = Session["acxUserSetupData"] as DataTable;
      if (acxUserSetupData != null)
      {
        foreach (DataRow row in acxUserSetupData.Rows)
        {
          string rowUserId = row.Field<string>("UserID");
          string rowPassword = row.Field<string>("Password");
          string rowDataAreaId = row.Field<string>("dataAreaId");

          if (string.Equals(rowUserId, userId) && string.Equals(rowPassword, Password))
          {
            if (string.Equals(rowDataAreaId, companyId, StringComparison.OrdinalIgnoreCase))
            {
              return true;
            }
          }
        }
      }

      return false;
    }
  }
}
