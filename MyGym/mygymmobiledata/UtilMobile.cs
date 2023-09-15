using Newtonsoft.Json;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections;
using System.Text.RegularExpressions;
using RestSharp;
using HtmlAgilityPack;
using System.IO;
using Xamarin.Forms;
using System.Globalization;
using System.Threading.Tasks;
using System.Net;
using System.Collections.ObjectModel;
using Telerik.XamarinForms.Input;

namespace mygymmobiledata
{
    [Serializable()]
    public class CustomListItemMobile
    {
        public string Value { get; set; }
        public string Text { get; set; }
    }

    [Serializable()]
    public class CustomListItemTimeMobile
    {
        public string Value { get; set; }
        public string Text { get; set; }
        public int DayOfWeek { get; set; }
        public DateTime ClassStart { get; set; }
        public DateTime FutureStart { get; set; }
        public int AllowEnroll { get; set; }
        public int AllowTrial { get; set; }
        public int AllowGuest { get; set; }
    }

    [Serializable()]
    public partial class HowHeardMobile
    {
        public int Id { get; set; }
        public int GymId { get; set; }
        public string Name { get; set; }
        public bool Default { get; set; }
        public int IdOrig { get; set; }
    }

    public static class UtilMobile
    {
        public static DateTime MaxDate = new DateTime(3000, 1, 1).Date;
        public static DateTime MinDate = new DateTime(2000, 1, 1).Date;
        public static string RestClient = "https://api.mygym.com";


        public static object CallApiPost<T>(string url)
        {
            object o = null;
            try
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                var client = new RestClient(RestClient);
                var request = new RestRequest(url, Method.Post);
                request.AddHeader("BearerToken", "9db6e193-8f43-4a32-86b8-49b3620d93da");
                request.RequestFormat = DataFormat.Json;
                var response = client.ExecuteAsync<T>(request);
                response.Wait();
                if (response.Status == TaskStatus.RanToCompletion && response.Result.Content.Contains("error") == false)
                {
                    string s = response.Result.Content.Replace("\\", "");
                    s = s.Substring(1, s.Length - 2);
                    o = JsonConvert.DeserializeObject<T>(s);
                }
            }
            catch (Exception ex)
            {
                string inner = ex.InnerException != null && string.IsNullOrEmpty(ex.InnerException.Message) == false ? ex.InnerException.Message : "";
                Xamarin.Essentials.Preferences.Set("error", "Message: An internet connection error has occurred. " + ex.Message + " Inner: " + inner + " Stack:" + ex.StackTrace);
                Xamarin.Essentials.Preferences.Set("action", "errorpage");
            }
            return o;
        }

