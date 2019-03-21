﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//enum of different weapon types and power-ups
public enum WeaponType
{
    none, //no weapon
    blaster, //simple blaster
    spread, //two simultaneous shots
    shield //adds shields
}

//allows setting the weapons' properties in the Inspector

    [System.Serializable]
    public class WeaponDefinition
{
    public WeaponType type = WeaponType.none;
    public string letter; //letter to show up on power-up
    public Color color = Color.white; //coler of collar and power-up
    public GameObject projectilePrefab; //prefab for projectiles
    public Color projectileColor = Color.white; //color for prefab
    public float damageOnHit = 0; //amount of damage caused
    public float delayBetweenShots = 0; //delay
    public float velocity = 20; //speed of projectiles
}

public class Weapon : MonoBehaviour
{
    static public Transform PROJECTILE_ANCHOR;
    [Header("Set Dynamically")]
    [SerializeField]
    private WeaponType _weaponType = WeaponType.none;
    public WeaponDefinition def;
    public GameObject collar;
    public float lastShotTime; //time last shot was fired
    private Renderer _collarRend;

    void Start()
    {
        collar = transform.Find("Collar").gameObject;
        _collarRend = collar.GetComponent<Renderer>();
        SetWeaponType(_weaponType); //call for deafault _weaponType WeaponType.none
        if(PROJECTILE_ANCHOR == null){
            GameObject gameobject = new GameObject("_ProjectileAnchor");
            PROJECTILE_ANCHOR = gameobject.transform;
        }
        GameObject rootGameObject = transform.root.gameObject;
        if (rootGameObject.GetComponent<Hero>() != null)
        {
            rootGameObject.GetComponent<Hero>().fireDelegate += Fire;
        }
    }

    public WeaponType weaponType
    {
        get
        {
            return (_weaponType);
        }
        set
        {
            SetWeaponType(value);
        }
    }

    public void SetWeaponType(WeaponType wType)
    {
        _weaponType = wType;
        if(weaponType == WeaponType.none)
        {
            this.gameObject.SetActive(false);
            return;
        }
        else
        {
            this.gameObject.SetActive(true);
        }
        def = Main.GetWeaponDefinition(_weaponType);
        _collarRend.material.color = def.color;
        lastShotTime = 0;
    }

    public void Fire()
    {
        if (!gameObject.activeInHierarchy) return; //if this.gameObject is inactive, return
        if(Time.time - lastShotTime < def.delayBetweenShots) //if it hasn't been enough time between shots, return
        {
            return;
        }
        Projectile projectile;
        Vector3 velocity = Vector3.up * def.velocity;
        if (transform.up.y < 0)
        {
            velocity.y = -velocity.y;
        }

        switch (weaponType)
        {
            case WeaponType.blaster:
                projectile = MakeProjectile(); //make middle projectile
                projectile.rigidBody.velocity = velocity;
                projectile = MakeProjectile(); //make right projectile
                projectile.transform.rotation = Quaternion.AngleAxis(30, Vector3.back);
                projectile.rigidBody.velocity = projectile.transform.rotation * velocity;
                projectile = MakeProjectile(); //make left projectile
                projectile.transform.rotation = Quaternion.AngleAxis(-30, Vector3.back);
                projectile.rigidBody.velocity = projectile.transform.rotation * velocity;
                break;
        }
    }

    public Projectile MakeProjectile()
    {
        GameObject gameObject = Instantiate<GameObject>(def.projectilePrefab);
        if (transform.parent.gameObject.tag == "Hero")
        {
            gameObject.tag = "ProjectileHero";
            gameObject.layer = LayerMask.NameToLayer("ProjectileHero");
        }
        else
        {
            gameObject.tag = "ProjectileEnemy";
            gameObject.layer = LayerMask.NameToLayer("ProjectileHero");
        }
        gameObject.transform.position = collar.transform.position;
        gameObject.transform.SetParent(PROJECTILE_ANCHOR, true);
        Projectile projectile = gameObject.GetComponent<Projectile>();
        projectile.weaponType = weaponType;
        lastShotTime = Time.time;
        return (projectile);
    }




    // Update is called once per frame
    void Update()
    {
        
    }
}
