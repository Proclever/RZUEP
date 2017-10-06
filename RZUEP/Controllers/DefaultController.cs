using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RZUEP.Models;
using HtmlAgilityPack;
using System.IO;

namespace RZUEP.Controllers
{
    public class DefaultController : Controller
    {
        private RZUEPDefaultModel db = new RZUEPDefaultModel();
        // GET: Default
        public ActionResult Index()
        {
            foreach (var c in Request.Cookies.AllKeys.ToList().Where(x => x.Contains("Plan_")).ToList())
            {
                var id = int.Parse(c.Split('.')[1]);
                bool isnull = false;
                var test = db.Proprowadzacies.Find(id);
                if (c[5] == 'p')
                {
                    if (db.Proprowadzacies.Find(id) == null) isnull = true;
                }
                else
                {
                    if (db.Plans.Find(id) == null) isnull = true;
                }
                if (Request.Cookies[c] != null && isnull)
                {
                    var plan = new HttpCookie(c)
                    {
                        Expires = DateTime.Now.AddMonths(-5),
                        Value = null
                    };
                    Response.Cookies.Add(plan);
                }
            }
            return View(Request.Cookies.AllKeys.ToList().Where(x => x.Contains("Plan_")).ToList());
        }
        public ActionResult Info(string returnurl = null, bool mobileinfo = false)
        {
            if(mobileinfo)
            {
                HttpCookie cookie = new HttpCookie("closemobileinfo");
                cookie.Expires = DateTime.Now.AddMonths(12);
                cookie.Value = "true";
                Response.Cookies.Add(cookie);
            }
            ViewBag.returnurl = returnurl;
            return View();
        }
        public ActionResult FAQ(string returnurl = null)
        {
            ViewBag.returnurl = returnurl;
            return View();
        }
        public ActionResult Pracownicy()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Pracownicy(int planid, bool mojplan)
        {
            if (mojplan)
            {
                HttpCookie cookie = new HttpCookie("Plan_p." + planid.ToString());
                cookie.Expires = DateTime.Now.AddMonths(5);
                cookie.Value = null;
                Response.Cookies.Add(cookie);
            }
            return RedirectToAction("Pracownik", new { id = planid });
        }
        public string GetProsemestr()
        {
            var prosemestr = db.Proprowadzacies.Select(x => x.semestr).Distinct().AsEnumerable().Reverse().ToList();
            string html = "";
            foreach (var p in prosemestr)
            {
                html += "<option value=\"" + p + "\">" + p.Replace("Rozkład zajęć w roku akademickim ", "") + "</option>" + Environment.NewLine;
            }
            return html;
        }
        public string GetProwydzial(string semestr)
        {
            var prowydzialy = db.Proprowadzacies.Where(x => x.semestr == semestr).Select(x => x.wydzial).Distinct().ToList();
            string html = "";
            foreach (var p in prowydzialy)
            {
                html += "<option value=\"" + p + "\">" + p + "</option>" + Environment.NewLine;
            }
            return html;
        }
        public string GetProjednostka(string semestr, string wydzial)
        {
            var projednostki = db.Proprowadzacies.Where(x => x.semestr == semestr && x.wydzial == wydzial).Select(x => x.jednostka).Distinct().ToList();
            string html = "";
            foreach (var p in projednostki)
            {
                html += "<option value=\"" + p + "\">" + p + "</option>" + Environment.NewLine;
            }
            return html;
        }
        public string GetPronazwa(string semestr, string wydzial, string jednostka)
        {
            var pronazwa = db.Proprowadzacies.Where(x => x.semestr == semestr && x.wydzial == wydzial && x.jednostka == jednostka).Select(x => x.nazwa).ToList();
            string html = "";
            foreach (var p in pronazwa)
            {
                html += "<option value=\"" + p + "\">" + p + "</option>" + Environment.NewLine;
            }
            return html;
        }
        public int GetProPlanid(string semestr, string wydzial, string jednostka, string nazwa)
        {
            return db.Proprowadzacies.Where(x => x.semestr == semestr && x.wydzial == wydzial && x.jednostka == jednostka && x.nazwa == nazwa).First().id;
        }

