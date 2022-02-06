using UnityEngine;

namespace ArrayModifiers.Scripts
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

        public abstract void Execute(Transform instance);

        public virtual void AfterExecute()
        {
        }
    }
}