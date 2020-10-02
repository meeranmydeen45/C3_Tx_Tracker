using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Web_Core_C3_App.Models;

namespace Web_Core_C3_App.Controllers
{
    public class HomeController : Controller
    {
        public AppDbContext dbContext;

        public HomeController(AppDbContext _dbcontext)
        {

            dbContext = _dbcontext;
        }

        public IActionResult Index()
        {
            return View();
        }


        public JsonResult AddCompanies(string[] EmployeeList, string CompanyName, double CompanyCode, double CompanyMobile)
        {
            Company company = dbContext.companies.SingleOrDefault(x => x.CompanyCode == CompanyCode);
            if (company != null)
            {
                company.MobileNumber = CompanyMobile;
                dbContext.Entry(company).State = Microsoft.EntityFrameworkCore.EntityState.Modified;

                for (int i = 0; i < EmployeeList.Length; i++)
                {
                    Employee employeeD = new Employee()
                    {

                        CompanyId = company.ID,
                        EmployeeName = EmployeeList[i],
                        CardRequested = true,
                        DateRequested = DateTime.Now,
                        CardDelivered = false,
                        CardRcvd = false
                    };
                    dbContext.employees.Add(employeeD);
                    dbContext.SaveChanges();
                }
            }
            else
            {
                Company companyD = new Company()
                {
                    CompanyName = CompanyName,
                    CompanyCode = CompanyCode,
                    MobileNumber = CompanyMobile
                };


                dbContext.companies.Add(companyD);
                dbContext.SaveChanges();

                Company c = dbContext.companies.First(x => x.CompanyCode == CompanyCode);

                for (int i = 0; i < EmployeeList.Length; i++)
                {
                    Employee employeeD = new Employee()
                    {

                        CompanyId = c.ID,
                        EmployeeName = EmployeeList[i],
                        CardRequested = true,
                        DateRequested = DateTime.Now,
                        CardDelivered = false,
                        CardRcvd = false
                    };
                    dbContext.employees.Add(employeeD);
                    dbContext.SaveChanges();
                }
            }

            string x = "Data Saved Successfully";
            return Json(x);
        }

        [HttpGet]
        public JsonResult SendEmail(string CompanyName, string Link, int NumberofEmployee)
        {
            
            string body = "Dear Sir/Madam, <br/><br/> We are Successfully Uploaded the CHF file Please approve.<br/><br/><br/><br/><br/><br/>"+ Link;
            MailMessage mailMessage = new MailMessage("multinettrustc3@gmail.com", "meeranmydeen@gmail.com");
            mailMessage.Body = body;
            mailMessage.Subject = "New C3 Requset- " +CompanyName+ " -No of Cards " + NumberofEmployee;
            mailMessage.IsBodyHtml = true;

           MailMessage mailMessage2 = new MailMessage("multinettrustc3@gmail.com", "cso.auh@multinettrust.com");
            mailMessage2.Body = body;
            mailMessage2.Subject = "New C3 Requset- " + CompanyName + " -No of Cards " + NumberofEmployee;
            mailMessage2.IsBodyHtml = true;

            MailMessage mailMessage3 = new MailMessage("multinettrustc3@gmail.com", "wps@multinettrust.com");
            mailMessage3.Body = body;
            mailMessage3.Subject = "New C3 Requset- " + CompanyName + " -No of Cards " + NumberofEmployee;
            mailMessage3.IsBodyHtml = true;

            SmtpClient smtpClient = new SmtpClient("smtp.gmail.com", 587);
            smtpClient.EnableSsl = true;
            smtpClient.Credentials = new System.Net.NetworkCredential()
            {
                UserName = "multinettrustc3@gmail.com",
                Password = "multi@123"
            };

            


            smtpClient.Send(mailMessage);
           // smtpClient.Send(mailMessage2);
           // smtpClient.Send(mailMessage3);

            var x = "Email Sent Successfully";
            return Json(x);
        }

