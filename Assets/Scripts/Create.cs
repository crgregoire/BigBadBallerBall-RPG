﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.Linq;

public class Create : MonoBehaviour
{
    //may need to call ScriptStart instead of Start due to the odd order of assignment
    //This is my way of getting around prefabs and vanishing values in editor
    //This is to be called by a random generator, or perhaps a create Gunman function
    //make the random functions (minus the spawner) all delegates, because they are all fairly the same object in new out, then you could randomize which of the functions are called. Etc etc manipulate however
    private static string[] shapes = { "sphere", "cube", "cylinder", "capsule" };
    #region basic
    public static GameObject NewObject(string shape = "sphere")
    {
        GameObject obj;
        switch (shape)
        {
            case "cube":
                obj = GameObject.CreatePrimitive(PrimitiveType.Cube);
                break;
            case "sphere":
                obj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                break;
            case "capsule":
                obj =  GameObject.CreatePrimitive(PrimitiveType.Capsule);
                break;
            case "cylinder":
                obj = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                break;
            case "empty":
                obj = new GameObject("empty");
                break;
            default:
                obj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                break;
           
        }
        //obj.SetActive(false);
        return obj;
    }
    public static Rigidbody AddRigidbody(GameObject obj, float mass = 1, float angDrag = 0.05f, float drag = 0, bool gravity = true, bool kinetic = false, bool noRot = false)
    {
        Rigidbody rb = obj.AddComponent<Rigidbody>();
        rb.mass = mass;
        rb.angularDrag = angDrag;
        rb.drag = drag;
        rb.useGravity = gravity;
        rb.isKinematic = kinetic;
        
        if (noRot)
        {
            rb.freezeRotation = true;
        }
        return rb;
    }

    public static void SetScale(GameObject obj, float x = 1, float y = 1, float z = 1, float mult = 1)
    {
        Transform tObj = obj.transform;

            tObj.localScale = new Vector3(x*mult,y*mult,z*mult);

    }
    public static void ModScale(GameObject obj, float x = 1, float y = 1, float z = 1, float mult = 1)
    {
        Transform tObj = obj.transform;
            tObj.localScale = new Vector3(tObj.localScale.x*x*mult, tObj.localScale.y*y*mult, tObj.localScale.z*z*mult);
        
    }

    public static GameObject GetPrefab(string prefab)
    {
        string location = "Prefabs/" + prefab;
        return Resources.Load(location, typeof(GameObject)) as GameObject;

    }

    public static GameObject CreateRandomObject()
    {
        return NewObject(shapes[Random.Range(0, 3)]);
    }
    public static void SetColor(GameObject obj, Color color, string mode = "_Color")
    {

        Renderer rend = obj.GetComponent<Renderer>();
        color.a = rend.material.color.a;
        rend.material.SetColor(mode, color);
    }
    public static void SetMaterial(GameObject obj, string material)
    {
        string location = "Materials/" + material;
        obj.GetComponent<Renderer>().material = Resources.Load(location, typeof(Material)) as Material;

    }
    public static void SetFade(GameObject obj, int alpha = 1, string mode = "_Color")
    {
        Color color = obj.GetComponent<Material>().color;
        obj.GetComponent<Renderer>().material.SetColor(mode, new Color(color.r, color.g, color.b, alpha));
    }
    #endregion
    #region target involved
    public static void AddReward(GameObject obj, int amount = 1, GameObject Drop = null, Vector3 offset = new Vector3(), float velocity = 0.001f)
    {
        if (Drop == null)
        {
            Drop = GetPrefab("Coin");
        }
        obj.AddComponent<DropScatter>();
        DropScatter script = obj.GetComponent<DropScatter>();
        script.amount = amount;
        script.drop = Drop;
        script.offset = offset;
        script.velocity = velocity;
    }
    public static Targeting AddTargeting(GameObject obj, string[] targets, string method = "nearest", float targetingSpeed = 3, int targetingRange = 100, float retargetingSpeed = 3)
    {
        Targeting ts = obj.AddComponent<Targeting>();
        ts.targetByTags = targets;
        ts.targeting = method;
        ts.targetingSpeed = targetingSpeed;
        ts.targetingRange = targetingRange;
        ts.retargetingSpeed = retargetingSpeed;
        //Actor is a void delegate.

        return ts;
    }
    //Needs major updating for new singular AI file.
    //Someway to allow these to be setters aswell?

