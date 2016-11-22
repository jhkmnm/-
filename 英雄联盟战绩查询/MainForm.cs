using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using stoneNamespace;
using System.Threading;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net;

namespace 英雄联盟战绩查询
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }
        // 我的功能函数
        static __Functions myFunctions = new __Functions();
        // 获取昵称转换成编码的网址
        static string getCodeUrl = "http://www.zhanjibao.com/core/search/player";//Dreamy%E7%81%ACBubles";
        // 获取战绩的链接
        string getMilitaryExploits = "http://www.zhanjibao.com/core/challenge/list";
        // 查询战绩数量
        static string MilExpENumber = "http://www.zhanjibao.com/core/game/count";
        // 查询网址
        static string searchUrl = "http://www.zhanjibao.com/overview.html"; // 通过此网址，来获取cookies
        // Dictionary<区,id>
        Dictionary<string, int> dicAreaId = new Dictionary<string, int>();
        // Dictionary<区_昵称,qquin>
        Dictionary<string, string> dicName = new Dictionary<string, string>();
        string strDir = myFunctions.GetExePath() + "data";
        // 保存data的TXT文件路径
        string filename = myFunctions.GetExePath() + "data.txt";
        // 线程同步锁
        private Object thisLock = new Object();
        // 网站cookies
        string cookies = "";
        // 主窗口加载函数
        private void MainForm_Load(object sender, EventArgs e)
        {
            comboBox1.SelectedIndex = 0;
            InitAredID();
            IniListView();
            //ReadSearchData();


        }

        private void ReadSearchData()
        {
            try
            {
                StreamReader sr = new StreamReader(filename, Encoding.UTF8);
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    char[] ch = new char[]{'_'};
                    string[] split = line.Split(ch, 3);
                    if (split.Length == 3)
                    {
                        string area_nickname = split[0] + '_' + split[1];
                        dicName.Add(area_nickname, split[2]);
                        ListViewItem lvi = new ListViewItem();
                        lvi.Text = split[1];
                        lvi.SubItems.Add(split[0]);
                        //this.listView1.Items.Add(lvi);
                    }
                }
                sr.Close();
            }
            catch (IOException e)
            {
                //MessageBox.Show(ToString(), "提示信息");
            }
        }
        // 初始化列表控件
        private void IniListView()
        {            
            //this.listView1.Columns.Add("游戏角色名", 100, HorizontalAlignment.Center);
            //this.listView1.Columns.Add("游戏大区", 100, HorizontalAlignment.Center);
            //this.listView1.Columns.Add("起始段位", 100, HorizontalAlignment.Center);
            //this.listView1.Columns.Add("当前段位", 100, HorizontalAlignment.Center);

            //int military = Int32.Parse(textBox2.Text);
            //for (int i = 0; i < military; i++)
            //{
            //    this.listView1.Columns.Add("战绩"+(i+1).ToString(), 100, HorizontalAlignment.Center);
            //}
        }
        // 限制文本框只能输入数字
        private void textBox2_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!Char.IsNumber(e.KeyChar) && e.KeyChar != (char)8)  // 允许输入退格键
            {
                e.Handled = true;   // 经过判断为数字，可以输入
            }
        }
        // 获取账号的qquin
        public delegate void ParameterizedThreadStart(object objID, object objName);   //ParameterizedThreadStart委托的声明
        private void button1_Click(object sender, EventArgs e)
        {// 锦帝丶,Dreamy灬Bubles, 
            // 获取Cookies

            string strAreaName = comboBox1.Text;
            string strParam = textBox1.Text;
            strParam = strParam.Trim();
            if (strParam.Length == 0)
            {
                MessageBox.Show("请输入召唤师名称！", "错误提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            Thread thread = new Thread(() => ThreadGetqquin(strAreaName, strParam));
            thread.Start();
        }
        private delegate void FlushClient(ListViewItem lvi);    // 代理【类似函数指针】
        private void ThreadGetqquin(string area, string nickname)
        {
            string area_nickname = area + "_" + nickname;
            if (FilaNameIsInList(area_nickname))
            {
                MessageBox.Show("当前召唤师已存在于列表中！", "提示信息", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            // 把昵称转换成
            string error = "";
            string strBuffer = myFunctions.PostWebRequest(getCodeUrl, "nickname=" + nickname, Encoding.UTF8, ref error);
            string qquin = "";
            int id = dicAreaId[area];
            if (strBuffer.Length == 0)
            {
                MessageBox.Show(error, "提示信息", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            qquin = GetQquinById(id, strBuffer);
        }

        private void AddItemToListView(ListViewItem lvi)
        {
            if (lvi != null)
            {
                //this.listView1.Items.Add(lvi);
            }
        }

        private bool FilaNameIsInList(string filename)
        {
            foreach (string key in dicName.Keys)
            {
                if (key == filename)
                {
                    return true;
                }
            }
            return false;
        }
        // POST提交数据
        private void PostUrl(object obj)
        {
            string strParam = obj as string;
            string error = "";
            string strBuffer = myFunctions.PostWebRequest(getCodeUrl, "nickname=" + strParam, Encoding.UTF8, ref error);
            if (strBuffer.Length > 0)
            {
                string strAreaName = comboBox1.Text;
                int nId = dicAreaId[strAreaName];
                string qquin = GetQquinById(nId, strBuffer);
                if (qquin.Length == 0)
                {
                    MessageBox.Show("查询失败！", "提示信息", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }
        }
        // 从josn中获取qquin
        private static string GetQquinById(int id, string strBuffer)
        {
            JObject jo = JObject.Parse(strBuffer);
            string[] values = jo.Properties().Select(item => item.Value.ToString()).ToArray();

            string qquin = "";
            string area_id = "\"area_id\":" + id.ToString() + ",";
            int nPos = strBuffer.IndexOf(area_id);
            if (nPos < 0) return "";
            nPos = strBuffer.IndexOf("qquin", nPos);
            if (nPos == -1) return "";
            nPos += (5 + 3);
            int nEnd = strBuffer.IndexOf('\"', nPos);
            if (nEnd == -1) return "";
            qquin = strBuffer.Substring(nPos, nEnd - nPos);
            return qquin;
        }
        // 初始化大区ID
        private void InitAredID()
        {
            dicAreaId.Add("艾欧尼亚", 1);
            dicAreaId.Add("祖安", 3);
            dicAreaId.Add("诺克萨斯", 4);
            dicAreaId.Add("比尔吉沃特", 2);
            dicAreaId.Add("德玛西亚", 6);
            dicAreaId.Add("弗雷尔卓德", 9);
            dicAreaId.Add("班德尔城", 5);
            dicAreaId.Add("皮尔特沃夫", 7);
            dicAreaId.Add("战争学院", 8);
            dicAreaId.Add("巨神峰", 10);
            dicAreaId.Add("雷瑟守备", 11);
            dicAreaId.Add("无畏先锋", 12);
            dicAreaId.Add("钢铁烈阳", 17);
            dicAreaId.Add("裁决之地", 13);
            dicAreaId.Add("黑色玫瑰", 14);
            dicAreaId.Add("暗影岛", 15);
            dicAreaId.Add("恕瑞玛", 16);
            dicAreaId.Add("均衡教派", 19);
            dicAreaId.Add("水晶之痕", 18);
            dicAreaId.Add("扭曲丛林", 20);
            dicAreaId.Add("教育网专区", 21);
            dicAreaId.Add("影流", 22);
            dicAreaId.Add("守望之海", 23);
            dicAreaId.Add("征服之海", 24);
            dicAreaId.Add("卡拉曼达", 25);
            dicAreaId.Add("巨龙之巢", 26);
            dicAreaId.Add("皮城警备", 27);
        }

        //WebBrowser browser = new WebBrowser();

        // 获取cookies
        private void button3_Click(object sender, EventArgs e)
        {
            button3.Enabled = false;

            //召唤师 user1 = new 召唤师("9天虹诺德牛排杯", "1");
            //var data = user1.查询战绩(7);

            //Search("1", "9天虹诺德牛排杯");

            button3.Enabled = true;
        }
    }
}
/*
 {"body":{"data":[{"area_id":18,"qquin":"Et2GuU0tNIFxPQ5oM5wJDiKChrdofpX3","icon_id":16,"name":"锦帝丶","level"
:3,"tier":255,"queue":255,"win_point":0},{"area_id":1,"qquin":"H8hnf5d+i9Hw9+lL1uGD4urfOPKRGbct","icon_id"
:1270,"name":"锦帝丶","level":30,"tier":1,"queue":4,"win_point":47},{"area_id":14,"qquin":"vQCOBe80YB5gQhi2bQ8yxCLpCaOPc0qM"
,"icon_id":25,"name":"锦帝丶","level":1,"tier":255,"queue":255,"win_point":0}],"retCode":"0","msg":""}}
 */
/*
                <option value="1">艾欧尼亚</option>
                <option value="3">祖安</option>
                <option value="4">诺克萨斯</option>
                <option value="2">比尔吉沃特</option>
                <option value="6">德玛西亚</option>
                <option value="9">弗雷尔卓德</option>
                <option value="5">班德尔城</option>
                <option value="7">皮尔特沃夫</option>
                <option value="8">战争学院</option>
                <option value="10">巨神峰</option>
                <option value="11">雷瑟守备</option>
                <option value="12">无畏先锋</option>
                <option value="17">钢铁烈阳</option>
                <option value="13">裁决之地</option>
                <option value="14">黑色玫瑰</option>
                <option value="15">暗影岛</option>
                <option value="16">恕瑞玛</option>
                <option value="19">均衡教派</option>
                <option value="18">水晶之痕</option>
                <option value="20">扭曲丛林</option>
                <option value="21">教育网专区</option>
                <option value="22">影流</option>
                <option value="23">守望之海</option>
                <option value="24">征服之海</option>
                <option value="25">卡拉曼达</option>
                <option value="26">巨龙之巢</option>
                <option value="27">皮城警备</option>
 */