        public ActionResult Pracownik(int id)
        {
            var p = db.Proprowadzacies.Find(id);
            var sem = p.semestr.Replace("Rozkład zajęć w roku akademickim ", "").Split('/')[0];
            var stronajednostki = "";
            var dziekanatwydziału = "";
            var aktualnosciwydzialu = "";
            var sylabus = "http://www.e-sylabus.ue.poznan.pl/pl/studia/step/study_level_configuration/" + sem + "/S";
            switch (p.wydzial)
            {
                case "Ekonomii":
                    dziekanatwydziału = "http://ue.poznan.pl/pl/uniwersytet,c13/wydzialy,c18/wydzial-ekonomii,c19/dziekanat,a573.html";
                    aktualnosciwydzialu = "http://ue.poznan.pl/pl/uniwersytet,c13/wydzialy,c18/wydzial-ekonomii,c19/aktualnosci,c642/";
                    break;
                case "Gospodarki Międzynarodowej":
                    dziekanatwydziału = "http://ue.poznan.pl/pl/uniwersytet,c13/wydzialy,c18/wydzial-gospodarki-miedzynarodowej,c21/dziekanat,a1456.html";
                    aktualnosciwydzialu = "http://ue.poznan.pl/pl/uniwersytet,c13/wydzialy,c18/wydzial-gospodarki-miedzynarodowej,c21/aktualnosci,c656/";
                    break;
                case "Informatyki i Gospodarki Elektronicznej":
                    dziekanatwydziału = "http://ue.poznan.pl/pl/uniwersytet,c13/wydzialy,c18/wydzial-informatyki-i-gospodarki-elektronicznej,c22/dziekanat,a1480.html";
                    aktualnosciwydzialu = "http://ue.poznan.pl/pl/uniwersytet,c13/wydzialy,c18/wydzial-informatyki-i-gospodarki-elektronicznej,c22/aktualnosci,c657/";
                    break;
                case "Towaroznawstwa":
                    dziekanatwydziału = "http://ue.poznan.pl/pl/uniwersytet,c13/wydzialy,c18/wydzial-towaroznawstwa,c23/dziekanat,c2327/";
                    aktualnosciwydzialu = "http://ue.poznan.pl/pl/uniwersytet,c13/wydzialy,c18/wydzial-towaroznawstwa,c23/aktualnosci,c707/";
                    break;
                case "Zarządzania":
                    dziekanatwydziału = "http://ue.poznan.pl/pl/uniwersytet,c13/wydzialy,c18/wydzial-zarzadzania,c20/dziekanat,a3060.html";
                    aktualnosciwydzialu = "http://ue.poznan.pl/pl/uniwersytet,c13/wydzialy,c18/wydzial-zarzadzania,c20/aktualnosci,c655/";
                    break;
                case "Inne Jednostki":
                    sylabus = "http://www.e-sylabus.ue.poznan.pl/pl/studia/step/study_level_configuration/" + sem + "/S";
                    if (p.jednostka == "Studium prawa") stronajednostki = "http://ue.poznan.pl/pl/uniwersytet,c13/wydzialy,c18/inne-jednostki-dydaktyczne,c1435/studium-prawa,c3278/";
                    else
                    {
                        if (p.jednostka == "Studium Wychowania Fizycznego i sportu") stronajednostki = "http://ue.poznan.pl/pl/zycie-studenckie,c24/sport,c2135/studium-wychowania-fizycznego-i-sportu,c2099/";
                        else
                        {
                            if (p.jednostka.Contains("SPNJO")) stronajednostki = "http://ue.poznan.pl/pl/oferta-edukacyjna,c3/studium-praktycznej-nauki-jezykow-obcych,c386/";
                            else
                            {
                                stronajednostki = null;
                            }
                        }
                    }
                    break;
                default:
                    break;
            }
            if (stronajednostki == "")
            {
                HtmlWeb website = new HtmlWeb();
                //website.OverrideEncoding = Encoding.GetEncoding(1250);
                var inner = website.Load(aktualnosciwydzialu.Split(new string[] { "aktualnosci" }, StringSplitOptions.None)[0]).DocumentNode.InnerHtml;
                StringWriter myWriter = new StringWriter();
                HttpUtility.HtmlDecode(inner, myWriter);
                var clearinner = myWriter.ToString();
                var doc = new HtmlDocument();
                doc.LoadHtml(clearinner);
                foreach (HtmlNode li in doc.DocumentNode.SelectSingleNode("/html/body/div[3]/section[5]/div/ul").SelectNodes("li"))
                {
                    var lijednostka = li.SelectSingleNode("a").GetAttributeValue("title", null);
                    if (lijednostka != "" && lijednostka != null)
                    {
                        while (lijednostka.Contains("  ")) lijednostka = lijednostka.Replace("  ", " ");
                        while (lijednostka[0] == ' ') lijednostka = lijednostka.Substring(1);
                        while (lijednostka[lijednostka.Length - 1] == ' ') lijednostka = lijednostka.Remove(lijednostka.Length - 1);
                    }
                    if (lijednostka == p.jednostka)
                    {
                        stronajednostki = li.SelectSingleNode("a").GetAttributeValue("href", null);
                    }
                }
            }
            ViewBag.stronajednostki = stronajednostki;
            ViewBag.dziekanatwydziału = dziekanatwydziału;
            ViewBag.aktualnosciwydzialu = aktualnosciwydzialu;
            ViewBag.sylabus = sylabus;
            ViewBag.aktualizacja = GlobalVariables.Aktualizacja;
            return View(db.Proprowadzacies.Find(id));
        }

