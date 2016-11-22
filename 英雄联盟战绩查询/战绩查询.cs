using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Media;

namespace 英雄联盟战绩查询
{
    public partial class 战绩查询 : Form
    {
        private Queue<召唤师> 召唤师队列 = new Queue<召唤师>();
        private string filename = System.AppDomain.CurrentDomain.BaseDirectory + "data.txt";
        private static Object thisLock = new Object();
        public delegate void UpdateControl(战绩数据 data);
        List<Area> areas = new List<Area>();
        SoundPlayer player = new SoundPlayer();
        private List<string> usernames = new List<string>();
        int count = 8;
        DateTime begintime;
        int Lost = 3;
        DAL dal = new DAL();
        List<战绩数据> DataList = new List<战绩数据>();

        public 战绩查询()
        {
            InitializeComponent();

            this.WindowState = FormWindowState.Maximized;

            txtCount.Text = GetAppConfig("Count");
            txtValer.Text = GetAppConfig("Interval");
            count = Convert.ToInt32(txtCount.Text);
            Lost = Convert.ToInt32(txtLost.Text);

            InitAredID();
            InitDataGridCol();
            ReadData();
            player.SoundLocation = "7989.wav";
            player.Load();
        }

        private void InitAredID()
        {
            areas.AddRange(new[] {
                new Area() {AreaName ="艾欧尼亚", AreaID = "1" },
                new Area() {AreaName ="祖安", AreaID = "3" },
                new Area() {AreaName ="诺克萨斯", AreaID = "4" },
                new Area() { AreaName = "比尔吉沃特", AreaID = "2" },
                new Area() {AreaName ="德玛西亚", AreaID = "6" },
                new Area() {AreaName ="弗雷尔卓德", AreaID = "9" },
                new Area() {AreaName ="班德尔城", AreaID = "5" },
                new Area() {AreaName ="皮尔特沃夫", AreaID = "7" },
                new Area() {AreaName ="战争学院",AreaID = "8" },
                new Area() {AreaName ="巨神峰",AreaID = "10" },
                new Area() {AreaName ="雷瑟守备",AreaID = "11" },
                new Area() {AreaName ="无畏先锋",AreaID = "12" },
                new Area() {AreaName ="钢铁烈阳",AreaID = "17" },
                new Area() {AreaName ="裁决之地",AreaID = "13" },
                new Area() {AreaName ="黑色玫瑰",AreaID = "14" },
                new Area() {AreaName ="暗影岛",AreaID = "15" },
                new Area() {AreaName ="恕瑞玛",AreaID = "16" },
                new Area() {AreaName ="均衡教派",AreaID = "19" },
                new Area() {AreaName ="水晶之痕",AreaID = "18" },
                new Area() {AreaName ="扭曲丛林",AreaID = "20" },
                new Area() {AreaName ="教育网专区",AreaID = "21" },
                new Area() {AreaName ="影流",AreaID = "22" },
                new Area() {AreaName ="守望之海",AreaID = "23" },
                new Area() {AreaName ="征服之海",AreaID = "24" },
                new Area() {AreaName ="卡拉曼达",AreaID = "25" },
                new Area() {AreaName ="巨龙之巢",AreaID = "26" },
                new Area() {AreaName ="皮城警备",AreaID = "27" }
            });
            dicAreaId.DataSource = areas;
            dicAreaId.DisplayMember = "AreaName";
            dicAreaId.ValueMember = "AreaID";
        }

