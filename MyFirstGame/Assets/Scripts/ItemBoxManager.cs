using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemBoxManager : MonoBehaviour
{
    [SerializeField]
    PrefabCacheData[] ItemBoxFiles;

    Dictionary<string, GameObject> FileCache = new Dictionary<string, GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        Prepare();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public GameObject Generate(int index, Vector3 position)
    {
        if (index < 0 || index >= ItemBoxFiles.Length)
        {
            Debug.LogError("Generate error! out of range! index = " + index);
            return null;
        }

        string filePath = ItemBoxFiles[index].filePath;
        GameObject go = SystemManager.Instance.GetCurrentSceneMain<InGameSceneMain>().ItemBoxCacheSystem.Archive(filePath);
        go.transform.position = position;

        ItemBox item = go.GetComponent<ItemBox>();
        item.FilePath = filePath;

        return go;
    }

    public GameObject Load(string resourcePath)
    {
        GameObject go = null;

        if (FileCache.ContainsKey(resourcePath))   // ĳ�� Ȯ��
        {
            go = FileCache[resourcePath];
        }
        else
        {
            // ĳ�ÿ� �����Ƿ� �ε�
            go = Resources.Load<GameObject>(resourcePath);
            if (!go)
            {
                Debug.LogError("Load error! path = " + resourcePath);
                return null;
            }
            // �ε� �� ĳ�ÿ� ����
            FileCache.Add(resourcePath, go);
        }

        return go;
    }

    public void Prepare()
    {
        for (int i = 0; i < ItemBoxFiles.Length; i++)
        {
            GameObject go = Load(ItemBoxFiles[i].filePath);
            SystemManager.Instance.GetCurrentSceneMain<InGameSceneMain>().ItemBoxCacheSystem.GenerateCache(ItemBoxFiles[i].filePath, go, ItemBoxFiles[i].cacheCount, this.transform);
        }
    }

    public bool Remove(ItemBox item)
    {
        SystemManager.Instance.GetCurrentSceneMain<InGameSceneMain>().ItemBoxCacheSystem.Restore(item.FilePath, item.gameObject);
        return true;
    }

}