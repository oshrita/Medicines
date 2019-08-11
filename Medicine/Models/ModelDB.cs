using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Medicine.Models
{
    public class ModelDB
    {
        private static MedicinesEntities DB;
        public static MedicinesEntities GetDB()
        {
            if (DB== null)
            {
                DB = new MedicinesEntities();

            }
            return DB;
        }
    }
}