        private void InitDataGridCol()
        {
            dgvData.Columns.AddRange(new[] {
                new DataGridViewCheckBoxColumn() { Name = "colSelected", HeaderText = "选择", DataPropertyName = "Selected", Width = 40, SortMode = DataGridViewColumnSortMode.NotSortable }                
            });

            DataGridViewCellStyle dgvStyle = new DataGridViewCellStyle();
            dgvStyle.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dgvStyle.WrapMode = System.Windows.Forms.DataGridViewTriState.True;

            dgvData.Columns.AddRange(new[] {                
                new DataGridViewTextBoxColumn() { Name = "colName", HeaderText = "游戏ID", DataPropertyName = "Name", ReadOnly = true, Width = 100, SortMode = DataGridViewColumnSortMode.NotSortable },
                new DataGridViewTextBoxColumn() { Name = "colServer", HeaderText = "游戏大区", DataPropertyName = "Server", ReadOnly = true, Width = 100, SortMode = DataGridViewColumnSortMode.NotSortable },
                new DataGridViewTextBoxColumn() { Name = "colShenLU", HeaderText = "当前胜率", DataPropertyName = "Shenlu", ReadOnly = true, Width = 100, SortMode = DataGridViewColumnSortMode.NotSortable },
                new DataGridViewTextBoxColumn() { Name = "colDuanwei", HeaderText = "当前段位", DataPropertyName = "Duanwei", ReadOnly = true, Width = 100, SortMode = DataGridViewColumnSortMode.NotSortable },
                new DataGridViewTextBoxColumn() { Name = "colWebUrl", HeaderText = "账号地址", DataPropertyName = "WebUrl", ReadOnly = true, Visible = false, SortMode = DataGridViewColumnSortMode.NotSortable },
                new DataGridViewTextBoxColumn() { Name = "colTime", HeaderText = "开始时间", DataPropertyName = "Time", ReadOnly = true, Visible = false, SortMode = DataGridViewColumnSortMode.NotSortable }
            });

            for (int i = 1; i <= count; i++)
            {
                dgvData.Columns.AddRange(new[] {
                    new DataGridViewTextBoxColumn() { Name = "colData"+i.ToString(), HeaderText = "战绩" + i.ToString(), DataPropertyName = "Data", ReadOnly = true, Width = 120, DefaultCellStyle = dgvStyle, SortMode = DataGridViewColumnSortMode.NotSortable }
                });
            }

            dgvData.Columns.AddRange(new[] {
                    new DataGridViewTextBoxColumn() { Name = "colShijian", HeaderText = "时间计时", DataPropertyName = "Shijian", ReadOnly = true, Width = 120, SortMode = DataGridViewColumnSortMode.NotSortable },
                    new DataGridViewTextBoxColumn() { Name = "colBeizhu", HeaderText = "备注", DataPropertyName = "Beizhu", ReadOnly = true, Width = 120, SortMode = DataGridViewColumnSortMode.NotSortable }
                });
        }

        /// <summary>
        /// 保存账号信息
        /// </summary>
        private void SaveData()
        {
            //StreamWriter sr = new StreamWriter(filename, false, Encoding.UTF8);
            //foreach (DataGridViewRow row in dgvData.Rows)
            //{
            //    var Name = row.Cells["colName"].Value.ToString();
            //    var Server = areas.First(a => a.AreaName == row.Cells["colServer"].Value.ToString()).AreaID;
            //    var WebUrl = row.Cells["colWebUrl"].Value == null ? "" : row.Cells["colWebUrl"].Value.ToString();                

            //    string str = string.Format("{0}|{1}|{2}", Name, Server, WebUrl);
            //    sr.WriteLine(str);
            //}
            //sr.Close();

            DataList.ForEach(item => {
                dal.AddAcount(item.账号信息);
                item.战绩.ForEach(a => {
                    dal.AddZhanji(a);
                });
            });
        }

        /// <summary>
        /// 读取账号信息
        /// </summary>
        private void ReadData()
        {
            //try
            //{
            //    StreamReader sr = new StreamReader(filename, Encoding.UTF8);
            //    string line;
            //    while ((line = sr.ReadLine()) != null)
            //    {
            //        string[] split = line.Split('|');
            //        if (split.Length == 3)
            //        {
            //            var index = dgvData.Rows.Add();
            //            dgvData.Rows[index].Cells["colName"].Value = split[0];
            //            dgvData.Rows[index].Cells["colServer"].Value = areas.First(a => a.AreaID == split[1]).AreaName;
            //            dgvData.Rows[index].Cells["colWebUrl"].Value = split[2];
            //            usernames.Add(split[0]);
            //        }
            //    }
            //    sr.Close();
            //}
            //catch (IOException e)
            //{
            //    //MessageBox.Show(ToString(), "提示信息");
            //}

            DataList = dal.GetGameData();
            if (DataList == null) return;

            DataList.ForEach(item =>
            {
                var index = dgvData.Rows.Add();
                dgvData.DefaultCellStyle.WrapMode = true;
                dgvData.AutoResizeColumn();
                dgvData.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.DisplayedCellsExceptHeaders;

                dgvData.Rows[index].Cells["colSelected"].Value = false;
                dgvData.Rows[index].Cells["colName"].Value = data.账号信息.Name;
                dgvData.Rows[index].Cells["colServer"].Value = areas.First(a => a.AreaID == data.账号信息.Server).AreaName;
                dgvData.Rows[index].Cells["colShenLU"].Value = data.Shenlu.ToString();
                dgvData.Rows[index].Cells["colDuanwei"].Value = data.账号信息.Duanwei;
                dgvData.Rows[index].Cells["colDuanwei"].Value = data.账号信息.Duanwei;
                dgvData.Rows[index].Cells["colWebUrl"].Value = data.账号信息.WebUrl;
                dgvData.Rows[index].Cells["colTime"].Value = data.账号信息.Time;
                dgvData.Rows[index].Cells["colShijian"].Value = data.Shijian;
                dgvData.Rows[index].Cells["colBeizhu"].Value = data.账号信息.Beizhu;

                for (int i = 1; i <= (count < data.战绩.Count ? count : data.战绩.Count); i++)
                {
                    var coldata = "colData" + i.ToString();
                    if (dgvData.Columns.Contains(coldata))
                    {
                        row.Cells[coldata].Value = string.Format("{0}{2}{1}", data.战绩[i - 1].Shijian, data.战绩[i - 1].Jieguo, Environment.NewLine);
                    }
                }
            });
        }

