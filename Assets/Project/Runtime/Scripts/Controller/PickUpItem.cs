using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PickUpItem : MonoBehaviour
{
    public PlayerInput playerInput;
    public Transform holdingObject;
    public LayerMask objectMask;
    public Transform handTransform;

    public Transform interactArea;
    public float interactRange;

    public GameObject trashBagPrefab;

    public TestMNGController playerController;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (playerInput.actions["Interact"].triggered)
        {
            if (holdingObject == null)
            {
                Collider[] cols = Physics.OverlapSphere(interactArea.position, interactRange, objectMask);
                if (cols.Length <= 0) return;

                if (cols[0].gameObject.layer == LayerMask.NameToLayer("Trash") && cols[0].transform.childCount > 0)
                {
                    holdingObject = Instantiate(trashBagPrefab, handTransform).transform;
                    Destroy(cols[0].gameObject);
                }
                else
                {
                    holdingObject = cols[0].transform;
                    holdingObject.SetParent(handTransform);
                }

                Bounds bounds = holdingObject.GetComponent<Collider>().bounds;

                Vector3 offset = bounds.center - holdingObject.transform.position;

                holdingObject.transform.position = handTransform.position - offset;
                holdingObject.transform.localEulerAngles = Vector3.zero;
                holdingObject.GetComponent<Rigidbody>().isKinematic = true;
                playerController.ChangeAnim("GrabIdle");
            }
            else
            {
                holdingObject.SetParent(null);
                Vector3 throwDirection = (transform.forward * 3f + Vector3.up * 4f).normalized;
                holdingObject.GetComponent<Rigidbody>().isKinematic = false;
                holdingObject.GetComponent<Rigidbody>().AddForce(throwDirection * 50f, ForceMode.Impulse);
                holdingObject = null;
                playerController.ChangeAnimImmidiate("Throw");
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(interactArea.position, interactRange);
    }
}
