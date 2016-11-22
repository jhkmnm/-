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
            Data.战绩 = Search(count);
            return Data;
        }

        private List<Zhanji> Search(int count)
        {
            if (string.IsNullOrWhiteSpace(Data.账号信息.WebUrl))
            {
                string url = string.Format("http://lol.te5.com/db/so.html?a={0}&k={1}", Data.账号信息.Server, System.Web.HttpUtility.UrlEncode(Data.账号信息.Name));
                browser.Navigate(url);
                while (browser.ReadyState != WebBrowserReadyState.Complete)
                {
                    Application.DoEvents();
                }

                var table = browser.Document.GetElementById("matchlists");
                if (table == null)
                {
                    return new List<Zhanji>();
                }
                Data.账号信息.WebUrl = browser.Url.ToString();
                return GetMatchlists("", count);
            }
            else
            {
                return GetMatchlists(Data.账号信息.WebUrl, count);
            }
            //var rows = table.GetElementsByTagName("tr");
            //var tds = rows.GetElementsByName("td");
            //var duanwei = tds[3].OuterText;
            //var zhanji = tds[5].All[0].GetAttribute("href");
        }

        private List<Zhanji> GetMatchlists(string url, int count)
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
            List<Zhanji> data = new List<Zhanji>(); 
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
                        data.Add(new Zhanji { Jieguo = result, Shijian = time, GameID = "", Name = Data.账号信息.Name });
                    }
                }
            }

            return data;
        }
    }

    public class 战绩数据
    {
        //public string Name { get; set; }

        //public string Server { get; set; }

        //public List<战绩> Data { get; set; }

        //public int Index { get; set; }

        //public string WebUrl { get; set; }
        public GameAccount 账号信息 { get; set; }
        public List<Zhanji> 战绩 { get; set; }

        public double Shenlu
        { 
            get 
            {
                return 战绩.Select(a => a.Jieguo == "胜利").Count() / (战绩.Count * 1.0);
            } 
        }

        public string Shijian
        {
            get 
            {
                var t = (DateTime.Now.AddDays(-3) - Convert.ToDateTime(账号信息.Time));
                return string.Format("{0:00}:{1:00}", t.Hours, t.Minutes);                
            }
        }
    }
}
