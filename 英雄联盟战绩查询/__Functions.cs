using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
//using System.Threading.Tasks;
using Microsoft.Win32;
using System.Drawing.Drawing2D;
using System.Drawing;
using System.Net;
using System.IO;

namespace stoneNamespace
{
    class __Functions
    {
        /// 获取日期间隔【单位：天】
        public int GetDays(int nYear, int nMonth, int nDay)
        {
            int nD = DateTime.Now.Day;
            int nM = DateTime.Now.Month;
            int nY = DateTime.Now.Year;
            int nDays = 365 * (nY - nYear) + 30 * (nM - nMonth) + (nD - nDay);
            return nDays;
        }

        public string PostWebRequest(string postUrl, string paramData, Encoding dataEncode, ref string strError)
        {
            string ret = string.Empty;
            try
            {
                byte[] byteArray = dataEncode.GetBytes(paramData); //转化
                HttpWebRequest webReq = (HttpWebRequest)WebRequest.Create(new Uri(postUrl));
                webReq.Method = "POST";
                webReq.ContentType = "application/x-www-form-urlencoded";

                webReq.ContentLength = byteArray.Length;
                Stream newStream = webReq.GetRequestStream();
                newStream.Write(byteArray, 0, byteArray.Length);//写入参数
                newStream.Close();
                HttpWebResponse response = (HttpWebResponse)webReq.GetResponse();
                StreamReader sr = new StreamReader(response.GetResponseStream(), dataEncode);
                ret = sr.ReadToEnd();
                sr.Close();
                response.Close();
                newStream.Close();
            }
            catch (Exception ex)
            {
                strError = ex.ToString();
            }
            return ret;
        }
        /// 读取网页内容
        public string ReadUrl(string strUrl)
        {
            string strBuff = "";
            char[] cbuffer = new char[256];
            int byteRead = 0; 
            HttpWebRequest httpReq;
            HttpWebResponse httpResp;

            Uri httpURL = new Uri(strUrl);
            ///HttpWebRequest类继承于WebRequest，并没有自己的构造函数，需通过WebRequest的Creat方法 建立，并进行强制的类型转换
            httpReq = (HttpWebRequest)WebRequest.Create(httpURL);
            ///通过HttpWebRequest的GetResponse()方法建立HttpWebResponse,强制类型转换
            httpResp = (HttpWebResponse)httpReq.GetResponse();
            ///GetResponseStream()方法获取HTTP响应的数据流,并尝试取得URL中所指定的网页内容
            ///若成功取得网页的内容，则以System.IO.Stream形式返回，若失败则产生ProtoclViolationException错误。
            ///在此正确的做法应将以下的代码放到一个try块中处理。这里简单处理
            Stream respStream = httpResp.GetResponseStream();
            ///返回的内容是Stream形式的，所以可以利用StreamReader类获取GetResponseStream的内容，
            ///并以StreamReader类的Read方法依次读取网页源程序代码每一行的内容，直至行尾（读取的编码格式：UTF8）
            StreamReader respStreamReader = new StreamReader(respStream,Encoding.UTF8);
            byteRead = respStreamReader.Read(cbuffer, 0, 256);
            while (byteRead != 0)
            {
                string strResp = new string(cbuffer, 0, byteRead);
                strBuff = strBuff + strResp;
                byteRead = respStreamReader.Read(cbuffer, 0, 256);
            }
            respStream.Close();

            return strBuff;
        }
        /// 获取字符串中，指定字符串之前的字符串
        public string GetBeforeString(string allText, string pos)
        {
            int nPos = allText.IndexOf(pos);
            if (nPos >= 0)
            {
                return allText.Substring(0, nPos);
            }
            return "";
        }
        /// 获取字符串中，指定字符串之后的字符串
        public string GetAfterString(string allText, string pos)
        {
            int nPos = allText.IndexOf(pos);
            if (nPos >= 0)
            {
                return allText.Substring(nPos + pos.Length);
            }
            return "";
        }
        /// <summary>
        /// 打开超链接
        /// </summary>
        public void OpenUrl(string url)
        {
            if (url.Length > 0)
            {
                System.Diagnostics.Process.Start(url);
            }
        }
        /// 设置按钮为平面透明等特性
        public void SetButtonStytle(System.Windows.Forms.Button bt)
        {
            bt.FlatStyle = System.Windows.Forms.FlatStyle.Flat;      //样式    
            bt.ForeColor = Color.Transparent;   //前景
            bt.BackColor = Color.Transparent;   //去背景 
            bt.FlatAppearance.BorderSize = 0;   //去边线 
            bt.FlatAppearance.MouseDownBackColor = Color.Transparent;   //鼠标经过 
            bt.FlatAppearance.MouseOverBackColor = Color.Transparent;   //鼠标按下
        }
        // 获取MD5值
        public static string GetMD5Hash(String sDataIn)
        {
            System.Security.Cryptography.MD5CryptoServiceProvider md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
            byte[] bytValue, bytHash;
            bytValue = System.Text.Encoding.UTF8.GetBytes(sDataIn);
            bytHash = md5.ComputeHash(bytValue);
            md5.Clear();
            string sTemp = "";
            for (int i = 0; i < bytHash.Length; i++)
            {
                sTemp += bytHash[i].ToString("X").PadLeft(2, '0');
            }
            return sTemp.ToLower();
        }
        // 绘制圆形矩形
        public static void DrawRoundRectangle(Graphics g, Pen pen, Rectangle rect, int cornerRadius)
        {
            using (GraphicsPath path = CreateRoundedRectanglePath(rect, cornerRadius))
            {
                g.DrawPath(pen, path);
            }
        }
        // 填充圆角矩形
        public static void FillRoundRectangle(Graphics g, Brush brush, Rectangle rect, int cornerRadius)
        {
            using (GraphicsPath path = CreateRoundedRectanglePath(rect, cornerRadius))
            {
                g.FillPath(brush, path);
            }
        }
        // 圆角绘制、填充的方法
        internal static GraphicsPath CreateRoundedRectanglePath(Rectangle rect, int cornerRadius)
        {
            GraphicsPath roundedRect = new GraphicsPath();
            roundedRect.AddArc(rect.X, rect.Y, cornerRadius * 2, cornerRadius * 2, 180, 90);
            roundedRect.AddLine(rect.X + cornerRadius, rect.Y, rect.Right - cornerRadius * 2, rect.Y);
            roundedRect.AddArc(rect.X + rect.Width - cornerRadius * 2, rect.Y, cornerRadius * 2, cornerRadius * 2, 270, 90);
            roundedRect.AddLine(rect.Right, rect.Y + cornerRadius * 2, rect.Right, rect.Y + rect.Height - cornerRadius * 2);
            roundedRect.AddArc(rect.X + rect.Width - cornerRadius * 2, rect.Y + rect.Height - cornerRadius * 2, cornerRadius * 2, cornerRadius * 2, 0, 90);
            roundedRect.AddLine(rect.Right - cornerRadius * 2, rect.Bottom, rect.X + cornerRadius * 2, rect.Bottom);
            roundedRect.AddArc(rect.X, rect.Bottom - cornerRadius * 2, cornerRadius * 2, cornerRadius * 2, 90, 90);
            roundedRect.AddLine(rect.X, rect.Bottom - cornerRadius * 2, rect.X, rect.Y + cornerRadius * 2);
            roundedRect.CloseFigure();
            return roundedRect;
        }

