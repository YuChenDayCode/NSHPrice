using NSH_Price.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NSH_Price.Controllers
{
    public class PriceController : Controller
    {
        // GET: Price
        public ActionResult Index()
        {
            return View();
        }
        //图表，有个python爬数据的做支持。。 
        public JsonResult GetArea()
        {
            List<area> list = new dbHelp().Select<area>("select Id,AreaName from area order by Id asc");
            return Json(list,JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetPriceInfo(int id)
        {
            List<priceinfo> list = new dbHelp().Select<priceinfo>("select * from priceinfo where AreaId = "+ id + "  order by CreateTime desc limit 1");
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetPriceInfoList(int id)
        {
            List<priceinfo> list = new dbHelp().Select<priceinfo>("select * from priceinfo where AreaId = " + id + " and (mod(hour(TheTime),4)=0) order by CreateTime desc limit 10");
            list = list.OrderBy(t => t.CreateTime).ToList();
            List<decimal> price = list.Select(s => s.UnitPrice).ToList();
            List<string> time = list.Select(s => s.CreateTime.ToString("MM-dd HH:mm")).ToList();
            return Json(new { AreaName = (list.Count() > 0 ? list.FirstOrDefault().AreaName:""), price = price, time= time }, JsonRequestBehavior.AllowGet);
        }
    }
}