using Sirenix.OdinInspector;

/// <summary>
/// 通用的 MonoBehaviour 单例基类。
/// 继承自 SerializedMonoBehaviour，支持 Odin Inspector 序列化。
/// 用法：public class MyManager : SingletonMonoBehaviour&lt;MyManager&gt; {}
/// </summary>
/// <typeparam name="T">单例类型</typeparam>
public class SingletonMonoBehaviour<T> : SerializedMonoBehaviour where T : SingletonMonoBehaviour<T>
{
    public static T Instance;

    protected virtual void Awake()
    {
        if (Instance == null) Instance = this as T;
        else Destroy(gameObject);
    }
}
