using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using Cinemachine;
public class Movement : MonoBehaviourPunCallbacks, IPunObservable
{
    private CharacterController controller;
    private new Transform transform;
    private Animator animator;
    private new Camera camera;

    private Plane plane; //������ plane�� ���� ĳ�����ϱ� ���� ����
    private Ray ray;
    private Vector3 hitPoint;
    public float moveSpeed = 10f;
    private PhotonView pv = null;
    private CinemachineVirtualCamera virtualCamera;
    private Vector3 Curpos;
    private Quaternion CurRot;
    public float damping = 10.0f;
    private Transform tr;
    void Start()
    {
        //tr = this.transform;
        controller = GetComponent<CharacterController>();
        transform = GetComponent<Transform>();
        animator = GetComponent<Animator>();
        camera = Camera.main;
        pv = GetComponent<PhotonView>();
        pv = PhotonView.Get(this);
        virtualCamera = GameObject.FindObjectOfType<CinemachineVirtualCamera>();
        if(pv.IsMine)
        {
            virtualCamera.Follow = transform;
            virtualCamera.LookAt = transform;
        }
        plane = new Plane(transform.up, transform.position);
        //������ �ٴ��� ���ΰ� ��ġ�� �������� ����
    }
    void Update()
    {
        if (pv.IsMine)
        {
            Move();
            Turn();
        }
        else
        {   //���ŵ� ��ǥ�� ������ �̵� ó��
            tr.position = Vector3.Lerp(tr.position, Curpos, Time.deltaTime * damping);
            //���ŵ� ȸ�������� ������ ȸ��ó��
            tr.rotation = Quaternion.Slerp(tr.rotation, CurRot, Time.deltaTime * damping);
        }
    }
    float h => Input.GetAxis("Horizontal");
    float v => Input.GetAxis("Vertical");
    void Move()
    {
        Vector3 cameraForward = camera.transform.forward;
        Vector3 cameraRight = camera.transform.right;
        cameraForward.y = 0.0f;
        cameraRight.y = 0.0f;
        //�̵��� ���� ���� ���
        Vector3 moveDir = (cameraForward * v) + (cameraRight * h);
        moveDir.Set(moveDir.x, 0f, moveDir.z);
        //���ΰ� ĳ���� �̵� ó��
        controller.SimpleMove(moveDir * moveSpeed);
        //���ΰ� ĳ���� �ִϸ��̼� ó��
        float forward = Vector3.Dot(moveDir, transform.forward);
        float strafe = Vector3.Dot(moveDir, transform.right);

        animator.SetFloat("Forward", forward);
        animator.SetFloat("Strafe", strafe);
    }
    void Turn()
    {
        ray = camera.ScreenPointToRay(Input.mousePosition);
        float enter = 0.0f;
        //������ �ٴڿ� ������ �߻��� �浹�� ������ �Ÿ��� enter������ ��ȯ
        plane.Raycast(ray, out enter);
        //������ �ٴڿ� ���̰� �浹�� ��ǥ�� ����
        hitPoint = ray.GetPoint(enter);

        Vector3 lookDir = hitPoint - transform.position;
        lookDir.y = 0;
        transform.localRotation = Quaternion.LookRotation(lookDir);
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if(stream.IsWriting)
        {
            stream.SendNext(tr.position);
            stream.SendNext(tr.rotation);
        }
        else if(stream.IsReading) //����
        {
            Curpos = (Vector3)stream.ReceiveNext();
            CurRot = (Quaternion)stream.ReceiveNext();
        }
    }
}
