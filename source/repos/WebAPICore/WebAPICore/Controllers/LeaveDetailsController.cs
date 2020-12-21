using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebAPICore.Data;
using WebAPICore.Models;

using System.Net.Mail;
using System.Reflection;
using System.Text;

namespace WebAPICore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("AllowOrigin")]
    public class LeaveDetailsController : ControllerBase
    {
        private readonly EmployeeContext _context;

        public LeaveDetailsController(EmployeeContext context)
        {
            _context = context;
        }

        //[HttpGet]
        //public IEnumerable<LeaveDetails> GetLeaveDetails()
        //{
        //    return _context.LeaveDetails;
        //}

        [HttpGet]
        public IEnumerable<EmployeeLeaveDetails> GetLeaveDetails()
        {
            return _context.EmployeeLeaveDetails.FromSql<EmployeeLeaveDetails>("spGetLeavesWithEmployeeDetails").ToList();
        }

        [HttpGet("{id}")]
        public IEnumerable<LeaveDetails> GetLeaveDetail([FromRoute] long id)
        {
            var leaveDetail = _context.LeaveDetails.FromSql<LeaveDetails>("spGetLeaveDetailsByEmployeeID {0}", id).ToList();

            return leaveDetail;
        }

        [HttpPost]
        public async Task<IActionResult> PostLeaveDetails([FromBody] LeaveDetails leaveDetails)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.LeaveDetails.Add(leaveDetails);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetLeaveDetails", new { id = leaveDetails.LeaveID }, leaveDetails);
        }

        [HttpPut("{id}/{leaveStatus}")]
        public void PutLeaveDetails([FromRoute] long id, [FromRoute] string leaveStatus)
        {
            //var para[] = new SqlParameter { ("p0", id), ("p1", leaveStatus )};
            _context.Database.ExecuteSqlCommand("spUpdateLeaveStatus @p0, @p1",id,leaveStatus);
            ////var leavedetail = new Department();
            ////department.Name = txtDepartment.text.trim();
            ////db.Departments.add(department);
            ////db.SaveChanges();
            ////// EF will populate department.DepartmentId
            //_context.UpdateLeaveStatus.FromSql<UpdateLeaves>("spUpdateLeaveStatus {0}{1}", id, leaveStatus);
            _context.SaveChanges();
            var leaveDetail = _context.EmployeeLeaveDetails.FromSql<EmployeeLeaveDetails>("spGetLeaveDetailsByLeaveID {0}", id).ToList().FirstOrDefault();
            Get_TestMail(leaveDetail,leaveStatus);

        }

        [HttpPost]
        [Route("MailSend")]

        public IActionResult SendMail(Mail MailDetails)
        {
            try
            {
                using (MailMessage EMail = new MailMessage())
                {
                    MailAddress fromAddress = new MailAddress(MailDetails.Sender, "From LTI");
                    List<System.Net.Mail.Attachment> mailattachment = new List<System.Net.Mail.Attachment>();
                    EMail.From = fromAddress;
                    if (!string.IsNullOrEmpty(MailDetails.To))
                    {
                        foreach (string email in MailDetails.To.Split(','))
                        {
                            if (!string.IsNullOrEmpty(email))
                            {
                                try
                                {
                                    EMail.To.Add(email);
                                }
                                catch { }
                            }
                        }
                    }
                    if (EMail.To.Count == 0)
                    {
                        throw new Exception("To address is not present in the mail");
                    }
                    if (!string.IsNullOrEmpty(MailDetails.CC))
                    {
                        foreach (string email in MailDetails.CC.Split(','))
                        {
                            if (!string.IsNullOrEmpty(email))
                            {
                                try
                                {
                                    EMail.CC.Add(email);
                                }
                                catch { }
                            }
                        }
                    }
                    if (!string.IsNullOrEmpty(MailDetails.BCC))
                    {
                        foreach (string email in MailDetails.BCC.Split(','))
                        {
                            if (!string.IsNullOrEmpty(email))
                            {
                                try
                                {
                                    EMail.Bcc.Add(email);
                                }
                                catch { }
                            }
                        }
                    }

                    if (MailDetails.Attachments != null && MailDetails.Attachments.Any())
                    {
                        foreach (var attachment in MailDetails.Attachments)
                        {
                            var contentType = new System.Net.Mime.ContentType(System.Net.Mime.MediaTypeNames.Application.Pdf);
                            var emailAttachment = new System.Net.Mail.Attachment(attachment.Value, contentType);
                            emailAttachment.ContentDisposition.FileName = attachment.Key;
                            EMail.Attachments.Add(emailAttachment);
                        }
                    }


                    EMail.IsBodyHtml = true;
                    EMail.Body = MailDetails.Body;
                    EMail.Subject = MailDetails.Subject;

                    using (SmtpClient smtpClient = new SmtpClient())
                    {
                        //  smtpClient.Host = "smtp.office365.com";
                        smtpClient.Host = "smtp.gmail.com";
                        //  smtpClient.EnableSsl = false;
                        smtpClient.Port = 587;
                        // smtpClient.Port = 25;
                        smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
                        smtpClient.UseDefaultCredentials = false;
                        //   smtpClient.Credentials = new System.Net.NetworkCredential("amitkumar.jaiswal@lntinfotech.com", "Lnt@0987654");
                        smtpClient.Credentials = new System.Net.NetworkCredential("vairokz7@gmail.com", "Gmailpass@1995");
                        smtpClient.EnableSsl = true;
                        smtpClient.Timeout = 6000000;
                        smtpClient.Send(EMail);
                    }
                    MailDetails.Status = "Success";
                    // return Content(HttpStatusCode.OK, true);
                    return Ok();
                }
            }
            catch (Exception ex)
            {
                MailDetails.Status = "Fail";
                return BadRequest(ex.Message.ToString());
            }
        }


        [HttpGet]
        [Route("Get_TestMail")]
        public IActionResult Get_TestMail(EmployeeLeaveDetails LeaveData,string leaveStatus)
        {
            string currentMethod = MethodBase.GetCurrentMethod().Name;
            
            #region Send Mail Code
            try
            {
                //  using (Proj context = new DevEntity())
                //{


                StringBuilder MailBody_PM = new StringBuilder();


                MailBody_PM.Append("<font face=\"calibri\" size=\"3\">Dear " + LeaveData.EmployeeName + ",<br/><br/></font>");
                //MailBody_PM.Append("Dear #Employee_Name#,< br/><br/>");

                MailBody_PM.Append("<font face=\"calibri\" size=\"3\">Your Earned Leave application from "+LeaveData.LStartDate.Value.Date.ToString("dd MMM yyyy") +" to "+ LeaveData.LEndDate.Value.Date.ToString("dd MMM yyyy") + " has been " + leaveStatus.ToLower() + " by your superior.<br/><br/></font>");

                MailBody_PM.Append("<br/><br/><br/>");
                MailBody_PM.Append("<font face=\"calibri\" size=\"3\">Regards,<br/>Leave Administrator</font><br/><br/></font>");
                MailBody_PM.Append("<font face=\"calibri\" size=\"3\"><hr width=\"96%\"><p style=\"color: red;\"> Note: This is an automatically generated email. Please do not reply directly to this mail.</p></font>");

                // var Get_EmployeeDetails = context.vw_EmployeeData_MyLTI.Where(f => f.Emp_PSNO == assignmentDetails.Emp_PSNO).Select(f => new { f.Emp_PSNO, f.Full_Name }).FirstOrDefault();
                //MailBody_PM.Replace("#Employee_Name#", Get_EmployeeDetails.Emp_Name);
                string status = "";

                if(leaveStatus == "Approved")
                {
                    status = "Approval";
                }
                else
                {
                    status = "Rejection";
                }


                Mail ObjMail_PM = new Mail()
                {
                    To = "10655380@lntinfotech.com",
                    CC = "10639274" + "@lntinfotech.com," + "10643265" + "@lntinfotech.com," + "10670758" + "@lntinfotech.com",
                    //CC = "10670758@lntinfotech.com",



                    Subject = "Automatic " + status +": Leave Application " +leaveStatus + " by Superior",
                    Body = MailBody_PM.ToString(),
                    Sender = "vairokz7@gmail.com",
                    CallingMethodName = MethodBase.GetCurrentMethod().Name,
                    Status = "Pending"
                };
                //MailController obj = new MailController();
                SendMail(ObjMail_PM);

                return Ok();
            }
            #endregion

            catch (Exception ex)
            {
                return BadRequest("Something went wrong");
            }
        }
    }
}