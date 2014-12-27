using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

namespace tpscabinet
{
    public class CookieWebClient : WebClient
    {
        public CookieContainer CookieContainer { get; set; }
        public CookieWebClient()
        {
            this.CookieContainer = new CookieContainer();
        }
        protected override WebRequest GetWebRequest(Uri address)
        {
            var request = base.GetWebRequest(address) as HttpWebRequest;
            if (request == null) return base.GetWebRequest(address);
            request.CookieContainer = CookieContainer;
            request.Timeout = 10000; // 10 sec
            request.ReadWriteTimeout = 120000; // 120 sec
            request.AllowAutoRedirect = true;
            request.UserAgent = "Mozilla/5.0 (Windows; U; Windows NT 5.1; en-US) AppleWebKit/534.7 (KHTML, like Gecko) Chrome/7.0.517.41 Safari/534.7"; /// google chrome
            return request;
        }
    }

    public class Error
    {
        public string Message;
        public bool isCritical;
        public Error(string Message, bool isCritical)
        {
            this.Message = Message;
            this.isCritical = isCritical;
        }
    }

    public class Package
    {
        public string Name;
        public float Cost;
        public DateTime StartDT;
        public DateTime EndDT;
        public float TraffAll;
        public float TraffLeft;
        public float TraffUsed { 
            get { 
                return this.TraffAll - this.TraffLeft; 
            }
        }
        public bool isActive
        {
            get
            {
                return (this.StartDT < DateTime.Now && this.EndDT > DateTime.Now && this.TraffLeft > 0);
            }
        }
        public int DaysLeft
        {
            get
            {
                return (int)(this.EndDT - DateTime.Now).TotalDays;
            }
        }
        public int TotalHoursLeft
        {
            get
            {
                return (int)(this.EndDT - DateTime.Now).TotalHours;
            }
        }
        public string TimeLeftStr
        {
            get
            {
                int Days = (int)Math.Truncate((this.EndDT - DateTime.Now).TotalDays);
                int Hours = (int)Math.Truncate((this.EndDT - DateTime.Now).TotalHours) - Days * 24;
                if (Days != 0)
                    return Days + " д." + Hours + " ч.";
                else
                    return Hours + " ч.";
            }
        }

        /*public string DaysLeftPlural
        {
            get
            {
                int x = this.DaysLeft % 100;
                if (x > 20) x %= 10;
                if (x > 4)
                    return "дней";
                else
                    switch (x)
                    {
                        case 1: return "день";
                        case 0: return "дней";
                        default: return "дня";
                    }
            }
        }*/

        public Package()
        {
            this.Name = "";
            this.Cost = 0;
            this.StartDT = this.EndDT = DateTime.MinValue;
            this.TraffLeft = this.TraffAll = 0;
        }
    }

    public class CabinetScraper
    {
        public string Login { get;set; }
        public string Password { get; set; }
        public Error LastError { get; private set; }
        public List<Package> Packages { get; private set; }
        public bool HasPackage { get {return this.Packages.Count>0;}}

        public DateTime LastUpdate;
        public float TraffUsed;
        public float TraffLeft;
        public float TraffAll { get { return this.TraffLeft + this.TraffUsed; } }
        public float Balance;
        public int CabinetID;
        public float AbonentPlata;
        public float Kurs;
        
        private string PackagesHTML;
        private string CabinetHTML;
        

        public CabinetScraper()
        {
            this.Login = "";
            this.Password = "";
            LastUpdate = DateTime.MinValue;
            TraffUsed = 0;
            TraffLeft = 0;
            Balance = 0;
            CabinetID = 0;
            AbonentPlata = 0;
            Kurs = 0;
            CabinetHTML = PackagesHTML = "";
            Packages = new List<Package>();
        }

        public void ClearErrors()
        {
            if (this.LastError != null) this.LastError.Message = "";
            this.LastError = null;
        }