        /// <summary>
        /// 从列表读取账号信息
        /// </summary>
        private void GetData()
        {
            //int index = 0;
            //foreach (DataGridViewRow row in dgvData.Rows)
            //{
            //    召唤师队列.Enqueue(new 召唤师()
            //    {
            //        Data = new 战绩数据()
            //        {
            //            账号信息 = new GameAccount()
            //            {
            //                Index = index,
            //                Name = row.Cells["colName"].Value.ToString(),
            //                Server = areas.First(a => a.AreaName == row.Cells["colServer"].Value.ToString()).AreaID,
            //                WebUrl = row.Cells["colWebUrl"].Value == null ? "" : row.Cells["colWebUrl"].Value.ToString(),
            //                Beizhu = "",
            //                Duanwei = ""
            //            }
            //        }
            //    });

            //    index++;
            //}
            DataList.ForEach(item => {
                召唤师队列.Enqueue(new 召唤师()
                {                    
                    Data = item
                });
            });

            RefData();
        }

        /// <summary>
        /// 刷新战绩
        /// </summary>
        private void RefData()
        {
            召唤师 item;
            //lock (thisLock)
            //{
            //    item = 召唤师队列.Dequeue();
            //}
            //if(item != null)
            //{
            //    var data = item.查询战绩(Convert.ToInt32(txtCount.Text));
            //    RefDgvData(data);
            //}
            while (召唤师队列.Count > 0)
            {
                item = 召唤师队列.Dequeue();

                if (item != null)
                {
                    item.查询战绩();
                    RefDgvData(item.Data);
                }
            }
        }

