using System.Collections;
using System.Collections.Generic;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;

public class BaseMenuButton : MonoBehaviour, IMenuButton
{
    [SerializeField] private Animator anim;
    public virtual void Click(){}

    public void Select()
    {
        anim.SetBool("Selected", true);
    }
    public void Deselect()
    {
        anim.SetBool("Selected", false);
    }
}
