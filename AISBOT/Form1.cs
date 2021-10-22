using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AISBOT
{
    public partial class Form1 : Form
    {
        string LineToken = "";
        public static bool Exit = false;

        Address Info_Address = new Address();
        Visacard Visacard = new Visacard();
        Linenof line_nof = new Linenof();

        List<Identity> Info_Data = new List<Identity>();
        List<Buytask> Task_Buy = new List<Buytask>();
        List<string> phone_task = new List<string>();

        List<string> Num_Condition = new List<string>();
        int Task_Working = 0;

        int SettingIDSelect = -1;
        int TaskSelect = -1;

        public static bool Pause_FindOdd = true;

        private string getBetween(string strSource, string strStart, string strEnd)
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

        private void SaveIDCardToFile()
        {
            String[] Settings = new String[Info_Data.Count()];
            for (int i = 0; i < Info_Data.Count(); i++)
            {
                Settings[i] = "Name=" + Info_Data[i].firstname + ";Surname=" + Info_Data[i].surname + ";Birthday=" + Info_Data[i].birthday + ";Idcard=" + Info_Data[i].idCard + ";";
            }
            File.WriteAllLines("IDCard.ini", Settings);
        }

        public Form1()
        {
            InitializeComponent();
        }

        private void LoadConfig()
        {
            if (File.Exists("LineToken.ini"))
            {
                LineToken = File.ReadAllText("LineToken.ini");
            }
            nsTextBox27.Text = LineToken;

            if (File.Exists("Visacard.ini"))
            {
                String fs = File.ReadAllText("Visacard.ini");
                Visacard.idcard = getBetween(fs.ToLower(), "number=", ";");
                Visacard.month_exp = getBetween(fs.ToLower(), "month=", ";");
                Visacard.year_exp = getBetween(fs.ToLower(), "year=", ";");
                Visacard.cvv = getBetween(fs.ToLower(), "cvv=", ";");
            }
            nsTextBox38.Text = Visacard.idcard;
            nsTextBox37.Text = Visacard.month_exp;
            nsTextBox22.Text = Visacard.year_exp;
            nsTextBox23.Text = Visacard.cvv;

            if (File.Exists("IDCard.ini"))
            {
                string[] Fs = File.ReadAllLines("IDCard.ini");
                for (int i = 0; i < Fs.Length; i++)
                {
                    string name = getBetween(Fs[i].ToLower(), "name=", ";");
                    string surname = getBetween(Fs[i].ToLower(), "surname=", ";");
                    string birthday = getBetween(Fs[i].ToLower(), "birthday=", ";");
                    string idcard = getBetween(Fs[i].ToLower(), "idcard=", ";");
                    if (name != "" && surname != "" && birthday != "" && idcard != "")
                    {
                        Identity Id = new Identity()
                        {
                            idCard = idcard,
                            firstname = name,
                            surname = surname,
                            birthday = birthday
                        };
                        Info_Data.Add(Id);
                        nsListView2.AddItem((Info_Data.Count - 1).ToString(), Id.firstname + " " + Id.surname, Id.birthday, idcard, "-");
                        nsListView2.Refresh();
                    }
                }
            }

            ReadConfig_Address();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //MessageBox.Show(AisAPI.POSTFORMDATA("http://127.0.0.1/paymentLTC/test.php", new string[] { "txt=", "สบายดี" }).ToString());
            Info_Address.address = "44/44";
            Info_Address.email = "asdjs@gmail.com";
            Info_Address.provincename = "ตรัง";
            Info_Address.province_code = "92";
            Info_Address.amphurname = "ย่านตาขาว";
            Info_Address.amphur_code = "9203";
            Info_Address.distictname = "ทุ่งกระบือ";
            Info_Address.distict_code = "920306";
            Info_Address.postcode = "92140";
            CheckForIllegalCrossThreadCalls = false;
            LoadConfig();
        }

        private void TaskBuy(Buytask task)
        {
            task.Pause = false;

            String jsession = AisAPI.Start();

            int Task = task.Tasknum;
            nsListView1.Items[Task].SubItems[1].Text = "ค้นหาเบอร์โทรศัพท์";
            nsListView1.Refresh();

            if (jsession != "")
            {
                while (true)
                {
                    if (task.Working_Max == 0)
                    {
                        string[] FindNumber = null;
                        if (task.Pause == false)
                        {
                            if (task.Mask == false && task.Num_true == false)
                                FindNumber = AisAPI.FindPhoneNumber(task.Number, jsession);
                            else if (task.Mask == true)
                                FindNumber = AisAPI.FindPhoneNumber_Mask(task.Number, jsession);
                            else if (task.Num_true == true)
                            {
                                FindNumber = AisAPI.FindPhoneNumber_Condition(task.Number, jsession);
                            }
                        }

                        task.Working = 0;
                        for (int i = 0; i < FindNumber.Length; i++)
                        {
                            if (FindNumber[i] != null)
                            {
                                task.Working_Max++;
                                Thread t = new Thread(() => BuyProcess(task, FindNumber[i], jsession, Num_Condition, nsListView1));
                                t.Start();
                                Thread.Sleep(500);
                                //  if (BuyProcess(task, FindNumber[i], jsession, Num_Condition));
                            }
                        }

                        if (task.Pause == true)
                        {
                            nsListView1.Items[Task].SubItems[1].Text = "หยุดทำงาน";
                            nsListView1.Refresh();
                        }

                    }
                    else
                    {
                        if (task.Working == task.Working_Max)
                        {
                            task.Working = 0;
                            task.Working_Max = 0;
                            Thread.Sleep(1000);
                        }
                    }
                    Thread.Sleep(100);
                }
            }
            else
            {
                nsListView1.Items[Task].SubItems[1].Text = "เชื่อมต่อล้มเหลว";
                nsListView1.Refresh();
                Thread.Sleep(10000);
                TaskBuy(task);
            }
        }

        private bool BuyProcess(Buytask task, string id, string jsession, List<string> condition, NSListView ls)
        {
            Random Ran_Data = new Random();
            int Ran = Ran_Data.Next(0, Info_Data.Count);

            int Task = task.Tasknum;

            if (id != null && task.Pause == false)
            {
                String[] PackageID = AisAPI.NumberCheckNBT(id, jsession);
                if (task.Num_true)
                {
                    string num_1 = PackageID[1][2].ToString() + PackageID[1][3].ToString();
                    string num_2 = PackageID[1][3].ToString() + PackageID[1][4].ToString();
                    string num_3 = PackageID[1][4].ToString() + PackageID[1][5].ToString();
                    string num_4 = PackageID[1][5].ToString() + PackageID[1][6].ToString();
                    string num_5 = PackageID[1][6].ToString() + PackageID[1][7].ToString();
                    string num_6 = PackageID[1][7].ToString() + PackageID[1][8].ToString();
                    string num_7 = PackageID[1][8].ToString() + PackageID[1][9].ToString();
                    string[] Splite = new string[] { num_1, num_2, num_3, num_4, num_5, num_6, num_7 };
                    for (int i = 0; i < Splite.Length; i++)
                    {
                        if (condition.Contains(Splite[i]) == false)
                        {
                            task.Working++;
                            return true;
                        }
                    }
                }
                JObject CheckIDCard = AisAPI.CheckIDCard(Info_Data[Ran].idCard, Info_Data[Ran].firstname, Info_Data[Ran].surname, Info_Data[Ran].birthday, jsession, PackageID[0]);
                if (CheckIDCard.ContainsKey("developerMessage") && CheckIDCard["developerMessage"].ToString() == "Success")
                {
                    Thread.Sleep(2000);
                    string payment_create = AisAPI.Payment(Info_Data[Ran].idCard, Info_Data[Ran].firstname, Info_Data[Ran].surname, Info_Address, jsession);
                    if (payment_create != "" && payment_create != "https://become-ais-family.ais.co.th/check-ais-number" && payment_create != "https://become-ais-family.ais.co.th/new-sim")
                    {
                        ls.Items[Task].SubItems[1].Text = "สร้างลิงค์จ่ายเงินสำเร็จ";
                        ls.Refresh();
                        task.PaymentURL = payment_create;
                        if (task.PaymentURL != "" && task.PaymentURL != null)
                        {
                            #region SeleniumPayment
                            JObject Ret = Selenium_Driver.Payment(task.PaymentURL, Visacard, task.Number);
                            if (Ret != null)
                            {
                                if (Ret["status"].ToString() == "99")
                                    nsListView1.Items[Task].SubItems[1].Text = "จ่ายเงินไม่สำเร็จ (" + Ret["OTP"].ToString() + ")";
                                else
                                    nsListView1.Items[Task].SubItems[1].Text = "ซื้อหมายเลข " + PackageID[1] + " เรียบร้อย (" + Ret["OTP"].ToString() + ")";
                                nsListView1.Refresh();

                                if (LineToken != "")
                                    line_nof.lineNotify("ทำการซื้อเบอร์โทรศัพท์หมายเลข " + PackageID[1] + "เรียบร้อยแล้ว !", LineToken);

                                task.Working++;
                                return false;
                            }
                            #endregion
                        }
                    }
                    else
                    {
                        ls.Items[Task].SubItems[1].Text = "สร้างลิงค์จ่ายเงินล้มเหลว";
                        ls.Refresh();
                    }
                }
                else
                {
                    SettingIDSelect = Ran;
                    if (SettingIDSelect >= 0)
                    {
                        Info_Data.RemoveAt(SettingIDSelect);
                        nsListView2.RemoveItemAt(SettingIDSelect);
                        SettingIDSelect = -1;

                        for (int i = 0; i < nsListView2.Items.Count(); i++)
                        {
                            nsListView2.Items[i].Text = i.ToString();
                        }
                        SaveIDCardToFile();
                        nsListView2.Refresh();
                    }

                    ls.Items[Task].SubItems[1].Text = "ไม่สามารถยืนยันข้อมูลส่วนบุคคล";
                    ls.Refresh();
                }
            }
            else
            {
                if (task.Pause == true)
                {
                    ls.Items[Task].SubItems[1].Text = "หยุดทำงาน";
                    ls.Refresh();
                }
                else
                {
                    ls.Items[Task].SubItems[1].Text = "ไม่พบเบอร์โทรศัพท์ Retry 10 Seconds";
                    ls.Refresh();
                    Thread.Sleep(10000);
                    ls.Items[Task].SubItems[1].Text = "ค้นหาเบอร์โทรศัพท์";
                    ls.Refresh();
                    Thread.Sleep(2000);
                }
            }
            task.Working++;
            return true;
        }

        private void Add_Info_Click(object sender, EventArgs e)
        {
            var NameSplit = Regex.Split(nsTextBox4.Text, " ");
            if (NameSplit.Length > 1)
            {
                Identity Id = new Identity()
                {
                    idCard = nsTextBox6.Text,
                    firstname = NameSplit[0],
                    surname = NameSplit[1],
                    birthday = nsTextBox5.Text
                };
                Info_Data.Add(Id);
                nsListView2.AddItem((Info_Data.Count - 1).ToString(), Id.firstname + " " + Id.surname, Id.birthday, Id.idCard, "-");
                SaveIDCardToFile();
                MessageBox.Show("เพิ่มข้อมูลเรียบร้อยแล้ว", "Alert", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("กรุณาตรวจสอบข้อมูล ชื่อ นามสกุล", "Alert", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void nsButton1_Click(object sender, EventArgs e)
        {
            if (nsTextBox1.Text.Length >= 9)
            {
                var Reg = Regex.Split(nsTextBox1.Text, "-");
                if (Reg.Length > 2)
                {
                    nsTextBox1.Text = Reg[0] + Reg[1] + Reg[2];
                }

                if (nsTextBox1.Text.Length == 10)
                {

                    if (nsCheckBox1.Checked == true)
                    {
                        // 09184xxxxx
                        if (phone_task.Contains(nsTextBox1.Text) == false)
                        {
                            Buytask A = new Buytask()
                            {
                                Tasknum = Task_Working,
                                Number = nsTextBox1.Text,
                                Mask = true
                            };
                            phone_task.Add(nsTextBox1.Text);
                            Task_Buy.Add(A);
                            Task_Working++;
                            nsListView1.AddItem(A.Tasknum.ToString(), A.Number, "รอการทำงาน");
                            Thread t = new Thread(() => TaskBuy(A));
                            t.Start();
                        }
                        else
                        {
                            MessageBox.Show("หมายเลขนี้มีอยู่ในรายการทำงานแล้ว", "Alert", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    else
                    {
                        if (phone_task.Contains(nsTextBox1.Text) == false)
                        {
                            Buytask A = new Buytask()
                            {
                                Tasknum = Task_Working,
                                Number = nsTextBox1.Text,
                                Mask = false
                            };
                            phone_task.Add(A.Number);
                            Task_Buy.Add(A);
                            Task_Working++;
                            nsListView1.AddItem(A.Tasknum.ToString(), A.Number, "รอการทำงาน");
                            Thread t = new Thread(() => TaskBuy(A));
                            t.Start();
                        }
                        else
                        {
                            MessageBox.Show("หมายเลขนี้มีอยู่ในรายการทำงานแล้ว", "Alert", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
                else
                    MessageBox.Show("กรุณากรอกเบอร์โทรศัพท์ให้ถูกต้อง", "Alert", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                MessageBox.Show("กรุณากรอกเบอร์โทรศัพท์ให้ถูกต้อง", "Alert", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void nsListView2_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                try
                {
                    int Index = int.Parse(nsListView2.SelectedItems[0].Text);
                    SettingIDSelect = Index;
                    nsContextMenu1.Show(Cursor.Position);
                }
                catch (Exception) { SettingIDSelect = -1; }
            }
            else
                SettingIDSelect = -1;
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (SettingIDSelect >= 0)
            {
                Info_Data.RemoveAt(SettingIDSelect);
                nsListView2.RemoveItemAt(SettingIDSelect);
                SettingIDSelect = -1;

                for (int i = 0; i < nsListView2.Items.Count(); i++)
                {
                    nsListView2.Items[i].Text = i.ToString();
                }
                SaveIDCardToFile();
            }
        }

        private void ReadConfig_Address()
        {
            if (File.Exists("Address.ini"))
            {
                String Fs = File.ReadAllText("Address.ini");

                Info_Address.phone = getBetween(Fs.ToLower(), "phone=", ";");

                Info_Address.email = getBetween(Fs.ToLower(), "email=", ";");

                Info_Address.address = getBetween(Fs.ToLower(), "address=", ";");

                Info_Address.moo = getBetween(Fs.ToLower(), "moo=", ";");

                Info_Address.mooban = getBetween(Fs.ToLower(), "mooban=", ";");

                Info_Address.room = getBetween(Fs.ToLower(), "room=", ";");

                Info_Address.floor = getBetween(Fs.ToLower(), "floor=", ";");
                Info_Address.buildingname = getBetween(Fs.ToLower(), "buildingname=", ";");
                Info_Address.soi = getBetween(Fs.ToLower(), "soi=", ";");
                Info_Address.road = getBetween(Fs.ToLower(), "road=", ";");
                Info_Address.provincename = getBetween(Fs.ToLower(), "provincename=", ";");
                Info_Address.amphurname = getBetween(Fs.ToLower(), "amphurname=", ";");
                Info_Address.distictname = getBetween(Fs.ToLower(), "distictname=", ";");
                Info_Address.postcode = getBetween(Fs.ToLower(), "postcode=", ";");
                Info_Address.province_code = getBetween(Fs.ToLower(), "province_code=", ";");
                Info_Address.amphur_code = getBetween(Fs.ToLower(), "amphur_code=", ";");
                Info_Address.distict_code = getBetween(Fs.ToLower(), "distict_code=", ";");

                nsTextBox2.Text = Info_Address.phone;
                nsTextBox3.Text = Info_Address.email;
                nsTextBox7.Text = Info_Address.address;
                nsTextBox8.Text = Info_Address.moo;
                nsTextBox9.Text = Info_Address.mooban;
                nsTextBox10.Text = Info_Address.room;
                nsTextBox11.Text = Info_Address.floor;
                nsTextBox12.Text = Info_Address.buildingname;
                nsTextBox13.Text = Info_Address.soi;
                nsTextBox14.Text = Info_Address.road;
                nsTextBox15.Text = Info_Address.provincename;
                nsTextBox16.Text = Info_Address.amphurname;
                nsTextBox17.Text = Info_Address.distictname;
                nsTextBox18.Text = Info_Address.postcode;
                nsTextBox19.Text = Info_Address.province_code;
                nsTextBox20.Text = Info_Address.amphur_code;
                nsTextBox21.Text = Info_Address.distict_code;
            }
        }

        private void nsButton2_Click(object sender, EventArgs e)
        {
            String[] Address_Setting = new String[17];
            Address_Setting[0] = "Phone=" + nsTextBox2.Text + ";";
            Address_Setting[1] = "Email=" + nsTextBox3.Text + ";";
            Address_Setting[2] = "Address=" + nsTextBox7.Text + ";";
            Address_Setting[3] = "Moo=" + nsTextBox8.Text + ";";
            Address_Setting[4] = "Mooban=" + nsTextBox9.Text + ";";
            Address_Setting[5] = "Room=" + nsTextBox10.Text + ";";
            Address_Setting[6] = "Floor=" + nsTextBox11.Text + ";";
            Address_Setting[7] = "Buildingname=" + nsTextBox12.Text + ";";
            Address_Setting[8] = "Soi=" + nsTextBox13.Text + ";";
            Address_Setting[9] = "Road=" + nsTextBox14.Text + ";";
            Address_Setting[10] = "Provincename=" + nsTextBox15.Text + ";";
            Address_Setting[11] = "Amphurname=" + nsTextBox16.Text + ";";
            Address_Setting[12] = "Distictname=" + nsTextBox17.Text + ";";
            Address_Setting[13] = "Postcode=" + nsTextBox18.Text + ";";
            Address_Setting[14] = "province_code=" + nsTextBox19.Text + ";";
            Address_Setting[15] = "amphur_code=" + nsTextBox20.Text + ";";
            Address_Setting[16] = "distict_code=" + nsTextBox21.Text + ";";
            File.WriteAllLines("Address.ini", Address_Setting);
            ReadConfig_Address();
            MessageBox.Show("บันทึกข้อมูลเรียบร้อยแล้ว", "Alert", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void nsButton3_Click(object sender, EventArgs e)
        {
            String[] Visa = new String[5];
            Visa[0] = "number=" + nsTextBox38.Text + ";";
            Visa[1] = "month=" + nsTextBox37.Text + ";";
            Visa[2] = "year=" + nsTextBox22.Text + ";";
            Visa[3] = "cvv=" + nsTextBox23.Text + ";";
            File.WriteAllLines("Visacard.ini", Visa);
            MessageBox.Show("บันทึกข้อมูลเรียบร้อยแล้ว", "Alert", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            Exit = true;
            for (int i = 0; i < Selenium_Driver.Driver.Count(); i++)
            {
                try
                {

                    Selenium_Driver.Driver[i].Quit();
                    Selenium_Driver.Driver[i].Close();
                    Selenium_Driver.Driver[i].Dispose();
                }
                catch (Exception) { }
            }
            Process.GetCurrentProcess().Kill();
            Application.Exit();
            Application.ExitThread();
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("Chrome", "https://www.youtube.com/watch?v=K_pW6N1YzDs");
        }

        private void nsButton5_Click(object sender, EventArgs e)
        {
            LineToken = nsTextBox27.Text;
            line_nof.lineNotify("[Tester] Message From Aisbuynumber.", LineToken);
        }

        private void nsButton4_Click(object sender, EventArgs e)
        {
            LineToken = nsTextBox27.Text;
            File.WriteAllText("LineToken.ini", LineToken);
            MessageBox.Show("บันทึกข้อมูลเรียบร้อยแล้ว", "Alert", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (TaskSelect > -1)
            {
                Task_Buy[TaskSelect].Pause = true;
                TaskSelect = -1;
            }
        }

        private void nsListView1_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                try
                {
                    int Index = int.Parse(nsListView1.SelectedItems[0].Text);
                    TaskSelect = Index;
                    nsContextMenu2.Show(Cursor.Position);
                }
                catch (Exception) { TaskSelect = -1; }
            }
            else
                TaskSelect = -1;
        }

        private void startToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (TaskSelect > -1)
            {
                Task_Buy[TaskSelect].Pause = false;
                TaskSelect = -1;
            }
        }

        private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Setting_Num st = new Setting_Num();
            st.ShowDialog();
        }

        private void nsButton6_Click(object sender, EventArgs e)
        {
            String num = File.ReadAllText("Numsetting.txt");
            var R = Regex.Split(num, ",");
            for (int i = 0; i < R.Length; i++)
            {
                if (Num_Condition.Contains(R[i]) == false)
                {
                    Num_Condition.Add(R[i]);
                }
            }

            var Reg = Regex.Split(nsTextBox1.Text, "-");
            if (Reg.Length > 2)
            {
                nsTextBox1.Text = Reg[0] + Reg[1] + Reg[2];
            }

            if (nsTextBox24.Text.Length == 10)
            {
                if ((nsCheckBox2.Checked == false && nsCheckBox3.Checked == false) || num == "")
                {
                    MessageBox.Show("กรุณาตั้งค่า เงื่อนไข", "Alert", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    Buytask A = new Buytask()
                    {
                        Tasknum = Task_Working,
                        Number = nsTextBox24.Text,
                        Num_true = nsCheckBox2.Checked,
                        Num_False = nsCheckBox3.Checked
                    };
                    Task_Buy.Add(A);
                    Task_Working++;
                    nsListView1.AddItem(A.Tasknum.ToString(), A.Number + " (เงื่อนไข)", "รอการทำงาน");
                    Thread t = new Thread(() => TaskBuy(A));
                    t.Start();
                }
            }
            else
            {
                MessageBox.Show("กรุณากรอกเบอร์โทรศัพท์ให้ถูกต้อง", "Alert", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void nsButton8_Click(object sender, EventArgs e)
        {
            if (Pause_FindOdd == true)
            {
                nsButton8.Text = "Stop";
                nsButton8.Refresh();
                Pause_FindOdd = false;

                if (FindOdd.IsBusy == false)
                    FindOdd.RunWorkerAsync();
            }
            else if (Pause_FindOdd == false)
            {
                nsButton8.Text = "Start";
                nsButton8.Refresh();
                Pause_FindOdd = true;
            }
        }

        private void FindOdd_DoWork(object sender, DoWorkEventArgs e)
        {
            while (true)
            {
                Thread.Sleep(100);
                if (Pause_FindOdd == false)
                {

                }
            }
        }
    }
}