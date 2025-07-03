using ExitGames.Client.Photon.StructWrapping;
using Fusion;
using System.Collections;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Playables;

[RequireComponent(typeof(NetworkCharacterController))]
public class MNGCauKinhController : NetworkBehaviour
{
    public CinemachineCamera cam;
    private Vector3 clientCamForward;

    private NetworkCharacterController controller;
    private Animator animator;
    public PlayableDirector introduceTimeline;

    public bool isGoal = false;

    public string currentAnim;

    public LayerMask glassLayer;

    GlassBreakManager manager;
    public Transform feet;

    public override void Spawned()
    {
        controller = GetComponent<NetworkCharacterController>();
        controller.enabled = true;
        animator = GetComponent<Animator>();
        manager = GlassBreakManager.instance;

        if (!HasInputAuthority) return;
        cam = GameObject.Find("FreeLook Camera").GetComponent<CinemachineCamera>();
        cam.Follow = transform;
        cam.LookAt = transform;
    }

    void Update()
    {
        if (!Object.HasInputAuthority) return;
        
        if (manager != null && manager.Object.IsValid && manager.isGameStarted)
        {
            cam.enabled = true;
            // Send input to host
            if(cam.enabled)
            RPC_SendInput(cam.transform.forward);
        }

        if (Physics.Raycast(feet.position, Vector3.down, out RaycastHit hit, 0.1f, glassLayer))
        {
            hit.collider.gameObject.GetComponent<BreakGlass>().TryBreak();
        }

    }

    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    private void RPC_SendInput(Vector3 camForward)
    {
        clientCamForward = camForward; 
    }

    public override void FixedUpdateNetwork()
    {
        Vector3 moveDir = Vector3.zero;

        if (GetInput(out NetworkInputData data))
        {
            if(data.buttons.IsSet(NetworkInputData.JUMPBUTTON))
            {
                controller.Jump();
                RPC_ChangeAnim("Jump");
            }
            
            // Movement
            data.direction.Normalize();
            Vector3 camForward = Vector3.ProjectOnPlane(clientCamForward, Vector3.up).normalized;
            Vector3 camRight = Vector3.Cross(Vector3.up, camForward).normalized;

            camForward.y = 0;
            camRight.y = 0;

            moveDir = camRight * data.direction.x + camForward * data.direction.z;

            controller.Move(moveDir);

            // Animation
            if (controller.Grounded)
            {
                if (moveDir.magnitude > 0)
                    RPC_ChangeAnim("Run");
                else
                    RPC_ChangeAnim("Idle");
            }
        }
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void RPC_ChangeAnim(string animName, float blendTime = 0.25f)
    {
        if (animName == currentAnim) return;
        currentAnim = animName;

        animator.CrossFade(animName, blendTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.name == "Goal")
        {
            manager.RequestAddRank(Object.InputAuthority, Object.Id);
            if (Object.HasInputAuthority)
            RPC_RequestSetGoal();
        }
        else if(other.name == "Deadzone")
        {
            StartCoroutine(DelayResetCamera());
        }
    }

    IEnumerator DelayResetCamera()
    {
        cam.Follow = null;
        cam.LookAt = null;

        yield return new WaitForSecondsRealtime(1.5f);
        Vector3 teleportTo = FindFirstObjectByType<PlayerSpawner>().spawnPosition[Runner.LocalPlayer.PlayerId - 1].position;
        Vector3 delta = transform.position - teleportTo;

        controller.Teleport(teleportTo);
        cam.transform.position = transform.position;
        yield return null;
        cam.Follow = transform;
        cam.LookAt = transform;
    }

    [Rpc(RpcSources.InputAuthority, RpcTargets.All)]
    void RPC_RequestSetGoal()
    {
        isGoal = true;
    }
}
