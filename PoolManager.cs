using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

//-------------------------------------------------------
// PoolManager is a singleton, which holds some object pool,
// if we need a pool to get object, we must know the name
// of the pool, so we can use this name to get the pool we
// want from PoolManager
//-------------------------------------------------------
public class PoolManager
{

    static private PoolManager m_instance;

    static private Dictionary<string, ObjectPool> m_pools;

    // use this method to get the singleton
    static public PoolManager GetInstance ( )
    {
        if (m_instance == null)
        {
            m_instance = new PoolManager();
            m_pools = new Dictionary<string, ObjectPool>();
        }
        return m_instance;
    }

    public ObjectPool GetPool (GameObject o)
    {
        string poolName = o.name;
        if (!m_pools.ContainsKey(poolName))
        {
            // if it is a new object, we must create a new pool for it
            ObjectPool _pool = new ObjectPool(o);
            m_pools.Add(poolName, _pool);
            return _pool;
        }
        return m_pools[poolName];
    }

    // we should explicitly know this pool exiting
    public ObjectPool GetPool (string poolName)
    {
        return m_pools[poolName];
    }



    //--------------------------------------------------------------
    // A ObjectPool holds some GameObject with same type, by a List. 
    // Active GameObjects are in first part of the List, unactive 
    // GameObjects, the availables, are in second part, so if we need
    // a GameObjects, we get it from the end of the List
    //--------------------------------------------------------------
    public class ObjectPool
    {
        // this list is like this:
        // ---------------------|---------------------
        //  active(unavailable)   unactive(available)
        private List<GameObject> m_objects;
        private int m_total;
        private int m_activeNum;

        private GameObject m_original;

        public ObjectPool (GameObject _o)
        {
            m_objects = new List<GameObject>();
            m_original = _o;
            m_total = 0;
            m_activeNum = 0;
        }

        public int GetCount ( )
        {
            return m_total;
        }

        public int GetActiveNum ( )
        {
            return m_activeNum;
        }

        public int GetAvailableNum ( )
        {
            return m_total = m_activeNum;
        }

        private void CreateObject ( )
        {
            m_total++;
            GameObject _go = (GameObject)Object.Instantiate(m_original);
            int _l = _go.name.Length;
            if (_go.name[_l - 1].Equals(')'))
                _go.name = _go.name.Remove(_l - 7);
            _go.SetActive(false);
            m_objects.Add(_go);
        }

        public GameObject GetObject (Transform parent = null)
        {
            if (m_activeNum >= m_total)
                CreateObject();

            m_activeNum++;
            if (parent != null)
                m_objects[m_activeNum - 1].transform.SetParent(parent);
            m_objects[m_activeNum - 1].SetActive(true);
            return m_objects[m_activeNum - 1];
        }

        // get a object and set it to given position before activing it
        public GameObject GetObject (Vector3 position)
        {
            if (m_activeNum >= m_total)
                CreateObject();

            m_activeNum++;
            m_objects[m_activeNum - 1].transform.position = position;
            m_objects[m_activeNum - 1].SetActive(true);
            return m_objects[m_activeNum - 1];
        }

        // get a object and set its position and direction
        public GameObject GetObject (Vector3 position, Vector3 direction)
        {
            if (m_activeNum >= m_total)
                CreateObject();

            m_activeNum++;
            m_objects[m_activeNum - 1].transform.position = position;
            m_objects[m_activeNum - 1].transform.forward = direction;
            m_objects[m_activeNum - 1].SetActive(true);
            return m_objects[m_activeNum - 1];
        }

        // add a component T to all objects
        public void AddComponent<T> ( ) where T : UnityEngine.Component
        {
            for (int i = 0; i < m_total; i++)
            {
                m_objects[i].AddComponent<T>();
            }
        }

        // get an active or unactive object with index
        public GameObject GetObject (int i)
        {
            if (i > m_total)
            {
                Debug.Log("list out of range");
                return null;
            }
            return m_objects[i];
        }

        // return the object back to the pool
        public void GivebackObject (GameObject _go)
        {
            _go.transform.parent = null;
            _go.SetActive(false);
            m_activeNum--;
            if (_go == m_objects[m_activeNum])
                return;

            GameObject _tmp = _go;
            _go = m_objects[m_activeNum];
            m_objects[m_activeNum] = _tmp;
        }

        public void GivebackObject (Transform _tf)
        {
            GivebackObject(_tf.gameObject);
        }

        public List<GameObject> GetAllObjects ( )
        {
            return m_objects;
        }

        public void Delete (GameObject _go)
        {
            m_objects.Remove(_go);
            _go = null;
        }
    }

}