        public string GetExePath()
        {
            return System.AppDomain.CurrentDomain.BaseDirectory;
        }
        /// <summary>
        /// 管理任务管理器的方法
        /// </summary>
        /// <param name="arg">0：启用任务管理器 1：禁用任务管理器</param>
        public void ManageTaskManager(int arg)
        {
            RegistryKey currentUser = Registry.CurrentUser;
            RegistryKey system = currentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Policies\System", true);
            //如果system项不存在就创建这个项
            if (system == null)
            {
                system = currentUser.CreateSubKey(@"Software\Microsoft\Windows\CurrentVersion\Policies\System");
            }
            system.SetValue("DisableTaskmgr", arg, RegistryValueKind.DWord);
            currentUser.Close();
        }
        // 设置软件开机开启
        public void SetAutoRun(string fileName, bool isAutoRun)
        {
            RegistryKey reg = null;
            try
            {
                if (!System.IO.File.Exists(fileName))
                    throw new Exception("该文件不存在!");
                String name = fileName.Substring(fileName.LastIndexOf(@"\") + 1);
                reg = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", true);
                if (reg == null)
                    reg = Registry.LocalMachine.CreateSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run");
                if (isAutoRun)
                    reg.SetValue(name, fileName);
                else
                    reg.SetValue(name, false);
            }
            catch
            {
                //throw new Exception(ex.ToString());   
            }
            finally
            {
                if (reg != null)
                    reg.Close();
            }
        }
    }
}
