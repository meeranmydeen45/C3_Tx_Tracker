using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Web_Core_C3_App.Models;

namespace Web_Core_C3_App.Models
{
    public class Company
    {
        public int ID { get; set; }
        public string CompanyName { get; set; }
        public double CompanyCode { get; set; }
        public double MobileNumber { get; set; }

        public IList<Employee> Employees { get; set; }

        public static Company ReturnCompany(Company c)
        {
            Company ce = new Company();
            ce.ID = c.ID;
            ce.CompanyCode = c.CompanyCode;
            ce.CompanyName = c.CompanyName;
            ce.MobileNumber = c.MobileNumber;
            ce.Employees = Employee.GetEmployees(c.Employees);

            return ce;
        }


    }

 
    
}
