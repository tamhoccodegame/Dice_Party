using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine;
[RequireComponent(typeof(Rigidbody))]
public class RollerConfigurableJoint : MonoBehaviour
{
    public TiltAI boardController;
    public Transform boardTransform;
    public bool isVertical = true; // true = roller đứng dọc, false = roller nằm ngang
    public float moveSpeed = 2f;
    public float rollerRadius = 0.5f;
    public Vector2 boardSize;

    private Vector3 localPos;
    private Quaternion baseOrientation;

    public float heightOffset = 0f; // offset thêm vào, mặc định = 0
    void Awake()
    {
        // Lấy size từ BoxCollider của Board
        BoxCollider box = boardTransform.GetComponent<BoxCollider>();
        if (box != null)
            boardSize = new Vector2(box.size.x, box.size.z);
    }

    void Start()
    {
        // Lưu local pos ban đầu
        localPos = boardTransform.InverseTransformPoint(transform.position);
        baseOrientation = transform.localRotation;

        // Cố định Y theo radius
        localPos.y = rollerRadius + heightOffset;
    }

    void Update()
    {
        UpdateRollerMovement();
        ApplyWorldTransform();
    }

    void UpdateRollerMovement()
    {
        Vector2 tilt = boardController.TiltInput;

        float moveX = 0f;
        float moveZ = 0f;

        // Horizontal roller (nằm ngang → lăn dọc theo Z)
        if (!isVertical)
            //moveZ = tilt.y;
            moveX = -tilt.x;


        // Vertical roller (đứng dọc → lăn ngang theo X)
        else
            moveZ = tilt.y;

        // Cập nhật local pos
        localPos += new Vector3(moveX, 0, moveZ) * moveSpeed * Time.deltaTime;

        // Clamp local pos trong board
        float halfX = (boardSize.x * 0.5f) - rollerRadius;
        float halfZ = (boardSize.y * 0.5f) - rollerRadius;

        localPos.x = Mathf.Clamp(localPos.x, -halfX, halfX);
        localPos.z = Mathf.Clamp(localPos.z, -halfZ, halfZ);
    }

    void ApplyWorldTransform()
    {
        // Local → World
        Vector3 worldPos = boardTransform.TransformPoint(localPos);

        // Dính chặt lên plane của Board
        Plane boardPlane = new Plane(boardTransform.up, boardTransform.position);
        worldPos = boardPlane.ClosestPointOnPlane(worldPos) + boardTransform.up * (rollerRadius + heightOffset);

        transform.position = worldPos;

        // Align Up theo Board, giữ orientation gốc
        Quaternion alignUp = Quaternion.FromToRotation(Vector3.up, boardTransform.up);
        transform.rotation = alignUp * baseOrientation;

        // Rolling animation dựa theo hướng di chuyển
        float rollAmount = moveSpeed * 360 * Time.deltaTime;

        if (isVertical)
        {
            // Vertical → lăn dọc (Z) → xoay quanh X
            transform.Rotate(Vector3.right, rollAmount * boardController.TiltInput.y, Space.Self);
        }
    
        else
        {
            // Roller ngang → lăn dọc → xoay quanh trục X (local)
            transform.Rotate(Vector3.right, rollAmount * -boardController.TiltInput.y, Space.Self);
        }
    }


}
