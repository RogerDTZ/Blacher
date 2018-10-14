using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Weapon : NetworkBehaviour {

    public string weaponName;

    private SpriteRenderer turretRenderer;
    private Animator turretRenderAnimator;

    private bool m_focus;

    private float m_CDTime, t_CDTimer;
    private bool m_CDDone;
    private float m_ReloadTime, t_ReloadTimer;
    private bool m_ReloadDone;
    private bool m_ReloadWhileUnfocus;

    private float m_shellSpeed;
    private float m_shellDamage;

    private int m_magazineCapacity;
    private int m_magazineCurrent;

    private Sprite m_shellSkin;
    private Sprite m_turretSkin;

    public bool my_isLocalPlayer;

    private int d_firecnt;

	// Use this for initialization
	void Start () {
        transform.localPosition = new Vector3(0, 0, 0);
        transform.localEulerAngles = new Vector3(0, 0, 0);
    }

    void Awake()
    {
        turretRenderer = gameObject.GetComponent<SpriteRenderer>();
        turretRenderAnimator = gameObject.GetComponent<Animator>();
    }
	
	// Update is called once per frame
	public void Update () {
        gameObject.transform.localPosition = new Vector3(0, 0, 0);
        if (my_isLocalPlayer)
        {
            if (m_ReloadDone)
                CoolDown();
            else if(m_focus || m_ReloadWhileUnfocus)
                Reload();
        }
    }

    public void Setup(bool islocalplayer, string name, Sprite shellSkin, Sprite turretSkin, float CDTime, float ReloadTime, bool ReloadWhileUnfocus, float speed, float damage, int magazinCapacity)
    {
        my_isLocalPlayer = islocalplayer;

        weaponName = name;

        SetSkin(shellSkin,turretSkin);

        m_CDTime = CDTime;
        t_CDTimer = 0f;
        m_CDDone = true;

        m_ReloadTime = ReloadTime;
        t_ReloadTimer = 0f;
        m_ReloadDone = true;
        m_ReloadWhileUnfocus = ReloadWhileUnfocus;


        m_shellSpeed = speed;
        m_shellDamage = damage;

        m_magazineCapacity = magazinCapacity;
        m_magazineCurrent = m_magazineCapacity;

        m_focus = false;
    }

    public void Fire()
    {
        if (m_ReloadDone == false || m_CDDone == false)
            return;
        d_firecnt++;
        m_magazineCurrent--;
        Debug.Log("Using "+weaponName + " Fire " + d_firecnt + " Times ("+ m_magazineCurrent + "/" + m_magazineCapacity + ")");
        OnStartCD();
        if (m_magazineCurrent == 0)
            OnStartReload();
    }

    public void SetUnfocus()
    {
        m_focus = false;
        turretRenderAnimator.SetTrigger("Disappear");
    }

    public void SetFocus()
    {
        m_focus = true;
        turretRenderAnimator.SetTrigger("Appear");
    }

    public void SetSkin(Sprite shellSkin, Sprite turretSkin)
    {
        m_shellSkin = shellSkin;
        m_turretSkin = turretSkin;
        turretRenderer.sprite = turretSkin;
    }

    public void SetCDTime(float CDTime, bool resetCDTimer=true)
    {
        m_CDTime = CDTime;
        if (resetCDTimer)
            OnCDDone();
    }

    public void SetReloadTime(float ReloadTime, bool resetReloadTimer=true)
    {
        m_ReloadTime = ReloadTime;
        if (resetReloadTimer)
            OnReloadDone();
    }

    public void SetReloadWhileUnfocus(bool flag)
    {
        m_ReloadWhileUnfocus = flag;
    }

    public void SetShellSpeed(float shellSpeed)
    {
        m_shellSpeed = shellSpeed; 
    }

    public void SetShellDamage(float shellDamage)
    {
        m_shellDamage = shellDamage;
    }

    public void SetMagazineCapacity(int magazineCapacity, bool reloadImmediately=true)
    {
        m_magazineCapacity = magazineCapacity;
        if (reloadImmediately)
            OnReloadDone();
    }

    private void CoolDown()
    {
        if (m_CDDone)
            return;
        t_CDTimer += Time.deltaTime;
        if (t_CDTimer >= m_CDTime)
            OnCDDone();
    }
    
    private void Reload()
    {
        t_ReloadTimer += Time.deltaTime;
        Debug.Log(weaponName + "reloading..." + (t_ReloadTimer / m_ReloadTime) * 100 + "%");
        if(t_ReloadTimer >= m_ReloadTime)
            OnReloadDone();
    }

    private void OnStartCD()
    {
        m_CDDone = false;
    }

    private void OnCDDone()
    {
        m_CDDone = true;
        t_CDTimer = 0f;
    }

    private void OnStartReload()
    {
        m_ReloadDone = false;
    }

    private void OnReloadDone()
    {
        m_ReloadDone = true;
        t_ReloadTimer = 0f;

        m_magazineCurrent = m_magazineCapacity;

        Debug.Log("Using " + weaponName + " Fire " + d_firecnt + " Times (" + m_magazineCurrent + "/" + m_magazineCapacity + ")");

        OnCDDone();
    }

}
