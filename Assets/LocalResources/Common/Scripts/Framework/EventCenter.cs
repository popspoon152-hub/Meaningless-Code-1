using System;
using System.Collections.Generic;
using UnityEngine;

public class EventCenter : SingletonPatternBase<EventCenter>
{
    private readonly Dictionary<string, Action<List<object>>> _events = new();

    /// <summary>
    /// �¼����Ĺ���������������з�������Ҫ����List��Ϊ����
    /// </summary>
    /// <param name="command"></param>
    /// <param name="call"></param>
    public void AddListener(object command, Action<List<object>> call)
    {
        var key = command.GetType().Name + "_" + command;
        if (_events.ContainsKey(key))
        {
            _events[key] += call;
        }
        else
        {
            _events.Add(key, call);
        }
    }

    public void RemoveListener(object command, Action<List<object>> call)
    {
        var key = command.GetType().Name + "_" + command;
        if (_events.ContainsKey(key))
        {
            _events[key] -= call;
        }
    }

    public void Dispatch(object command, List<object> args = null)
    {
        var key = command.GetType().Name + "_" + command;
        if (_events.ContainsKey(key))
        {
            _events[key]?.Invoke(args);
        }
    }

    public void RemoveListeners(object command)
    {
        var key = command.GetType().Name + "_" + command;
        if (_events.ContainsKey(key))
        {
            _events[key] = null;
        }
    }
}
