using Medicine.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Medicine.Controllers
{
    public class WorkerController : Controller
    {
        // GET: Worker
        [HttpGet]
        public ActionResult Login()
        {
            Session.Remove("Id");
            Session.Remove("Name");
            Session.Remove("Layout");
            return View();
        }
        [HttpPost]
        public ActionResult Login(Workers worker)
        {
            if(worker.Id == null)
            {
                ViewBag.error = "נא הכנס ת.ז.";
                return View(worker);
            }
            Workers found = ModelDB.GetDB().Workers.Where(w => w.Id == worker.Id).FirstOrDefault();
            if (found != null)
            {
                //העובד נמצא
                //בדיקת סיסמא
                if (found.Password == worker.Password)
                {
                    //נשמור את הת.ז. של העובד בסשן
                    Session["Id"] = found.Id;
                    Session["Name"] = found.FirstName + " " + found.LastName;
                    //אם הסיסמא תקינה - נגדיר את התפריט המתאים לפי סוג העובד
                    string layout = "";
                    switch (found.Roles.Name)
                    {
                        case "מנהל":
                            layout = "~/Views/Shared/_ManagerLayout.cshtml";
                            break;
                        case "רופא":
                            layout = "~/Views/Shared/_DoctorLayout.cshtml";
                            break;
                        case "כח עזר":
                            layout = "~/Views/Shared/_HelpingPowerLayout.cshtml";
                            break;
                        case "אחות":
                            layout = "~/Views/Shared/_NourseLayout.cshtml";
                            break;
                    }
                    Session["Layout"] = layout;
                    return RedirectToAction("Index", "Worker");
                }
                else
                {
                    ViewBag.error = "סיסמא שגויה";
                    return View(worker);
                }
            }
            else
            {
                ViewBag.error = "עובד לא קיים";
                return View(worker);
            }
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Update()//עדכון
        {

            string WorkerId = Session["Id"].ToString();
           Workers worker= ModelDB.GetDB().Workers.Single(w => w.Id == WorkerId);


            return View(worker);
        }
        [HttpPost]
        public ActionResult Update(Workers worker)
        {
            //if (ModelState.IsValid)
            //{  }
                Workers w = ModelDB.GetDB().Workers.Where(x => x.Id == worker.Id).Single();
                ModelDB.GetDB().Workers.Remove(w);
                ModelDB.GetDB().Workers.Add(worker);                
                //עדכון בDB
                ModelDB.GetDB().SaveChanges();
                //להחזיר הודעה עודכן בהצלחה
                return RedirectToAction("AllWorker");
            
        }
        public ActionResult Edit(string id)//עריכה
        {
            Workers worker = ModelDB.GetDB().Workers.Single(w => w.Id == id);

            return View("Update",worker);
        }
        public ActionResult SetActiveState(string id)//הפוך מצב פעיל
        {
            Workers worker = ModelDB.GetDB().Workers.Single(w => w.Id == id);
            worker.Active = !worker.Active;
            //עדכון בבסיס הנתונים
            ModelDB.GetDB().SaveChanges();
            return RedirectToAction("AllWorkers");
        }

        public ActionResult AllWorkers()
        {
            return View(ModelDB.GetDB().Workers);
        }
        public ActionResult Neworker()
        {
            //string WorkerId = Session["Id"].ToString();
            //Workers worker = ModelDB.GetDB().Workers.Single(w => w.Id == WorkerId);
            //worker
            var roles = ModelDB.GetDB().Roles.ToList();//רשימת כל התפקידים
            ViewBag.MyRoles = new SelectList(roles, "CodeRole", "Name");
            return View();
        }
        [HttpPost]
        public ActionResult Neworker(Workers worker)
        {
            if (ModelState.IsValid)
            {
                bool flag = true;
                int num = 0;
                Workers w = ModelDB.GetDB().Workers.Where(a => a.Id == worker.Id).FirstOrDefault();
                if (w != null)//בדיקה האם עובד קיים במערכת
                {
                    ModelState.AddModelError("Id","עובד קיים במערכת");
                    ViewBag.MyRoles = new SelectList(ModelDB.GetDB().Roles.ToList(), "CodeRole", "Name");
                    return View("Neworker");
                }
                while (flag == true)//הגרלת סיסמא
                {
                    Random r = new Random();
                    num = r.Next(1, 99999);
                    flag = false;
                    foreach (var item in ModelDB.GetDB().Workers)
                    {
                        if (item.Password == num.ToString())
                        {
                            flag = true;
                            break;
                        }

                    }
                }
                //שליחת סיסמא למייל של עובד חדש
                string mail = worker.Email;
                SendEmail(mail, "Password", num.ToString());
                worker.Active = true;
                worker.Password = num.ToString();
                ModelDB.GetDB().Workers.Add(worker);
                ModelDB.GetDB().SaveChanges();              
                return View("Index");
            }
            var roles = ModelDB.GetDB().Roles.ToList();//רשימת כל התפקידים
            ViewBag.MyRoles = new SelectList(roles, "CodeRole", "Name");
            return View("Neworker");
        }
        //פונ' לשליחת מייל
        public static void SendEmail(string ToAddress, /*string CcAddress,*/ string Subject, /*AlternateView*/ string Body)
        {
            var smtpCleint = new System.Net.Mail.SmtpClient("smtp.gmail.com");
            var MailMessage = new System.Net.Mail.MailMessage
            {
                From = new System.Net.Mail.MailAddress("medicationsdevision@gmail.com", "חלוקת תרופות"),
                Subject = Subject,
                IsBodyHtml = true,
            };
            MailMessage.Body = Body;
            //MailMessage.AlternateViews.Add(Body);
            MailMessage.To.Add(ToAddress);
            //MailMessage.CC.Add(CcAddress);
            smtpCleint.Credentials = new System.Net.NetworkCredential("medicationsdevision@gmail.com", "Med3238666");
            smtpCleint.EnableSsl = true;
            smtpCleint.Send(MailMessage);
        }


    }
}