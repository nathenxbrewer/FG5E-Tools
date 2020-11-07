using Critter2FG.Character;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Serialization;
using System.Xml.Linq;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Linq;
using System.Security.AccessControl;
using Microsoft.Win32.SafeHandles;

namespace Critter2FG
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string URL = "https://critterdb.com/api/publishedbestiaries/5ea4b8b463a0580dfd76d379/creatures/:page";
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(URL);
            request.ContentType = "application/json; charset=utf-8";
            request.Headers["Authorization"] = "Basic " + Convert.ToBase64String(Encoding.GetEncoding("ISO-8859-1").GetBytes("username:password"));
            request.PreAuthenticate = true;
            HttpWebResponse response = request.GetResponse() as HttpWebResponse;
            Stream stream = response.GetResponseStream();
            StreamReader reader = new StreamReader(stream, Encoding.UTF8);
            Console.WriteLine(reader.ReadToEnd());
        }

        private void btnCreate_Click(object sender, EventArgs e)
        {
            // StatBlockDisplay test = new StatBlockDisplay();
            //test.name = "Aaric sucks";
            //panel1.Controls.Add(test);

            // MessageBox.Show(test.name);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            StatBlockDisplay testdisplay = new StatBlockDisplay();
            panel1.Controls.Add(testdisplay);

            testdisplay.ExportToXML("test");
           // AbilityScore test = new AbilityScore();

        }




 


        private void Form1_Load(object sender, EventArgs e)
        {
            
        }

        private void btnCharacterParse_Click(object sender, EventArgs e)
        {
            //AbilityScore score = new AbilityScore("testscore", 17);

            //score.addBonus(4);
            //ParseCharacter();



        }
    }
}
