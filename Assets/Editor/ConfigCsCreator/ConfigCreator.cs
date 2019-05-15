using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Text;


public static class ConfigCreator
{
    public static void Creat(string csFilePath, string dbFilePath, string tableName, bool firstCreat)
    {
        List<string> tableInfo = new List<string>();
        Sqlite sqlite = new Sqlite(dbFilePath);
        tableInfo = sqlite.GetTableInfo(tableName);
        sqlite.Close();
        sqlite = null;


        if (firstCreat)
        {
            var me = new StreamWriter(csFilePath + "/" + tableName + "Method.cs", false, new UTF8Encoding(false));
            var meBuilder = new StringBuilder();
            meBuilder.AppendFormat("public partial class {0} : IConfig", tableName);
            meBuilder.AppendLine();
            meBuilder.AppendLine("{");
            meBuilder.AppendLine("}");
            me.Write(meBuilder);
            me.Flush();
            me.Close();
        }

        var sw = new StreamWriter(csFilePath + "/" + tableName + ".cs", false, new UTF8Encoding(false));
        var strBuilder = new StringBuilder();

        strBuilder.AppendLine("/****************************************************************************");
        strBuilder.AppendFormat(" * {0}.{1} {2}\n", DateTime.Now.Year, DateTime.Now.Month, SystemInfo.deviceName);
        strBuilder.AppendLine(" ****************************************************************************/");
        strBuilder.AppendLine();

        strBuilder.AppendFormat("public partial class {0} : IConfig", tableName);
        strBuilder.AppendLine();
        strBuilder.AppendLine("{");


        for (int i = 0; i < tableInfo.Count; i += 6)
        {
            strBuilder.AppendFormat("\t[ConfigField(\"{0}\")]", tableInfo[i + 1]).AppendLine();
            if (tableInfo[i + 2] == "TEXT" || tableInfo[i + 2] == "VARCHAR(255)")
            {
                strBuilder.AppendFormat("\tpublic string {0}", tableInfo[i + 1]);
                strBuilder.AppendLine(" { get; set; }");
            }
            else if (tableInfo[i + 2] == "INTEGER" || tableInfo[i + 2] == "INT")
            {
                strBuilder.AppendFormat("\tpublic int {0} ", tableInfo[i + 1]);
                strBuilder.AppendLine(" { get; set; }");
            }
            else if (tableInfo[i + 2] == "FLOAT")
            {
                strBuilder.AppendFormat("\tpublic float {0} ", tableInfo[i + 1]);
                strBuilder.AppendLine(" { get; set; }");
            }
            strBuilder.AppendLine();
        }

        strBuilder.AppendLine("}");

        sw.Write(strBuilder);
        sw.Flush();
        sw.Close();
    }

    public static void CreatAll(string csFliePath, string dbFilePath, bool firstCreat)
    {
        List<string> tableNames = new List<string>();

        Sqlite sqlite = new Sqlite(dbFilePath);
        tableNames = sqlite.GetAllTableName();
        sqlite.Close();
        sqlite = null;

        foreach (var name in tableNames)
        {
            Creat(csFliePath, dbFilePath, name, firstCreat);
        }
    }
}
