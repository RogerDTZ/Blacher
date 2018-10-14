using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class TankWeaponSystem : NetworkBehaviour {

    public const int WeaponNum=2;
    public const int MaxGrade=1;

    private float[,] weapon_CDTime = { {1,1,1,0.8f },
                                       {3,2,2,1.5f }
                                     };
    private float[,] weapon_reloadTime = { {7,6,5,4},
                                           {15,14,13,12}
                                         };
    private bool[] weapon_reloadWhileUnfocus = { false,
                                                 true
                                               };
    private float[,] weapon_shellSpeed = { {10f,11f,12f,13f},
                                           {3f,4f,5f,6f }
                                         };
    private float[,] weapon_damage = { {4f,5f,6f,7f},
                                       {10f,11f,12f,13f}
                                     };
    private int[,] weapon_magazineCapacity = { {10,10,13,15},
                                               {3,3,3,4}
                                             };

    private int m_side;

    public string SkinPath;
    private Sprite[,,] ShellSkin;
    private Sprite[,] TurretSkin;

    private const int MaxTaking = 2;
    protected string[] WeaponName = {"Origin","Heavy","Bouncy","Minigun"};
    private Dictionary<string,int> dic = new Dictionary<string, int>();
    private Weapon[] m_weapon;
    private bool[] m_have;
    private int[] m_grade;

    private int m_currentOwn;
    private int m_currentID;
    private int[] m_slot = new int[MaxTaking];

    public GameObject turretset;
    private Vector3 m_turretDirection;
    private float m_turretRotation;

    // private Spell m_spell;

	// Use this for initialization
	void Start () {
        LoadSkin();

        InitWeaponList();
        InitSlot();

        m_currentOwn = 0;
        m_currentID = -1;
        GetNewWeapon(dic["Origin"]);
        TakeWeapon(dic["Origin"], -1);
        GetNewWeapon(dic["Heavy"]);
        TakeWeapon(dic["Heavy"], -1);
        SetCurrentSlot(0);
	}
	
	// Update is called once per frame
	void Update () {
        if (isLocalPlayer)
        {
            if (Input.GetKeyDown(KeyCode.Q))
                SetCurrentSlot(0);
            if (Input.GetKeyDown(KeyCode.E))
                SetCurrentSlot(1);
            UpdateFace();
            CheckFire();
        }
    }

    public void SetSide(int side)
    {
        m_side = side;
    }

    private void LoadSkin()
    {
        ShellSkin = new Sprite[WeaponNum,MaxGrade,2];
        TurretSkin = new Sprite[WeaponNum,MaxGrade];
        for(int i = 0; i < WeaponNum; i++)
        {
            for(int j = 0; j < MaxGrade; j++)
            {
                ShellSkin[i, j, 0] = Resources.Load<Sprite>(SkinPath + "/" + i + "/" + "shell_" + j + "_b");
                ShellSkin[i, j, 1] = Resources.Load<Sprite>(SkinPath + "/" + i + "/" + "shell_" + j + "_r");
                TurretSkin[i, j] = Resources.Load<Sprite>(SkinPath + "/" + i + "/" + "turret_" + j);
            }
        }
    }

    private void InitWeaponList()
    {
        m_weapon = new Weapon[WeaponNum];
        m_have = new bool[WeaponNum];
        m_grade = new int[WeaponNum];
        Weapon t;
        for(int i = 0; i < WeaponNum; i++)
        {
            dic[WeaponName[i]] = i;

            GameObject turret = (GameObject)Instantiate(Resources.Load("Prefabs/Turret"));
            turret.transform.parent = turretset.transform;
            m_weapon[i] = turret.GetComponent<Weapon>();
            m_grade[i] = 0;
            m_weapon[i].Setup(isLocalPlayer,
                              WeaponName[i],
                              ShellSkin[i, m_grade[i], m_side], 
                              TurretSkin[i, m_grade[i]], 
                              weapon_CDTime[i, m_grade[i]], 
                              weapon_reloadTime[i, m_grade[i]], 
                              weapon_reloadWhileUnfocus[i], 
                              weapon_shellSpeed[i, m_grade[i]], 
                              weapon_damage[i, m_grade[i]], 
                              weapon_magazineCapacity[i, m_grade[i]]
                              );
            m_have[i] = false;
        }
    }

    private void InitSlot()
    {
        m_slot = new int[MaxTaking];
        for (int i = 0; i < MaxTaking; i++)
            m_slot[i] = -1;
    }

    private void CheckFire()
    {
        if (m_currentID == -1)
            return;
        if (Mathf.Abs(Input.GetAxis("Fire1")) > 0f)
        {
            m_weapon[m_currentID].Fire();
        }
    }

    private void UpdateFace()
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = 0f;
//        Debug.Log(mousePosition);
        m_turretDirection = (mousePosition - transform.position).normalized;
        m_turretRotation = angle_360(Vector3.right, m_turretDirection);
        CmdSetTurretRotation(m_turretRotation);
        Debug.Log("Rotation: " + m_turretRotation);
    }

    [Command]
    private void CmdSetTurretRotation(float z)
    {
        RpcSetTurretRotation(z);
    }

    [ClientRpc]
    private void RpcSetTurretRotation(float z)
    {
        Quaternion rotation = turretset.transform.rotation;
        Vector3 euler = rotation.eulerAngles;
        euler.z = z;
        rotation.eulerAngles = euler;
        turretset.transform.rotation = rotation;
    }

    private float angle_360(Vector3 from, Vector3 to)
    {
        Vector3 v3 = Vector3.Cross(from, to);
        if (v3.z > 0)
            return Vector3.Angle(from, to);
        else
            return 360f - Vector3.Angle(from, to);
    }

    public bool GetNewWeapon(int id)
    {
        if (m_have[id])
            return false;
        m_have[id] = true;
        return true;
    }

    public void TakeWeapon(int weaponID,int slotID)
    {
        if (m_currentOwn < MaxTaking)
        {
            m_slot[m_currentOwn] = weaponID;
            m_currentOwn++;
        }
        else
        {
            if (m_slot[slotID] != -1)
                OnUntaking(m_slot[slotID]);
            m_slot[slotID] = weaponID;
        }
    }

    // can be only call by local player
    public void SetCurrentSlot(int id)
    {
        if (!isLocalPlayer)
            return;
        CmdSetCurrentSlot(id);
    }

    [Command]
    void CmdSetCurrentSlot(int id)
    {
        RpcSetCurrentSlot(id);
    }

    [ClientRpc]
    void RpcSetCurrentSlot(int id)
    {
        if (id == m_currentID)
            return;
        if (id >= m_currentOwn)
            return;
        if (m_currentID != -1)
            m_weapon[m_slot[m_currentID]].SetUnfocus();
        m_currentID = id;
        m_weapon[m_slot[m_currentID]].SetFocus();
    }

    private void OnUntaking(int weaponID)
    {

    }

}