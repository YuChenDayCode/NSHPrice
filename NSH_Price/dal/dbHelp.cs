using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Reflection;
using System.Text;

public class dbHelp
{
    static string ConStr = ConfigurationManager.AppSettings["ConStr"];
    public dbHelp()
    {
    }
    public dbHelp(string str)
    {
        ConStr = str;
    }
    public static int Excute(string sql)
    {
        DataSet ds = new DataSet();
        using (MySqlConnection con = new MySqlConnection(ConStr))
        {
            con.Open();
            if (con.State == ConnectionState.Open)
            {
                using (MySqlCommand cmd = new MySqlCommand(sql, con))
                {
                    return cmd.ExecuteNonQuery();
                }
            }
            else return -1;
        }
    }

    /// <summary>
    /// 返回单个数据
    /// </summary>
    /// <param name="sql"></param>
    /// <returns></returns>
    public object SelectSingleBySql(string sql)
    {
        try
        {
            using (MySqlConnection con = new MySqlConnection(ConStr))
            {
                con.Open();
                using (MySqlCommand cmd = new MySqlCommand(sql, con))
                {
                    object obj = cmd.ExecuteScalar();
                    return obj;
                }
            }
        }
        catch (Exception ex) { throw new Exception(ex.Message); }
    }


    #region 反射查询法
    public List<T> Select<T>(string sql)
    {
        try
        {
            using (MySqlConnection con = new MySqlConnection(ConStr))
            {
                con.Open();
                using (MySqlCommand cmd = new MySqlCommand(sql, con))
                {
                    MySqlDataReader rd = cmd.ExecuteReader();
                    List<T> list = new List<T>();
                    Type type = typeof(T);
                    T model = (T)Activator.CreateInstance(type);
                    while (rd.Read())
                    {
                        ValueToModel<T>(rd.FieldCount, type, rd, model);
                        list.Add(model);
                        model = (T)Activator.CreateInstance(model.GetType());//重新创建实体对象
                    }
                    return list;
                }
            }
        }
        catch (Exception ex) { throw new Exception(ex.Message); }
    }

    public static void ValueToModel<T>(int _counts, Type type, IDataReader reader, T model)
    {

        for (int i = 0; i < _counts; i++)
        {
            string fieldname = reader.GetName(i);
            object value = reader[i];
            if (value != null && !Convert.IsDBNull(value))
            {
                PropertyInfo pi = type.GetProperty(fieldname);
                if (pi != null)
                    pi.SetValue(model, value, null);
            }
        }

    }
    #endregion

    #region 生成插入语句
    public static string InsertSql<T>(T t, string tablename)
    {
        Type type = typeof(T);
        string tableName = string.IsNullOrEmpty(tablename) ? type.Name : tablename;
        StringBuilder sb = new StringBuilder();
        StringBuilder sb_v = new StringBuilder();
        sb.Append("insert into ");
        sb.Append(tableName);
        sb.Append("(");

        sb_v.Append(" values (");
        foreach (var pi in type.GetProperties())
        {
            if (pi != null)
            {
                //object[] pk = pi.GetCustomAttributes(false);
                object value = pi.GetValue(t, null);
                if (value == null)
                    continue; //未给值，null

                sb.Append("`" + pi.Name + "`,");
                if (value == null)
                    sb_v.Append("null" + ",");
                else if (value.GetType() == typeof(int) || value.GetType() == typeof(decimal) || value.GetType() == typeof(double) || value.GetType() == typeof(long) || value.GetType() == typeof(float))
                    sb_v.Append(value + ",");
                else
                    sb_v.Append("'" + value + "',");
            }
        }
        sb.Remove(sb.Length - 1, 1);
        sb.Append(")");

        sb_v.Remove(sb_v.Length - 1, 1);
        sb_v.Append(")");

        sb.Append(sb_v);


        return sb.ToString();
    }

    public static string InsertSql<T>(List<T> t, string tableName)
    {
        StringBuilder sb = new StringBuilder();
        sb.Append("begin ");
        foreach (T item in t)
        {
            sb.Append(InsertSql(item, tableName));
            sb.Append(";");
        }
        sb.Append("end;");
        return sb.ToString();
    }
    #endregion

}
