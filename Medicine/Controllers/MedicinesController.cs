using Medicine.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Medicine.Controllers
{
    public class MedicinesController : Controller
    {
        // GET: Medicines
        public ActionResult AllMedicines()
        {
            return View(ModelDB.GetDB().Medicines.ToList());
        }
        public ActionResult AddMedicines()
        {
            var medicines = ModelDB.GetDB().MedicinesKind.ToList();//רשימת כל התרופות
            ViewBag.MyMedi = new SelectList(medicines, "CodeKind", "NameKind");
            return View();
        }
        //public ActionResult AddMedicines()
        //{
        //    int code = ModelDB.GetDB().Medicines.ToList().Last().CodeMedicine + 1;
        //    Departments newM = new Medicines() { CodeMedicine = code ,CodeKind};
        //    return View(newM);
        //}     
        [HttpPost]//פונקציה להוספת תרופה
        public ActionResult AddMedicines(Medicines medicine)
        {
            if (ModelState.IsValid)
            {
                //בדיקת תקינות
                Medicines med = ModelDB.GetDB().Medicines.Where(a => a.MedicineName == medicine.MedicineName).FirstOrDefault();
                if (med != null)
                {
                    ModelState.AddModelError("MedicineName", "תרופה קיימת במערכת");
                    ViewBag.MyMedi = new SelectList(ModelDB.GetDB().MedicinesKind.ToList(), "CodeKind", "NameKind");
                    return View("AddMedicines");
                }
                ModelDB.GetDB().Medicines.Add(medicine);
                ModelDB.GetDB().SaveChanges();
                return RedirectToAction("AllMedicines");
            }
            var medicines = ModelDB.GetDB().MedicinesKind.ToList();//רשימת כל התרופות
            ViewBag.MyMedi = new SelectList(medicines, "CodeKind", "NameKind");
            return View("AddMedicines");
        }
        public ActionResult GivingMedicines()
        {
            //שמירת המחלקות והחדרים
            var d = new SelectList(ModelDB.GetDB().Departments, "CodeDepartment", "DepartmentName");
            ViewBag.departments = d;
            var r = new SelectList(ModelDB.GetDB().Rooms, "CodeRoom", "CodeRoom");
            ViewBag.rooms = r;
            return View();
        }
        [HttpPost]//הפונקציה שמציגה את יומן השעות למיטה
        public PartialViewResult GetSchedual(int BedId)
        {
            //רשימת שעות לתרופה - המודל
            List<HourForMedicine> hm = new List<HourForMedicine>();
            //המיטה שנבחרה
            var Bed = ModelDB.GetDB().Beds.Single(d => d.CodeBed == BedId);
            ////בדיקה האם המיטה שנבחרה מאושפזים בה כעת
            //HospitalizationBeds checkUsu = ModelDB.GetDB().HospitalizationBeds.Where(x => x.CodeBed == Bed.CodeBed && x.EndDate == null).FirstOrDefault();
            if (Bed.Active)
            { 
                //המאושפז שנמצא במיטה
                HospitalizationBeds current = Bed.HospitalizationBeds.Single(b => b.EndDate == null);
                if (current != null)
                {
                    //קוד האישפוז
                    int hospitalizationId = current.CodeHospitalization;
                    //תרופות לאישפוז
                    List<int> medicineToHospitalization = ModelDB.GetDB().HospitalizationMedicine
                        .Where(m => m.CodeHospitalizatin == hospitalizationId).Select(c => c.CodeHospitalizationMedicine).ToList();
                    //שעות לתרופה, שהתרופה שלהם נמצאת ברשימת התרופות לאשפוז והתאריך של היום...
                    hm = ModelDB.GetDB().HourForMedicine.Where(h => medicineToHospitalization.Contains(h.CodeMedicineToHospitalize) &&
                    h.BeginDate <= DateTime.Today && h.EndDate >= DateTime.Today).ToList();
                }
            }
            return PartialView("schedual", hm);
        }
        //פונקציה לבחירת מיטה המחזירה רק את המיטות הפעילות
        [HttpGet]
        public ActionResult GetBeds(int roomId)
        {
            List<Beds> listBeds = ModelDB.GetDB().Beds.Where(x => x.CodeRoom == roomId).ToList();
            List<Beds> s = new List<Beds>();
            foreach (var item in listBeds)
            {
                //בדיקה אם כרגע מאושפזים במיטה זו
               HospitalizationBeds m = ModelDB.GetDB().HospitalizationBeds.Where(x => x.CodeBed == item.CodeBed && x.EndDate == null).FirstOrDefault();
                if (m != null)//אם קיים כאלה
                {
                    //Beds current = ModelDB.GetDB().Beds.Where(x => x.CodeBed == m.CodeBed).FirstOrDefault();
                    s.Add(item);
                }
                //else
                //{
                //    //שליפת המיטות שעדיין אף פעם לא התאשפזו בהם
                //    HospitalizationBeds k = ModelDB.GetDB().HospitalizationBeds.Where(x => x.CodeBed == item.CodeBed).FirstOrDefault();
                //    if (k == null)//אם קיים כאלה
                //        s.Add(item);//תוסיף אותם לרשימת המיטות לאישפוז
                //}

            }
            var beds2 = s.Select(b => new { Text = b.CodeBed, Value = b.CodeBed });
            return Json(beds2, JsonRequestBehavior.AllowGet);
        }
        //פונקציה לעדכון הלקיחת תרופה
        [HttpGet]
        public ActionResult AddTakeMedicine(int codeHourForMedicine, string dosage)
        {
            string idGiving = Session["id"].ToString();
            TakeMedicine t = new TakeMedicine() { CodeHourForMedicine = codeHourForMedicine,
                TimeTaking = DateTime.Today, Dosage = dosage, IdGivingMedicine = idGiving };
            //הוספת התרופה ב DB
            ModelDB.GetDB().TakeMedicine.Add(t);
            //שמירת הנתונים ב DB
            ModelDB.GetDB().SaveChanges();
            return Json(1, JsonRequestBehavior.AllowGet);
        }

    }
}