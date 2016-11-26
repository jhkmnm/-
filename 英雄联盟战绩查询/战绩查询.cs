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
        List<Area> areas = new List<Area>();
        SoundPlayer player = new SoundPlayer();
        private List<string> usernames = new List<string>();
        int count = 8;        
        int Lost = 3;
        DAL dal = new DAL();
        List<战绩数据> DataList = new List<战绩数据>();
        int SearchCount = 1;

        public 战绩查询()
        {
            InitializeComponent();

            this.WindowState = FormWindowState.Maximized;

            txtCount.Text = GetAppConfig("Count");
            txtValer.Text = GetAppConfig("Interval");
            chkIsPlay.Checked = GetAppConfig("Play") == "1" ? true : false;
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

            var zhanjiStyle = new DataGridViewCellStyle();
            zhanjiStyle.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;

            dgvData.Columns.AddRange(new[] {                
                new DataGridViewTextBoxColumn() { Name = "colName", HeaderText = "游戏ID", DataPropertyName = "Name", ReadOnly = true, Width = 100, SortMode = DataGridViewColumnSortMode.NotSortable },
                new DataGridViewTextBoxColumn() { Name = "colServer", HeaderText = "游戏大区", DataPropertyName = "Server", ReadOnly = true, Width = 100, SortMode = DataGridViewColumnSortMode.NotSortable },
                new DataGridViewTextBoxColumn() { Name = "colShenLU", HeaderText = "当前胜率", DataPropertyName = "Shenlu", ReadOnly = true, Width = 70, SortMode = DataGridViewColumnSortMode.NotSortable },
                new DataGridViewTextBoxColumn() { Name = "colDuanwei", HeaderText = "当前段位", DataPropertyName = "Duanwei", ReadOnly = true, Width = 80, SortMode = DataGridViewColumnSortMode.NotSortable },
                new DataGridViewTextBoxColumn() { Name = "colWebUrl", HeaderText = "账号地址", DataPropertyName = "WebUrl", ReadOnly = true, Visible = false, SortMode = DataGridViewColumnSortMode.NotSortable },
                new DataGridViewTextBoxColumn() { Name = "colTime", HeaderText = "开始时间", DataPropertyName = "Time", ReadOnly = true, Visible = false, SortMode = DataGridViewColumnSortMode.NotSortable }
            });

            for (int i = 1; i <= count; i++)
            {
                dgvData.Columns.AddRange(new[] {
                    new DataGridViewTextBoxColumn() { Name = "colData"+i.ToString(), HeaderText = "战绩" + i.ToString(), DataPropertyName = "Data", ReadOnly = true, Width = 120, DefaultCellStyle = zhanjiStyle, SortMode = DataGridViewColumnSortMode.NotSortable }
                });
            }

            dgvData.Columns.AddRange(new[] {
                    new DataGridViewTextBoxColumn() { Name = "colShijian", HeaderText = "时间计时", DataPropertyName = "Shijian", ReadOnly = true, Width = 90, SortMode = DataGridViewColumnSortMode.NotSortable },
                    new DataGridViewTextBoxColumn() { Name = "colBeizhu", HeaderText = "备注", DataPropertyName = "Beizhu", Width = 200, SortMode = DataGridViewColumnSortMode.NotSortable }
                });
        }

        /// <summary>
        /// 保存账号信息
        /// </summary>
        private void SaveData()
        {
            DataList.ForEach(item => {
                dal.AddAcount(item.账号信息);
                if (item.战绩 != null && item.战绩.Count > 0)
                {
                    item.战绩.ForEach(a =>
                    {
                        dal.AddZhanji(a);
                    });
                }
            });
        }

        /// <summary>
        /// 读取账号信息
        /// </summary>
        private void ReadData()
        {
            DataList = dal.GetGameData();
            if (DataList == null) return;

            DataList.ForEach(item =>
            {
                var index = dgvData.Rows.Add();
                dgvData.Rows[index].Cells["colSelected"].Value = false;
                dgvData.Rows[index].Cells["colName"].Value = item.账号信息.Name;
                dgvData.Rows[index].Cells["colServer"].Value = areas.First(a => a.AreaID == item.账号信息.Server).AreaName;
                dgvData.Rows[index].Cells["colShenLU"].Value = item.Shenlu.ToString();
                dgvData.Rows[index].Cells["colDuanwei"].Value = item.账号信息.Duanwei;                
                dgvData.Rows[index].Cells["colWebUrl"].Value = item.账号信息.WebUrl;
                dgvData.Rows[index].Cells["colTime"].Value = item.账号信息.Time;
                dgvData.Rows[index].Cells["colShijian"].Value = item.Shijian;
                dgvData.Rows[index].Cells["colBeizhu"].Value = item.账号信息.Beizhu;

                for (int i = 1; i <= (count < item.战绩.Count ? count : item.战绩.Count); i++)
                {
                    var coldata = "colData" + i.ToString();
                    if (dgvData.Columns.Contains(coldata))
                    {
                        dgvData.Rows[index].Cells[coldata].Value = item.战绩[i - 1].Shijian + item.战绩[i - 1].Jieguo;
                    }
                }
            });
        }

        /// <summary>
        /// 从列表读取账号信息
        /// </summary>
        private void GetData()
        {
            DataList.ForEach(item => {
                var t = (DateTime.Now - Convert.ToDateTime(item.账号信息.Time));
                if (t.Hours < 24)
                {
                    召唤师队列.Enqueue(new 召唤师()
                    {
                        Data = item
                    });
                }
            });

            new System.Threading.Thread(RefData).Start();            
        }

        /// <summary>
        /// 刷新战绩
        /// </summary>
        private void RefData()
        {
            召唤师 item;
            
            while (召唤师队列.Count > 0)
            {
                item = 召唤师队列.Dequeue();

                if (item != null)
                {
                    DataGridViewRow row = null;
                    foreach (DataGridViewRow r in dgvData.Rows)
                    {
                        if (r.Cells["colName"].Value.ToString() == item.Data.账号信息.Name)
                        {
                            row = r;
                            break;
                        }
                    }
                    if (row != null)
                    {
                        this.BeginInvoke(new Action(() =>{
                            row.Selected = true;
                            label6.Text = string.Format("第{0}次查询中......{1}", SearchCount, row.Index);
                        }));                        
                    }                        

                    item.查询战绩();
                    this.Invoke(new Action<战绩数据>(RefDgvData), new object[] { item.Data });
                }
            }
            SearchCount++;
        }

        /// <summary>
        /// 更新控件上的数据
        /// </summary>
        /// <param name="data"></param>
        private void RefDgvData(战绩数据 data)
        {
            DataGridViewRow row = null;
            foreach (DataGridViewRow r in dgvData.Rows)
            {
                if (r.Cells["colName"].Value.ToString() == data.账号信息.Name)
                {
                    row = r;
                    row.Selected = true;
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
            row.Cells["colWebUrl"].Value = data.账号信息.WebUrl;
            row.Cells["colTime"].Value = data.账号信息.Time;
            row.Cells["colShijian"].Value = data.Shijian;
            row.Cells["colBeizhu"].Value = data.账号信息.Beizhu;

            int 失败次数 = 0;
            for (int i = 1; i <= (count < data.战绩.Count ? count : data.战绩.Count); i++)
            {
                var coldata = "colData" + i.ToString();
                if (dgvData.Columns.Contains(coldata))
                {
                    row.Cells[coldata].Value = string.Format("{0}{2}{1}", data.战绩[i - 1].Shijian, data.战绩[i - 1].Jieguo, Environment.NewLine);
                    if (data.战绩[i - 1].Jieguo == "失败" && i <= 8) 失败次数++;
                }
            }
            if (失败次数 >= Lost)
            {
                row.DefaultCellStyle.BackColor = Color.Red;
                player.Play();
            }
            row.Selected = false;
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
                账号信息 = new GameAccount { Name = name, Server = areaID, Time = DateTime.Now.ToString() }
            });
            
            var index = dgvData.Rows.Add();
            dgvData.Rows[index].Cells["colSelected"].Value = false;
            dgvData.Rows[index].Cells["colName"].Value = name;
            dgvData.Rows[index].Cells["colServer"].Value = strAreaName;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if(DataList.Count == 0)
            {
                MessageBox.Show("请先添加需要查询的账号");
                return;
            }

            button2.Enabled = false;
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
            SetValue("Play", chkIsPlay.Checked ? "1" : "0");
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            timer1.Enabled = false;
            timer1.Interval = Convert.ToInt32(txtValer.Text) * 60 * 1000;

            var v = DataList.Where(a =>
            {
                var t = (DateTime.Now - Convert.ToDateTime(a.账号信息.Time));
                return t.Hours < 24;
            });

            if (v != null && v.Count() > 0)
            {                
                GetData();
                timer1.Enabled = true;
                SaveData();
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

        private void dgvData_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (dgvData.Columns[e.ColumnIndex].Name == "colBeizhu")
            {
                var value = dgvData.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString();
                var name = dgvData.Rows[e.RowIndex].Cells["colName"].Value.ToString();
                DataList.ForEach(item =>
                {
                    if (item.账号信息.Name == name)
                    {
                        item.账号信息.Beizhu = value;
                    }
                });
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            SaveData();
        }
    }

    public class Area
    {
        public string AreaName { get; set; }
        public string AreaID { get; set; }
    }
}
