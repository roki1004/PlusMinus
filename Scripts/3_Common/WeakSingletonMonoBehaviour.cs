using System.Collections;
using UnityEngine;

public class WeakSingletonMonoBehaviour<T> : MonoBehaviour
{
    static System.WeakReference instance_ = null;
    public static T Instance
    {
        get
        {
            if (instance_ == null)
                return default(T);
            return (T)instance_.Target;
        }
    }

    protected virtual void Awake()
    {
        instance_ = new System.WeakReference(this);
    }

    protected virtual void OnDestroy()
    {
        instance_ = null;
    }
}
