using Mono.Data.Sqlite;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Text;
using UnityEngine;

public class Sqlite
{
    //Sqlite连接前缀
    private const string DB_CONNECTION_PREFIX = "URI=file:";
    //连接器
    private IDbConnection m_dbConn;
    //查询命令
    private IDbCommand m_command;
    //事物
    private IDbTransaction m_dbTrans;

    //新建连接时传入库路径
    public Sqlite(string path)
    {
        OpenDataBase(path);
    }

    //创建连接，与命令
    private void OpenDataBase(string path)
    {
        m_dbConn = new SqliteConnection(DB_CONNECTION_PREFIX + path);
        m_dbConn.Open();
        m_command = m_dbConn.CreateCommand();
    }

    //获取单个表的信息
    public List<string> GetTableInfo(string tableName)
    {
        List<string> tableInfo = new List<string>();

        //查询语句
        m_command.CommandText = "PRAGMA table_info(" + tableName + ")";
        //查询结果
        IDataReader reader = m_command.ExecuteReader();

        while (reader.Read())
        {
            for (int i = 0; i < reader.FieldCount; i++)
            {
                tableInfo.Add(reader.GetValue(i).ToString());
                Debug.Log(reader.GetValue(i));
            }
        }
        reader.Close();

        return tableInfo;
    }

    //获取库的所有表的名字
    public List<string> GetAllTableName()
    {
        List<string> tableNames = new List<string>();

        m_command.CommandText = "select name from sqlite_master where type='table' order by name;";

        IDataReader reader = m_command.ExecuteReader();

        while (reader.Read())
        {
            for (int i = 0; i < reader.FieldCount; i++)
            {
                tableNames.Add(reader.GetValue(i).ToString());
                Debug.Log(reader.GetValue(i));
            }
        }
        reader.Close();

        return tableNames;
    }



    /// <summary>
    /// 根据某个字段来修改更新表
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="tc"></param>
    /// <param name="tableName">表名</param>
    /// <param name="cFAName">字段名</param>
    public void UpdateTable<T, Y>(List<T> ts, string cFAName, Y condi) where T : IConfig
    {
        BeginTrans();

        string tableName = typeof(T).Name;
        int condiID = -1;
        StringBuilder sql = new StringBuilder();
        StringBuilder sqls = new StringBuilder();
        //用来存储判断语句
        StringBuilder judge = new StringBuilder();

        //获取到 T 的属性信息
        PropertyInfo[] propertys = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.SetProperty | BindingFlags.Instance);
        ConfigFieldAttribute[] cFAs = new ConfigFieldAttribute[propertys.Length];
        for (int i = 0; i < cFAs.Length; i++)
        {
            cFAs[i] = (propertys[i].
                            GetCustomAttributes(typeof(ConfigFieldAttribute), false))[0]
                            as ConfigFieldAttribute;
            if (cFAs[i].FiledName == cFAName)
                condiID = i;
        }

        if (condiID == -1)
        {
            Debug.LogError("特性名（字段名）输入错误");
            return;
        }
        else
        {
            judge.Append(" where ");
            judge.Append(cFAs[condiID].FiledName);
            if (typeof(Y) == typeof(string))
            {
                judge.Append(" = '");
                judge.Append(condi);
                judge.Append("';");
            }
            else
            {
                judge.Append(" = ");
                judge.Append(condi);
                judge.Append(";");
            }
        }

        for (int i = 0; i < ts.Count; i++)
        {
            if (((Y)propertys[condiID].GetValue(ts[i], null)).Equals(condi))
            {

                sql.Append("update ");
                sql.Append(tableName);
                sql.Append(" set ");
                for (int j = 0; j < propertys.Length; j++)
                {
                    if (j == condiID)
                    {
                        continue;
                    }

                    if (propertys[j].PropertyType == typeof(string))
                    {
                        sql.Append(cFAs[j].FiledName);
                        sql.Append(" = '");
                        sql.Append(propertys[j].GetValue(ts[i], null));
                        sql.Append("',");

                        continue;
                    }

                    sql.Append(cFAs[j].FiledName);
                    sql.Append(" = ");
                    sql.Append(propertys[j].GetValue(ts[i], null));
                    sql.Append(",");
                }
                //去除掉最后一条语句的 ","    where前没有","
                sql.Remove(sql.Length - 1, 1);
                //填充判断语句
                sql.Append(judge);
                sqls.Append(sql);

                sql.Remove(0, sql.Length);
            }

        }