        public void LoadData() {
            using (CookieWebClient wc = new CookieWebClient())
            {
                ///System.Threading.Thread.Sleep(3000);
                wc.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
                NameValueCollection post_vars = new NameValueCollection();
                post_vars.Add("LoginForm[username]", this.Login);
                post_vars.Add("LoginForm[password]", this.Password);
                post_vars.Add("yt0", "submit");
                this.CabinetHTML = UTF8Encoding.UTF8.GetString(wc.UploadValues("https://cabinet.tps.uz/login", "POST", post_vars));
#if DEBUG
                System.IO.File.WriteAllText("out" + DateTime.Now.Ticks+".html", this.CabinetHTML, Encoding.UTF8);
#endif                
                if (String.IsNullOrEmpty(this.CabinetHTML)) {
                    this.LastError = new Error("Данные кабинета не получены", false);
                    return;
                }
                if (this.CabinetHTML.Contains("Вход в персональный кабинет")) {
                    this.LastError = new Error("Логин\\пароль не верны", true);
                    return;
                }
                /// if all ok ///
                if (!String.IsNullOrEmpty(this.CabinetHTML))
                {
                    float.TryParse(GetCabinetVal(@"traffic\s*:\s*'Использовано',\s*value:\s*(\d+(?:\.\d{1,2})?)\s*}"), out this.TraffUsed);
                    float.TryParse(GetCabinetVal(@"traffic\s*:\s*'Осталось',\s*value:\s*(-?\d+(?:\.\d{1,2})?)\s*}"), out this.TraffLeft);
                    if (TraffLeft < 0) TraffLeft = 0;
#if DEBUG 
                    TraffLeft = 1500;
#endif
                    float.TryParse(GetCabinetVal(@"<strong\s+class=""balance""\s+data-accid=""\d+"">\s*(-?\d+(?:\.\d{1,2})?)\s*</strong>"), out this.Balance);
                    int.TryParse(GetCabinetVal(@"/pppoe_session\?id=(\d+)"), out this.CabinetID);
                    float.TryParse(GetCabinetVal(@"<th>Статус</th>[\s\S]+?<td>\s*\$(\d+(?:\.\d{1,2})?)\s*</td>"), out this.AbonentPlata);
                    float.TryParse(GetCabinetVal(@"<p>1\s*доллар\s*США\s*=\s*(\d+(?:\.\d{1,2})?)"), out this.Kurs);
                    /// load packages page
                    if (this.CabinetID != 0)
                        this.PackagesHTML = UTF8Encoding.UTF8.GetString(wc.DownloadData("https://cabinet.tps.uz/packages/list/" + this.CabinetID));

                    if (!String.IsNullOrEmpty(this.PackagesHTML))
                    {
                        MatchCollection PackageMatches = Regex.Matches(this.PackagesHTML,
                            @"<p class=""service-header"">[\s\S]+?<span>([\s\S]+?)</span>[\s\S]+?Стоимость[\s\S]+?<p class=""col-sm-7""><strong>\$?(\d+(?:\.\d{1,2})?)[\s\S]+?Дата начала действия[\s\S]+?<p class=""col-sm-7"">([\.\s0-9:]+)</p>[\s\S]+?Дата окончания действия[\s\S]+?<p class=""col-sm-7"">([\.\s0-9:]+)</p>[\s\S]+?Трафик по пакету[\s\S]+?<p class=""col-sm-7"">(\d+(?:\.\d{1,2})?)[\s\S]+?Остаток трафика по пакету[\s\S]+?<p class=""col-sm-7"">(\d+(?:\.\d{1,2})?)[\s\S]+?",
                            RegexOptions.Compiled | RegexOptions.Multiline | RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
                        if (PackageMatches.Count > 0)
                        {
                            this.Packages.Clear();
                            foreach (Match PackangeMatch in PackageMatches)
                            {
                                if (!PackangeMatch.Success) continue;
                                Package NewPackage = new Package();
                                NewPackage.Name = PackangeMatch.Groups[1].Value.Trim();
                                float.TryParse(PackangeMatch.Groups[2].Value, out NewPackage.Cost);
                                DateTime.TryParseExact(PackangeMatch.Groups[3].Value, "dd.MM.yyyy HH:mm", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out NewPackage.StartDT);
                                DateTime.TryParseExact(PackangeMatch.Groups[4].Value, "dd.MM.yyyy HH:mm", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out NewPackage.EndDT);
                                float.TryParse(PackangeMatch.Groups[5].Value, out NewPackage.TraffAll);
                                float.TryParse(PackangeMatch.Groups[6].Value, out NewPackage.TraffLeft);
                                
#if DEBUG 
                                if (NewPackage.StartDT.Day == 22)
                                {
                                    NewPackage.StartDT = DateTime.Now.AddDays(-40);
                                    NewPackage.EndDT = DateTime.Now.AddDays(40);
                                    NewPackage.TraffAll = 3000;
                                    NewPackage.TraffLeft = 600;
                                }
#endif 

                                if (NewPackage.isActive) // add only active packages
                                    this.Packages.Add(NewPackage);
                            }
                            /// sort by start date... package with early start date in use first
                            this.Packages.Sort(delegate(Package pk1, Package pk2)
                                {
                                    return pk1.StartDT.CompareTo(pk2.StartDT);
                                }
                            );
                        }
#if DEBUG //////////////////
                        //this.Packages.Clear();    
                        StringBuilder pkStr = new StringBuilder();
                            foreach (Package pk in this.Packages)
                            {
                                pkStr.Append(pk.Cost.ToString("f2") + "\t");
                                pkStr.Append(pk.StartDT.ToString("dd.MM.yyyy HH:mm") + "\t");
                                pkStr.Append(pk.EndDT.ToString("dd.MM.yyyy HH:mm") + "\t");
                                pkStr.Append(pk.TraffAll.ToString("f2") + "\t");
                                pkStr.Append(pk.TraffLeft.ToString("f2") + "\t");
                                pkStr.Append(pk.isActive + "\t");
                                pkStr.AppendLine();
                            }
                            System.IO.File.WriteAllText("packages" + DateTime.Now.Ticks + ".txt", pkStr.ToString(), Encoding.UTF8);
#endif //////////////////
                    }
                    else
                    {
                        this.LastError = new Error("Данные по пакетам не получены", false);
                        return;
                    }

                    this.LastError = null;
                    this.LastUpdate = DateTime.Now;
                }
            }
        }

        private string GetCabinetVal(string pattern)
        {
            Match CabinetValMatch = Regex.Match(this.CabinetHTML, pattern, RegexOptions.Compiled | RegexOptions.Multiline | RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
            if (CabinetValMatch.Success)
                return CabinetValMatch.Groups[1].Value;
            else
                return "";
        }
    }
}
