using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyFactory : MonoBehaviour
{

    public const string EnemyPath = "Prefabs/Enemy";

    Dictionary<string, GameObject> EnemyFileCache = new Dictionary<string, GameObject>();
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public GameObject Load(string resourcePath)
    {
        GameObject go = null;

        if (EnemyFileCache.ContainsKey(resourcePath))
        {
            go = EnemyFileCache[resourcePath];
        }
        else
        {
            go = Resources.Load<GameObject>(resourcePath);
            if (!go)
            {
                Debug.LogError("로드 에러! 경로 = " + resourcePath);
                return null;
            }
            EnemyFileCache.Add(resourcePath, go);
        }
        GameObject instanced = Instantiate < GameObject >(go);
        return instanced;
    }
}
