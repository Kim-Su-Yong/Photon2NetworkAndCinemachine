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
        //FirePos 하위에 있는 총구 화염 효과 연결
        muzzleflash = firePos.Find("MuzzleFlash").GetComponent<ParticleSystem>();
    }

    void Update()
    {
        if (MouseHover.mouseHover.isHover == true) return;
        if (pv.IsMine && IsMouseClick)
        {
            FireBullet();
            //RPC로 원격지에 있는 함수 호출
            pv.RPC("FireBullet", RpcTarget.Others, null);
        }
    }
    [PunRPC]
    void FireBullet()
    {   //총구 화염 효과가 실행중이 아닌 경우 ㅔ총구 화염 효과 실행
        if (!muzzleflash.isPlaying) muzzleflash.Play(true);
        GameObject bullet = Instantiate(bulletPrefab, firePos.position, firePos.rotation);
    }
}