    public static void AddSendable(GameObject obj, List<string> behaviourTargets = null, List<string> affectedTargets = null)
    {
        Sendable to = obj.AddComponent<Sendable>();
        to.targets = behaviourTargets;
        to.affected = affectedTargets;
    }
    public static void AddMovement(GameObject obj, float speed = 30, string movement = "accelerate")
    {
        Movement ms = obj.AddComponent<Movement>();
        ms.speed = speed;
        ms.SetMovement(movement);
        ms.currentMovement = movement;
    }
    public static AI AddAI(GameObject obj, float distance = 0, int retreatDuration = 1, float chaseRange = -1, bool relative = false, string ai = "charge", bool onDeath = true, float pointSpeed = 0.1f)
    {
        
        AI ais = obj.AddComponent<AI>();
        ais.pointSpeed = pointSpeed;
        ais.retreatDuration = retreatDuration;

        ais.distance = distance;
        switch (ai)
        {
            case "hold":
                ais.kites = false;
                ais.charges = false;
                ais.hold = true;
                break;
            case "charge":
                ais.kites = false;
                ais.charges = true;
                ais.hold = false;
                break;
            case "kite":
                ais.kites = true;
                ais.charges = false;
                ais.hold = false;
                break;
            default:
                ais.kites = false;
                ais.charges = false;
                ais.hold = false;
                break;
        }

        return ais;
    }
    public static void AddImpact(GameObject obj, int dmgMin = 0, int dmgMult = 1)
    {
        obj.AddComponent<ImpactDamage>();
        ImpactDamage script = obj.GetComponent<ImpactDamage>();

        script.minDamage = dmgMin;
        script.impactDamage = dmgMult;
    }
    #endregion
    #region assembling
    public static void AddHealth(GameObject toWhat,int  health = 50)
    {
        GameObject healthBox = GetPrefab("HealthBox");
        GameObject healthBar = Instantiate(healthBox);
        Follow followScript = healthBar.GetComponent<Follow>();
        followScript.target = toWhat;
        followScript.offset = new Vector3(0f, toWhat.transform.localScale.y, 0f);
        
        Health healthScript = toWhat.AddComponent<Health>();
        healthScript.maxHealth = health;
        healthScript.healthBox = healthBar.transform;
        SetScale(healthBar, 1,0.1f,0.1f);
        ModScale(healthBar, x: ((float)health) / 100);
        healthBar.transform.parent = toWhat.transform.parent.transform;

    }
    public static GameObject MakeUnit(GameObject center, string name = "generic", int health = 100, int speed = 20, int maxSpeed = -1, bool frozen = true)
    {

        GameObject parent = new GameObject(name);

        AddRigidbody(center,noRot:frozen);

        center.name = "body";
        center.AddComponent<CenterMass>();
        //LOL the redundancy
        center.GetComponent<CenterMass>().center = center.transform;
        if (maxSpeed >= 0)
        {
            center.AddComponent<MaxSpeed>();
            center.GetComponent<MaxSpeed>().maxSpeed = maxSpeed;
        }
        center.transform.parent = parent.transform;


        AddHealth(center, health);
        return parent;
    }
    //amount should probably be moved to vars so it could be updated easily and accessed easily. Make higher worth have worth bar etc etc bounty yata yata. Plus it would be settable at make unit.

