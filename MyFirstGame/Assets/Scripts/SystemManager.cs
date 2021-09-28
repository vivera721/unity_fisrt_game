using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SystemManager : MonoBehaviour
{
    static SystemManager instance = null;

    public static SystemManager Instance
    {
        get
        {
            return instance;
        }
    }
    [SerializeField]
    EnemyTable enemyTable;

    public EnemyTable EnemyTable
    {
        get
        {
            return enemyTable;
        }
    }

    [SerializeField]
    ItemTable itemTable;

    public ItemTable ItemTable
    {
        get
        {
            return itemTable;
        }
    }

    [SerializeField]
    ItemDropTable itemDropTable;

    public ItemDropTable ItemDropTable
    {
        get
        {
            return itemDropTable;
        }
    }
    BaseSceneMain currentSceneMain;

    public BaseSceneMain CurrentSceneMain
    {
        set
        {
            currentSceneMain = value;
        }
    }
    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("시스템매니저 에러! 싱글톤 에러!");
            Destroy(gameObject);
            return;
        }

        instance = this;

        DontDestroyOnLoad(gameObject);
    }


    // Start is called before the first frame update
    void Start()
    {
        BaseSceneMain baseSceneMain = GameObject.FindObjectOfType<BaseSceneMain>();
        Debug.Log("OnSceneLoaded ! baseSceneMain.name = " + baseSceneMain.name);
        SystemManager.Instance.CurrentSceneMain = baseSceneMain;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public T GetCurrentSceneMain<T>()
         where T : BaseSceneMain
    {
        return currentSceneMain as T;
    }
}
