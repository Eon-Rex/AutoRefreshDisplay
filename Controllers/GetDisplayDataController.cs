using AmberProject.Models;
using Microsoft.Ajax.Utilities;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Web.Configuration;
using System.Web.Mvc;
using System.Web.Mvc.Filters;

namespace AmberProject.Controllers
{
  public class GetDisplayDataController : Controller
  {

    // Connection URL authentication
    private static readonly string clientid = WebConfigurationManager.AppSettings["clientidD365"];
    private static readonly string clientsecret = WebConfigurationManager.AppSettings["seckeyD365"];
    private static readonly string tenant_id = WebConfigurationManager.AppSettings["TenetIdD365"];
    private static readonly string resource = WebConfigurationManager.AppSettings["ResourceD365"];
    private static readonly string D365DataUrl = WebConfigurationManager.AppSettings["D365DataUrl"];


    public async Task<ActionResult> GetDisplayView(string[] selectedSiteIds)
    {
      Response.Headers.Add("Refresh", "30");
      HomeController homeController = new HomeController();


      //LoginViewModel loginViewModel = new LoginViewModel();   
      //string companyId = loginViewModel.CompanyId;

      string companyId = Session["CompanyId"] as string;

      int daysToSubtract = int.Parse(ConfigurationManager.AppSettings["DaysToSubtract"]);
      DateTime currentDate = DateTime.Now;
      DateTime resultDate = currentDate.AddDays(-daysToSubtract);
      string resultDateStr = resultDate.ToString("yyyy-MM-ddTHH:mm:ssZ");

      //and IsVehicleExit eq Microsoft.Dynamics.DataEntities.NoYes'Yes'
      DataTable result = await homeController.GetD365EntityOutput("VendPortVehicleArrIntimations", $"?$filter=SourceDataAreaID eq '{companyId}' and Expired eq Microsoft.Dynamics.DataEntities.NoYes'No' and AcxVehicleArrivalDate ge {resultDateStr}").ConfigureAwait(false);

      List<AcxVendPortVehicleArrIntimationEntityGetDisplay> modelList = new List<AcxVendPortVehicleArrIntimationEntityGetDisplay>();
      try
      {
        foreach (DataRow row in result.Rows)
        {

          DateTime exittime = (DateTime)(row["EXITDATEANDTIME"]);

          DateTime arrivaldatetime = (DateTime)(row["ARRIVALDATEANDTIME"]/*.ToString(), "dd-MM-yyyy HH:mm:ss", null, System.Globalization.DateTimeStyles.None, out arrivaldatetime*/);

          string ExitFixFormat;
          string ArrivalFixFormat;
          // Check if exittime and arrivaldatetime are not equal to the specified default values
          if (exittime != DateTime.Parse("01-01-1900 00:00:00") /*&& exittime != DateTime.Parse("1/1/1900 12:00:00 AM") && exittime != DateTime.Parse("1 / 1 / 0001 12:00:00 AM")*/)
          {
            exittime = exittime.AddHours(5).AddMinutes(30);
            ExitFixFormat = exittime.ToString("dd/MM/yyyy hh:mm:ss tt");
          }
          else
          {
            exittime = DateTime.MinValue;
            ExitFixFormat = exittime.ToString("dd/MM/yyyy hh:mm:ss tt");
          }
          if (arrivaldatetime != DateTime.Parse("01-01-1900 00:00:00")/* && arrivaldatetime != DateTime.Parse("1/1/1900 12:00:00 AM") && exittime != DateTime.Parse("1 / 1 / 0001 12:00:00 AM")*/)
          {
            arrivaldatetime = arrivaldatetime.AddHours(5).AddMinutes(30);
            ArrivalFixFormat = arrivaldatetime.ToString("dd/MM/yyyy hh:mm:ss tt");
          }
          else
          {
            arrivaldatetime = DateTime.MinValue;
            ArrivalFixFormat = arrivaldatetime.ToString("dd/MM/yyyy hh:mm:ss tt");
          }
          int arrivalTimeSeconds;
          int.TryParse(row["ACXVEHICLEARRIVALTIME"].ToString(), out arrivalTimeSeconds);
          TimeSpan arrivalTime = TimeSpan.FromSeconds(arrivalTimeSeconds);

          DateTime dateTime = DateTime.Today.Add(arrivalTime);

          string formattedTime = dateTime.ToString("hh:mm:ss tt");

          DateTime acxVehicleArrivalDate;
          DateTime.TryParse(row["ACXVEHICLEARRIVALDATE"].ToString(), out acxVehicleArrivalDate);
          acxVehicleArrivalDate = acxVehicleArrivalDate.Date.AddHours(arrivalTime.Hours).AddMinutes(arrivalTime.Minutes).AddSeconds(arrivalTime.Seconds);

          string vehicleArrivalDate = acxVehicleArrivalDate.ToString("dd/MM/yyyy");/*hh:mm:ss tt"); */

          int AcxGateDisplayHours;
          int.TryParse(row["AcxGateDisplayHours"].ToString(), out AcxGateDisplayHours);

          DateTime expectedExitTime = exittime.AddMinutes(AcxGateDisplayHours);

          string status = row["Status"].ToString();

          int AcxVehicalArrivalDay;
          int.TryParse(row["AcxVehicalArrivalDay"].ToString(), out AcxVehicalArrivalDay);
          TimeSpan arrivalDay = TimeSpan.FromDays(AcxVehicalArrivalDay);

          DateTime newDate = acxVehicleArrivalDate.AddDays(arrivalDay.TotalDays);

          if (newDate >= System.DateTime.Now)
          {
            if ((expectedExitTime > System.DateTime.Now) || exittime == DateTime.MinValue)
            {
              TimeSpan duration;

              if (exittime != DateTime.MinValue) // Check if exit time is not the default value
              {
                duration = (exittime - arrivaldatetime).Duration();
              }
              else
              {
                duration = TimeSpan.Zero;
              }

              AcxVendPortVehicleArrIntimationEntityGetDisplay model = new AcxVendPortVehicleArrIntimationEntityGetDisplay
              {
                SiteId = row["SiteId"].ToString(),
                ACXTOKEN = row["ACXTOKEN"].ToString(),
                ACXVECHILESIZE = row["ACXVECHILESIZE"].ToString(),
                ACXVEHICLEARRIVALDATE = vehicleArrivalDate,
                ACXVEHICLEARRIVALTIME = formattedTime,
                ACXVEHICLENUMBERALTERNATE = row["ACXVEHICLENUMBERALTERNATE"].ToString(),
                ACXVPREFDOCTYPE = row["ACXVPREFDOCTYPE"].ToString(),
                EXITTIME = ExitFixFormat,
                INVOICENUMBER = row["INVOICENUMBER"].ToString(),
                NAME = row["NAME"].ToString(),
                NUMBERSEQUENCE = row["NUMBERSEQUENCE"].ToString(),
                TOSITEID = row["TOSITEID"].ToString(),
                TRANSPORTERNAME = row["TRANSPORTERNAME"].ToString(),
                VEHICLENUMBER = row["VEHICLENUMBER"].ToString(),
                VehicleEnter = row["IsVehicleEnter"].ToString(),
                VehicleExit = row["IsVehicleExit"].ToString(),
                ArrivalDateAndTime = ArrivalFixFormat,
                Duration = $"{duration.Hours:00}:{duration.Minutes:00}:{duration.Seconds:00}"
              };

              modelList.Add(model);
              //}
            }
          }
        }
        modelList = modelList.OrderBy(m => m.ArrivalDateAndTime).ToList();
        var acxUserSetupData = Session["acxUserSetupData"] as DataTable;
        string UserID = Session["User"] as string;
        string pass = Session["pwd"] as string;
        List<string> gateStoreIDs = acxUserSetupData.AsEnumerable()
              .Where(row => row.Field<string>("UserID") == UserID
                        && row.Field<string>("Password") == pass
                        && row.Field<string>("dataAreaId").Equals(companyId, StringComparison.OrdinalIgnoreCase))
              .Select(row => row.Field<string>("GateStoreSite"))
              .FirstOrDefault()?.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries)?.Select(s => s.Trim()).ToList();



        ViewBag.selectedSiteId = gateStoreIDs
            .OrderBy(id => id)
            .Select(id => new SelectListItem { Value = id, Text = id })
            .ToList();

        if (selectedSiteIds != null && selectedSiteIds.Length > 0)
        {
          modelList = modelList.Where(x => selectedSiteIds.Contains(x.SiteId)).ToList();
        }

        ViewBag.ShowData = selectedSiteIds != null && selectedSiteIds.Length > 0;

        
      }
      catch (Exception )
      {
        ViewBag.ErrorMessage = "You have no data to show in Entity " ;
      }
      return View(modelList);
    }  

  }
}