    public static void AttachFixed(GameObject obj, GameObject to)
    {
        FixedJoint joint = obj.AddComponent<FixedJoint>();
        joint.connectedBody = to.GetComponent<Rigidbody>();

    }
    #endregion
    #region creators
    public static GameObject Gun(List<string> targets)
    {

        GameObject rail = NewObject("cube");
        SetMaterial(rail, "Gun");
        GameObject projectile = NewObject("sphere");
        SetMaterial(projectile, "Gun");
        AddRigidbody(projectile, 0.01f, gravity: false, noRot: true);
        AddSendable(projectile, targets, targets);
        AddProjectile(projectile, firer: rail, collisionsAllowed: 0, distance: 0.7f);
        AddTargeting(projectile, targets.ToArray(),targetingSpeed:0.1f,retargetingSpeed:10f,targetingRange:1000);
        AddAI(projectile,ai: "none",pointSpeed: 1,relative:true);
        AddMovement(projectile, speed: 30f, movement: "velocity");
        //projectile.AddComponent<Move>();
       
        AddRigidbody(rail, gravity: false);
       
        HitDamage hs = projectile.AddComponent<HitDamage>();
        hs.damage = 20;
        projectile.SetActive(false);
        ModScale(projectile, mult: 0.1f);
        Duration ds = projectile.AddComponent<Duration>();
        ds.duration = 15f;
        AddSpawner(rail, projectile, reloadTime: 10f);
        SetScale(rail, 0.1f, 0.1f);



        ToolController ts = rail.GetComponent<ToolController>();
        Actor action = ts.Use;
        OnInterval ois = rail.AddComponent<OnInterval>();
        ois.Set(action, ts.reloadTime);
        return rail;
    }
    public static GameObject Charger(Vector3 location, string faction = "Enemy", string[] OpposingFactions = null, float speed = 10,int reward = 1, string ai = "charge")
    {
            OpposingFactions = OpposingFactions ?? new string[] { "Player", "Character"};

        GameObject body = NewObject("sphere");
        body.tag = faction;
        SetMaterial(body, faction);
        AddTargeting(body, OpposingFactions);
        AddAI(body, ai:ai, pointSpeed: -1);
        AddMovement(body,movement:"accelerate", speed:speed);
        AddSendable(body, behaviourTargets: new List<string>(OpposingFactions));
        AddProjectile(body);
        AddImpact(body);
        if (faction != "Player")
        {
            AddReward(body, amount: reward);
        }

        GameObject unit= MakeUnit(body, (faction + "Charger"), health: 10,frozen: false);
        unit.transform.position = location;
        return unit;

    }
    public static GameObject Gunner(Vector3 location, string faction = "Enemy", string[] OpposingFactions = null, float speed = 20, float pointSpeed = 0.001f,string movement = "accelerate", int reward = 1, int targetingRange = 1000, string ai = "kite")
    {
        OpposingFactions = OpposingFactions ?? new string[] { "Player", "Character" };

        GameObject body = NewObject("sphere");
        body.tag = faction;
        SetMaterial(body, faction);
     
        Targeting ts = AddTargeting(body, OpposingFactions,targetingRange:targetingRange);
     AddAI(body, ai:ai, distance: 20,pointSpeed: pointSpeed);
        AddMovement(body, movement: movement, speed: speed);
        
        
        GameObject unit = MakeUnit(body, (faction + "Gunner"), health: 10);
        unit.transform.position = location;
        GameObject gun = Gun(new List<string>(OpposingFactions));
        gun.transform.parent = unit.transform;
        gun.transform.localPosition = new Vector3(0, 0, 1.5f);
        if (faction != "Player")
        {
            AddReward(body, amount: reward);
        }
        AttachSpawner(gun, body);
        return unit;
        //\rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;

       
        
    }
    //different method for spot lights as rotation is involved.
    public static GameObject ALight(Vector3 position, Color? color = null, int range = 10, float intensity = 1f, float indirect = 0, Vector3 angle = new Vector3(), LightType? type = null)
    {


        GameObject lightGameObject = new GameObject("The Light");
        GameObject sphere = NewObject("sphere");
        SetMaterial(sphere, "Emissive");
        SetColor(sphere, color ?? Color.white, "_EmissionColor");


        sphere.transform.parent = lightGameObject.transform;
        Light lightComp = lightGameObject.AddComponent<Light>();
        lightComp.color = color ?? Color.white;
        lightGameObject.transform.position = position;
        lightComp.range = range;
        lightComp.intensity = intensity;
        lightComp.bounceIntensity = indirect;
        lightGameObject.transform.eulerAngles = angle;

        lightComp.type = type ?? LightType.Point;
        return lightGameObject;
    }
    #endregion

