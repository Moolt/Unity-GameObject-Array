using UnityEngine;

namespace ArrayModifiers.Scripts
{
    public abstract class PostProcessor : MonoBehaviour
    {
        public virtual void BeforeExecute()
        {
        }

        public abstract void Execute(Transform instance);

        public virtual void AfterExecute()
        {
        }
    }
}