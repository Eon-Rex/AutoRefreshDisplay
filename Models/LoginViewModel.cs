using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Web.Mvc;


namespace AmberProject.Models
{
    public class LoginViewModel
    {
        public string UserID { get; set; }

        public string Password { get; set; }

        public string CompanyId { get; set; }

        public IEnumerable<SelectListItem> CompanyList { get; set; }

    }
}
