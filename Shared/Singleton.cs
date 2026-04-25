using Sirenix.OdinInspector;
using UnityEngine;

public class Singleton<T> : SerializedMonoBehaviour where T : Singleton<T>
{
    [SerializeField, FoldoutGroup("Singleton Settings")]
    private bool isPersistent = false;

    protected virtual bool DestroyTargetGameObject => false;

    public static T I { get; private set; } = null;

    public static bool IsValid() => I != null;

    private void Awake()
    {
        if (I == null)
        {
            I = this as T;

            if (isPersistent)
            {
                transform.SetParent(null);
                DontDestroyOnLoad(gameObject);
            }

            I.Init();
            return;
        }

        if (DestroyTargetGameObject)
        {
            Destroy(gameObject);
        }
        else
        {
            Destroy(this);
        }
    }

    protected virtual void Init() { }

    private void OnDestroy()
    {
        if (I == this)
        {
            I = null;
        }
        OnRelease();
    }

    protected virtual void OnRelease() { }
}