    #region spawner


    //Make a master projectile creator for randomization. Takes list of behaviours to add, and randomizes their inputs. SetProjectileAspects
    public static void AddProjectile(GameObject obj, GameObject firer = null, GameObject spawner = null, int collisionsAllowed = -1, float damageMult = 1, float distance = 1)
    {
        Projectile ps = obj.AddComponent<Projectile>();
        ps.firer = firer;
        ps.spawner = spawner;
        ps.collisionsAllowed = collisionsAllowed;
        ps.damageMult = damageMult;
        ps.distance = distance;

    }
    //may want isPlayers to be isSeperate. That way its easy to add shields and such to things. that need seperate collisions. Add rotate about and have velocity of (var) have rot of (var)
    public static void AttachSpawner(GameObject spawner, GameObject body, GameObject kicked = null, GameObject kickedRot = null/*, bool isPlayers = false*/)
    {
        kicked = kicked ?? body;
        kickedRot = kickedRot ?? body;
        ToolController toolScript = spawner.GetComponent<ToolController>();
        toolScript.kickWhat = kicked.GetComponent<Rigidbody>();
        toolScript.kickedRot = kickedRot.GetComponent<Rigidbody>();
        AttachFixed(spawner, body);
        
        /*
        if (isPlayers)
        {
            RotateAbout
        }*/
    }

    
    public static void AddSpawner( GameObject spawner, GameObject projectile, GameObject kickBy = null, int kick = 0, int kickVert = 0, int kickRot = 0, int kickHor = 0, float reloadTime = 10, bool randKick = false, bool hasKick = true /*,Buff[]? buffs = null*/)
    {
            kickBy = kickBy ?? spawner;

        ToolController toolScript = spawner.AddComponent<ToolController>();
        toolScript.projectile = projectile;
        toolScript.kick = kick;
        toolScript.kickBy = kickBy.transform;
        toolScript.kickVert = kickVert;
        toolScript.kickHor = kickHor;
        toolScript.rotKick = kickRot;
        toolScript.randKick = randKick;
        toolScript.hasKick = hasKick;
        toolScript.reloadTime = reloadTime;
    }
    #endregion
    #region audio
    public void Sound(string name, Vector3? location = null, float duration = -1, int times = 1, GameObject altLocation = null, float volume = 1f)
    {
        AudioClip clip = Resources.Load<AudioClip>("Sounds/"+name);
        if (duration < 0)
        {
            duration = clip.length;
        }
    
       
        Vector3 pos;
        if (altLocation != null)
        {
            pos = altLocation.transform.position;
        }
        else
        {
            pos = location ?? GameObject.Find("Main Camera").transform.position;
        }
        StartCoroutine(SoundRepeat(clip, pos, duration, times, volume));
    }



    public IEnumerator SoundRepeat(AudioClip clip, Vector3 location, float duration, int number, float volume)
    {
        int i = 0;
        while (i < number)
        {
            AudioSource.PlayClipAtPoint(clip, location, volume);
            yield return new WaitForSeconds(duration); //wait 1 second per interval
            i++;
        }
    }
    #endregion






}