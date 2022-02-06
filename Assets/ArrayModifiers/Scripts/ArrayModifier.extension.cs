using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ArrayModifiers.Scripts
{
    public partial class ArrayModifier
    {
        public ArrayModifier FirstInstance()
        {
            var components = AllInstances();
            return !components.Any() ? null : components[0];
        }
        
        public bool IsFirstInstance()
        {
            return FirstInstance() == this;
        }
    
        public IList<ArrayModifier> AllInstances()
        {
            return GetComponents<ArrayModifier>()
                .Where(a => !a._executionLocked)
                .ToList();
        }

        private bool TryGetPreviousInstance(out ArrayModifier neighboringInstance)
        {
            neighboringInstance = TryGetNeighboringInstance(-1);
            return neighboringInstance != null;
        }

        private bool TryGetNextInstance(out ArrayModifier neighboringInstance)
        {
            neighboringInstance = TryGetNeighboringInstance(1);
            return neighboringInstance != null;
        }

        private ArrayModifier TryGetNeighboringInstance(int offset)
        {
            var components = AllInstances();
            var selfIndex = components.IndexOf(this);
            var index = Mathf.Clamp(selfIndex + offset, 0, components.Count - 1);

            if (components.Count == 1 || index == selfIndex)
            {
                return null;
            }

            return components[selfIndex + offset];
        }
    }
}