        public static object CallApiGet<T>(string url)
        {
            object o = null;
            try
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                var client = new RestClient(RestClient);
                var request = new RestRequest(url, Method.Get);
                request.AddHeader("BearerToken", "9db6e193-8f43-4a32-86b8-49b3620d93da");
                request.RequestFormat = DataFormat.None;
                var response = client.ExecuteAsync<T>(request);
                response.Wait();
                if (response.Status == TaskStatus.RanToCompletion && response.Result.Content.Contains("error") == false)
                {
                    string s = response.Result.Content.Replace("\\", "");
                    s = s.Substring(1, s.Length - 2);
                    o = JsonConvert.DeserializeObject<T>(s);
                }
            }
            catch (Exception ex)
            {
                string inner = ex.InnerException != null && string.IsNullOrEmpty(ex.InnerException.Message) == false ? ex.InnerException.Message : "";
                Xamarin.Essentials.Preferences.Set("error", "Message: An internet connection error has occurred. " + ex.Message + " Inner: " + inner + " Stack:" + ex.StackTrace);
                Xamarin.Essentials.Preferences.Set("action", "errorpage");
            }
            return o;
        }

        public static object CallApiGetParams<T>(string url, Dictionary<string, object> ps)
        {
            object o = null;
            try
            {
                var client = new RestClient(RestClient);
                var request = new RestRequest(url, Method.Get);
                request.RequestFormat = DataFormat.Json;
                request.AddHeader("BearerToken", "9db6e193-8f43-4a32-86b8-49b3620d93da");
                foreach (string s in ps.Keys)
                {
                    request.AddParameter(s, Convert.ToString(ps[s]));
                }
                var response = client.ExecuteAsync<T>(request);
                response.Wait();
                if (response.Status == TaskStatus.RanToCompletion && response.Result.Content.Contains("error") == false)
                {
                    string s = response.Result.Content;
                    s = s.Replace("\\\\r\\\\n", "CRX");
                    s = s.Replace("\\", "");
                    s = s.Replace("CRX", "\\r\\n");
                    if (s != "")
                    {
                        s = s.Replace("<br>", "\r\n");
                        s = s.Substring(1, s.Length - 2);
                        o = JsonConvert.DeserializeObject<T>(s);
                    }
                }
            }
            catch(Exception ex)
            {
                string inner = ex.InnerException != null && string.IsNullOrEmpty(ex.InnerException.Message) == false ? ex.InnerException.Message : "";
                Xamarin.Essentials.Preferences.Set("error", "Message: An internet connection error has occurred. " + ex.Message + " Inner: " + inner + " Stack:" + ex.StackTrace);
                Xamarin.Essentials.Preferences.Set("action", "errorpage");
            }
            return o;
        }

        public static object CallApiGetParamsSignature<T>(string url, Dictionary<string, object> ps, string signature)
        {
            object o = null;
            try
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                var client = new RestClient(RestClient);
                var request = new RestRequest(url, Method.Get);
                request.AddHeader("BearerToken", "9db6e193-8f43-4a32-86b8-49b3620d93da");
                request.AddHeader("Signature", signature);
                foreach (string s in ps.Keys)
                {
                    request.AddParameter(s, Convert.ToString(ps[s]));
                }
                request.RequestFormat = DataFormat.Json;
                var response = client.ExecuteAsync<T>(request);
                response.Wait();
                if (response.Status == TaskStatus.RanToCompletion && response.Result.Content.Contains("error") == false)
                {
                    string s = response.Result.Content.Replace("\\", "");
                    s = s.Replace("<br>", "\r\n");
                    s = s.Substring(1, s.Length - 2);
                    o = JsonConvert.DeserializeObject<T>(s);
                }
            }
            catch (Exception ex)
            {
                string inner = ex.InnerException != null && string.IsNullOrEmpty(ex.InnerException.Message) == false ? ex.InnerException.Message : "";
                Xamarin.Essentials.Preferences.Set("error", "Message: An internet connection error has occurred. " + ex.Message + " Inner: " + inner + " Stack:" + ex.StackTrace);
                Xamarin.Essentials.Preferences.Set("action", "errorpage");
            }
            return o;
        }

        public static object CallApiGetParamsSignatures<T>(string url, Dictionary<string, object> ps, string signature, string signaturea, string signatureb)
        {
            object o = null;
            try
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                var client = new RestClient(RestClient);
                var request = new RestRequest(url, Method.Get);
                request.AddHeader("BearerToken", "9db6e193-8f43-4a32-86b8-49b3620d93da");
                request.AddHeader("Signature", signature);
                request.AddHeader("Signaturea", signaturea);
                request.AddHeader("Signatureb", signatureb);
                foreach (string s in ps.Keys)
                {
                    request.AddParameter(s, Convert.ToString(ps[s]));
                }
                request.RequestFormat = DataFormat.Json;
                var response = client.ExecuteAsync<T>(request);
                response.Wait();
                if (response.Status == TaskStatus.RanToCompletion && response.Result.Content.Contains("error") == false)
                {
                    string s = response.Result.Content.Replace("\\", "");
                    s = s.Replace("<br>", "\r\n");
                    s = s.Substring(1, s.Length - 2);
                    o = JsonConvert.DeserializeObject<T>(s);
                }
            }
            catch (Exception ex)
            {
                string inner = ex.InnerException != null && string.IsNullOrEmpty(ex.InnerException.Message) == false ? ex.InnerException.Message : "";
                Xamarin.Essentials.Preferences.Set("error", "Message: An internet connection error has occurred. " + ex.Message + " Inner: " + inner + " Stack:" + ex.StackTrace);
                Xamarin.Essentials.Preferences.Set("action", "errorpage");
            }
            return o;
        }

        public static object CallApiGetBody<T>(string url, string body)
        {
            object o = null;
            try
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                var client = new RestClient(RestClient);
                var request = new RestRequest(url, Method.Post);
                request.AddHeader("BearerToken", "9db6e193-8f43-4a32-86b8-49b3620d93da");
                request.AddJsonBody(body);
                request.RequestFormat = DataFormat.Json;
                var response = client.ExecuteAsync<T>(request);
                response.Wait();
                if (response.Status == TaskStatus.RanToCompletion && response.Result.Content.Contains("error") == false)
                {
                    string s = response.Result.Content.Replace("\\", "");
                    s = s.Substring(1, s.Length - 2);
                    o = JsonConvert.DeserializeObject<T>(s);
                }
            }
            catch (Exception ex)
            {
                string inner = ex.InnerException != null && string.IsNullOrEmpty(ex.InnerException.Message) == false ? ex.InnerException.Message : "";
                Xamarin.Essentials.Preferences.Set("error", "Message: An internet connection error has occurred. " + ex.Message + " Inner: " + inner + " Stack:" + ex.StackTrace);
                Xamarin.Essentials.Preferences.Set("action", "errorpage");
            }
            return o;
        }

        public static string CallApiGetParamsString(string url, Dictionary<string, object> ps)
        {
            string t = "";
            try
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                var client = new RestClient(RestClient);
                var request = new RestRequest(url, Method.Get);
                request.AddHeader("BearerToken", "9db6e193-8f43-4a32-86b8-49b3620d93da");
                foreach (string s in ps.Keys)
                {
                    request.AddParameter(s, Convert.ToString(ps[s]));
                }
                request.RequestFormat = DataFormat.None;
                var response = client.ExecuteAsync<string>(request);
                response.Wait();
                if (response.Status == TaskStatus.RanToCompletion && response.Result.Content.Contains("error") == false)
                {
                    t = response.Result.Content.Replace("\\", "");
                    t = t.Substring(1, t.Length - 2);
                }

            }
            catch (Exception ex)
            {
                string inner = ex.InnerException != null && string.IsNullOrEmpty(ex.InnerException.Message) == false ? ex.InnerException.Message : "";
                Xamarin.Essentials.Preferences.Set("error", "Message: An internet connection error has occurred. " + ex.Message + " Inner: " + inner + " Stack:" + ex.StackTrace);
                Xamarin.Essentials.Preferences.Set("action", "errorpage");
            }
            return t;
        }

        public static void CopyPropertyValues(object source, object destination)
        {
            var destProperties = destination.GetType().GetProperties();
            foreach (var sourceProperty in source.GetType().GetProperties())
            {
                foreach (var destProperty in destProperties)
                {
                    if (destProperty.Name == sourceProperty.Name && destProperty.PropertyType.IsAssignableFrom(sourceProperty.PropertyType))
                    {
                        object v = sourceProperty.GetValue(source, new object[] { });
                        if (v != null)
                        {
                            destProperty.SetValue(destination, v, new object[] { });
                        }
                        break;
                    }
                }
            }
        }

        public static bool ContainsNumeric(string str)
        {
            return str.Where(x => Char.IsDigit(x)).Any();
        }

        public static bool ContainsAlpha(string str)
        {
            return str.Where(x => Char.IsLetter(x)).Any();
        }

        public static void ReplaceNulls(object source)
        {
            foreach (var sourceProperty in source.GetType().GetProperties())
            {
                var val = sourceProperty.GetValue(source, new object[] { });
                if (sourceProperty.PropertyType == typeof(string) && val == null)
                {
                    sourceProperty.SetValue(source, "");
                }
                else if (sourceProperty.PropertyType == typeof(DateTime) && (val == null || Convert.ToDateTime(val) < new DateTime(1900, 1, 1)))
                {
                    sourceProperty.SetValue(source, UtilMobile.MaxDate);
                }
            }
        }

        public static void RemoveScriptCharactersObject(object o)
        {
            foreach (var p in o.GetType().GetProperties())
            {
                if (p.PropertyType == typeof(string))
                {
                    string val = Convert.ToString(p.GetValue(o, new object[] { }));
                    val = RemoveScriptCharacters(val, p.Name == "Email");
                    p.SetValue(o, val);
                }
            }
        }

        public static string RemoveScriptCharacters(string s, bool email = false)
        {
            string res = "";
            if (s != null)
            {
                res = s.Replace(">", "").Replace("<", "").Replace("%", "").Replace("\"", "");
                if (email == false)
                {
                    res = s.Replace(".", "").Replace("@", "");
                }
            }
            return res.Trim();
        }

        public static string RemoveSpecialCharacters(string s, bool email = false)
        {
            string res = "";
            if (s != null)
            {
                for (int x = 0; x != s.Length; x++)
                {
                    if (Char.IsLetterOrDigit(s[x]) || s[x] == '[' || s[x] == ']' || s[x] == '|')
                    {
                        res += s[x];
                    }
                }
            }
            return res.Trim();
        }


        public static string ProperCase(string s)
        {
            string res = string.Empty;
            if (string.IsNullOrEmpty(s) == false)
            {

                char[] seps = new char[2] { ' ', '-' };
                string[] a = s.Split(seps);
                ArrayList so = new ArrayList();
                int i = 0;
                for (int x = 0; x != s.Length; x++)
                {
                    if (s[x] == ' ')
                    {
                        so.Add(0);
                        i++;
                    }
                    else if (s[x] == '-')
                    {
                        so.Add(1);
                        i++;
                    }
                }
                bool first = true;
                for (int x = 0; x != a.Length; x++)
                {
                    if (first == false)
                    {
                        if (so.Count > x - 1 && (int)so[x - 1] == 0)
                        {
                            res += " ";
                        }
                        else
                        {
                            res += "-";
                        }
                    }
                    if (a[x].Length > 1)
                    {
                        res += a[x].Substring(0, 1).ToUpper() + a[x].Substring(1, a[x].Length - 1).ToLower();
                    }
                    else
                    {
                        res += a[x].ToUpper();
                    }
                    first = false;
                }
                if (res != "" && res.Length > 4)
                {
                    if (res.Substring(0, 2).ToLower() == "mc" || res.Substring(0, 2).ToLower() == "o'")
                    {
                        res = res.Substring(0, 2) + res.Substring(2, 1).ToUpper() + res.Substring(3);
                    }
                }
            }
            return RemoveScriptCharacters(res);
        }


        public static string RemoveNonNumeric(string s)
        {
            string res = "";
            if (s != null)
            {
                for (int x = 0; x != s.Length; x++)
                {
                    if (Char.IsDigit(s[x]))
                    {
                        res += s[x];
                    }
                }
            }
            return res;
        }

        public static string RemoveNumeric(string s)
        {
            string res = "";
            if (s != null)
            {
                for (int x = 0; x != s.Length; x++)
                {
                    if (Char.IsDigit(s[x]) == false)
                    {
                        res += s[x];
                    }
                }
            }
            return res;
        }

        public static bool ValidEmail(string email)
        {
            bool res = false;
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                res = true;
            }
            catch
            {
                res = false;
            }
            return res;
        }

        public static bool ValidCC(string cardNo, string expiryDate, string cvv)
        {
            var masterCardCheck = new Regex(@"^5[1-5][0-9]{14}|^(222[1-9]|22[3-9]\\d|2[3-6]\\d{2}|27[0-1]\\d|2720)[0-9]{12}$");
            var amexCardCheck = new Regex(@"^3[47][0-9]{13}$");
            var visaCardCheck = new Regex(@"^4[0-9]{12}(?:[0-9]{3})?$");
            var discoverCardCheck = new Regex(@"^65[4-9][0-9]{13}|64[4-9][0-9]{13}|6011[0-9]{12}|(622(?:12[6-9]|1[3-9][0-9]|[2-8][0-9][0-9]|9[01][0-9]|92[0-5])[0-9]{10})$");
            var monthCheck = new Regex(@"^(0[1-9]|1[0-2])$");
            var yearCheck = new Regex(@"^20[0-9]{2}$");
            var cvvCheck1 = new Regex(@"^\d{3}$");
            var cvvCheck2 = new Regex(@"^\d{4}$");

            if (!masterCardCheck.IsMatch(cardNo) && !amexCardCheck.IsMatch(cardNo) && !visaCardCheck.IsMatch(cardNo) && !discoverCardCheck.IsMatch(cardNo)) // <1>check card number is valid
                return false;
            if (!cvvCheck1.IsMatch(cvv) && !cvvCheck2.IsMatch(cvv)) // <2>check cvv is valid as "999"
                return false;

            var dateParts = expiryDate.Split('/'); //expiry date in from MM/yyyy            
            if (!monthCheck.IsMatch(dateParts[0]) || !yearCheck.IsMatch(dateParts[1])) // <3 - 6>
                return false; // ^ check date format is valid as "MM/yyyy"

            var year = int.Parse(dateParts[1]);
            var month = int.Parse(dateParts[0]);
            var lastDateOfExpiryMonth = DateTime.DaysInMonth(year, month); //get actual expiry date
            var cardExpiry = new DateTime(year, month, lastDateOfExpiryMonth, 23, 59, 59);

            //check expiry greater than today & within next 6 years <7, 8>>
            return (cardExpiry > DateTime.Now && cardExpiry < DateTime.Now.AddYears(6));
        }

        public static DateTime MoveDate2(DateTime d1, DateTime d2)
        {
            int d1WeekDay = (int)d1.DayOfWeek;
            while (d1.DayOfWeek != d2.DayOfWeek)
            {
                if (d1WeekDay != 0 && d2.DayOfWeek > d1.DayOfWeek)
                {
                    d1 = d1.AddDays(1);
                }
                else
                {
                    d1 = d1.AddDays(-1);
                }
            }
            return d1;
        }
        public static DateTime MoveDate(DateTime d1, DateTime d2)
        {
            int dayOfWeek1 = (int)d1.DayOfWeek;
            int dayOfWeek2 = (int)d2.DayOfWeek;
            while (d1.DayOfWeek != d2.DayOfWeek)
            {
                if (dayOfWeek2 == 0 || (d2.DayOfWeek > d1.DayOfWeek && dayOfWeek1 != 0))
                {
                    d1 = d1.AddDays(1);
                }
                else
                {
                    d1 = d1.AddDays(-1);
                }
            }
            return d1;
        }

        public static string ConcatenateEmails(string email1, string email2)
        {
            string res = email1;
            if (email2 != "")
            {
                if (email1 == "")
                {
                    res = email2;
                }
                else
                {
                    res += ";" + email2;
                }
            }
            return res;
        }

        public static string PreparePhone(string phone)
        {
            string res = "";
            if (phone != null)
            {
                foreach (char c in phone)
                {
                    if (char.IsDigit(c))
                    {
                        res += c;
                    }
                }
            }
            return res.Trim();
        }

        public static string DisplayToIdString(string display)
        {
            return display.Replace(" ", "").Replace("'", "").Replace("/", "").Replace("\\", "").Replace("&", "").Replace(",", "").ToLower();
        }

        public static string SplitCamelCase(string s)
        {
            if (s == null) return "";
            string res = "";
            string[] ss = Regex.Split(s, @"(?<!^)(?=[A-Z])");
            for (int x = 0; x != ss.Length; x++)
            {
                if (res != "")
                {
                    res += " ";
                }
                res += ss[x];
            }
            return res;
        }

        public static List<CustomListItemMobile> FillCreditCardMonths()
        {
            List<CustomListItemMobile> list = new List<CustomListItemMobile>();
            list.Add(new CustomListItemMobile { Text = "Month", Value = "" });
            for (int x = 1; x != 13; x++)
            {
                string s = x.ToString();
                if (x < 10)
                {
                    s = "0" + s;
                }
                list.Add(new CustomListItemMobile { Text = s, Value = s });
            }
            return list;
        }

        public static List<CustomListItemMobile> FillCreditCardYears()
        {
            List<CustomListItemMobile> list = new List<CustomListItemMobile>();
            list.Add(new CustomListItemMobile { Text = "Year", Value = "" });
            for (int x = DateTime.Now.Year; x != DateTime.Now.Year + 15; x++)
            {
                string s = x.ToString();
                list.Add(new CustomListItemMobile { Text = s, Value = s });
            }
            return list;
        }

        public static decimal RoundUp(decimal m)
        {
            return Math.Ceiling(m * 100M) / 100M;
        }

        public static void FirstLastNames(string fullname, out string firstname, out string lastname)
        {
            firstname = "";
            lastname = "";
            string[] ss = fullname.Split(' ');
            if (ss.Length > 0)
            {
                lastname = ss[ss.Length - 1];
                for (int x = 0; x != ss.Length - 1; x++)
                {
                    if (x > 0)
                    {
                        firstname += " ";
                    }
                    firstname += ss[x];
                }
            }
        }

        public static bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        public static bool EmailExists(int gymId, string email)
        {
            Dictionary<string, object> ps = new Dictionary<string, object>();
            ps.Add("gymId", gymId);
            ps.Add("email", email);
            int accountId = Convert.ToInt32(UtilMobile.CallApiGetParamsString("/api/gym/emailexists", ps));
            return accountId > 0;
        }

        public static bool EmailExistsEdit(int gymId, string email, int accountId)
        {
            bool exists = false;
            var client = new RestSharp.RestClient("https://api.mygym.com");
            email = email.Replace(".", "||");
            var request = new RestSharp.RestRequest($"/api/gym/emailexistsedit?gymId={gymId}&email={email}&accountId={accountId}");
            var response = client.ExecuteAsync(request);
            if (response.Status == TaskStatus.RanToCompletion && response.Result.Content.Contains("error") == false)
            {
                int a = Convert.ToInt32(response.Result.Content.Replace("\"", ""));
                if (a > 0)
                {
                    exists = true;
                }
            }
            return exists;
        }

        public static string ConvertHtml(string html)
        {
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(html);

            StringWriter sw = new StringWriter();
            ConvertTo(doc.DocumentNode, sw);
            sw.Flush();
            return sw.ToString();
        }

        public static void ConvertContentTo(HtmlNode node, TextWriter outText)
        {
            foreach (HtmlNode subnode in node.ChildNodes)
            {
                ConvertTo(subnode, outText);
            }
        }

        public static void ConvertTo(HtmlNode node, TextWriter outText)
        {
            string html;
            switch (node.NodeType)
            {
                case HtmlNodeType.Comment:
                    break;
                case HtmlNodeType.Document:
                    ConvertContentTo(node, outText);
                    break;
                case HtmlNodeType.Text:
                    string parentName = node.ParentNode.Name;
                    if ((parentName == "script") || (parentName == "style"))
                    {
                        break;
                    }
                    html = ((HtmlTextNode)node).Text;
                    if (HtmlNode.IsOverlappedClosingElement(html))
                    {
                        break;
                    }
                    if (html.Trim().Length > 0)
                    {
                        outText.Write(HtmlEntity.DeEntitize(html));
                    }
                    break;
                case HtmlNodeType.Element:
                    switch (node.Name)
                    {
                        case "p":
                            outText.Write("\r\n");
                            break;
                        case "br":
                            outText.Write("\r\n");
                            break;
                    }
                    if (node.HasChildNodes)
                    {
                        ConvertContentTo(node, outText);
                    }
                    break;
            };
        }

        public static string GetDayOfWeekInt(int i, out DayOfWeek dayofweek)
        {
            dayofweek = DayOfWeek.Monday;
            string day = "";
            switch (i)
            {
                case 0: day = "Mon"; dayofweek = DayOfWeek.Monday; break;
                case 1: day = "Tue"; dayofweek = DayOfWeek.Tuesday; break;
                case 2: day = "Wed"; dayofweek = DayOfWeek.Wednesday; break;
                case 3: day = "Thu"; dayofweek = DayOfWeek.Thursday; break;
                case 4: day = "Fri"; dayofweek = DayOfWeek.Friday; break;
                case 5: day = "Sat"; dayofweek = DayOfWeek.Saturday; break;
                case 6: day = "Sun"; dayofweek = DayOfWeek.Sunday; break;
            }
            return day;
        }

        public static List<CustomListItemMobile> GetStates(bool canada = false, bool includeLabel = true)
        {
            List<CustomListItemMobile> states = new List<CustomListItemMobile>();
            if (canada == true)
            {
                if (includeLabel == true)
                {
                    states.Add(new CustomListItemMobile { Text = "--Province--", Value = "" });
                }
                states.Add(new CustomListItemMobile { Text = "Alberta", Value = "AB" });
                states.Add(new CustomListItemMobile { Text = "British Columbia", Value = "BC" });
                states.Add(new CustomListItemMobile { Text = "Manitoba", Value = "MB" });
                states.Add(new CustomListItemMobile { Text = "New Brunswick", Value = "NB" });
                states.Add(new CustomListItemMobile { Text = "Newfoundland/Labrador", Value = "NL" });
                states.Add(new CustomListItemMobile { Text = "Nova Scotia", Value = "NS" });
                states.Add(new CustomListItemMobile { Text = "Northwest Territories", Value = "NT" });
                states.Add(new CustomListItemMobile { Text = "Nunavut", Value = "NU" });
                states.Add(new CustomListItemMobile { Text = "Ontario", Value = "ON" });
                states.Add(new CustomListItemMobile { Text = "Prince Edward Island", Value = "PE" });
                states.Add(new CustomListItemMobile { Text = "Quebec", Value = "QC" });
                states.Add(new CustomListItemMobile { Text = "Saskatchewan", Value = "SK" });
                states.Add(new CustomListItemMobile { Text = "Yukon", Value = "YT" });
            }
            else
            {
                if (includeLabel == true)
                {
                    states.Add(new CustomListItemMobile { Text = "--State--", Value = "" });
                }
                states.Add(new CustomListItemMobile { Text = "Alabama", Value = "AL" });
                states.Add(new CustomListItemMobile { Text = "Alaska", Value = "AK" });
                states.Add(new CustomListItemMobile { Text = "Arizona", Value = "AZ" });
                states.Add(new CustomListItemMobile { Text = "Arkansas", Value = "AR" });
                states.Add(new CustomListItemMobile { Text = "California", Value = "CA" });
                states.Add(new CustomListItemMobile { Text = "Colorado", Value = "CO" });
                states.Add(new CustomListItemMobile { Text = "Connecticut", Value = "CT" });
                states.Add(new CustomListItemMobile { Text = "Delaware", Value = "DE" });
                states.Add(new CustomListItemMobile { Text = "Florida", Value = "FL" });
                states.Add(new CustomListItemMobile { Text = "Georgia", Value = "GA" });
                states.Add(new CustomListItemMobile { Text = "Hawaii", Value = "HI" });
                states.Add(new CustomListItemMobile { Text = "Idaho", Value = "ID" });
                states.Add(new CustomListItemMobile { Text = "Illinois", Value = "IL" });
                states.Add(new CustomListItemMobile { Text = "Indiana", Value = "IN" });
                states.Add(new CustomListItemMobile { Text = "Iowa", Value = "IA" });
                states.Add(new CustomListItemMobile { Text = "Kansas", Value = "KS" });
                states.Add(new CustomListItemMobile { Text = "Kentucky", Value = "KY" });
                states.Add(new CustomListItemMobile { Text = "Louisiana", Value = "LA" });
                states.Add(new CustomListItemMobile { Text = "Maine", Value = "ME" });
                states.Add(new CustomListItemMobile { Text = "Maryland", Value = "MD" });
                states.Add(new CustomListItemMobile { Text = "Massachusetts", Value = "MA" });
                states.Add(new CustomListItemMobile { Text = "Michigan", Value = "MI" });
                states.Add(new CustomListItemMobile { Text = "Minnesota", Value = "MN" });
                states.Add(new CustomListItemMobile { Text = "Mississippi", Value = "MS" });
                states.Add(new CustomListItemMobile { Text = "Missouri", Value = "MO" });
                states.Add(new CustomListItemMobile { Text = "Montana", Value = "MT" });
                states.Add(new CustomListItemMobile { Text = "Nebraska", Value = "NE" });
                states.Add(new CustomListItemMobile { Text = "Nevada", Value = "NV" });
                states.Add(new CustomListItemMobile { Text = "New Hampshire", Value = "NH" });
                states.Add(new CustomListItemMobile { Text = "New Jersey", Value = "NJ" });
                states.Add(new CustomListItemMobile { Text = "New Mexico", Value = "NM" });
                states.Add(new CustomListItemMobile { Text = "New York", Value = "NY" });
                states.Add(new CustomListItemMobile { Text = "North Carolina", Value = "NC" });
                states.Add(new CustomListItemMobile { Text = "North Dakota", Value = "ND" });
                states.Add(new CustomListItemMobile { Text = "Ohio", Value = "OH" });
                states.Add(new CustomListItemMobile { Text = "Oklahoma", Value = "OK" });
                states.Add(new CustomListItemMobile { Text = "Oregon", Value = "OR" });
                states.Add(new CustomListItemMobile { Text = "Pennsylvania", Value = "PA" });
                states.Add(new CustomListItemMobile { Text = "Rhode Island", Value = "RI" });
                states.Add(new CustomListItemMobile { Text = "South Carolina", Value = "SC" });
                states.Add(new CustomListItemMobile { Text = "South Dakota", Value = "SD" });
                states.Add(new CustomListItemMobile { Text = "Tennessee", Value = "TN" });
                states.Add(new CustomListItemMobile { Text = "Texas", Value = "TX" });
                states.Add(new CustomListItemMobile { Text = "Utah", Value = "UT" });
                states.Add(new CustomListItemMobile { Text = "Vermont", Value = "VT" });
                states.Add(new CustomListItemMobile { Text = "Virginia", Value = "VA" });
                states.Add(new CustomListItemMobile { Text = "Washington", Value = "WA" });
                states.Add(new CustomListItemMobile { Text = "Washington D.C.", Value = "DC" });
                states.Add(new CustomListItemMobile { Text = "West Virginia", Value = "WV" });
                states.Add(new CustomListItemMobile { Text = "Wisconsin", Value = "WI" });
                states.Add(new CustomListItemMobile { Text = "Wyoming", Value = "WY" });
            }
            return states;
        }

        public static string FecthCCPanLast4(byte[] encodedcc)
        {
            if (encodedcc == null || encodedcc.Length == 0 || encodedcc.Length == 1)
            {
                return "";
            }
            byte[] last4Part = new byte[2];
            Array.Copy(encodedcc, 0, last4Part, 0, 2);
            return DecodeBytesTo4Digits(last4Part);
        }

        public static string DecodeBytesTo4Digits(byte[] bytes)
        {
            if (bytes.Length != 2)
            {
                throw new Exception("Invalid bytes to convert to 4 digits");
            }
            if (!BitConverter.IsLittleEndian)
            {
                bytes = bytes.Reverse().ToArray();
            }
            string digits = BitConverter.ToInt16(bytes, 0).ToString("0000");
            if (digits.Length != 4)
            {
                throw new Exception("Invalid bytes to convert to 4 digits");
            }
            return digits;
        }

        public static string CostSummaryEvent(bool summary)
        {
            string CostSummary = "";
            EventMobile ev = (EventMobile)Application.Current.Properties["camp"];
            CostSummary += $"{ev.EventCost.Desc}\r\n";
            decimal creditApply = 0;
            if (Application.Current.Properties.ContainsKey("creditapply"))
            {
                creditApply = Convert.ToDecimal(Application.Current.Properties["creditapply"]);
            }
            GymMobile gym = (GymMobile)Application.Current.Properties["gym"];
            ev.EventCost.SocksPrice = 0M;
            ev.EventCost.SocksTax = 0M;
            if (summary == true && gym.ShowSocksOnCheckout && gym.SocksPrice > 0)
            {
                AccountMobile account = (AccountMobile)Application.Current.Properties["account"];
                foreach (ChildMobile c in account.Children)
                {
                    bool include = false;
                    if (ev.SelectedDates == null || ev.SelectedDates.Count == 0)
                    {
                        include = true;
                    }
                    else
                    {
                        foreach (EventDateMobile d in ev.SelectedDates)
                        {
                            if (d.ChildId == c.ChildId)
                            {
                                include = true;
                                break;
                            }
                        }
                    }
                    if (include && c.IncludeSocks == true)
                    {
                        ev.EventCost.SocksPrice += gym.SocksPrice;
                        if (gym.SocksTax > 0)
                        {
                            ev.EventCost.SocksTax += gym.SocksTax;
                        }
                    }
                }
            }
            ev.EventCost.Total = ev.EventCost.SubTotal + ev.EventCost.Tax + ev.EventCost.SocksPrice + ev.EventCost.SocksTax;
            decimal total = ev.EventCost.Total;
            if (total > 0)
            {
                CostSummary += $"Camp/Event Cost: {ev.EventCost.SubTotal:c}\r\n";
                if (ev.EventCost.SocksPrice > 0)
                {
                    CostSummary += $"My Gym Socks: {ev.EventCost.SocksPrice:c}\r\n";
                }
                if (gym.EventTax > 0)
                {
                    CostSummary += $"Camp/Event Tax: {ev.EventCost.Tax:c}\r\n";
                }
                if (ev.EventCost.SocksTax > 0)
                {
                    CostSummary += $"My Gym Socks Tax: {ev.EventCost.SocksTax:c}\r\n";
                }
                if (creditApply > 0)
                {
                    CostSummary += string.Format(new CultureInfo(gym.Culture), "Credit: {0:c}\r\n",-creditApply);
                    total = ev.EventCost.Total - creditApply;
                    ev.EventCost.Credit = creditApply;
                }
                CostSummary += $"Total: {string.Format(new CultureInfo(gym.Culture), "{0:c}", total)}\r\n";
            }
            return CostSummary;
        }

        public static void ReadCamps()
        {
            GymMobile gym = (GymMobile)Application.Current.Properties["gym"];
            Dictionary<string, object> ps = new Dictionary<string, object>();
            ps.Add("gymId", Convert.ToInt32(gym.Id));
            Application.Current.Properties["camps"] = (List<EventMobile>)UtilMobile.CallApiGetParams<List<EventMobile>>("/api/gym/readcamps", ps);
            foreach (EventMobile e in (List<EventMobile>)Application.Current.Properties["camps"])
            {
                e.Display = e.Display.Replace("|", "'");
                e.DescLong = e.DescLong.Replace("|", "'");
                foreach (EventDiscountMobile d in e.EventDiscounts)
                {
                    d.DisplayList = d.DisplayList.Replace("|", "'");
                    d.Desc = d.Desc.Replace("|", "'");
                }
                foreach (EventDiscountMobile d in e.EventDiscountsAdd)
                {
                    d.DisplayList = d.DisplayList.Replace("|", "'");
                    d.Desc = d.Desc.Replace("|", "'");
                }
                foreach (EventItemMobile d in e.EventItems)
                {
                    d.Item = d.Item.Replace("|", "'");
                }
                e.StartMonth = new DateTime(e.Start.Year, e.Start.Month, 1);
                e.EndMonth = new DateTime(e.End.Year, e.End.Month, DateTime.DaysInMonth(e.End.Year, e.End.Month));
                e.AgeDesc = UtilMobile.ConvertHtml(e.AgeDesc);
                e.DescLong = UtilMobile.ConvertHtml(e.DescLong);
                e.DateStr = string.Format(new CultureInfo(gym.Culture), "{0:MMMMMMMMMMMM dd, yyy} - {1:MMMMMMMMMMMM dd, yyy}", e.Start, e.End);
                e.Appointments = new ObservableCollection<Appointment>();
                e.EventDiscountsStr = "";
                e.SelectedDates = new ObservableCollection<EventDateMobile>();
                foreach (EventDiscountMobile d in e.EventDiscounts)
                {
                    e.EventDiscountsStr += d.DisplayList + "\r\n";
                }
                e.EventDates = new ObservableCollection<EventDateMobile>();
                List<DateTime> full = new List<DateTime>();
                e.ListOfDates = "";
                foreach (EventInstanceMobile i in e.EventInstances)
                {
                    EventDateMobile edm = new EventDateMobile();
                    edm.ClassId = i.ClassId;
                    edm.Date = new DateTime(i.EventEndDate.Year, i.EventStartDate.Month, i.EventStartDate.Day);
                    edm.DateStart = i.EventStartDate;
                    edm.DateStr = "";
                    edm.TimeStr = "";
                    if (i.IsFull == false)
                    {
                        edm.DateStr = string.Format(new CultureInfo(gym.Culture), "{0:MM/dd}", edm.Date);
                        edm.TimeStr = string.Format(new CultureInfo(gym.Culture), "{0:hh:mmtt}-{1:hh:mmtt}", i.EventStartDate, i.EventEndDate);
                        e.ListOfDates += string.Format(new CultureInfo(gym.Culture), "{0:MM/dd} {1:hh:mmtt}-{2:hh:mmtt}\r\n", edm.Date, i.EventStartDate, i.EventEndDate).ToLower();
                        e.Appointments.Add(new Appointment { IsAllDay = true, StartDate = edm.Date, EndDate = edm.Date, Color = Color.DarkGreen });
                        e.EventDates.Add(edm);
                    }
                    else
                    {
                        e.ListOfDates += string.Format(new CultureInfo(gym.Culture), "{0:MM/dd} {1:hh:mmtt}-{2:hh:mmtt}*\r\n", edm.Date, i.EventStartDate, i.EventEndDate).ToLower();
                        e.Appointments.Add(new Appointment { IsAllDay = true, StartDate = edm.Date, EndDate = edm.Date, Color = Color.IndianRed });
                    }
                }
                e.ListOfDates += string.Format(new CultureInfo(gym.Culture), "\r\n* indicates camp/event is full");
            }
        }
    }
}
