using mshtml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Windows.Forms;

namespace 英雄联盟战绩查询
{
    public class 召唤师
    {
        private WebBrowser browser;
        public 战绩数据 Data { get; set; }

        //public string Name { get; set; }

        //public string Server { get; set; }

        //public string WebUrl { get; set; }

        //public bool IsUser { get; set; }

        //public int Index { get; set; }

        public 召唤师()
        {
            browser = new WebBrowser();
            browser.DocumentCompleted += Browser_DocumentCompleted;
            browser.ScriptErrorsSuppressed = true;
        }

        private void Browser_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            IHTMLDocument2 vDocument = (IHTMLDocument2)browser.Document.DomDocument;
            vDocument.parentWindow.execScript("function confirm(str){return true;} ", "javascript"); //弹出确认
            vDocument.parentWindow.execScript("function alert(str){return true;} ", "javaScript");//弹出提示
        }

        public 战绩数据 查询战绩(int count)
        {            
            Data.Data = Search(count);            
            return Data;
        }

        private List<战绩> Search(int count)
        {
            if (string.IsNullOrWhiteSpace(Data.WebUrl))
            {
                string url = string.Format("http://lol.te5.com/db/so.html?a={0}&k={1}", Data.Server, System.Web.HttpUtility.UrlEncode(Data.Name));
                browser.Navigate(url);
                while (browser.ReadyState != WebBrowserReadyState.Complete)
                {
                    Application.DoEvents();
                }

                var table = browser.Document.GetElementById("matchlists");
                if (table == null)
                {
                    return new List<战绩>();
                }
                Data.WebUrl = browser.Url.ToString();
                return GetMatchlists("", count);
            }
            else
            {
                return GetMatchlists(Data.WebUrl, count);
            }
            //var rows = table.GetElementsByTagName("tr");
            //var tds = rows.GetElementsByName("td");
            //var duanwei = tds[3].OuterText;
            //var zhanji = tds[5].All[0].GetAttribute("href");
        }

        private List<战绩> GetMatchlists(string url, int count)
        {
            if(!string.IsNullOrWhiteSpace(url))
            {
                browser.Navigate(url);
                while (browser.ReadyState != WebBrowserReadyState.Complete)
                {
                    Application.DoEvents();
                }
            }
            var table = browser.Document.GetElementById("matchlists");
            List<战绩> data = new List<战绩>(); 
            if (table != null)
            {                
                var rows = table.GetElementsByTagName("tr");
                if (rows.Count > 1)
                {
                    for (int i = 1; i < (count < rows.Count ? count : rows.Count); i++)
                    {
                        //var heroname = rows[i].GetElementsByTagName("a")[0].OuterText;
                        //var model = rows[i].Children[1].OuterText;
                        var result = rows[i].Children[2].OuterText;
                        var time = rows[i].Children[3].OuterText;
                        data.Add(new 战绩 { 结果 = result, 时间 = time });
                    }
                }
            }

            return data;
        }
    }

    public class 战绩数据
    {
        public string Name { get; set; }

        public string Server { get; set; }

        public List<战绩> Data { get; set; }

        public int Index { get; set; }

        public string WebUrl { get; set; }        
    }

    public class 战绩
    {
        public string 结果 { get; set; }

        public string 时间 { get; set; }
    }
}
