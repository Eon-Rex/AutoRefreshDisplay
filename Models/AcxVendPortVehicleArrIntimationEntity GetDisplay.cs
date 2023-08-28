using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace AmberProject.Models
{
    

    public class AcxVendPortVehicleArrIntimationEntityGetDisplay
    {
        //public string IsVehicleExit { get; set; }
        //public string Expired { get; set; }
        public string VehicleExit { get; set; }
        public string VehicleEnter { get; set; }
        public string SiteId { get; set; }          
        public string ACXTOKEN { get; set; }
        public string ACXVECHILESIZE { get; set; }
        public string ACXVEHICLEARRIVALDATE { get; set; }
        public string ACXVEHICLEARRIVALTIME { get; set; }
        public string ACXVEHICLENUMBERALTERNATE { get; set; }
        public string ACXVPREFDOCTYPE { get; set; }
        public string EXITTIME { get; set; }
        public string INVOICENUMBER { get; set; }
        public string NAME { get; set; }
        public string NUMBERSEQUENCE { get; set; }
        public string TOSITEID { get; set; }
        public string TRANSPORTERNAME { get; set; }
        public string VEHICLENUMBER { get; set; }
        public string Duration { get; set; }
        public string AcxGateDisplayHours { get; set; }
        public string ArrivalDateAndTime { get; set; }

    }
}