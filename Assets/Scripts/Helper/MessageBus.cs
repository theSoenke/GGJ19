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
            if (s.Action != null)
                s.Action(message);
        }
    }  

    public static void Subscribe<T>(Action<T> subscriber)
    {
        var entry = new Entry<T>();
        entry.TType = typeof(T);
        entry.Action = subscriber;

        _entries.Add(entry);
    }

    private class Entry
    {
        public Type TType;
    }

    private class Entry<TV> : Entry
    {
        public Action<TV> Action;       
    }
}
