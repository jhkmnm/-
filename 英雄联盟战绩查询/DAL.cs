using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dos.ORM;

namespace 英雄联盟战绩查询
{
    public class DAL
    {
        public GameAccount GetAccountByName(string name)
        {
            var where = new Where<GameAccount>();
            if(!string.IsNullOrWhiteSpace(name))
            {
                where.And(d => d.Name == name);
            }
            var fs = DB.Context.From<GameAccount>()
                .Where(where);

            return fs.First();
        }

        public List<GameAccount> GetAccounts()
        {
            return DB.Context.From<GameAccount>()
                .ToList();
        }

        public int AddAcount(GameAccount data)
        {
            var v = GetAccountByName(data.Name);
            if(v == null)
                return DB.Context.Insert<GameAccount>(data);
            else
                return DB.Context.Update(data);
        }

        public Zhanji GetZhanjiByGameID(string gameID)
        {
            var where = new Where<Zhanji>();
            if (!string.IsNullOrWhiteSpace(gameID))
            {
                where.And(d => d.GameID == gameID);
            }
            var fs = DB.Context.From<Zhanji>()
                .Where(where);

            return fs.First();
        }

        public List<Zhanji> GetZhanjiByGameName(string name)
        {
            var where = new Where<Zhanji>();
            if (!string.IsNullOrWhiteSpace(name))
            {
                where.And(d => d.Name == name);
            }
            return DB.Context.From<Zhanji>()
                .Where(where)
                .ToList();
        }

        public int AddZhanji(Zhanji data)
        {
            var v = GetZhanjiByGameID(data.GameID);
            if (v == null)
                return DB.Context.Insert<Zhanji>(data);
            return 0;
        }

        public List<战绩数据> GetGameData()
        {
            var accountList = GetAccounts();

            List<战绩数据> datas = new List<战绩数据>();
            foreach(var item in accountList)
            {
                战绩数据 data = new 战绩数据();
                data.账号信息 = item;
                data.战绩 = GetZhanjiByGameName(item.Name);
                datas.Add(data);
            }
            return datas;
        }

        public void SaveGameData(List<战绩数据> datas)
        {
            foreach(var item in datas)
            {
                AddAcount(item.账号信息);
                foreach(var zj in item.战绩)
                {
                    AddZhanji(zj);
                }
            }
        }

        public void DelGameData(string name)
        {
            DB.Context.Delete<GameAccount>(a => a.Name == name);
            DB.Context.Delete<Zhanji>(a => a.Name == name);
        }
    }
}
