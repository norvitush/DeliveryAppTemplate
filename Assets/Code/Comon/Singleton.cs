using UnityEngine;

public abstract class Singleton<InhClass> : MonoBehaviour where InhClass : Singleton<InhClass>
{
    public static InhClass Instance = null;
    public static object _lock = new object();
    protected void Awake()
    {
        if (Instance == null)
        {
            lock (_lock) //onetime protect
            {
                if (Instance == null)       //DOUBLE TAP
                {
                    Instance = (InhClass)this;
                    Init();
                }
                else
                    Destroy(gameObject);
            }
        }
        else
            Destroy(gameObject);
    }
    protected virtual void Init()
    {
        gameObject.name = "S_" + typeof(InhClass).ToString();
        if (gameObject.transform.parent == null)
        {
            DontDestroyOnLoad(gameObject);
        }
    }
}