using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class Projectile : MonoBehaviour
{

    #region Variable Declarations
    // Serialized Variables
    [SerializeField] protected GameSettings gameSettings = null;
    [SerializeField] protected Points points = null;

    // Private Variables
    protected float lifeTime = 1f;
    protected int damage = 10;
    protected PlayerConfig playerConfig;
    protected PlayerColor playerColor;
    protected new Rigidbody rigidbody = null;
    #endregion



    #region Unity Event Functions
    virtual protected void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
    }

    virtual protected void Start()
    {
        StartCoroutine(DestroyObject());
    }

    virtual protected void OnTriggerEnter(Collider other)
    {
        if (other.tag.Contains(Constants.TAG_WALL))
        {
            Destroy(gameObject);
        }
    }
    #endregion



    #region Public Functions
    public virtual void Initialize(int damage, PlayerConfig playerConfig, Vector3 velocity, float lifeTime = 1f)
    {
        this.playerConfig = playerConfig;
        this.damage = damage;
        this.lifeTime = lifeTime;
        rigidbody.velocity = velocity;
    }

    public virtual void Initialize(int damage, PlayerColor playerColor, Vector3 velocity, float lifeTime = 1f)
    {
        this.playerColor = playerColor;
        this.damage = damage;
        this.lifeTime = lifeTime;
        rigidbody.velocity = velocity;
    }
    #endregion



    IEnumerator DestroyObject()
    {
        yield return new WaitForSeconds(lifeTime);
        Destroy(gameObject);
    }
}
