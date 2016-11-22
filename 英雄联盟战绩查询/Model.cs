using Dos.ORM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace 英雄联盟战绩查询
{
    /// <summary>
    /// 实体类GameAccount。(属性说明自动提取数据库字段的描述信息)
    /// </summary>
    [Table("GameAccount")]
    [Serializable]
    public partial class GameAccount : Entity
    {
        #region Model
        private int _ID;
        private string _Name;
        private string _Server;
        private string _WebUrl;
        private string _Duanwei;
        private string _Time;
        private string _Beizhu;

        /// <summary>
        /// 
        /// </summary>
        [Field("ID")]
        public int ID
        {
            get { return _ID; }
            set
            {
                this.OnPropertyValueChange("ID");
                this._ID = value;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        [Field("Name")]
        public string Name
        {
            get { return _Name; }
            set
            {
                this.OnPropertyValueChange("Name");
                this._Name = value;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        [Field("Server")]
        public string Server
        {
            get { return _Server; }
            set
            {
                this.OnPropertyValueChange("Server");
                this._Server = value;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        [Field("WebUrl")]
        public string WebUrl
        {
            get { return _WebUrl; }
            set
            {
                this.OnPropertyValueChange("WebUrl");
                this._WebUrl = value;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        [Field("Duanwei")]
        public string Duanwei
        {
            get { return _Duanwei; }
            set
            {
                this.OnPropertyValueChange("Duanwei");
                this._Duanwei = value;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        [Field("Time")]
        public string Time
        {
            get { return _Time; }
            set
            {
                this.OnPropertyValueChange("Time");
                this._Time = value;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        [Field("Beizhu")]
        public string Beizhu
        {
            get { return _Beizhu; }
            set
            {
                this.OnPropertyValueChange("Beizhu");
                this._Beizhu = value;
            }
        }
        #endregion

        #region Method
        /// <summary>
        /// 获取实体中的主键列
        /// </summary>
        public override Field[] GetPrimaryKeyFields()
        {
            return new Field[] {
                _.ID,
            };
        }
        /// <summary>
        /// 获取实体中的标识列
        /// </summary>
        public override Field GetIdentityField()
        {
            return _.ID;
        }
        /// <summary>
        /// 获取列信息
        /// </summary>
        public override Field[] GetFields()
        {
            return new Field[] {
                _.ID,
                _.Name,
                _.Server,
                _.WebUrl,
                _.Duanwei,
                _.Time,
                _.Beizhu,
            };
        }
        /// <summary>
        /// 获取值信息
        /// </summary>
        public override object[] GetValues()
        {
            return new object[] {
                this._ID,
                this._Name,
                this._Server,
                this._WebUrl,
                this._Duanwei,
                this._Time,
                this._Beizhu,
            };
        }
        /// <summary>
        /// 是否是v1.10.5.6及以上版本实体。
        /// </summary>
        /// <returns></returns>
        public override bool V1_10_5_6_Plus()
        {
            return true;
        }
        #endregion

        #region _Field
        /// <summary>
        /// 字段信息
        /// </summary>
        public class _
        {
            /// <summary>
            /// * 
            /// </summary>
            public readonly static Field All = new Field("*", "GameAccount");
            /// <summary>
			/// 
			/// </summary>
			public readonly static Field ID = new Field("ID", "GameAccount", "");
            /// <summary>
			/// 
			/// </summary>
			public readonly static Field Name = new Field("Name", "GameAccount", "");
            /// <summary>
			/// 
			/// </summary>
			public readonly static Field Server = new Field("Server", "GameAccount", "");
            /// <summary>
			/// 
			/// </summary>
			public readonly static Field WebUrl = new Field("WebUrl", "GameAccount", "");
            /// <summary>
			/// 
			/// </summary>
			public readonly static Field Duanwei = new Field("Duanwei", "GameAccount", "");
            /// <summary>
			/// 
			/// </summary>
			public readonly static Field Time = new Field("Time", "GameAccount", "");
            /// <summary>
			/// 
			/// </summary>
			public readonly static Field Beizhu = new Field("Beizhu", "GameAccount", "");
        }
        #endregion
    }


    /// <summary>
    /// 实体类Zhanji。(属性说明自动提取数据库字段的描述信息)
    /// </summary>
    [Table("Zhanji")]
	[Serializable]
	public partial class Zhanji : Entity
	{
		#region Model
		private string _Name;
		private string _Shijian;
		private string _Jieguo;
		private string _GameID;

		/// <summary>
		/// 
		/// </summary>
		[Field("Name")]
		public string Name
		{
			get { return _Name; }
			set
			{
				this.OnPropertyValueChange("Name");
				this._Name = value;
			}
		}
		/// <summary>
		/// 
		/// </summary>
		[Field("Shijian")]
		public string Shijian
		{
			get { return _Shijian; }
			set
			{
				this.OnPropertyValueChange("Shijian");
				this._Shijian = value;
			}
		}
		/// <summary>
		/// 
		/// </summary>
		[Field("Jieguo")]
		public string Jieguo
		{
			get { return _Jieguo; }
			set
			{
				this.OnPropertyValueChange("Jieguo");
				this._Jieguo = value;
			}
		}
		/// <summary>
		/// 
		/// </summary>
		[Field("GameID")]
		public string GameID
		{
			get { return _GameID; }
			set
			{
				this.OnPropertyValueChange("GameID");
				this._GameID = value;
			}
		}
		#endregion

		#region Method
		/// <summary>
		/// 获取实体中的主键列
		/// </summary>
		public override Field[] GetPrimaryKeyFields()
		{
			return new Field[] {
			};
		}
		/// <summary>
		/// 获取列信息
		/// </summary>
		public override Field[] GetFields()
		{
			return new Field[] {
				_.Name,
				_.Shijian,
				_.Jieguo,
				_.GameID,
			};
		}
		/// <summary>
		/// 获取值信息
		/// </summary>
		public override object[] GetValues()
		{
			return new object[] {
				this._Name,
				this._Shijian,
				this._Jieguo,
				this._GameID,
			};
		}
		/// <summary>
		/// 是否是v1.10.5.6及以上版本实体。
		/// </summary>
		/// <returns></returns>
		public override bool V1_10_5_6_Plus()
		{
			return true;
		}
		#endregion

		#region _Field
		/// <summary>
		/// 字段信息
		/// </summary>
		public class _
		{
			/// <summary>
			/// * 
			/// </summary>
			public readonly static Field All = new Field("*", "Zhanji");
			/// <summary>
			/// 
			/// </summary>
			public readonly static Field Name = new Field("Name", "Zhanji", "");
			/// <summary>
			/// 
			/// </summary>
			public readonly static Field Shijian = new Field("Shijian", "Zhanji", "");
			/// <summary>
			/// 
			/// </summary>
			public readonly static Field Jieguo = new Field("Jieguo", "Zhanji", "");
			/// <summary>
			/// 
			/// </summary>
			public readonly static Field GameID = new Field("GameID", "Zhanji", "");
		}
		#endregion
	}
}
