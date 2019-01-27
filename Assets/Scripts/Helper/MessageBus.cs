using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public static class MessageBus
{
    private static List<Entry> _entries = new List<Entry>();

    public static void Push<T>(T message)
    {
        var ttype = typeof(T);
        var subscriber = _entries.Where(e => e.TType.IsAssignableFrom(ttype)).Select(e => e as Entry<T>).Where(e => e != null);
        foreach (var s in subscriber)
        {
            try
            {
                s.Action(message);
            }
            catch (Exception ex)
            {
                Debug.LogError("Error on pushing message: " + ex);
            }
        }
    }  

    public static void Subscribe<T>(object target, Action<T> subscriber)
    {
        var entry = new Entry<T>();
        entry.Target = target;
        entry.TType = typeof(T);
        entry.Action = subscriber;

        _entries.Add(entry);
    }

    public static void UnSubscribe<T>(object target)
    {
        var type = typeof(T);
        var l = new List<Entry>();
        foreach (var e in _entries)
        {
            if(e.Target == target && e.TType == type)            
               l.Add(e);            
        }

        foreach (var ll in l)
        {
            _entries.Remove(ll);
        }
    }

    private class Entry
    {
        public Type TType;
        public object Target;
    }

    private class Entry<TV> : Entry
    {
        public Action<TV> Action;       
    }
}
