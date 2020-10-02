using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web_Core_C3_App.Models
{
    public class Employee
    {
        public int Id { get; set; }

        public string EmployeeName { get; set; }        
        

        public bool? CardRequested { get; set; }
        public DateTime? DateRequested { get; set; }

        public bool? CardRcvd { get; set; }
        public DateTime? DateRcvd { get; set; }

        public bool? CardDelivered { get; set; }
        public DateTime? DateDelevired { get; set; }

        public int CompanyId { get; set; }
        public Company Company { get; set; }



        public static IList<Employee> GetEmployees(IList<Employee> emps)
        {   
            IList<Employee> employees = new List<Employee>();
            foreach (Employee e in emps)
            {
                Employee employee = new Employee();
                employee.EmployeeName = e.EmployeeName;
                employee.Id = e.Id;
                employees.Add(employee);

            }

            return employees;
        }

    }


   
}