        public ActionResult PlanPracownika(int id, bool? onlyblock = null, int? idtohide = null, bool unhide = false)
        {
            var pracownik = db.Proprowadzacies.Find(id);
            if (pracownik.Prozajecias.Count == 0) RedirectToAction("Pracownik", new { id = id });
            List<string> timelist = new List<string>();
            if(idtohide!=null)
            {
                HttpCookie cookie = new HttpCookie("hide.p." + id.ToString() + "." + idtohide.ToString());
                cookie.Expires = DateTime.Now.AddMonths(5);
                cookie.Value = null;
                Response.Cookies.Add(cookie);
            }
            if(unhide)
            {
                foreach(var h in Request.Cookies.AllKeys.ToList().Where(x => x.Contains("hide.p.")).ToList())
                {
                    if (h.Split('.')[2] != id.ToString()) continue;
                    else
                    {
                        if (Request.Cookies[h] != null)
                        {
                            var c = new HttpCookie(h)
                            {
                                Expires = DateTime.Now.AddDays(-1),
                                Value = null
                            };
                            Response.Cookies.Add(c);
                        }
                    }
                }
                ViewBag.unhide = true;
            }
            ViewBag.onlyblock = null;
            if (onlyblock != null)
            {
                if (onlyblock.Value)
                {
                    HttpCookie cookie = new HttpCookie("onlyblock.p." + id.ToString());
                    cookie.Expires = DateTime.Now.AddMonths(5);
                    cookie.Value = null;
                    Response.Cookies.Add(cookie);
                }
                else
                {
                    if (Request.Cookies["onlyblock.p." + id.ToString()] != null)
                    {
                        var ob = new HttpCookie("onlyblock.p." + id.ToString())
                        {
                            Expires = DateTime.Now.AddDays(-1),
                            Value = null
                        };
                        Response.Cookies.Add(ob);
                        ViewBag.onlyblock = false;
                    }
                }
            }
            
            if (ViewBag.onlyblock == null)
            {
                if(Request.Cookies.AllKeys.ToList().Where(x => x.Equals("onlyblock.p." + id.ToString())).Count() > 0) ViewBag.onlyblock = true;
                else ViewBag.onlyblock = false;
            }
            bool normal = true;
            int maxzajecias = 0;
            for (int j = 0; j < 5; j++)
            {
                if (pracownik.Prozajecias.Where(x => x.dzien == j).OrderBy(x => x.godzinaod).ToList().Count > maxzajecias) maxzajecias = pracownik.Prozajecias.Where(x => x.dzien == j).OrderBy(x => x.godzinaod).ToList().Count;
                Prozajecias poprzednie = null;
                foreach (var z in pracownik.Prozajecias.Where(x => x.dzien == j).OrderBy(x => x.godzinaod).ToList())
                {
                    if (Request.Cookies.AllKeys.ToList().Where(x => x.Equals("hide.p." + z.proprowadzacyid.ToString() + "." + z.id.ToString())).Count() > 0 && ViewBag.unhide == null) { continue; }
                    if (poprzednie == null) poprzednie = z;
                    else
                    {
                        if (new TimeSpan(int.Parse(poprzednie.godzinado.Split(':')[0]), int.Parse(poprzednie.godzinado.Split(':')[1]), 0) > new TimeSpan(int.Parse(z.godzinaod.Split(':')[0]), int.Parse(z.godzinaod.Split(':')[1]), 0))
                        {
                            normal = false;
                            break;
                        }
                        poprzednie = z;
                    }
                }
            }
            if (!normal || ViewBag.onlyblock)
            {
                ViewBag.height = ((110 * maxzajecias) - 10).ToString();
                goto end;
            }
            try
            {
                var hod = db.Proprowadzacies.Find(id).Prozajecias.OrderBy(x => x.godzinaod).First().godzinaod.Split(':')[0];
                var hdo = db.Proprowadzacies.Find(id).Prozajecias.OrderBy(x => x.godzinado).Last().godzinado.Split(':')[0];
                var mdo = db.Proprowadzacies.Find(id).Prozajecias.OrderBy(x => x.godzinado).Last().godzinado.Split(':')[1];
                timelist = new List<string> { hod + ":00" };
                var i = int.Parse(hod);
                while (i < int.Parse(hdo))
                {
                    timelist.Add((i.ToString().Length == 1 ? "0" + i.ToString() : i.ToString()) + ":30");
                    timelist.Add(((i + 1).ToString().Length == 1 ? "0" + (i + 1).ToString() : (i + 1).ToString()) + ":00");
                    i++;
                }
                if (mdo != "00")
                {
                    timelist.Add((i.ToString().Length == 1 ? "0" + i.ToString() : i.ToString()) + ":30");
                    timelist.Add(((i + 1).ToString().Length == 1 ? "0" + (i + 1).ToString() : (i + 1).ToString()) + ":00");
                }
            }
            catch { }
            end:
            ViewBag.timelist = timelist;
            ViewBag.normal = normal;
            
            ViewBag.aktualizacja = GlobalVariables.Aktualizacja;
            return View(pracownik);
        }

        public ActionResult Pszczegoly(int id)
        {
            return View(db.Prozajecias.Find(id));
        }

