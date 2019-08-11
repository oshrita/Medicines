using Medicine.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Medicine.Controllers
{
    public class PatiantController : Controller
    {
        
        // GET: Patiant
        public ActionResult AllPatiant()
        {
            Session["statusShow"] = false;
            return View(ModelDB.GetDB().Burdeneds.ToList());
           
        }
        public ActionResult PatiantDetailes(string id)
        {
            //if(ViewBag.searcDate!=null)
            //{
            //    Session["searcDate"] = ViewBag.searcDate;
            //}
            var patient = ModelDB.GetDB().Burdeneds.Single(b => b.Id == id);
            Session["Burdened"] = patient;
            Session["IdBurdened"] = id;
            return View(patient);
        }
        [HttpPost]
        public ActionResult PatiantDetailes(string id, string searchDate)
        {
            Session["searcDate"] = searchDate;
            Session["statusShow"] = true;
            //if (ViewBag.searcDate != null)
            //{
            //    Session["searcDate"] = ViewBag.searcDate;
            //}
            var patient = ModelDB.GetDB().Burdeneds.Single(b => b.Id == id);
            Session["Burdened"] = patient;
            Session["IdBurdened"] = id;
            return View(patient);
        }
        //פונקציה שמציגה את המיטה הנוכחית ותאריך השיבוץ
        public ActionResult PatiantMoveBed(string id)
        {
            //שמירת ת"ז המטופל
            var patient = ModelDB.GetDB().Burdeneds.Single(b => b.Id == id);
            Session["Burdened"] = patient;
            Session["IdBurdened"] = id;
            //שמירת נתוני המחלקה ברשימה
            var d = new SelectList(ModelDB.GetDB().Departments, "CodeDepartment", "DepartmentName");
            ViewBag.departments = d;
            //שמירת נתוני החדר ברשימה
            var r = new SelectList(ModelDB.GetDB().Rooms, "CodeRoom", "CodeRoom");
            ViewBag.rooms = r;
            //שליפת האשפוז הנוכחי של הטופל הספציפי
            Hospitalizations h = ModelDB.GetDB().Hospitalizations.Where(a => a.IdBurdened == id &&
            a.ExitDate == null).FirstOrDefault();
            //שמירה במשתנה רשימה מסוג מיטות לאשפוז
            var beds = new List<HospitalizationBeds>();
            //בדיקה האם המטופל אכן מאושפז
            if (h != null)
            {
                beds = h.HospitalizationBeds.Where(bed => bed.EndDate != null).ToList();
                //שליפת המיטה הנוכחית
                HospitalizationBeds current = h.HospitalizationBeds.Where(bed => bed.EndDate != null).FirstOrDefault();
                if (current != null)
                {
                    //שמירה ב viewBag
                    ViewBag.currentBed = "מספר מיטה " + current.CodeBed + " . תאריך שיבוץ : " + current.BeginDate;
                }
            }
            return View("Bed", beds);
            //ViewBag.message = "מטופל אינו משובץ במיטה";
            //return PartialView("Messege");
        }
        //string BurdenedId = ((Burdeneds)Session["Burdened"]).Id;
        //    return View(aa.Burdeneds.Single(b => b.Id == BurdenedId).Hospitalizations);

        //     string WorkerId = Session["Id"].ToString();
        //Workers worker = ModelDB.GetDB().Workers.Single(w => w.Id == WorkerId);
        //    return View(worker);
        public PartialViewResult Bed()
        {
            var d = new SelectList(ModelDB.GetDB().Departments, "CodeDepartment", "DepartmentName");
            ViewBag.departments = d;
            var r = new SelectList(ModelDB.GetDB().Rooms, "CodeRoom", "CodeRoom");
            ViewBag.rooms = r;
            string BurdenedId = Session["IdBurdened"].ToString();
            Hospitalizations h = ModelDB.GetDB().Hospitalizations.Where(a => a.IdBurdened == BurdenedId && a.ExitDate == null).FirstOrDefault();
            var beds = new List<HospitalizationBeds>();
            if (h != null)
            {
                beds = h.HospitalizationBeds.Where(bed => bed.EndDate != null).ToList();
                HospitalizationBeds current = h.HospitalizationBeds.Where(bed => bed.EndDate != null).FirstOrDefault();
                if (current != null)
                {
                    ViewBag.currentBed = "מספר מיטה " + current.CodeBed + " . תאריך שיבוץ : " + current.BeginDate;
                }
            }
            return PartialView(beds);
        }
        //פונקציה לבחירת חדר המחזירה את חדרי המחלקה
        [HttpGet]
        public ActionResult GetRooms(int departmentId)
        {
            //שליפת החדרים הפעילים בתוך המחלקה הספציפית והכנסתם לרשימה
            var rooms = ModelDB.GetDB().Departments.Single(d => d.CodeDepartment == departmentId).Rooms
                .Where(r => r.Active == true).Select(r => new { Text = r.CodeRoom, Value = r.CodeRoom });
            //foreach (var item in rooms)
            //{
            //    ModelDB.GetDB().HospitalizationBeds.Where(x=>x.CodeRoom == item.Value && x.e)
            //}
            return Json(rooms, JsonRequestBehavior.AllowGet);
        }
        //פונקציה לבחירת מיטה המחזירה רק את המיטות הפעילות
        [HttpGet]
        public ActionResult GetBeds(int roomId)
        {
            //רשימת כל המיטות בחדר המבוקש שפעילות
            List<Beds> beds = ModelDB.GetDB().Rooms.Single(r => r.CodeRoom == roomId).Beds.Where(b => b.Active == true).ToList();
            List<Beds> s = new List<Beds>();
            foreach (var item in beds)
            {               
                //שליפת כל המיטות לאישפוז המתאימות שכרגע לא מאושפזים בהם
                HospitalizationBeds m = ModelDB.GetDB().HospitalizationBeds
                    .Where(x => x.CodeBed == item.CodeBed && x.EndDate != null).FirstOrDefault();
                if (m != null)//אם קיים כאלה
                {
                    //Beds current = ModelDB.GetDB().Beds.Where(x => x.CodeBed == m.CodeBed).FirstOrDefault();
                    s.Add(item);
                }
                else
                {
                    //שליפת המיטות שעדיין אף פעם לא התאשפזו בהם
                    HospitalizationBeds k = ModelDB.GetDB().HospitalizationBeds.Where(x => x.CodeBed == item.CodeBed).FirstOrDefault();
                    if (k == null)//אם קיים כאלה
                        s.Add(item);//תוסיף אותם לרשימת המיטות לאישפוז
                }

            }
            var beds2 = s.Select(b => new { Text = b.CodeBed, Value = b.CodeBed });
            return Json(beds2, JsonRequestBehavior.AllowGet);
        }
        public PartialViewResult Medicines()
        {
            List<HospitalizationMedicine> medicines = new List<HospitalizationMedicine>();
            string BurdenedId = ((Burdeneds)Session["Burdened"]).Id;
            //aa.Burdeneds.Single(b => b.Id == BurdenedId).Hospitalizations.Where(h=>h.ExitDate==null);
            foreach (var item in ModelDB.GetDB().Burdeneds.Single(b => b.Id == BurdenedId).Hospitalizations.Where(h => h.ExitDate != null))
            {
                medicines.AddRange(item.HospitalizationMedicine);
            }
            return PartialView("Medicines", medicines);
        }
        //public ActionResult MedicinesDetailes()//פרטי תרופה
        //{
        //    string BurdenedId = ((Burdeneds)Session["Burdened"]).Id;
        //    return View(ModelDB.GetDB().Burdeneds.Single(b => b.Id == BurdenedId).HourForMedicine);
        //}
        public ActionResult Search(string searchDate, string id)
        {
            DateTime d = DateTime.Parse(searchDate);
            Hospitalizations hospitalInDates = ModelDB.GetDB().Hospitalizations.Where(x => x.IdBurdened == id && x.EnteringDate <=d && x.ExitDate >= d).FirstOrDefault();
            return RedirectToAction("OldHospitalization");
        }

        public ActionResult Demobilization()
        {
            return View();
        }
        public ActionResult NewPatiant()
        {
            return View();
        }
        [HttpPost]
        public ActionResult NewPatiant(Burdeneds burdend)
        {
            if (ModelState.IsValid)
            {
                //בדיקת תקינות
                ViewBag.patiantIsExist = "";
                Burdeneds b = ModelDB.GetDB().Burdeneds.Where(a => a.Id == burdend.Id).FirstOrDefault();
                if (b != null)
                {
                    ViewBag.patiantIsExist = "מטופל קיים במערכת";
                    return View("NewPatiant");
                }
                //ViewBag.contactNotExist = "";
                //Contact c = ModelDB.GetDB().Contact.Where(a => a.CodeContact == burdend.CodeContact).FirstOrDefault();
                //if (c == null)
                //{
                //    ViewBag.contactNotExist = "איש קשר אינו קיים";
                //    return View("NewPatiant");
                //}
                burdend.CodeContact = 1;
                ModelDB.GetDB().Burdeneds.Add(burdend);
                ModelDB.GetDB().SaveChanges();
                //עדכון בDB
                //להחזיר הודעה עודכן בהצלחה
                return View("AllPatiant");
            }
            else
                return View("NewPatiant");
        }

        //public ActionResult OldHospitalization()
        //{
        //    string PatiantId = Session["IdBurdened"].ToString();
        //    //Burdeneds patiant = ModelDB.GetDB().Burdeneds.Single(b => b.Id == PatiantId).Hospitalizations;
        //    List<Hospitalizations> f = ModelDB.GetDB().Hospitalizations.ToList();
        //    List<Hospitalizations> h = ModelDB.GetDB().Hospitalizations.Where(a => a.IdBurdened == PatiantId).ToList();
        //    return View(h);
        //}
        public PartialViewResult OldHospitalization()
        {
            string PatiantId = Session["IdBurdened"].ToString();
            //Burdeneds patiant = ModelDB.GetDB().Burdeneds.Single(b => b.Id == PatiantId).Hospitalizations;
            List<Hospitalizations> h = ModelDB.GetDB().Hospitalizations.Where(a => a.IdBurdened == PatiantId).ToList();
            return PartialView(h);
        }
        [HttpPost]
        public PartialViewResult OldHospitalization(string id)
        {
            string searchDate = Session["searcDate"].ToString();
            DateTime d = DateTime.Parse(searchDate);
            List<Hospitalizations> hospitalInDates = ModelDB.GetDB().Hospitalizations.Where(x => x.IdBurdened == id && x.EnteringDate <= d && x.ExitDate >= d).ToList();
           
            //string PatiantId = Session["IdBurdened"].ToString();
            ////Burdeneds patiant = ModelDB.GetDB().Burdeneds.Single(b => b.Id == PatiantId).Hospitalizations;
            //List<Hospitalizations> h = ModelDB.GetDB().Hospitalizations.Where(a => a.IdBurdened == PatiantId).ToList();
            return PartialView(hospitalInDates);
        }
        //פונקציית העברת המיטה
        [HttpPost]
        public ActionResult ChangeBed(int CodeHospitalization, int CodeBed)
        {
            string idWorker = Session["Id"].ToString();//שליפת מבצע השיבוץ
            //יצירת מיטה לאישפוז חדשה
            HospitalizationBeds newH = new HospitalizationBeds() {
                CodeHospitalization = CodeHospitalization, CodeBed = CodeBed, BeginDate = DateTime.Today, IDAchievementPlacemen = idWorker };
            string BurdenedId = Session["IdBurdened"].ToString();//שליפת המטופל הספציפי
            //שליפת האישפוז הנוכחי של המטופל  
            Hospitalizations h = ModelDB.GetDB().Hospitalizations.Where(a => a.IdBurdened == BurdenedId && a.ExitDate == null).FirstOrDefault();
            if (h != null)
            {
                //שליפת המיטה שכרגע הוא מאושפז בה
                HospitalizationBeds n = h.HospitalizationBeds.Where(x => x.EndDate == null).FirstOrDefault();
                n.EndDate = DateTime.Today;//עדכון התאריך סיום 
            }
            ModelDB.GetDB().HospitalizationBeds.Add(newH);//הוספת המיטה לאישפוז למודל
            ModelDB.GetDB().SaveChanges();//שמירה ב DB
            ViewBag.note = "המאושפז הועבר למיטה " + CodeBed + " בהצלחה!";//הערה ללקוח            
            return RedirectToAction("PatiantDetailes",
              new { returnUrl = Request.UrlReferrer.ToString(), id = h.IdBurdened });
        }
    }
} 
//שליפת רשימת המיטות של המאושפז הנוכחי
            // IEnumerable<HospitalizationBeds> listBeds = ModelDB.GetDB().HospitalizationBeds.Where(x => x.CodeHospitalization == h.CodeHospitalization).ToList();
            //return View("PatiantDetailes", h.IdBurdened);
            //   return View("PatiantDetailes",h.IdBurdened.ToString());