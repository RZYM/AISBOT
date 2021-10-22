using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AISBOT
{
    class AisAPI
    {
        public static string EndPoint = "https://become-ais-family.ais.co.th";

        private static string getBetween(string strSource, string strStart, string strEnd)
        {
            if (strSource.Contains(strStart) && strSource.Contains(strEnd))
            {
                int Start, End;
                Start = strSource.IndexOf(strStart, 0) + strStart.Length;
                End = strSource.IndexOf(strEnd, Start);
                return strSource.Substring(Start, End - Start);
            }

            return "";
        }

        private static string[] getBetween2(string strSource, string strStart, string strEnd)
        {
            if (strSource.Contains(strStart) && strSource.Contains(strEnd))
            {
                int Start, End;
                Start = strSource.IndexOf(strStart, 0) + strStart.Length;
                End = strSource.IndexOf(strEnd, Start);
                string found = strSource.Substring(Start, End - Start);
                return new string[] { found, Start.ToString() };
            }
            return new string[] { "", "" };
        }

        private static String GetJSESSION(string url)
        {
            String session = "";
            try
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                HttpWebRequest httpWebRequest = (HttpWebRequest)HttpWebRequest.Create(url);
                httpWebRequest.CookieContainer = new CookieContainer();

                httpWebRequest.Method = "GET";
                httpWebRequest.KeepAlive = false;
                httpWebRequest.Timeout = 5000;
                httpWebRequest.ProtocolVersion = HttpVersion.Version11;
                httpWebRequest.UserAgent = "Mozilla/5.0 (iPhone; CPU iPhone OS 12_2 like Mac OS X) AppleWebKit/605.1.15 (KHTML, like Gecko) Mobile/15E148";
                using (HttpWebResponse response = (HttpWebResponse)httpWebRequest.GetResponse())
                {
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        foreach (System.Net.Cookie cook in response.Cookies)
                        {
                            if (cook.Name == "JSESSIONID")
                            {
                                session = response.Cookies["JSESSIONID"].Value.ToString();
                            }
                        }
                    }
                }
            }
            catch (Exception) { /*SwitchEndpointAtomic();*/ }
            return session;
        }

        private static String GET(string url, String Jsession)
        {
            try
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                HttpWebRequest httpWebRequest = (HttpWebRequest)HttpWebRequest.Create(url);

                httpWebRequest.Method = "GET";
                httpWebRequest.KeepAlive = false;
                httpWebRequest.Timeout = 5000;
                httpWebRequest.ProtocolVersion = HttpVersion.Version11;
                httpWebRequest.Headers["Cookie"] = "JSESSIONID=" + Jsession + ";";
                httpWebRequest.UserAgent = "Mozilla/5.0 (iPhone; CPU iPhone OS 12_2 like Mac OS X) AppleWebKit/605.1.15 (KHTML, like Gecko) Mobile/15E148";
                using (HttpWebResponse response = (HttpWebResponse)httpWebRequest.GetResponse())
                {
                    if (response.StatusCode == HttpStatusCode.OK)
                    { 
                        var encoding = ASCIIEncoding.ASCII;
                        using (var reader = new System.IO.StreamReader(response.GetResponseStream(), encoding))
                        {
                            string responseText = reader.ReadToEnd();
                            return responseText;
                        }
                    }
                }
            }
            catch (Exception) { /*SwitchEndpointAtomic();*/ }
            return null;
        }

        public static String POSTJSONCM(string url, string json, String Jsession, string referer = "")
        {
            try
            {

                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                httpWebRequest.ProtocolVersion = HttpVersion.Version11;
                httpWebRequest.ContentType = "application/json";
                httpWebRequest.Method = "POST";
                httpWebRequest.KeepAlive = false;
                httpWebRequest.Timeout = 5000;
                httpWebRequest.Headers["Cookie"] = "JSESSIONID=" + Jsession + ";";
                if (referer != "")
                    httpWebRequest.Referer = referer;

                using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {
                    streamWriter.Write(json);
                    streamWriter.Flush();
                    streamWriter.Close();
                }

                using (HttpWebResponse response = (HttpWebResponse)httpWebRequest.GetResponse())
                {
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        var encoding = ASCIIEncoding.ASCII;
                        using (var reader = new System.IO.StreamReader(response.GetResponseStream(), encoding))
                        {
                            string responseText = reader.ReadToEnd();
                            return responseText;
                        }
                    }
                }
            }
            catch (Exception ex) {}
            return null;
        }

        public static String POSTFORMDATA(string url, string[] postdata, String Jsession = "", bool payment = false)
        {
            try
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                byte[] data = Encoding.UTF8.GetBytes(string.Concat(postdata));
                HttpWebRequest httpWebRequest = (HttpWebRequest)HttpWebRequest.Create(url);
                httpWebRequest.ContentType = "application/x-www-form-urlencoded";
                httpWebRequest.Method = "POST";
                httpWebRequest.KeepAlive = true;
                httpWebRequest.Timeout = 5000;
                httpWebRequest.ProtocolVersion = HttpVersion.Version11;
                httpWebRequest.Referer = "https://become-ais-family.ais.co.th/fill-address";
                httpWebRequest.Headers["Cookie"] = "JSESSIONID=" + Jsession + ";";
                httpWebRequest.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/92.0.4515.159 Safari/537.36";

                if (payment == true)
                    httpWebRequest.AllowAutoRedirect = true;

                httpWebRequest.ContentLength = data.Length;
                Stream requestStream = httpWebRequest.GetRequestStream();
                requestStream.Write(data, 0, data.Length);

                using (HttpWebResponse response = (HttpWebResponse)httpWebRequest.GetResponse())
                {
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        if (payment == true)
                        {
                            return response.ResponseUri.ToString();
                        }
                        var encoding = ASCIIEncoding.ASCII;
                        using (var reader = new System.IO.StreamReader(response.GetResponseStream(), encoding))
                        {
                            string responseText = reader.ReadToEnd();
                            return responseText;
                        }
                    }
                }
                requestStream.Close();
            }
            catch (Exception ex) { Console.WriteLine(ex.Message.ToString()); }
            return null;
        }

        public static String Start()
        {
            String response = GetJSESSION(EndPoint);
            return response;
        }

        public static String[] FindPhoneNumber(string number, String Jsession)
        {
            try
            {
                String get_data = POSTFORMDATA(EndPoint + "/find-by-mobile", new string[] {
                "num1=", number[1].ToString(),
                "&num2=", number[2].ToString(),
                "&num3=", number[3].ToString(),
                "&num4=", number[4].ToString(),
                "&num5=", number[5].ToString(),
                "&num6=", number[6].ToString(),
                "&num7=", number[7].ToString(),
                "&num8=", number[8].ToString(),
                "&num9=", number[9].ToString(),
                "&like1=", "",
                "&like2=", "",
                "&like3=", "",
                "&like4=", "",
                "&dislike1=", "",
                "&dislike1=", "",
                "&dislike1=", "",
                "&dislike1=", "",
                "&sum=", "",
                "&simType=", "null",
                "&pageName=", "/new-sim",
            }, Jsession);
                String getRedirect = getBetween(get_data, "onclick=\"redirect('", "')");
                String getRedirect2 = getBetween(get_data, "select-number/", "\"");
                String reredict_return = "";
                if (getRedirect != "")
                    reredict_return = getRedirect;
                if (getRedirect2 != "")
                    reredict_return = getRedirect2;

                return new string[] { reredict_return };
            }
            catch (Exception) { }
            return new string[] { null };
        }
        public static String[] FindPhoneNumber_Mask(string numbers, String Jsession)
        {
            int page = 1;
            string[] number = new string[10];

            for (int i = 1; i < numbers.Length; i++)
            {
                if (numbers[i].ToString().ToLower() == "x")
                    number[i] = "";
                else
                {
                    number[i] = numbers[i].ToString();
                    if (number[i] == "x")
                        number[i] = "";
                }

            }

            List<string> return_id = new List<string>();

            while (true)
            {
                try
                {
                    String get_data = POSTFORMDATA(EndPoint + "/find-by-mobile", new string[] {
                    "num1=", number[1].ToString(),
                    "&num2=", number[2].ToString(),
                    "&num3=", number[3].ToString(),
                    "&num4=", number[4].ToString(),
                    "&num5=", number[5].ToString(),
                    "&num6=", number[6].ToString(),
                    "&num7=", number[7].ToString(),
                    "&num8=", number[8].ToString(),
                    "&num9=", number[9].ToString(),
                    "&like1=", "",
                    "&like2=", "",
                    "&like3=", "",
                    "&like4=", "",
                    "&dislike1=", "",
                    "&dislike1=", "",
                    "&dislike1=", "",
                    "&dislike1=", "",
                    "&sum=", "",
                    "&simType=", "null",
                    "&pageName=", "/new-sim",
                    "&page=", page.ToString()
                }, Jsession);

                    int BeforeCont = return_id.Count();
                    while (true)
                    {
                        String[] getRedirect = getBetween2(get_data, "onclick=\"redirect('", "')");
                        String[] getRedirect2 = getBetween2(get_data, "select-number/", "\"");
                        if (getRedirect[0] != "")
                        {
                            if (return_id.Contains(getRedirect[0]) == false)
                                return_id.Add(getRedirect[0]);

                            get_data = get_data.Substring(int.Parse(getRedirect[1]));
                        }
                        else if (getRedirect2[0] != "")
                        {
                            if (return_id.Contains(getRedirect2[0]) == false)
                                return_id.Add(getRedirect2[0]);

                            get_data = get_data.Substring(int.Parse(getRedirect2[1]));
                        }
                        else
                            break;
                    }

                    if (BeforeCont == return_id.Count())
                        break;
                    else
                        page++;
                }
                catch (Exception) { }
                return return_id.ToArray();
            }
            return new string[] { null };
        }

        public static String[] FindPhoneNumber_Condition(string numbers, String Jsession)
        {
            int page = 1;
            string[] number = new string[10];

            for (int i = 1; i < numbers.Length; i++)
            {
                if (numbers[i].ToString().ToLower() == "x")
                    number[i] = "";
                else
                {
                    number[i] = numbers[i].ToString();
                    if (number[i] == "x")
                        number[i] = "";
                }
            }

            List<string> return_id = new List<string>();

            while (true)
            {
                try
                {
                    String get_data = POSTFORMDATA(EndPoint + "/find-by-mobile", new string[] {
                    "num1=", number[1].ToString(),
                    "&num2=", number[2].ToString(),
                    "&num3=", number[3].ToString(),
                    "&num4=", number[4].ToString(),
                    "&num5=", number[5].ToString(),
                    "&num6=", number[6].ToString(),
                    "&num7=", number[7].ToString(),
                    "&num8=", number[8].ToString(),
                    "&num9=", number[9].ToString(),
                    "&like1=", "",
                    "&like2=", "",
                    "&like3=", "",
                    "&like4=", "",
                    "&dislike1=", "",
                    "&dislike1=", "",
                    "&dislike1=", "",
                    "&dislike1=", "",
                    "&sum=", "",
                    "&simType=", "null",
                    "&pageName=", "/new-sim",
                    "&page=", page.ToString()
                }, Jsession);

                    int BeforeCont = return_id.Count();
                    while (true)
                    {
                        String[] getRedirect = getBetween2(get_data, "onclick=\"redirect('", "')");
                        String[] getRedirect2 = getBetween2(get_data, "select-number/", "\"");
                        if (getRedirect[0] != "")
                        {
                            if (return_id.Contains(getRedirect[0]) == false)
                                return_id.Add(getRedirect[0]);

                            get_data = get_data.Substring(int.Parse(getRedirect[1]));
                        }
                        else if (getRedirect2[0] != "")
                        {
                            if (return_id.Contains(getRedirect2[0]) == false)
                                return_id.Add(getRedirect2[0]);

                            get_data = get_data.Substring(int.Parse(getRedirect2[1]));
                        }
                        else
                            break;
                    }

                    if (BeforeCont == return_id.Count())
                        break;
                    else
                        page++;
                }
                catch (Exception) { }
                return return_id.ToArray();
            }
            return new string[] { null };
            /*try
            {
                string[] number = new string[10];

                for (int i = 1; i < numbers.Length; i++)
                {
                    if (numbers[i].ToString().ToLower() == "x")
                        number[i] = "";
                    else
                    {
                        number[i] = numbers[i].ToString();
                        if (number[i] == "x")
                            number[i] = "";
                    }
                }

                String get_data = POSTFORMDATA(EndPoint + "/find-by-mobile", new string[] {
                    "num1=", number[1].ToString(),
                    "&num2=", number[2].ToString(),
                    "&num3=", number[3].ToString(),
                    "&num4=", number[4].ToString(),
                    "&num5=", number[5].ToString(),
                    "&num6=", number[6].ToString(),
                    "&num7=", number[7].ToString(),
                    "&num8=", number[8].ToString(),
                    "&num9=", number[9].ToString(),
                    "&like1=", "",
                    "&like2=", "",
                    "&like3=", "",
                    "&like4=", "",
                    "&dislike1=", "",
                    "&dislike1=", "",
                    "&dislike1=", "",
                    "&dislike1=", "",
                    "&sum=", "",
                    "&simType=", "null",
                    "&pageName=", "/new-sim",
                }, Jsession);
                string phone_found = "";
                while (true)
                {
                    String[] get_Phone = getBetween2(get_data, "step3-box-detail_num", "<span>");
                    if (get_Phone[0] == "")
                        break;
                    var find_Final = getBetween(get_Phone[0] + "#", "0", "#");
                    var Reg = Regex.Split(find_Final, "-");
                    string phone_num = "0" + Reg[0] + Reg[1] + number;
                    #region CheckCoodination
                    string[] Fetch = new string[] { (phone_num[3].ToString() + phone_num[4]).ToString(), (phone_num[4].ToString() + phone_num[5]).ToString(), (phone_num[5].ToString() + phone_num[6]).ToString(), (phone_num[6].ToString() + phone_num[7]).ToString(), (phone_num[7].ToString() + phone_num[8]).ToString(), (phone_num[8].ToString() + phone_num[9]).ToString() };
                    phone_found = phone_num[0].ToString() + phone_num[1].ToString() + phone_num[2].ToString();
                    for (int i = 0; i < Fetch.Length; i++)
                    {
                        if (condition.Contains(Fetch[i]) == false)
                        {
                            phone_found = "";
                            break;
                        }
                        else
                        {
                            phone_found += Fetch[i];
                        }
                    }
                    #endregion
                    get_data = get_data.Substring(int.Parse(get_Phone[1]));
                }

                if (phone_found != "")
                {
                    get_data = POSTFORMDATA(EndPoint + "/find-by-mobile", new string[] {
                        "num1=", phone_found[1].ToString(),
                        "&num2=", phone_found[2].ToString(),
                        "&num3=", phone_found[3].ToString(),
                        "&num4=", phone_found[4].ToString(),
                        "&num5=", phone_found[5].ToString(),
                        "&num6=", phone_found[6].ToString(),
                        "&num7=", phone_found[7].ToString(),
                        "&num8=", phone_found[8].ToString(),
                        "&num9=", phone_found[9].ToString(),
                        "&like1=", "",
                        "&like2=", "",
                        "&like3=", "",
                        "&like4=", "",
                        "&dislike1=", "",
                        "&dislike1=", "",
                        "&dislike1=", "",
                        "&dislike1=", "",
                        "&sum=", "",
                        "&simType=", "null",
                        "&pageName=", "/new-sim",
                    }, Jsession);
                    String getRedirect = getBetween(get_data, "onclick=\"redirect('", "')");
                    String getRedirect2 = getBetween(get_data, "select-number/", "\"");
                    String reredict_return = "";
                    if (getRedirect != "")
                        reredict_return = getRedirect;
                    if (getRedirect2 != "")
                        reredict_return = getRedirect2;

                    return reredict_return;
                }
                else
                {
                    return "";
                }
            }
            catch (Exception) { }*/
        }

        public static String[] NumberCheckNBT(string id, String Jsession)
        {
            try
            {
                String get_data = GET(EndPoint + "/select-number/" + id, Jsession);
                String phone_num = getBetween(get_data, "num_txt_packtel\">", "<");
                String getRedirect = getBetween(get_data, "/check-ais-number?id=", "\"");
                return new string[] { getRedirect, phone_num };
            }
            catch (Exception) { }
            return new string[] { "", "" };
        }

        public static JObject CheckIDCard(string idcard, string firstname, string lastname, string birthday, String JSession, string packageid)
        {
            dynamic Data = new JObject();
            Data.idCard = idcard;
            Data.firstname = firstname;
            Data.surname = lastname;
            Data.birthday = birthday;
            Data.chkCloneProfile = false;

            try
            {
                String get_data = POSTJSONCM(EndPoint + "/page/checkidCard", Data.ToString(), JSession, EndPoint + "/check-ais-number?id=" + packageid);
                if (get_data != null)
                {
                    JObject Object = JObject.Parse(get_data);
                    return Object;
                }
                return null;
            }
            catch (Exception) { }
            return null;
        }

        public static String Payment(string idcardSession, string name, string surname, Address adr_info, String Jsession)
        {
            try
            {
                var client = new RestClient("https://become-ais-family.ais.co.th/page/payment");
                client.Timeout = -1;
                var request = new RestRequest(Method.POST);
                request.AddHeader("User-Agent", "Mozilla/5.0 (iPhone; CPU iPhone OS 12_2 like Mac OS X) AppleWebKit/605.1.15 (KHTML, like Gecko) Mobile/15E148");
                request.AddHeader("Referer", "https://become-ais-family.ais.co.th/fill-address");
                request.AddCookie("JSESSIONID", Jsession);
                request.AddParameter("name", name);
                request.AddParameter("surname", surname);
                request.AddParameter("contactMobileNo", adr_info.phone);
                request.AddParameter("email", adr_info.email);
                request.AddParameter("address", adr_info.address);
                request.AddParameter("moo", adr_info.moo);
                request.AddParameter("mooBan", adr_info.mooban);
                request.AddParameter("room", adr_info.room);
                request.AddParameter("floor", adr_info.floor);
                request.AddParameter("buildingName", adr_info.buildingname);
                request.AddParameter("soi", adr_info.soi);
                request.AddParameter("streetName", adr_info.road);
                request.AddParameter("province", adr_info.province_code);
                request.AddParameter("provinceName", adr_info.provincename);
                request.AddParameter("amphur", adr_info.amphur_code);
                request.AddParameter("amphurName", adr_info.amphurname);
                request.AddParameter("district", adr_info.distict_code);
                request.AddParameter("districtName", adr_info.distictname);
                request.AddParameter("postcode", adr_info.postcode);
                request.AddParameter("myCheckBill", "SMS and eBill");
                request.AddParameter("billEmail", "");
                request.AddParameter("myCheckBillAdd", "billadd1");
                request.AddParameter("addressBill", "");
                request.AddParameter("mooBill", "");
                request.AddParameter("mooBanBill", "");
                request.AddParameter("roomBill", "");
                request.AddParameter("floorBill", "");
                request.AddParameter("buildingNameBill", "");
                request.AddParameter("soiBill", "");
                request.AddParameter("streetNameBill", "");
                request.AddParameter("provinceBill", null);
                request.AddParameter("provinceNameBill", "");
                request.AddParameter("amphurNameBill", "");
                request.AddParameter("districtNameBill", "");
                request.AddParameter("postcodeBill", "");
                request.AddParameter("receiptCheck", "SHORT");
                request.AddParameter("receiptType", "THAI");
                request.AddParameter("idCardReceipt", "");
                request.AddParameter("passportReceipt", "");
                request.AddParameter("nameReceipt", name);
                request.AddParameter("surnameReceipt", surname);
                request.AddParameter("branch", "");
                request.AddParameter("mobileNoReceipt", "");
                request.AddParameter("addressReceipt", "");
                request.AddParameter("mooReceipt", "");
                request.AddParameter("mooBanReceipt", "");
                request.AddParameter("roomReceipt", "");
                request.AddParameter("floorReceipt", "");
                request.AddParameter("buildingNameReceipt", "");
                request.AddParameter("soiReceipt", "");
                request.AddParameter("streetNameReceipt", "");
                request.AddParameter("provinceReceipt", null);
                request.AddParameter("provinceNameReceipt", "");
                request.AddParameter("amphurNameReceipt", "");
                request.AddParameter("districtNameReceipt", "");
                request.AddParameter("postcodeReceipt", "");
                request.AddParameter("chm", "1");
                request.AddParameter("allow", "");
                request.AddParameter("esim", "false");
                request.AddParameter("postpaidKycAllow", "false");
                request.AddParameter("esimKycAllow", "false");
                request.AddParameter("idcardSession", idcardSession);
                request.AddParameter("cardType", "VISA");
                IRestResponse response = client.Execute(request);
                return response.ResponseUri.ToString();
            }
            catch (Exception) { }
            return "";
        }
    }
}
