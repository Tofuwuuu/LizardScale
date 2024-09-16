using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

public class PathingManager : MonoBehaviour
{
    [SerializeField] Dictionary<Platform, ConnectionPlatform[]> connectionDictionary;
    GameObject player;

    public delegate void PathUpdate();
    public static event PathUpdate updated;

    private void Awake()
    {
        connectionDictionary = new Dictionary<Platform, ConnectionPlatform[]>();
        player = GameObject.FindGameObjectWithTag("Player");
    }
    
    public void AddConnections(Platform platform, ConnectionPlatform[] connections)
    {
        connectionDictionary.Add(platform, connections);
    }
     
    public void EventUpdate()
    {
        updated();
    }
             
    
    public List<ConnectionPlatform> FindPath(Platform currentPlatform, Platform targetPlatform)
    {
        Dictionary<Platform, ConnectionPlatform> parentMap = new Dictionary<Platform, ConnectionPlatform>();
        Dictionary<Platform, float> distanceMap = new Dictionary<Platform, float>();
        PlatformQueue<ConnectionPlatform> queue = new PlatformQueue<ConnectionPlatform>();

        foreach (ConnectionPlatform conn in connectionDictionary[currentPlatform])
        {
            conn.cost = Vector2.Distance(player.transform.position, conn.connectedPlatformJumpPoint);

            queue.Enqueue(conn);
            distanceMap[conn.otherPlatform.GetComponent<Platform>()] = conn.cost;
            parentMap[conn.otherPlatform.GetComponent<Platform>()] = conn;
        }

        while (queue.Count > 0)
        {
            ConnectionPlatform currentlyExploring = queue.Dequeue();

            if (currentlyExploring.otherPlatform.GetComponent<Platform>() == targetPlatform)
            {
                return (MakePath(currentPlatform, targetPlatform, parentMap));
            }
            if(connectionDictionary.ContainsKey(currentlyExploring.otherPlatform.GetComponent<Platform>()) || connectionDictionary[currentlyExploring.otherPlatform.GetComponent<Platform>()].Length == 0) 
            {
                currentlyExploring = parentMap[currentlyExploring.otherPlatform.GetComponent<Platform>()];
                continue;
            }
            foreach(ConnectionPlatform neighbor in connectionDictionary[currentPlatform])
            {
                float calcDist = distanceMap[currentlyExploring.otherPlatform.GetComponent<Platform>()];
                if (!distanceMap.ContainsKey(neighbor.otherPlatform.GetComponent<Platform>()) || calcDist < distanceMap[neighbor.otherPlatform.GetComponent<Platform>()])
                {
                    distanceMap[neighbor.otherPlatform.GetComponent<Platform>()] = calcDist;
                    parentMap[neighbor.otherPlatform.GetComponent<Platform>()] = currentlyExploring;
                    queue.Enqueue(neighbor);
                }
            }
        }
        return null;
    }

    public List<ConnectionPlatform> MakePath(Platform currentPlatform, Platform targetPlatform, Dictionary<Platform, ConnectionPlatform> parentMap)
    {
        List<ConnectionPlatform> path = new List<ConnectionPlatform>();
        Platform presentPlatform = currentPlatform;

        while(presentPlatform != currentPlatform && parentMap.ContainsKey(presentPlatform))
        {
            ConnectionPlatform conn = parentMap[presentPlatform];
            path.Insert(0, conn);
            presentPlatform = currentPlatform;
        }
        return path;
    }
}















public class PlatformQueue<T> : IEnumerable<T> where T : IComparable<T>
{
    LinkedList<T> _queue = new LinkedList<T>();

    public void Enqueue(T item)
    {
        if(_queue.Count == 0)
        {
            _queue.AddLast(item);
        }
        else
        {
            var current = _queue.First;
            while(current != null && current.Value.CompareTo(item) > 0)
            {
                current = current.Next;
            }
            if(current == null)
            {
                _queue.AddLast(item);
            }
            else
            {
                _queue.AddBefore(current, item);
            }
        }
    }

    public T Dequeue()
    {
        if (_queue.Count == 0)
        {
            throw new InvalidOperationException("Queue is empty");
        }
        T value = _queue.First.Value;
        _queue.RemoveFirst();
        return value;
    }

    public T Peek()
    {
        if ( _queue.Count == 0)
        {
            throw new InvalidOperationException("Queue is empty");
        }
        return _queue.First.Value;
    }

    public int Count
    {
        get
        { 
            return _queue.Count; 
        }
    }

    public void Clear()
    {
        _queue.Clear();
    }

    public IEnumerator<T> GetEnumerator()
    {
        return _queue.GetEnumerator();
    }
    IEnumerator System.Collections.IEnumerable.GetEnumerator()
    {
        return _queue.GetEnumerator();
    }

}