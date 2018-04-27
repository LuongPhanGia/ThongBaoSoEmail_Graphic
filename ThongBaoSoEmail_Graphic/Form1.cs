using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Xml;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Net;

namespace ThongBaoSoEmail_Graphic
{
    public partial class Form1 : Form
    {
        public class IgnoreBadCert : ICertificatePolicy
        {
            public bool CheckValidationResult(ServicePoint sp,
            X509Certificate cert, WebRequest request, int err)
            {
                return true;
            }
        }
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string username = txtUserName.Text;
            string pass = txtPassword.Text;
#pragma warning disable CS0618 // Type or member is obsolete
            ServicePointManager.CertificatePolicy = new IgnoreBadCert();
#pragma warning restore CS0618 // Type or member is obsolete
            NetworkCredential cred = new NetworkCredential();
            cred.UserName = username;
            cred.Password = pass;

            WebRequest webr = WebRequest.Create("https://mail.google.com/mail/feed/atom");
            webr.Credentials = cred;
            Stream stream = webr.GetResponse().GetResponseStream();
            StreamReader sr = new StreamReader(stream);
            string s = sr.ReadToEnd();
            s = s.Replace("<feed version=\"0.3\" xmlns =\"http://purl.org/atom/ns#\">", @"<feed>");
            StreamWriter sw = new StreamWriter("emaildata.txt");
            sw.Write(s);
            sr.Close();
            sw.Close();

            XmlDocument xmldoc = new XmlDocument();
            xmldoc.Load("emaildata.txt");
            string nr = xmldoc.SelectSingleNode(@"/feed/fullcount").InnerText;

            MessageBox.Show("Count: " + nr);
            foreach (XmlNode node in xmldoc.SelectNodes(@"/feed/entry"))
            {
                string title = node.SelectSingleNode("title").InnerText;
                string summary = node.SelectSingleNode("summary").InnerText;
                MessageBox.Show(title + "\n" + summary + "\n");
            }
        }
    }
}
