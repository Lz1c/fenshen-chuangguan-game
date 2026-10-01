using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class OnMouseTrigger : MonoBehaviour
{
    public Behaviour componentToActivate;

    private void OnMouseEnter()
    {
        if (componentToActivate != null)
        {
            componentToActivate.enabled = true;
        }
    }
    private void OnMouseExit()
    {
        if (componentToActivate != null)
        {
            componentToActivate.enabled = false;
        }
    }


}
