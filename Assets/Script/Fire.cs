using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
public class Fire : MonoBehaviour
{
    public Transform firePos;
    public GameObject bulletPrefab;
    private ParticleSystem muzzleflash;
    private PhotonView pv;
    private bool IsMouseClick => Input.GetMouseButtonDown(0);
    void Start()
    {
        pv = GetComponent<PhotonView>();
        //FirePos ������ �ִ� �ѱ� ȭ�� ȿ�� ����
        muzzleflash = firePos.Find("MuzzleFlash").GetComponent<ParticleSystem>();
    }

    void Update()
    {
        if (MouseHover.mouseHover.isHover == true) return;
        if (pv.IsMine && IsMouseClick)
        {
            FireBullet();
            //RPC�� �������� �ִ� �Լ� ȣ��
            pv.RPC("FireBullet", RpcTarget.Others, null);
        }
    }
    [PunRPC]
    void FireBullet()
    {   //�ѱ� ȭ�� ȿ���� �������� �ƴ� ��� ���ѱ� ȭ�� ȿ�� ����
        if (!muzzleflash.isPlaying) muzzleflash.Play(true);
        GameObject bullet = Instantiate(bulletPrefab, firePos.position, firePos.rotation);
    }
}