        [HttpGet]
        public JsonResult UpdateRcvdCards_Get()
        {
            //List<Empcompany> employeesobj = new List<Empcompany>();
            //var employees = dbContext.employees.AsEnumerable().GroupBy(x=> x.CompanyId);

            List<Employee> listofEmployees = new List<Employee>();
            List<Company> listcompanies = new List<Company>();
            var employeesbyCompany = dbContext.companies.AsEnumerable()
                 .GroupJoin(dbContext.employees.Where(x=> x.CardRequested == true && x.CardRcvd != true && x.CardDelivered != true),

                 d => d.ID,
                 e => e.CompanyId,
                 (company, employees) => new
                 {
                     Company = company,
                     Employees = employees
                 }).ToList();

            foreach (var companies in employeesbyCompany)
            {
                Company company = new Company()
                {
                    ID = companies.Company.ID,
                    CompanyName = companies.Company.CompanyName,
                    CompanyCode = companies.Company.CompanyCode,
                    MobileNumber = companies.Company.MobileNumber,
                    Employees = companies.Employees != null ? Employee.GetEmployees(companies.Employees.ToList<Employee>()):null
                };
                listcompanies.Add(company);
                // foreach (var employee in companies.Employees)
                // {
                //   Employee e = new Employee()
                // {
                //     Id = employee.Id,
                //    EmployeeName = employee.EmployeeName,
                //    CompanyId = employee.CompanyId,
                //   Company = Company.ReturnCompany(employee.Company)
                //  };
                //  listofEmployees.Add(e);
                // }


            }
                return Json(listcompanies);

            }

        [HttpPost]
        public JsonResult UpdateRcvdCards_Post(string[] Employeeids)
        {
            
            string CompanyNameMatchingCheck = null;
            List<UpdateCardsPdfObj> listofPdfObj = new List<UpdateCardsPdfObj>();
            int idForObj = 1;
            foreach (string id in Employeeids)
            {
               
                int Id = Convert.ToInt32(id);
                Employee x = dbContext.employees.SingleOrDefault(x => x.Id == Id);
                x.CardRcvd = true;
                x.DateRcvd = DateTime.Now;

                dbContext.Entry(x).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                dbContext.SaveChanges();

                //PDF
                string CompanyName = dbContext.companies.Single(c => c.ID == x.CompanyId).CompanyName;
                
                if (CompanyNameMatchingCheck == CompanyName || CompanyNameMatchingCheck == null)
                {
                    
                    UpdateCardsPdfObj obj = new UpdateCardsPdfObj();
                    obj.CompanyName = CompanyName;
                    obj.EmpName = x.EmployeeName;
                    obj.Id = idForObj;
                    listofPdfObj.Add(obj);
                    CompanyNameMatchingCheck = CompanyName;
                }
                else
                {
                    idForObj += 1;
                    UpdateCardsPdfObj obj = new UpdateCardsPdfObj();
                    obj.CompanyName = CompanyName;
                    obj.EmpName = x.EmployeeName;
                    obj.Id = idForObj;
                    listofPdfObj.Add(obj);
                    CompanyNameMatchingCheck = CompanyName;


                }
               

                //employeePDF.EmployeeName = x.EmployeeName;
                //Company c = new Company();
                //c.CompanyName = CompanyNamePDF;
                //employeePDF.Company = c;
                //employeesforPdf.Add(employeePDF);
                //

                
            }
            //Grouping For Pdf based on Company
            var groupObj = listofPdfObj.GroupBy(x => x.Id);
            List<UpdateCardsPdfObj> listofGroupedObj = new List<UpdateCardsPdfObj>();
            foreach (var group in groupObj)
            {
                
                foreach(UpdateCardsPdfObj obj in group)
                {
                    obj.totalCount = group.Count();
                    listofGroupedObj.Add(obj);
                }
            }

            return Json(listofGroupedObj);

            


        }

        [HttpGet]
        public JsonResult AutoCompleteData_UpdateCards_Get(string name)
        {
            var Companies = dbContext.companies
                .Where(x => x.CompanyName.StartsWith(name) || x.CompanyName == null).ToList();
            return Json(Companies);
        }

        public JsonResult AvailableCards_Get()
        {
            List<Employee> listofEmployees = new List<Employee>();
            List<Company> listcompanies = new List<Company>();
            var employeesbyCompany = dbContext.companies.AsEnumerable()
                 .GroupJoin(dbContext.employees.Where(x => x.CardRequested == true && x.CardRcvd == true && x.CardDelivered != true),

                 d => d.ID,
                 e => e.CompanyId,
                 (company, employees) => new
                 {
                     Company = company,
                     Employees = employees
                 }).ToList();

            foreach (var companies in employeesbyCompany)
            {
                Company company = new Company()
                {
                    ID = companies.Company.ID,
                    CompanyName = companies.Company.CompanyName,
                    CompanyCode = companies.Company.CompanyCode,
                    MobileNumber = companies.Company.MobileNumber,
                    Employees = companies.Employees != null ? Employee.GetEmployees(companies.Employees.ToList<Employee>()) : null
                };
                listcompanies.Add(company);
               
            }
            return Json(listcompanies);
        }

