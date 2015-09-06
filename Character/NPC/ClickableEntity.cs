using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

public class ClickableEntity : MonoBehaviour
{

    private Transform m_playerTF;

    public float distanceThreshold = 5;

    // Update is called once per frame
    protected void Update ( )
    {
        HandleClick();
    }

    // callback when click/touch on a clickable gameobject, such as a Merchant
    virtual public void OnClick ( )
    {

    }

    private void HandleClick ( )
    {
        if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit = new RaycastHit();
            if (Physics.Raycast(ray, out hit))
            {
                GameObject go = hit.collider.gameObject;
                if (m_playerTF == null)
                {
                    m_playerTF = CharacterManager.charMng.player.transform;
                }
                if (go.Equals(gameObject) && Vector3.Distance(m_playerTF.position, transform.position) < distanceThreshold)
                {
                    OnClick();
                }
            }
        }
    }

    private void HandleTouch ( )
    {
        if (Input.touchCount == 1 && !EventSystem.current.IsPointerOverGameObject(0))
        {
            Touch touch = Input.GetTouch(0);

        }
    }

}
