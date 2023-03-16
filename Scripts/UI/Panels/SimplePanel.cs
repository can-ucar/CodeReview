
using UnityEngine;

namespace AnotherWorld.UI
{
    public class SimplePanel : UIBase
    {
        protected override void OnEnable()
        {
            base.OnEnable();
        }

        public override void OpenPanel()
        {
            base.OpenPanel();
        }

        public override void ClosePanel()
        {
            base.ClosePanel();
        }

        public void TogglePanel()
        {
            if (gameObject.activeSelf)
            {
                ClosePanel();
            }
            else
            {
                OpenPanel();
            }
        }
    }
}



