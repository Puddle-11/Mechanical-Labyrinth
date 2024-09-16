using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIButtonFunction : MonoBehaviour
{
    private UIMenuManager managerRef;
    
    public void ChangeMenu(GameObject _menu)
    {
        if(GetManagerRef() != null)
        {
            managerRef.ActivateMenu(_menu);
        }
    }
    public UIMenuManager GetManagerRef()
    {
        if (managerRef != null) return managerRef;

        Transform[] parents = GetComponentsInParent<Transform>();
        for (int i = 0; i < parents.Length; i++)
        {
            if (parents[i].TryGetComponent(out managerRef))
            {
                return managerRef;
            }
        }
        return null;

    }
}
