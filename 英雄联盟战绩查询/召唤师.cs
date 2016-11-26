﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using HtmlAgilityPack;

namespace 英雄联盟战绩查询
{
    public class 召唤师
    {        
        public 战绩数据 Data { get; set; }
        private bool isInsert = false;
        HtmlWeb web = new HtmlWeb();
        HtmlDocument document = new HtmlDocument();

        public 召唤师()
        {

        }

        public void 查询战绩()
        {
            isInsert = !(Data.战绩 == null || Data.战绩.Count == 0);
            if (Data.战绩 == null)
            {
                Data.战绩 = new List<Zhanji>();
            }
            Search();
        }

        private void Search()
        {
            if (string.IsNullOrWhiteSpace(Data.账号信息.WebUrl))
            {
                string url = string.Format("http://lol.te5.com/db/so.html?a={0}&k={1}", Data.账号信息.Server, System.Web.HttpUtility.UrlEncode(Data.账号信息.Name));

                document = web.Load(url);

                var span = document.DocumentNode.SelectSingleNode("/html[1]/body[1]/div[2]/ul[1]/li[2]/strong[1]");
                if (span != null)
                {
                    if (span.InnerText == Data.账号信息.Name)
                    {
                        Data.账号信息.WebUrl = web.ResponseUri.AbsoluteUri;

                        GetMatchlists("");
                    }
                }
            }
            else
            {
                GetMatchlists(Data.账号信息.WebUrl);
            }
        }

        private void GetMatchlists(string url)
        {
            if (!string.IsNullOrWhiteSpace(url))
            {
                document = web.Load(url);
            }

            var span = document.DocumentNode.SelectSingleNode("//span[@class = 'f16']");
            if (span != null && !string.IsNullOrWhiteSpace(span.InnerText))
                Data.账号信息.Duanwei = span.InnerText.Replace("&nbsp;", " ");

            HtmlNodeCollection trs = document.DocumentNode.SelectNodes("//*[@id='matchlists']//tbody//tr");
            if (trs != null)
            {
                foreach(HtmlNode tr in trs)
                {
                    var tds = tr.ChildNodes.Where(w => w.Name == "td").ToList();
                    
                    var result = tds[2].InnerText;
                    var time = tds[3].InnerText;
                    if (Convert.ToDateTime(time) >= Convert.ToDateTime(Data.账号信息.Time))
                    {
                        var href = tds[4].Element("a").Attributes["href"].Value;

                        var gameid = href == null ? "" : href.Substring(href.LastIndexOf("=") + 1);
                        if (!Data.战绩.Exists(a => a.GameID == gameid) && !string.IsNullOrWhiteSpace(gameid))
                        {
                            var z = new Zhanji { GameID = gameid, Jieguo = result, Shijian = time, Name = Data.账号信息.Name };

                            if (isInsert)
                                Data.战绩.Insert(0, z);
                            else
                                Data.战绩.Add(z);
                        }
                    }
                }                
            }
        }
    }

    public class 战绩数据
    {
        public GameAccount 账号信息 { get; set; }
        public List<Zhanji> 战绩 { get; set; }

        public string Shenlu
        {
            get
            {
                var v = 战绩.Where(a => a.Jieguo == "胜利").Count();

                return string.Format("{0:0.00%}", (v / (战绩.Count * 1.0)));
            }
        }

        public string Shijian
        {
            get
            {
                var t = (DateTime.Now - Convert.ToDateTime(账号信息.Time));
                return string.Format("{0:00}:{1:00}", t.Hours, t.Minutes);
            }
        }
    }
}
