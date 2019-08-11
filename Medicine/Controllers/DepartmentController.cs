using Medicine.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Medicine.Controllers
{
    public class DepartmentController : Controller
    {
        // GET: Department
        public ActionResult AllDepartments()
        {
            return View(ModelDB.GetDB().Departments.ToList());
        }
         public ActionResult EditDepartment(int codeDepartment)
        {
            var Department = ModelDB.GetDB().Departments.Single(d =>d.CodeDepartment == codeDepartment);
            return View(Department);
        }
        [HttpPost]
        public ActionResult EditDepartment(Departments department)
        {
            if (ModelState.IsValid)
            {
                //פה יהיה עדכון
                return RedirectToAction("AllDepartments");
            }
            return View(department);
        }      
         public ActionResult EditRoom(int codeRoom)
        {
            var room = ModelDB.GetDB().Rooms.Single(d =>d.CodeRoom == codeRoom);
            //Session["department"] = Department;
            return View(room);
        }
        [HttpPost]
        public ActionResult EditRoom(Rooms room)
        {
            if (ModelState.IsValid)
            {
                //פה יהיה עדכון
                return RedirectToAction("AllDepartments");
            }
            return View(room);
        }
 public ActionResult AllRooms(int codeDepartment)
        {
           var Department = ModelDB.GetDB().Departments.Single(d =>d.CodeDepartment == codeDepartment);
            //Session["department"] = Department;
            return View(Department);
        }       
        public ActionResult AllBeds(int codeRooms)
        {
            var Room = ModelDB.GetDB().Rooms.Single(r => r.CodeRoom == codeRooms);
            return View(Room);
        }
        public ActionResult SetActiveState(int codeRoom )//הפוך מצב פעיל
        {
            Rooms room = ModelDB.GetDB().Rooms.Where(r => r.CodeRoom == codeRoom).Single();
            room.Active = !room.Active;
            //עדכון בבסיס הנתונים
            ModelDB.GetDB().SaveChanges();
            return RedirectToAction("AllRooms");
        }

        public ActionResult SetActiveStateBed(int codeBed)//הפוך מצב פעיל
        {
            Beds Bed = ModelDB.GetDB().Beds.Where(b => b.CodeBed == codeBed).Single();
            Bed.Active = !Bed.Active;
            //עדכון בבסיס הנתונים
            ModelDB.GetDB().SaveChanges();
            return RedirectToAction("AllRooms");
        }
        public ActionResult NewDepartment()
        {
            r = new List<Rooms>();
            int code = ModelDB.GetDB().Departments.ToList().Last().CodeDepartment + 1;
            Departments newD = new Departments() { CodeDepartment = code };
            return View(newD);
        }
        static List<Rooms> r = new List<Rooms>();
        static List<Beds> b = new List<Beds>();
        [HttpPost]
        public ActionResult NewDepartment(Departments department)
        {
            //בדיקת תקינות
            if (ModelState.IsValid)
            {
                //עדכון בDB
                ModelDB.GetDB().Departments.Add(department);
                ModelDB.GetDB().Rooms.AddRange(r);
                ModelDB.GetDB().Beds.AddRange(b);
                ModelDB.GetDB().SaveChanges();
                return RedirectToAction("AllDepartments");
            }
            //להחזיר הודעה עודכן בהצלחה
            return View(department);
        }
      
        public ActionResult AddRoom(int departmentId, int numBeds)
        {
            Rooms newRoom = new Rooms() { Active = true, CodeDepartment = departmentId, CodeRoom = int.Parse(departmentId + "" + r.Count) };
            r.Add(newRoom);
            for (int i = 0; i < numBeds; i++)
            {
                b.Add(new Beds() { CodeRoom = newRoom.CodeRoom, Active = true, CodeBed = int.Parse(newRoom.CodeRoom + "" + i) });
            }
            return Json(newRoom.CodeRoom, JsonRequestBehavior.AllowGet);
        }
    }
}