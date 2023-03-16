using System;
using UnityEngine;

namespace AnotherWorld.Core
{
    public interface IInputManager
    {
        public void SetEnabled(bool enabled);
    }


    public abstract class InputManager : Manager<InputManager>, IInputManager
    {
        public void SetEnabled(bool enabled)
        {
            this.enabled = enabled;
        }
    }
}