        public ActionResult Studenci()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Studenci(int planid, bool mojplan)
        {
            if (mojplan)
            {
                HttpCookie cookie = new HttpCookie("Plan_s." + planid.ToString());
                cookie.Expires = DateTime.Now.AddMonths(5);
                cookie.Value = null;
                Response.Cookies.Add(cookie);
            }
            return RedirectToAction("Student", new { id = planid });
        }
        public string GetSemestr()
        {
            var semestr = db.Plans.Select(x => x.semestr).Distinct().AsEnumerable().Reverse().ToList();
            string html = "";
            foreach (var p in semestr)
            {
                html += "<option value=\"" + p + "\">" + p.Replace("Rozkład zajęć w roku akademickim ", "") + "</option>" + Environment.NewLine;
            }
            return html;
        }
        public string GetStopien(string semestr)
        {
            var stopien = db.Plans.Where(x => x.semestr == semestr).Select(x => x.stopien).Distinct().ToList();
            string html = "";
            foreach (var p in stopien)
            {
                html += "<option value=\"" + p + "\">" + p + "</option>" + Environment.NewLine;
            }
            return html;
        }
        public string GetRok(string semestr, string stopien)
        {
            var rok = db.Plans.Where(x => x.semestr == semestr && x.stopien == stopien).Select(x => x.rok).Distinct().ToList();
            string html = "";
            foreach (var p in rok)
            {
                html += "<option value=\"" + p + "\">" + p + "</option>" + Environment.NewLine;
            }
            return html;
        }
        public string GetWydzial(string semestr, string stopien, string rok)
        {
            var wydzialy = db.Plans.Where(x => x.semestr == semestr && x.stopien == stopien && x.rok == rok).Select(x => x.wydzial).Distinct().ToList();
            string html = "";
            foreach (var p in wydzialy)
            {
                html += "<option value=\"" + p + "\">" + p + "</option>" + Environment.NewLine;
            }
            return html;
        }
        public string GetKierunek(string semestr, string stopien, string rok, string wydzial)
        {
            var kierunek = db.Plans.Where(x => x.semestr == semestr && x.stopien == stopien && x.rok == rok && x.wydzial == wydzial).Select(x => x.kierunek).Distinct().ToList();
            string html = "";
            foreach (var p in kierunek)
            {
                html += "<option value=\"" + p + "\">" + p + "</option>" + Environment.NewLine;
            }
            return html;
        }
        public string GetGrupa(string semestr, string stopien, string rok, string wydzial, string kierunek)
        {
            var grupa = db.Plans.Where(x => x.semestr == semestr && x.stopien == stopien && x.rok == rok && x.wydzial == wydzial && x.kierunek == kierunek).ToList();
            string html = "";
            foreach (var p in grupa)
            {
                html += "<option value=\"" + p.grupa + "\">" + p.grupa + (p.specjalnosc!=null&&p.specjalnosc!=""?(" ("+p.specjalnosc+")"):"") + "</option>" + Environment.NewLine;
            }
            return html;
        }
        public int GetPlanid(string semestr, string stopien, string rok, string wydzial, string kierunek, string grupa)
        {
            return db.Plans.Where(x => x.semestr == semestr && x.stopien == stopien && x.rok == rok && x.wydzial == wydzial && x.kierunek == kierunek && x.grupa == grupa).First().id;
        }
        public ActionResult Student(int id)
        {
            var p = db.Plans.Find(id);
            var sem = p.semestr.Replace("Rozkład zajęć w roku akademickim ", "").Split('/')[0];
            var sto = p.stopien[1] == '.' ? "1" : "2";
            var dziekanatwydziału = "";
            var aktualnosciwydzialu = "";
            var sylabus = "";
            switch (p.wydzial)
            {
                case "Ekonomii":
                    dziekanatwydziału = "http://ue.poznan.pl/pl/uniwersytet,c13/wydzialy,c18/wydzial-ekonomii,c19/dziekanat,a573.html";
                    aktualnosciwydzialu = "http://ue.poznan.pl/pl/uniwersytet,c13/wydzialy,c18/wydzial-ekonomii,c19/aktualnosci,c642/";
                    sylabus = "http://www.e-sylabus.ue.poznan.pl/pl/studia/step/curriculum_configuration/" + sem + "/S/" + sto + "/WE";
                    break;
                case "Gospodarki Międzynarodowej":
                    dziekanatwydziału = "http://ue.poznan.pl/pl/uniwersytet,c13/wydzialy,c18/wydzial-gospodarki-miedzynarodowej,c21/dziekanat,a1456.html";
                    aktualnosciwydzialu = "http://ue.poznan.pl/pl/uniwersytet,c13/wydzialy,c18/wydzial-gospodarki-miedzynarodowej,c21/aktualnosci,c656/";
                    sylabus = "http://www.e-sylabus.ue.poznan.pl/pl/studia/step/curriculum_configuration/" + sem + "/S/" + sto + "/WGM";
                    break;
                case "Informatyki i Gospodarki Elektronicznej":
                    dziekanatwydziału = "http://ue.poznan.pl/pl/uniwersytet,c13/wydzialy,c18/wydzial-informatyki-i-gospodarki-elektronicznej,c22/dziekanat,a1480.html";
                    aktualnosciwydzialu = "http://ue.poznan.pl/pl/uniwersytet,c13/wydzialy,c18/wydzial-informatyki-i-gospodarki-elektronicznej,c22/aktualnosci,c657/";
                    sylabus = "http://www.e-sylabus.ue.poznan.pl/pl/studia/step/curriculum_configuration/" + sem + "/S/" + sto + "/WIGE";
                    break;
                case "Towaroznawstwa":
                    dziekanatwydziału = "http://ue.poznan.pl/pl/uniwersytet,c13/wydzialy,c18/wydzial-towaroznawstwa,c23/dziekanat,c2327/";
                    aktualnosciwydzialu = "http://ue.poznan.pl/pl/uniwersytet,c13/wydzialy,c18/wydzial-towaroznawstwa,c23/aktualnosci,c707/";
                    sylabus = "http://www.e-sylabus.ue.poznan.pl/pl/studia/step/study_level_configuration/" + sem + "/S";
                    break;
                case "Zarządzania":
                    dziekanatwydziału = "http://ue.poznan.pl/pl/uniwersytet,c13/wydzialy,c18/wydzial-zarzadzania,c20/dziekanat,a3060.html";
                    aktualnosciwydzialu = "http://ue.poznan.pl/pl/uniwersytet,c13/wydzialy,c18/wydzial-zarzadzania,c20/aktualnosci,c655/";
                    sylabus = "http://www.e-sylabus.ue.poznan.pl/pl/studia/step/curriculum_configuration/" + sem + "/S/" + sto + "/WZ";
                    break;
                default:
                    break;
            }
            ViewBag.dziekanatwydziału = dziekanatwydziału;
            ViewBag.aktualnosciwydzialu = aktualnosciwydzialu;
            ViewBag.sylabus = sylabus;
            ViewBag.aktualizacja = GlobalVariables.Aktualizacja;
            return View(db.Plans.Find(id));
        }

