using UnityEngine;
using System.Collections.Generic;

public class TestResManager : MonoBehaviour
{
    private class ResCache
    {
        public int RefTimes { get { return m_RefTimes; } }

        private int m_RefTimes;
        private GameObject m_Obj;

        public ResCache(string path)
        {
            m_RefTimes = 0;
            m_Obj = Resources.Load<GameObject>(path);
        }

        public GameObject Load()
        {
            m_RefTimes += 1;
            return m_Obj;
        }

        public void UnLoad()
        {
            m_RefTimes -= 1;
        }

        public void Release()
        {
            m_Obj = null;
        }
    }

    private Dictionary<string, ResCache> m_Caches;

    private static TestResManager instance;

    private static TestResManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<TestResManager>();
            }
            if (instance == null)
            {
                instance = new GameObject("[Test ResManager]").AddComponent<TestResManager>();
                DontDestroyOnLoad(instance.gameObject);
            }
            return instance;
        }
    }

    public static GameObject Load(string path)
    {
        if (Instance.m_Caches == null)
            Instance.m_Caches = new Dictionary<string, ResCache>();
        if (!Instance.m_Caches.ContainsKey(path))
            Instance.m_Caches[path] = new ResCache(path);
        return Instance.m_Caches[path].Load();
    }

    public static void UnLoad(string path)
    {
        if (Instance.m_Caches == null)
            return;
        if (!Instance.m_Caches.ContainsKey(path))
            return;
        Instance.m_Caches[path].UnLoad();
        if (Instance.m_Caches[path].RefTimes <= 0)
        {
            Instance.m_Caches[path].Release();
            Instance.m_Caches.Remove(path);
        }
    }

}