        [HttpGet]
        public JsonResult GetData_BasedonAutoComplete(int id)
        {
            List<Employee> listofEmployees = new List<Employee>();
            List<Company> listcompanies = new List<Company>();
            var employeesbyCompany = dbContext.companies.Where(c => c.ID == id).AsEnumerable()
                 .GroupJoin(dbContext.employees.Where(x => x.CardRequested == true && x.CardRcvd == true && x.CardDelivered != true),

                 d => d.ID,
                 e => e.CompanyId,
                 (company, employees) => new
                 {
                     Company = company,
                     Employees = employees
                 }).ToList();

            foreach (var companies in employeesbyCompany)
            {
                Company company = new Company()
                {
                    ID = companies.Company.ID,
                    CompanyName = companies.Company.CompanyName,
                    CompanyCode = companies.Company.CompanyCode,
                    MobileNumber = companies.Company.MobileNumber,
                    Employees = companies.Employees != null ? Employee.GetEmployees(companies.Employees.ToList<Employee>()) : null
                };
                listcompanies.Add(company);
               

            }

            return Json(listcompanies);
        }

        public JsonResult DeliveredCards_Post(string[] Employeeids)
        {
            string CompanyNameMatchingCheck = null;
            List<UpdateCardsPdfObj> listofPdfObj = new List<UpdateCardsPdfObj>();
            int idForObj = 1;
            foreach (string id in Employeeids)
            {

                int Id = Convert.ToInt32(id);
                Employee x = dbContext.employees.SingleOrDefault(x => x.Id == Id);
                x.CardDelivered = true;
                x.DateDelevired = DateTime.Now;

                dbContext.Entry(x).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                dbContext.SaveChanges();
                //PDF
                string CompanyName = dbContext.companies.Single(c => c.ID == x.CompanyId).CompanyName;

                if (CompanyNameMatchingCheck == CompanyName || CompanyNameMatchingCheck == null)
                {

                    UpdateCardsPdfObj obj = new UpdateCardsPdfObj();
                    obj.CompanyName = CompanyName;
                    obj.EmpName = x.EmployeeName;
                    obj.Id = idForObj;
                    listofPdfObj.Add(obj);
                    CompanyNameMatchingCheck = CompanyName;
                }
                else
                {
                    idForObj += 1;
                    UpdateCardsPdfObj obj = new UpdateCardsPdfObj();
                    obj.CompanyName = CompanyName;
                    obj.EmpName = x.EmployeeName;
                    obj.Id = idForObj;
                    listofPdfObj.Add(obj);
                    CompanyNameMatchingCheck = CompanyName;


                }

                
            }
            //Grouping For Pdf based on Company
            List<UpdateCardsPdfObj> listofGroupedObj = new List<UpdateCardsPdfObj>();
            var groupObj = listofPdfObj.GroupBy(x => x.Id);
            foreach (var group in groupObj)
            {
                
                foreach (UpdateCardsPdfObj obj in group)
                {
                    obj.totalCount = group.Count();
                    listofGroupedObj.Add(obj);
                }
            }

            return Json(listofGroupedObj);


        }

        [HttpGet]
        public JsonResult DropDownData_Get()
        {
            List<Company> companies = dbContext.companies.ToList();
            return Json(companies);

        }