        public ActionResult PlanStudenta(int id, bool? onlyblock = null, int? idtohide = null, bool unhide = false)
        {
            var student = db.Plans.Find(id);
            if (student.Zajecias.Count == 0) RedirectToAction("Student", new { id = id });
            List<string> timelist = new List<string>();
            if (idtohide != null)
            {
                HttpCookie cookie = new HttpCookie("hide.s." + id.ToString() + "." + idtohide.ToString());
                cookie.Expires = DateTime.Now.AddMonths(5);
                cookie.Value = null;
                Response.Cookies.Add(cookie);
            }
            if (unhide)
            {
                foreach (var h in Request.Cookies.AllKeys.ToList().Where(x => x.Contains("hide.s.")).ToList())
                {
                    if (h.Split('.')[2] != id.ToString()) continue;
                    else
                    {
                        if (Request.Cookies[h] != null)
                        {
                            var c = new HttpCookie(h)
                            {
                                Expires = DateTime.Now.AddDays(-1),
                                Value = null
                            };
                            Response.Cookies.Add(c);
                        }
                    }
                }
                ViewBag.unhide = true;
            }
            bool normal = true;
            ViewBag.onlyblock = null;
            if (onlyblock != null)
            {
                if (onlyblock.Value)
                {
                    HttpCookie cookie = new HttpCookie("onlyblock.s." + id.ToString());
                    cookie.Expires = DateTime.Now.AddMonths(12);
                    cookie.Value = null;
                    Response.Cookies.Add(cookie);
                }
                else
                {
                    if (Request.Cookies["onlyblock.s." + id.ToString()] != null)
                    {
                        var ob = new HttpCookie("onlyblock.s." + id.ToString())
                        {
                            Expires = DateTime.Now.AddDays(-1),
                            Value = null
                        };
                        Response.Cookies.Add(ob);
                        ViewBag.onlyblock = false;
                    }
                }
            }
            if (ViewBag.onlyblock == null)
            {
                if(Request.Cookies.AllKeys.ToList().Where(x => x.Equals("onlyblock.s." + id.ToString())).Count() > 0) ViewBag.onlyblock = true;
                else ViewBag.onlyblock = false;
            }
            int maxzajecias = 0;
            for (int j = 0; j < 5; j++)
            {
                if (student.Zajecias.Where(x => x.dzien == j).OrderBy(x => x.godzinaod).ToList().Count > maxzajecias) maxzajecias = student.Zajecias.Where(x => x.dzien == j).OrderBy(x => x.godzinaod).ToList().Count;
                Zajecias poprzednie = null;
                foreach (var z in student.Zajecias.Where(x => x.dzien == j).OrderBy(x => x.godzinaod).ToList())
                {
                    if (Request.Cookies.AllKeys.ToList().Where(x => x.Equals("hide.s." + z.planid.ToString() + "." + z.id.ToString())).Count() > 0 && ViewBag.unhide == null) { continue; }
                    if (poprzednie == null) poprzednie = z;
                    else
                    {
                        if (new TimeSpan(int.Parse(poprzednie.godzinado.Split(':')[0]), int.Parse(poprzednie.godzinado.Split(':')[1]), 0) > new TimeSpan(int.Parse(z.godzinaod.Split(':')[0]), int.Parse(z.godzinaod.Split(':')[1]), 0))
                        {
                            normal = false;
                            break;
                        }
                        poprzednie = z;
                    }
                }
            }
            if (!normal || ViewBag.onlyblock)
            {
                ViewBag.height = ((110 * maxzajecias) - 10).ToString();
                goto end;
            }
            try
            {
                var hod = db.Plans.Find(id).Zajecias.OrderBy(x => x.godzinaod).First().godzinaod.Split(':')[0];
                var hdo = db.Plans.Find(id).Zajecias.OrderBy(x => x.godzinado).Last().godzinado.Split(':')[0];
                var mdo = db.Plans.Find(id).Zajecias.OrderBy(x => x.godzinado).Last().godzinado.Split(':')[1];
                timelist = new List<string> { hod + ":00" };
                var i = int.Parse(hod);
                while (i < int.Parse(hdo))
                {
                    timelist.Add((i.ToString().Length == 1 ? "0" + i.ToString() : i.ToString()) + ":30");
                    timelist.Add(((i + 1).ToString().Length == 1 ? "0" + (i + 1).ToString() : (i + 1).ToString()) + ":00");
                    i++;
                }
                if (mdo != "00")
                {
                    timelist.Add((i.ToString().Length == 1 ? "0" + i.ToString() : i.ToString()) + ":30");
                    timelist.Add(((i + 1).ToString().Length == 1 ? "0" + (i + 1).ToString() : (i + 1).ToString()) + ":00");
                }
            }
            catch { }
            end:
            ViewBag.timelist = timelist;
            ViewBag.normal = normal;
            
            ViewBag.aktualizacja = GlobalVariables.Aktualizacja;
            return View(student);
        }

