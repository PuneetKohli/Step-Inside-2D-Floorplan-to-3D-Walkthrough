 using UnityEngine;
 using System.Collections;
 using System.Collections.Generic;
 using System;
 using System.Threading;
 using System.Linq;
         
 public class Loom : MonoBehaviour
 {
     private static Loom _current;
     private int _count;
     
     public static Loom Current
     {
         get
         {
             if (_current == null)
             {
                 var g = new GameObject("Loom");
                 _current = g.AddComponent<Loom>();
             }
                     
             return _current;
         }
     }
     
     private List<Action> _actions = new List<Action>();
     public class DelayedQueueItem
     {
         public float time;
         public Action action;
     }
     private List<DelayedQueueItem> _delayed = new  List<DelayedQueueItem>();
             
     public static void QueueOnMainThread(Action action, float time = 0f)
     {
         if (time != 0)
         {
             lock (Current._delayed)
             {
                 Current._delayed.Add(new DelayedQueueItem { time = Time.time + time, action = action});
             }
         }
         else
         {
             lock (Current._actions)
             {
                 Current._actions.Add(action);
             }
         }
     }
             
     public static void RunAsync(Action a)
     {
         var t = new Thread(RunAction);
         t.Priority = System.Threading.ThreadPriority.AboveNormal;
         t.Start(a);
     }
             
     private static void RunAction(object action)
     {
         ((Action)action)();
     }
             
     void OnDisable()
     {
         if (_current == this)
         {
                     
             _current = null;
         }
     }
             
     // Update is called once per frame
     void Update()
     {
         var actions = new List<Action>();
         lock (_actions)
         {
             actions.AddRange(_actions);
             _actions.Clear();
         }
         foreach (var a in actions)
         {
             a();
         }
         var delayList = new List<DelayedQueueItem>(); 
         lock (_delayed)
         {
             delayList.AddRange(_delayed);
         }
         foreach (var delayed in delayList.Where(d=>d.time <= Time.time).ToList())
         {
             lock (_delayed)
             {
                 _delayed.Remove(delayed);
             }
             delayed.action();
         }
                 
                 
                 
     }
 }
     