using UnityEngine;
using System.Collections;
using System;

//自定义配置类特性，用于跟数据库中单条数据匹配
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
public class ConfigFieldAttribute : Attribute
{
    public string FiledName
    {
        get;
        private set;
    }

    public ConfigFieldAttribute(string name)
    {
        FiledName = name;
    }

}
