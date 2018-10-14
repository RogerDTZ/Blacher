using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class TankManager : NetworkBehaviour {

    public Sprite app_blue, app_red;
    public int m_playerID;
    public int m_side;

    private CameraFollow cameraFollow;
    private TankMovement tankMovement;
    private SpriteRenderer renderer;
    private TankWeaponSystem weaponSystem;

    private const int BLU = 0, RED = 1;

    // Use this for initialization
    void Start()
    {
        cameraFollow = GameObject.FindObjectOfType<Camera>().GetComponent<CameraFollow>();
        UpdateCameraTrack();
        renderer = gameObject.GetComponent<SpriteRenderer>();
        tankMovement = gameObject.GetComponent<TankMovement>();
        weaponSystem = gameObject.GetComponent<TankWeaponSystem>();
        SetParameter(0, BLU);
    }

    // Update is called once per frame
    void Update()
    {
        /*
        if (isLocalPlayer)
        {
            if (Input.GetKeyDown(KeyCode.Q))
                weaponSystem.CmdSetCurrentSlot(0);
            if (Input.GetKeyDown(KeyCode.E))
                weaponSystem.CmdSetCurrentSlot(1);
        }
        */
    }

    private void UpdateCameraTrack()
    {
        if (gameObject.activeSelf)
            cameraFollow.SetTarget(gameObject);
        else
            cameraFollow.RemoveTarget();
    }

    public void SetParameter(int playerID, int side)
    {
        m_playerID = playerID;
        SetSide(side);
    }

    private void SetSide(int side)
    {
        m_side = side;
        if (m_side == BLU)
            renderer.sprite = app_blue;
        else
            renderer.sprite = app_red;
        weaponSystem.SetSide(m_side);
    }

}