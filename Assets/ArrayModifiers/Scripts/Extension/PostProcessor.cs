using UnityEngine;

namespace ArrayModifiers.Scripts.Extension
{
    [ExecuteInEditMode]
    public abstract class PostProcessor : MonoBehaviour
    {
        [HideInInspector] [SerializeField] private bool initialized;

        public static class Fields
        {
            public const string Initialized = nameof(initialized);
        }
        
        public virtual void BeforeExecute()
        {
        }

        public abstract void Execute(InstanceInfo info);

        public virtual void AfterExecute()
        {
        }
    }
}