        ExcuteQuery(sqls.ToString());
        Commit();

    }

    /// <summary>
    /// 根据 cFAName 来更新表
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="ts"></param>
    /// <param name="cFAName"></param>
    public void UpdateTable<T>(List<T> ts, string cFAName) where T : IConfig
    {
        BeginTrans();

        string tableName = typeof(T).Name;
        StringBuilder sql = new StringBuilder();
        StringBuilder sqls = new StringBuilder();
        //用来存储判断语句
        StringBuilder judge = new StringBuilder();

        //获取到 T 的属性信息
        PropertyInfo[] propertys = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.SetProperty | BindingFlags.Instance);
        ConfigFieldAttribute[] cFAs = new ConfigFieldAttribute[propertys.Length];
        for (int i = 0; i < cFAs.Length; i++)
        {
            cFAs[i] = (propertys[i].
                            GetCustomAttributes(typeof(ConfigFieldAttribute), false))[0]
                            as ConfigFieldAttribute;
        }


        for (int i = 0; i < ts.Count; i++)
        {
            sql.Append("update ");
            sql.Append(tableName);
            sql.Append(" set ");
            for (int j = 0; j < propertys.Length; j++)
            {


                if (cFAs[j].FiledName == cFAName)
                {
                    judge.Append(" where ");
                    judge.Append(cFAs[j].FiledName);
                    if (propertys[j].PropertyType == typeof(string))
                    {
                        judge.Append(" = '");
                        judge.Append(propertys[j].GetValue(ts[i], null));
                        judge.Append("';");
                    }
                    else
                    {
                        judge.Append(" = ");
                        judge.Append(propertys[j].GetValue(ts[i], null));
                        judge.Append(";");
                    }


                    continue;
                }

                if (propertys[j].PropertyType == typeof(string))
                {
                    sql.Append(cFAs[j].FiledName);
                    sql.Append(" = '");
                    sql.Append(propertys[j].GetValue(ts[i], null));
                    sql.Append("',");

                    continue;
                }

                sql.Append(cFAs[j].FiledName);
                sql.Append(" = ");
                sql.Append(propertys[j].GetValue(ts[i], null));
                sql.Append(",");
            }
            //去除掉最后一条语句的 ","    where前没有","
            sql.Remove(sql.Length - 1, 1);
            //填充判断语句
            sql.Append(judge);
            sqls.Append(sql);

            sql.Remove(0, sql.Length);
            judge.Remove(0, judge.Length);
        }

        Debug.Log(sqls.ToString());
        ExcuteQuery(sqls.ToString());
        Commit();
    }


    /// <summary>
    /// 表设置了单一索引时可用
    /// 根据索引更新表，若不存在该索引则添加对应项
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="ts"></param>
    public void UpdateTable<T>(List<T> ts) where T : IConfig
    {
        BeginTrans();

        string tableName = typeof(T).Name;

        StringBuilder sql1 = new StringBuilder();
        StringBuilder sql2 = new StringBuilder();
        StringBuilder sqls = new StringBuilder();
        StringBuilder sql = new StringBuilder();

        //获取到 T 的属性信息
        PropertyInfo[] propertys = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.SetProperty | BindingFlags.Instance);
        ConfigFieldAttribute[] cFAs = new ConfigFieldAttribute[propertys.Length];
        for (int i = 0; i < cFAs.Length; i++)
        {
            cFAs[i] = (propertys[i].
                            GetCustomAttributes(typeof(ConfigFieldAttribute), false))[0]
                            as ConfigFieldAttribute;
        }


        sql1.Append("REPLACE into ");
        sql1.Append(tableName);
        sql1.Append(" values ");
        sql1.Append("(");

        for (int i = 0; i < ts.Count; i++)
        {
            for (int j = 0; j < propertys.Length; j++)
            {

                if (propertys[j].PropertyType == typeof(string))
                {
                    sql2.Append("'");
                    sql2.Append(propertys[j].GetValue(ts[i], null));
                    sql2.Append("',");

                    continue;
                }

                sql2.Append(propertys[j].GetValue(ts[i], null));
                sql2.Append(",");
            }
            //去除掉最后一条语句的 ","    where前没有","
            sql2.Remove(sql2.Length - 1, 1);

            sql.Append(sql1);
            sql.Append(sql2);
            sql.Append("); \n");

            sqls.Append(sql);

            sql2.Remove(0, sql2.Length);
            sql.Remove(0, sql.Length);
        }

        Debug.Log(sqls.ToString());
        ExcuteQuery(sqls.ToString());
        Commit();
    }

    ///// <summary>
    ///// 根据 id 将 列表中的数据全部更新到表
    ///// </summary>
    ///// <typeparam name="T"></typeparam>
    ///// <param name="ts"></param>
    //public void UpdateTable<T>(List<T> ts) where T:IConfig
    //{
    //    BeginTrans();

    //    string tableName = typeof(T).Name;
    //    StringBuilder sql = new StringBuilder();
    //    StringBuilder sqls = new StringBuilder();
    //    //用来存储判断语句
    //    StringBuilder judge = new StringBuilder();

    //    //获取到 T 的属性信息
    //    PropertyInfo[] propertys = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.SetProperty | BindingFlags.Instance);
    //    ConfigFieldAttribute[] cFAs = new ConfigFieldAttribute[propertys.Length];
    //    for (int i = 0; i < cFAs.Length; i++)
    //    {
    //        cFAs[i] = (propertys[i].
    //                        GetCustomAttributes(typeof(ConfigFieldAttribute), false))[0]
    //                        as ConfigFieldAttribute;
    //    }


    //    for (int i = 0; i < ts.Count; i++)
    //    {
    //        sql.Append("update ");
    //        sql.Append(tableName);
    //        sql.Append(" set ");
    //        for (int j = 0; j < propertys.Length; j++)
    //        {


    //            if (cFAs[j].FiledName == "id")
    //            {
    //                judge.Append(" where ");
    //                judge.Append(cFAs[j].FiledName);
    //                if (propertys[j].PropertyType == typeof(string))
    //                {
    //                    judge.Append(" = '");
    //                    judge.Append(propertys[j].GetValue(ts[i], null));
    //                    judge.Append("';");
    //                }
    //                else
    //                {
    //                    judge.Append(" = ");
    //                    judge.Append(propertys[j].GetValue(ts[i], null));
    //                    judge.Append(";");
    //                }


    //                continue;
    //            }

    //            if (propertys[j].PropertyType == typeof(string))
    //            {
    //                sql.Append(cFAs[j].FiledName);
    //                sql.Append(" = '");
    //                sql.Append(propertys[j].GetValue(ts[i], null));
    //                sql.Append("',");

    //                continue;
    //            }

    //            sql.Append(cFAs[j].FiledName);
    //            sql.Append(" = ");
    //            sql.Append(propertys[j].GetValue(ts[i], null));
    //            sql.Append(",");
    //        }
    //        //去除掉最后一条语句的 ","    where前没有","
    //        sql.Remove(sql.Length - 1, 1);
    //        //填充判断语句
    //        sql.Append(judge);
    //        sqls.Append(sql);

    //        sql.Remove(0,sql.Length);
    //        judge.Remove(0, judge.Length);
    //    }

    //    Debug.Log(sqls.ToString());
    //    ExcuteQuery(sqls.ToString());
    //    Commit();

    //}

    /// <summary>
    /// 根据两个条件查询数据
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="Y"></typeparam>
    /// <typeparam name="Z"></typeparam>
    /// <param name="cFAName1"></param>
    /// <param name="condi1"></param>
    /// <param name="cFAName2"></param>
    /// <param name="condi2"></param>
    /// <returns></returns>
    public List<T> SelectTable<T, Y, Z>(string cFAName1, Y condi1, string cFAName2, Z condi2) where T : IConfig
    {
        string tableName = typeof(T).Name;
        StringBuilder sql = new StringBuilder();
        List<T> ts = new List<T>();
        sql.Append("select * from " + tableName + " where ");

        sql.Append(cFAName1);
        if (typeof(Y) == typeof(string))
        {
            sql.Append(" = '");
            sql.Append(condi1);
            sql.Append("' ");
        }
        else
        {
            sql.Append(" = ");
            sql.Append(condi1);
            sql.Append(" ");
        }

        sql.Append(" and ");
        sql.Append(cFAName2);
        if (typeof(Z) == typeof(string))
        {
            sql.Append(" = '");
            sql.Append(condi1);
            sql.Append("';");
        }
        else
        {
            sql.Append(" = ");
            sql.Append(condi2);
            sql.Append(";");
        }

        Debug.Log(sql.ToString());

        foreach (var tc in ExcuteSelectQuery<T>(sql.ToString()))
        {
            ts.Add((T)tc);
            Debug.Log(tc);
        }
        return ts;
    }
    /// <summary>
    /// 根据一个条件查数据
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="Y"></typeparam>
    /// <param name="cFAName"></param>
    /// <param name="condi"></param>
    /// <returns></returns>
    public List<T> SelectTable<T, Y>(string cFAName, Y condi) where T : IConfig
    {
        string tableName = typeof(T).Name;
        StringBuilder sql = new StringBuilder();
        List<T> ts = new List<T>();
        sql.Append("select * from " + tableName + " where ");

        sql.Append(cFAName);
        if (typeof(Y) == typeof(string))
        {
            sql.Append(" = '");
            sql.Append(condi);
            sql.Append("';");
        }
        else
        {
            sql.Append(" = ");
            sql.Append(condi);
            sql.Append(";");
        }

        Debug.Log(sql.ToString());

        foreach (var tc in ExcuteSelectQuery<T>(sql.ToString()))
        {
            ts.Add((T)tc);
            Debug.Log(tc);
        }
        return ts;
    }
    /// <summary>
    /// 填充类列表
    /// </summary>
    /// <typeparam name="T"> 用来接收表的类 </typeparam>
    /// <param name="tableName"> 表名 </param>
    /// <returns> 返回类列表 </returns>
    public List<T> SelectTable<T>() where T : IConfig
    {
        string tableName = typeof(T).Name;
        string sql = "select * from " + tableName;
        List<T> ts = new List<T>();

        foreach (var tc in ExcuteSelectQuery<T>(sql))
        {
            ts.Add((T)tc);
        }

        return ts;
    }


    //查询函数
    //IConfig是自定义的配置类接口，最好使数据库中的配置类都继承同一个接口，方便以后扩展
    private IEnumerable ExcuteSelectQuery<T>(string sqlQuery)
    {
        return ExcuteSelectQuery(sqlQuery, typeof(T));
    }
    //查询函数,这里使用反射 反序列数据，简化对对象的赋值
    private IEnumerable ExcuteSelectQuery(string sqlQuery, Type type)
    {
        BeginTrans();
        //查询语句
        m_command.CommandText = sqlQuery;
        //查询结果
        IDataReader reader = m_command.ExecuteReader();

        //反序列化操作 所定义的变量
        PropertyInfo[] newpropertys = null;
        PropertyInfo[] oldpropertys = null;
        bool init = false;

        while (reader.Read())
        {
            //创建类对象
            IConfig config = Activator.CreateInstance(type) as IConfig;
            for (int i = 0; i < reader.FieldCount; i++)
            {
                if (!init)
                {
                    //newpropertys将类的属性 顺序的对应到数据库中的列名，方便后续的赋值操作
                    if (newpropertys == null)
                    {
                        newpropertys = new PropertyInfo[reader.FieldCount];
                    }
                    //获取类的所有属性
                    if (oldpropertys == null)
                    {
                        oldpropertys = type.GetProperties(BindingFlags.Public | BindingFlags.SetProperty | BindingFlags.Instance);
                    }
                    //当前数据的列名
                    string filedName = reader.GetName(i);

                    bool find = false;
                    PropertyInfo pro = null;
                    //查找与列名相同的自定义特性名称，相同则把查询到的数据值赋值到对象中
                    for (int j = 0; j < oldpropertys.Length; j++)
                    {
                        pro = oldpropertys[j];
                        //获取自定义特性
                        object[] objs = pro.GetCustomAttributes(typeof(ConfigFieldAttribute), false);
                        if (objs.Length == 0)
                        {
                            continue;
                        }
                        //特性的列名是否与数据库中列名相同
                        ConfigFieldAttribute cfgfield = objs[0] as ConfigFieldAttribute;
                        if (cfgfield.FiledName == filedName)
                        {
                            find = true;
                            break;
                        }
                    }
                    newpropertys[i] = find ? pro : null;
                }
                //已经排好序的类属性数组
                PropertyInfo info = newpropertys[i];
                if (info == null)
                {
                    continue;
                }

                //对象的属性赋值

                if (!(reader.GetValue(i).ToString() == "") && !(reader.GetValue(i).GetType() == typeof(long)))
                {
                    info.SetValue(config, reader.GetValue(i), null);
                }
                else if (!(reader.GetValue(i).ToString() == ""))
                {
                    Debug.Log("进入 integer1");
                    info.SetValue(config, Convert.ToInt32(reader.GetValue(i)), null);
                    Debug.Log("进入 integer2");
                }
                else
                {
                    Debug.LogError("表格中存在 Null 字段");
                    yield break;
                }

            }

            if (!init)
            {
                init = true;
            }
            yield return config;
        }
        Commit();
        reader.Close();
    }

    //使用事物
    private void BeginTrans()
    {
        m_dbTrans = m_dbConn.BeginTransaction();
        m_command.Transaction = m_dbTrans;
    }

    //事物回滚
    private void Rollback()
    {
        m_dbTrans.Rollback();
    }

    //事物生效
    private void Commit()
    {
        m_dbTrans.Commit();
    }

    //执行其他sql语句
    private void ExcuteQuery(string sqlQuery)
    {
        Debug.Log(sqlQuery);
        m_command.CommandText = sqlQuery;
        m_command.ExecuteNonQuery();
    }

    //关闭连接
    public void Close()
    {
        if (m_dbTrans != null)
        {
            m_dbTrans.Dispose();
            m_dbTrans = null;
        }
        if (m_command != null)
        {
            m_command.Dispose();
            m_command = null;
        }
        m_dbConn.Close();
        m_dbConn = null;
    }

}