        [HttpGet]
        public JsonResult GetReportData(int companyId, int cardStatus, string fromDate, string toDate)
        {
            List<Employee> listEmp = new List<Employee>();
            List<Company> listcomp = new List<Company>();

            if (companyId == 0  || companyId == -1)
            {
                if (cardStatus == 1)
                {
                    var employeesbyCompany = dbContext.companies.AsEnumerable()
                   .GroupJoin(dbContext.employees.Where(x => x.DateRequested >= Convert.ToDateTime(fromDate) && x.DateRequested <= Convert.ToDateTime(toDate) && x.CardRequested == true && x.CardRcvd == false && x.CardDelivered == false),

                   d => d.ID,
                   e => e.CompanyId,
                   (company, employees) => new
                   {
                       Company = company,
                       Employees = employees
                   }).ToList();

                    foreach (var companies in employeesbyCompany)
                    {
                        Company company = new Company()
                        {
                            ID = companies.Company.ID,
                            CompanyName = companies.Company.CompanyName,
                            CompanyCode = companies.Company.CompanyCode,
                            MobileNumber = companies.Company.MobileNumber,
                            Employees = companies.Employees != null ? Employee.GetEmployees(companies.Employees.ToList<Employee>()) : null
                        };
                        listcomp.Add(company);
                    }

                    return Json(listcomp);

                }

                if(cardStatus == 2)
                {
                   var employeesbyCompany = dbContext.companies.AsEnumerable()
                  .GroupJoin(dbContext.employees.Where(x => x.DateRcvd >= Convert.ToDateTime(fromDate) && x.DateRcvd <= Convert.ToDateTime(toDate) && x.CardRcvd == true && x.CardDelivered == false),

                  d => d.ID,
                  e => e.CompanyId,
                  (company, employees) => new
                  {
                      Company = company,
                      Employees = employees
                  }).ToList();

                    foreach (var companies in employeesbyCompany)
                    {
                        Company company = new Company()
                        {
                            ID = companies.Company.ID,
                            CompanyName = companies.Company.CompanyName,
                            CompanyCode = companies.Company.CompanyCode,
                            MobileNumber = companies.Company.MobileNumber,
                            Employees = companies.Employees != null ? Employee.GetEmployees(companies.Employees.ToList<Employee>()) : null
                        };
                        listcomp.Add(company);
                    }

                    return Json(listcomp);



                }

                if(cardStatus == 3)
                {
                  var employeesbyCompany = dbContext.companies.AsEnumerable()
                 .GroupJoin(dbContext.employees.Where(x => x.DateDelevired >= Convert.ToDateTime(fromDate) && x.DateDelevired <= Convert.ToDateTime(toDate) && x.CardDelivered == true),

                 d => d.ID,
                 e => e.CompanyId,
                 (company, employees) => new
                 {
                     Company = company,
                     Employees = employees
                 }).ToList();

                    foreach (var companies in employeesbyCompany)
                    {
                        Company company = new Company()
                        {
                            ID = companies.Company.ID,
                            CompanyName = companies.Company.CompanyName,
                            CompanyCode = companies.Company.CompanyCode,
                            MobileNumber = companies.Company.MobileNumber,
                            Employees = companies.Employees != null ? Employee.GetEmployees(companies.Employees.ToList<Employee>()) : null
                        };
                        listcomp.Add(company);
                    }

                    return Json(listcomp);

                }

                var employeesbyComp = dbContext.companies.AsEnumerable()
                .GroupJoin(dbContext.employees.Where(x => x.DateRequested >= Convert.ToDateTime(fromDate)),

                d => d.ID,
                e => e.CompanyId,
                (company, employees) => new
                {
                    Company = company,
                    Employees = employees
                }).ToList();

                foreach (var companies in employeesbyComp)
                {
                    Company company = new Company()
                    {
                        ID = companies.Company.ID,
                        CompanyName = companies.Company.CompanyName,
                        CompanyCode = companies.Company.CompanyCode,
                        MobileNumber = companies.Company.MobileNumber,
                        Employees = companies.Employees != null ? Employee.GetEmployees(companies.Employees.ToList<Employee>()) : null
                    };
                    listcomp.Add(company);
                }

                return Json(listcomp);
            }
            else
            {
                if(cardStatus == 1)
                {
                    listEmp = dbContext.employees.Where(x => x.CompanyId == companyId && x.DateRequested >= Convert.ToDateTime(fromDate) && x.DateRequested <= Convert.ToDateTime(toDate) && x.CardRequested == true && x.CardRcvd == false && x.CardDelivered == false).ToList();
                    return Json(listEmp);
                }
                if (cardStatus == 2)
                {
                    listEmp = dbContext.employees.Where(x => x.CompanyId == companyId && x.DateRcvd >= Convert.ToDateTime(fromDate) && x.DateRcvd <= Convert.ToDateTime(toDate) && x.CardRequested == true && x.CardRcvd == true && x.CardDelivered == false).ToList();
                    return Json(listEmp);
                }
                if (cardStatus == 3)
                {
                    listEmp = dbContext.employees.Where(x => x.CompanyId == companyId && x.DateDelevired >= Convert.ToDateTime(fromDate) && x.DateDelevired <= Convert.ToDateTime(toDate) && x.CardRequested == true && x.CardRcvd == true && x.CardDelivered == true).ToList();
                    return Json(listEmp);
                }
                listEmp = dbContext.employees.Where(x => x.CompanyId == companyId).ToList();
                return Json(listEmp);
            }
        }


        public IActionResult Privacy()
        {
            return View();
        }




        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