        /// <summary>
        /// 更新控件上的数据
        /// </summary>
        /// <param name="data"></param>
        private void RefDgvData(战绩数据 data)
        {
            if(dgvData.InvokeRequired)
            {
                this.BeginInvoke(new UpdateControl(RefDgvData), new object[] { data });
            }
            else
            {
                DataGridViewRow row = null;
                foreach (DataGridViewRow r in dgvData.Rows)
                {
                    if (r.Cells["colName"].Value.ToString() == data.账号信息.Name)
                    {
                        row = r;
                        break;
                    }
                }

                if (row == null)
                    return;

                row.Cells["colSelected"].Value = false;
                row.Cells["colName"].Value = data.账号信息.Name;
                row.Cells["colServer"].Value = areas.First(a => a.AreaID == data.账号信息.Server).AreaName;
                row.Cells["colShenLU"].Value = data.Shenlu.ToString();
                row.Cells["colDuanwei"].Value = data.账号信息.Duanwei;
                row.Cells["colDuanwei"].Value = data.账号信息.Duanwei;
                row.Cells["colWebUrl"].Value = data.账号信息.WebUrl;
                row.Cells["colTime"].Value = data.账号信息.Time;
                row.Cells["colShijian"].Value = data.Shijian;
                row.Cells["colBeizhu"].Value = data.账号信息.Beizhu;

                int 失败次数 = 0;
                for (int i = 1; i <= (count < data.战绩.Count ? count : data.战绩.Count) ; i++)
                {
                    var coldata = "colData" + i.ToString();
                    if (dgvData.Columns.Contains(coldata))
                    {
                        row.Cells[coldata].Value = string.Format("{0}{2}{1}", data.战绩[i - 1].Shijian, data.战绩[i - 1].Jieguo, Environment.NewLine);
                        if (data.战绩[i - 1].Jieguo == "失败" && i < 8) 失败次数++;
                    }
                }
                if (失败次数 >= Lost)
                {
                    row.DefaultCellStyle.BackColor = Color.Red;
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string strAreaName = dicAreaId.Text;
            string areaID = dicAreaId.SelectedValue.ToString();
            string name = textBox1.Text;
            name = name.Trim();
            if (name.Length == 0)
            {
                MessageBox.Show("请输入召唤师名称！", "错误提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (DataList.Count > 0)
            {
                if (DataList.Exists(a => a.账号信息.Name == name))
                {
                    MessageBox.Show("当前召唤师已存在于列表中！", "提示信息", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }

            DataList.Add(new 战绩数据
            {
                账号信息 = new GameAccount { Name = name, Server = areaID }
            });
            
            var index = dgvData.Rows.Add();
            dgvData.DefaultCellStyle.WrapMode = true;
            dgvData.AutoResizeColumn();
            dgvData.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.DisplayedCellsExceptHeaders;
            dgvData.Rows[index].Cells["colSelected"].Value = false;
            dgvData.Rows[index].Cells["colName"].Value = name;
            dgvData.Rows[index].Cells["colServer"].Value = strAreaName;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            button2.Enabled = false;
            begintime = DateTime.Now;
            label6.Text = "查询中......";
            timer1.Enabled = true;
        }

        private void 战绩查询_FormClosed(object sender, FormClosedEventArgs e)
        {
            SaveData();
        }

        private void btnDel_Click(object sender, EventArgs e)
        {
            bool isselected = false;
            foreach(DataGridViewRow row in dgvData.Rows)
            {
                var selected = (bool)row.Cells["colSelected"].Value;
                if(selected)
                {
                    isselected = true;
                    var name = row.Cells["colName"].Value.ToString();
                    dal.DelGameData(name);
                    DataList.RemoveAll(item => item.账号信息.Name == name);
                    dgvData.Rows.RemoveAt(row.Index);
                }
            }

            if(!isselected)
            {
                MessageBox.Show("请选择要删除的行");
            }

            //if (dgvData.SelectedRows.Count == 0)
            //{
            //    MessageBox.Show("请选择要删除的行");
            //}
            //else
            //{
            //    for(int i=0;i<dgvData.SelectedRows.Count;i++)
            //    {
            //        var row = dgvData.SelectedRows[i];
            //        usernames.Remove(row.Cells["colName"].Value.ToString());
            //        dgvData.Rows.RemoveAt(dgvData.SelectedRows[i].Index);
            //    }
            //}
        }

        public string GetAppConfig(string strKey)
        {
            return System.Configuration.ConfigurationSettings.AppSettings[strKey].ToString();
        }

        public static void SetValue(string AppKey, string AppValue)
        {
            System.Xml.XmlDocument xDoc = new System.Xml.XmlDocument();
            xDoc.Load(System.Windows.Forms.Application.ExecutablePath + ".config");

            System.Xml.XmlNode xNode;
            System.Xml.XmlElement xElem1;
            System.Xml.XmlElement xElem2;
            xNode = xDoc.SelectSingleNode("//appSettings");

            xElem1 = (System.Xml.XmlElement)xNode.SelectSingleNode("//add[@key='" + AppKey + "']");
            if (xElem1 != null) xElem1.SetAttribute("value", AppValue);
            else
            {
                xElem2 = xDoc.CreateElement("add");
                xElem2.SetAttribute("key", AppKey);
                xElem2.SetAttribute("value", AppValue);
                xNode.AppendChild(xElem2);
            }
            xDoc.Save(System.Windows.Forms.Application.ExecutablePath + ".config");
        }

        private void button3_Click_1(object sender, EventArgs e)
        {
            SetValue("Count", string.IsNullOrWhiteSpace(txtCount.Text) ? "8" : txtCount.Text);
            SetValue("Interval", string.IsNullOrWhiteSpace(txtValer.Text) ? "10" : txtValer.Text);
            SetValue("Lost", string.IsNullOrWhiteSpace(txtLost.Text) ? "3" : txtLost.Text);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            timer1.Enabled = false;
            timer1.Interval = Convert.ToInt32(txtValer.Text) * 60 * 1000;
            if ((DateTime.Now - begintime).TotalMinutes < 1)
            {
                GetData();
                timer1.Enabled = true;
            }
            else
            {
                label6.Text = "查询结束";
                button2.Enabled = true;
            }
        }

        private void txtCount_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!Char.IsNumber(e.KeyChar) && e.KeyChar != (char)8)  // 允许输入退格键
            {
                e.Handled = true;   // 经过判断为数字，可以输入
            }
        }

        private void txtValer_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!Char.IsNumber(e.KeyChar) && e.KeyChar != (char)8)  // 允许输入退格键
            {
                e.Handled = true;   // 经过判断为数字，可以输入
            }
        }

        private void txtLost_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!Char.IsNumber(e.KeyChar) && e.KeyChar != (char)8)  // 允许输入退格键
            {
                e.Handled = true;   // 经过判断为数字，可以输入
            }
        }

        private void dgvData_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dgvData.Columns[e.ColumnIndex].Name == "colSelected")
            {
                dgvData.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = !(bool)dgvData.Rows[e.RowIndex].Cells[e.ColumnIndex].Value;                
            }
        }
    }

    public class Area
    {
        public string AreaName { get; set; }
        public string AreaID { get; set; }
    }
}