        public ActionResult Sszczegoly(int id)
        {
            return View(db.Zajecias.Find(id));
        }
        public ActionResult Dodaj(int id, string actionname)
        {
            var charr = actionname == "Pracownik" ? "p" : "s";
            HttpCookie cookie = new HttpCookie("Plan_" + charr + "." + id.ToString());
            cookie.Expires = DateTime.Now.AddMonths(5);
            cookie.Value = null;
            Response.Cookies.Add(cookie);
            return RedirectToAction(actionname, new { id = id });
        }
        public ActionResult Usun(string key, string actionname, int id)
        {
            if (Request.Cookies[key] != null)
            {
                var plan = new HttpCookie(key)
                {
                    Expires = DateTime.Now.AddDays(-1),
                    Value = null
                };
                Response.Cookies.Add(plan);
            }
            return RedirectToAction(actionname, new { id = id });
        }

        public string Najblizsze(bool pracownik, int id)
        {
            string obecnie = "";
            bool onormal = true;
            string nastepne = "";
            bool nnormal = true;
            var dzien = (int)DateTime.Now.AddHours(2).DayOfWeek - 1;
            var godzina = DateTime.Now.AddHours(2).TimeOfDay;
            if (pracownik)
            {
                Prozajecias obezaj = null;
                Prozajecias nastzaj = null;
                var zajecia = db.Proprowadzacies.Find(id).Prozajecias.Where(x => x.dzien == dzien).OrderBy(x => x.godzinaod).ToList();
                foreach (var z in zajecia)
                {
                    if (Request.Cookies.AllKeys.ToList().Where(x => x.Equals("hide.p." + z.proprowadzacyid.ToString() + "." + z.id.ToString())).Count() > 0) continue;
                    TimeSpan god = new TimeSpan(int.Parse(z.godzinaod.Split(':')[0]), int.Parse(z.godzinaod.Split(':')[1]), 0);
                    TimeSpan gdo = new TimeSpan(int.Parse(z.godzinado.Split(':')[0]), int.Parse(z.godzinado.Split(':')[1]), 0);
                    if (int.Parse(god.TotalMinutes.ToString("0")) == int.Parse(godzina.TotalMinutes.ToString("0")))
                    {
                        obecnie = "Rozpoczyna" + (z.rodzaj.ToLower()[z.rodzaj.ToLower().Length - 1] == 'a' ? "ją" : "") + " się " + z.rodzaj.ToLower() + " " + z.nazwa + (z.sala.Contains("armonogram") ? "" : (" w sali " + z.sala.Replace("Sala ", "").Replace("sala ", "")));
                        obezaj = z;
                        if (obezaj.info != "" && obezaj.info != null) onormal = false;
                        continue;
                    }
                    if (int.Parse(godzina.TotalMinutes.ToString("0")) > int.Parse(god.TotalMinutes.ToString("0")) && int.Parse(godzina.TotalMinutes.ToString("0")) < int.Parse(gdo.TotalMinutes.ToString("0")))
                    {
                        obecnie = "Trwa" + (z.rodzaj.ToLower()[z.rodzaj.ToLower().Length - 1] == 'a' ? "ją " : " ") + z.rodzaj.ToLower() + " " + z.nazwa + (z.sala.Contains("armonogram") ? "" : (" w sali " + z.sala.Replace("Sala ", "").Replace("sala ", "")) + " ");
                        if ((godzina - god).TotalMinutes < (gdo - godzina).TotalMinutes) obecnie += " (od " + (godzina - god).TotalMinutes.ToString("0") + " min.)";
                        else obecnie += " (jeszcze " + (gdo - godzina).TotalMinutes.ToString("0") + " min.)";
                        obezaj = z;
                        if (obezaj.info != "" && obezaj.info != null) onormal = false;
                        continue;
                    }
                    if (nastzaj == null && int.Parse(god.TotalMinutes.ToString("0")) > int.Parse(godzina.TotalMinutes.ToString("0")))
                    {
                        nastzaj = z;
                        break;
                    }
                    if(obezaj != null && (god < new TimeSpan(int.Parse(obezaj.godzinado.Split(':')[0]), int.Parse(obezaj.godzinado.Split(':')[1]), 0)))
                    {
                        onormal = false;
                    }
                }
                if(nastzaj == null)
                {
                    var dzien2 = (dzien + 1)%5;
                    while(nastzaj==null)
                    {
                        var zaj = db.Proprowadzacies.Find(id).Prozajecias.Where(x => x.dzien == dzien2).OrderBy(x => x.godzinaod).ToList();
                        if (zaj.Count == 0) dzien2 = (dzien2 + 1)%5;
                        else
                        {
                            foreach(var zzz in zaj)
                            {
                                if (Request.Cookies.AllKeys.ToList().Where(x => x.Equals("hide.p." + zzz.proprowadzacyid.ToString() + "." + zzz.id.ToString())).Count() > 0) continue;
                                else
                                {
                                    nastzaj = zzz;
                                    break;
                                }
                            }
                        }
                    }
                }
                if (nastzaj != null)
                {
                    nastepne = "Następne zajęcia to " + nastzaj.rodzaj.ToLower() + " " + nastzaj.nazwa + " " + (nastzaj.sala.Contains("armonogram") ? "" : (" w sali " + nastzaj.sala.Replace("Sala ", "").Replace("sala ", ""))) + " ";
                    TimeSpan god = new TimeSpan(int.Parse(nastzaj.godzinaod.Split(':')[0]), int.Parse(nastzaj.godzinaod.Split(':')[1]), 0);           
                    if (nastzaj.dzien == dzien && god > godzina)
                    {
                        var roznica = god - godzina;   
                        if(roznica.Hours<3) nastepne += "(za " + (roznica.Hours == 0 ?" ": roznica.Hours.ToString() + " godz.") + (roznica.Minutes % 60 == 0 ? "" : " " + (roznica.Minutes%60).ToString() + " min.") + ")";
                        else nastepne += "(o godz. " + nastzaj.godzinaod + ")";
                    }
                    else
                    {
                        if (nastzaj.dzien == (dzien + 1)%5) nastepne += "(jutro o godz. " + nastzaj.godzinaod + ")";
                        else
                        {
                            if (nastzaj.dzien == (dzien + 2) % 5) nastepne += "(pojutrze o godz. " + nastzaj.godzinaod + ")";
                            else
                            {
                                var dzienstr = "";
                                switch (nastzaj.dzien)
                                {
                                    case 0:
                                        dzienstr = "pon.";
                                        break;
                                    case 1:
                                        dzienstr = "wt.";
                                        break;
                                    case 2:
                                        dzienstr = "śr.";
                                        break;
                                    case 3:
                                        dzienstr = "czw.";
                                        break;
                                    case 4:
                                        dzienstr = "pt.";
                                        break;
                                }
                                nastepne += "(" + dzienstr + " o godz. " + nastzaj.godzinaod + ")";
                            }
                        }
                    }
                }
                if (nastzaj!=null)
                {
                    if (nastzaj.info != "" && nastzaj.info != null) nnormal = false;
                    else
                    {
                        bool found = false;
                        foreach (var z in db.Proprowadzacies.Find(id).Prozajecias.Where(x => x.dzien == nastzaj.dzien).OrderBy(x => x.godzinaod).ToList())
                        {
                            if (Request.Cookies.AllKeys.ToList().Where(x => x.Equals("hide.p." + z.proprowadzacyid.ToString() + "." + z.id.ToString())).Count() > 0) continue;
                            if (found)
                            {
                                if (new TimeSpan(int.Parse(z.godzinaod.Split(':')[0]), int.Parse(z.godzinaod.Split(':')[1]), 0) < new TimeSpan(int.Parse(nastzaj.godzinado.Split(':')[0]), int.Parse(nastzaj.godzinado.Split(':')[1]), 0)) nnormal = false;
                                break;
                            }
                            if (z.godzinaod == nastzaj.godzinaod)
                            {
                                found = true;
                            }
                        }
                    }
                }
            }
            else
            {
                Zajecias nastzaj = null;
                Zajecias obezaj = null;
                var zajecia = db.Plans.Find(id).Zajecias.Where(x => x.dzien == dzien).OrderBy(x => x.godzinaod).ToList();
                foreach (var z in zajecia)
                {
                    if (Request.Cookies.AllKeys.ToList().Where(x => x.Equals("hide.s." + z.planid.ToString() + "." + z.id.ToString())).Count() > 0) continue;
                    TimeSpan god = new TimeSpan(int.Parse(z.godzinaod.Split(':')[0]), int.Parse(z.godzinaod.Split(':')[1]), 0);
                    TimeSpan gdo = new TimeSpan(int.Parse(z.godzinado.Split(':')[0]), int.Parse(z.godzinado.Split(':')[1]), 0);
                    if (int.Parse(god.TotalMinutes.ToString("0")) == int.Parse(godzina.TotalMinutes.ToString("0")))
                    {
                        obecnie = "Rozpoczyna" + (z.rodzaj.ToLower()[z.rodzaj.ToLower().Length - 1] == 'a' ? "ją" : "") + " się " + z.rodzaj.ToLower() + " " + z.nazwa + (z.sala.Contains("armonogram") ? "" : (" w sali " + z.sala.Replace("Sala ", "").Replace("sala ", "")));
                        obezaj = z;
                        if (obezaj.info != "" && obezaj.info != null) onormal = false;
                        continue;
                    }
                    if (int.Parse(godzina.TotalMinutes.ToString("0")) > int.Parse(god.TotalMinutes.ToString("0")) && int.Parse(godzina.TotalMinutes.ToString("0")) < int.Parse(gdo.TotalMinutes.ToString("0")))
                    {
                        obecnie = "Trwa" + (z.rodzaj.ToLower()[z.rodzaj.ToLower().Length-1]=='a'?"ją ": " ") + z.rodzaj.ToLower() + " " + z.nazwa + (z.sala.Contains("armonogram") ? "" : (" w sali " + z.sala.Replace("Sala ", "").Replace("sala ", "")) + " ");
                        if ((godzina - god).TotalMinutes < (gdo - godzina).TotalMinutes) obecnie += " (od " + (godzina - god).TotalMinutes.ToString("0") + " min.)";
                        else obecnie += " (jeszcze " + (gdo - godzina).TotalMinutes.ToString("0") + " min.)";
                        obezaj = z;
                        if (obezaj.info != "" && obezaj.info != null) onormal = false;
                        continue;
                    }
                    if (nastzaj == null && int.Parse(god.TotalMinutes.ToString("0")) > int.Parse(godzina.TotalMinutes.ToString("0")))
                    {
                        nastzaj = z;
                        break;
                    }
                    if (obezaj != null && (god < new TimeSpan(int.Parse(obezaj.godzinado.Split(':')[0]), int.Parse(obezaj.godzinado.Split(':')[1]), 0)))
                    {
                        onormal = false;
                    }
                }
                if (nastzaj == null)
                {
                    var dzien2 = (dzien + 1) % 5;
                    while (nastzaj == null)
                    {
                        var zaj = db.Plans.Find(id).Zajecias.Where(x => x.dzien == dzien2).OrderBy(x => x.godzinaod).ToList();
                        if (zaj.Count == 0) dzien2 = (dzien2 + 1) % 5;
                        else
                        {
                            foreach (var zzz in zaj)
                            {
                                if (Request.Cookies.AllKeys.ToList().Where(x => x.Equals("hide.s." + zzz.planid.ToString() + "." + zzz.id.ToString())).Count() > 0) continue;
                                else
                                {
                                    nastzaj = zzz;
                                    break;
                                }
                            }
                        }
                    }
                }
                if (nastzaj != null)
                {
                    nastepne = "Następne zajęcia to " + nastzaj.rodzaj.ToLower() + " " + nastzaj.nazwa + " " + (nastzaj.sala.Contains("armonogram") ? "" : (" w sali " + nastzaj.sala.Replace("Sala ", "").Replace("sala ", ""))) + " ";
                    TimeSpan god = new TimeSpan(int.Parse(nastzaj.godzinaod.Split(':')[0]), int.Parse(nastzaj.godzinaod.Split(':')[1]), 0);
                    if (nastzaj.dzien == dzien && god > godzina)
                    {
                        var roznica = god - godzina;
                        if (roznica.Hours < 3) nastepne += "(za " + (roznica.Hours == 0 ? " " : roznica.Hours.ToString() + " godz.") + (roznica.Minutes % 60 == 0 ? "" : " " + (roznica.Minutes % 60).ToString() + " min.") + ")";
                        else nastepne += "(o godz. " + nastzaj.godzinaod + ")";
                    }
                    else
                    {
                        if (nastzaj.dzien == (dzien + 1) % 5) nastepne += "(jutro o godz. " + nastzaj.godzinaod + ")";
                        else
                        {
                            if (nastzaj.dzien == (dzien + 2) % 5) nastepne += "(pojutrze o godz. " + nastzaj.godzinaod + ")";
                            else
                            {
                                var dzienstr = "";
                                switch (nastzaj.dzien)
                                {
                                    case 0:
                                        dzienstr = "pon.";
                                        break;
                                    case 1:
                                        dzienstr = "wt.";
                                        break;
                                    case 2:
                                        dzienstr = "śr.";
                                        break;
                                    case 3:
                                        dzienstr = "czw.";
                                        break;
                                    case 4:
                                        dzienstr = "pt.";
                                        break;
                                }
                                nastepne += "(" + dzienstr + " o godz. " + nastzaj.godzinaod + ")";
                            }
                        }
                    }
                }
                if (nastzaj!=null)
                {
                    if (nastzaj.info != "" && nastzaj.info != null) nnormal = false;
                    else
                    {
                        bool found = false;
                        foreach (var z in db.Plans.Find(id).Zajecias.Where(x => x.dzien == nastzaj.dzien).OrderBy(x => x.godzinaod).ToList())
                        {
                            if (Request.Cookies.AllKeys.ToList().Where(x => x.Equals("hide.s." + z.planid.ToString() + "." + z.id.ToString())).Count() > 0) continue;
                            if (found)
                            {
                                if (new TimeSpan(int.Parse(z.godzinaod.Split(':')[0]), int.Parse(z.godzinaod.Split(':')[1]), 0) < new TimeSpan(int.Parse(nastzaj.godzinado.Split(':')[0]), int.Parse(nastzaj.godzinado.Split(':')[1]), 0)) nnormal = false;
                                break;
                            }
                            if (z.godzinaod == nastzaj.godzinaod)
                            {
                                found = true;
                            }
                        }
                    }
                }
            }
            var returnvalue = (obecnie == "" ? "" : "<p id=\"obecnie\" style=\"text-align:center\">" + obecnie + (onormal?"":" *") + "</p>") + "<p id=\"nastepne\" style=\"text-align:center\">" + nastepne + (nnormal?"":" *") + "</p>" + (onormal&&nnormal?"": "<p id=\"uwaga\" style=\"text-align:center;font-size:100%\">* może nie dotyczyć - szczegóły w pełnym planie zajęć</p>");
            return returnvalue;
        }
        public bool AktualizacjaZmiana (bool status, string key)
        {
            if(key == "6b6c4474-e160-40d8-acdc-ad28f8001a49")
            {
                GlobalVariables.Aktualizacja = status;
            }
            return GlobalVariables.Aktualizacja;
        }
    }
}