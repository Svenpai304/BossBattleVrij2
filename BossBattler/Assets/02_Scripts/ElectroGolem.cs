using System.Collections;
using UnityEngine;
using EGStates;
using System;
using System.Collections.Generic;

public class ElectroGolem : MonoBehaviour, IStatus, IDamageable
{
    [Header("References")]
    public Rigidbody2D rb;
    [HideInInspector] public StateMachine stateMachine = new();
    [HideInInspector] public CharacterStatus currentTarget;
    [HideInInspector] public Animator animator;
    public SpriteRenderer spriteRenderer;
    public Vector2 relocateDestination;
    private BossHealthBar healthBar;

    [Header("Ground Check")]
    [SerializeField] private float groundCheckLength;
    [SerializeField] private float groundCheckOffset;
    public LayerMask groundMask;
    public LayerMask defaultExclude;
    public LayerMask descendingExclude;
    public bool isGrounded;

    [Header("General stats")]
    public float MaxHealth;
    public float arenaMinX = -20, arenaMinY = 0, arenaMaxX = 20, arenaMaxY = 20;

    [Header("Idle")]
    public GameObject ElectroSuitObject;
    public ParticleSystem ElectroSuitBurst;
    public float ElectroSuitDamage;
    public float ElectroSuitInterval;
    public float ElectroSuitDuration;
    private float timer;


    [Header("Charge")]
    public float ChargeYThreshold;
    public float ChargeXThreshold;
    public GameObject ChargeCollider;
    public float ChargeDamage;

    [Header("Jump")]
    public bool isDescending;
    public ParticleSystem hoverParticles;

    [Header("Copper Assault")]
    public float CaDistance;
    public Transform CaOffset;
    public GameObject CopperAssaultProjectile;
    public GameObject CopperAssaultExplosion;

    [Header("Electron Orb")]
    public Transform EoOffset;
    public GameObject ElectronOrbProjectile;
    public float ElectronOrbDamage;


    public float DamageDealMult { get; set; }
    public float DamageTakenMult { get; set; }
    public float GroundSpeedMult { get; set; }
    public float PowerRegenMult { get; set; }
    public bool Invulnerable { get; set; }
    public float Health { get; set; }

    private void Start()
    {
        DamageDealMult = 1;
        DamageTakenMult = 1;
        GroundSpeedMult = 1;
        PowerRegenMult = 1;
        Invulnerable = false;
        Health = MaxHealth;
        healthBar = BossHealthBarManager.Instance.CreateHealthBar();
        healthBar.Setup(MaxHealth, "Electro Golem");
        animator = GetComponent<Animator>();
        SetupStateMachine();
    }

    private void FixedUpdate()
    {
        isGrounded = CheckGrounded();
        stateMachine.OnFixedUpdate();

        timer += Time.deltaTime;
        if(timer > ElectroSuitInterval)
        {
            timer = 0;
            StartCoroutine(ActivateElectroSuit());
        }

        if(isDescending && transform.position.y <= relocateDestination.y + 4)
        {
            rb.excludeLayers = defaultExclude;
            isDescending = false;
        }
    }

    private IEnumerator ActivateElectroSuit()
    {
        if (ElectroSuitObject == null) { yield break; }
        if (ElectroSuitBurst != null) { ElectroSuitBurst.Play(); }
        ElectroSuitObject.SetActive(true);

        yield return new WaitForSeconds(ElectroSuitDuration);
        ElectroSuitObject.SetActive(false);
    }

    public void OnESHit(Collider2D other)
    {
        CharacterStatus cs = other.GetComponent<CharacterStatus>();
        if (cs != null)
        {
            cs.TakeDamage(ElectroSuitDamage);
        }
    }

    public void OnChargeHit(Collider2D other)
    {
        CharacterStatus cs = other.GetComponent<CharacterStatus>();
        if (cs != null)
        {
            cs.TakeDamage(ChargeDamage);
        }
    }

    private void SetupStateMachine()
    {
        stateMachine.AddState(new Idle(this));

        stateMachine.AddState(new StartCharge(this));
        stateMachine.AddState(new Charge(this));

        stateMachine.AddState(new Jump(this));
        stateMachine.AddState(new Hover(this));

        stateMachine.AddState(new CopperAssault(this));
        stateMachine.AddState(new ElectronOrb(this));

        stateMachine.SwitchState(new Idle(this));
    }

    public CharacterStatus CheckChargeAllowed()
    {
        foreach (CharacterStatus player in PlayerConnector.instance.players)
        {
            if (Mathf.Abs(player.transform.position.y - transform.position.y) < ChargeYThreshold && Mathf.Abs(player.transform.position.x - transform.position.x) < ChargeXThreshold)
            {
                return player;
            }
        }
        Debug.Log("No charge target found");
        return null;
    }

    public CharacterStatus ClosestPlayer()
    {
        float closest = Mathf.Infinity;
        CharacterStatus cs = null;
        foreach (CharacterStatus player in PlayerConnector.instance.players)
        {
            float dist = player.Dist(transform);
            if (dist < closest)
            {
                closest = dist;
                cs = player;
            }
        }
        return cs;
    }

    public CharacterStatus FarthestPlayer()
    {
        float farthest = 0;
        CharacterStatus cs = null;
        foreach (CharacterStatus player in PlayerConnector.instance.players)
        {
            float dist = player.Dist(transform);
            if (dist > farthest)
            {
                farthest = dist;
                cs = player;
            }
        }
        return cs;
    }

    public void TakeDamage(float damage)
    {
        if (Invulnerable) { return; }
        damage *= DamageTakenMult;
        Health = Mathf.Clamp(Health - damage, 0, MaxHealth);
        healthBar.SetHealth(Health);
        if (Health == 0) { Die(); }
    }

    public void Die()
    {
        Destroy(gameObject);
    }

    private bool CheckGrounded()
    {
        Debug.DrawRay(transform.position, Vector2.down * groundCheckLength);
        if (Physics2D.Raycast(transform.position, Vector2.down, groundCheckLength, groundMask)) { return true; }
        if (Physics2D.Raycast(transform.position + new Vector3(groundCheckOffset, 0, 0), Vector2.down, groundCheckLength, groundMask)) { return true; }
        if (Physics2D.Raycast(transform.position + new Vector3(-groundCheckOffset, 0, 0), Vector2.down, groundCheckLength, groundMask)) { return true; }
        return false;
    }

    public void HealDamage(float damage)
    {
        Health = Mathf.Clamp(Health + damage, 0, MaxHealth);
        healthBar.SetHealth(Health);
    }
}

namespace EGStates
{
    public class Idle : State<ElectroGolem>
    {
        // Decides next action to perform

        private float minIdleTime = 0.2f;
        private float maxIdleTime = 2f;
        private bool coroutineActive;

        List<Type> validMoves = new();

        public Idle(ElectroGolem owner) : base(owner) { }

        public override void OnEnter()
        {
            Owner.animator.SetInteger("state", 0);
        }

        public override void OnUpdate()
        {
            Debug.Log("State selection coroutine running: " + coroutineActive);
            if(coroutineActive) { return; }
            Owner.StartCoroutine(ProcessPhase1());
        }

        public override void OnExit()
        {
        }

        private IEnumerator ProcessPhase1()
        {
            if (!Owner.isGrounded || Owner.isDescending || PlayerConnector.instance.players.Count == 0) { yield break; }
            coroutineActive = true;
            yield return new WaitForSeconds(UnityEngine.Random.Range(minIdleTime, maxIdleTime));
            Owner.rb.velocity = Vector2.zero;

            validMoves.Clear();
            Owner.currentTarget = Owner.ClosestPlayer();

            CharacterStatus chargeTarget = Owner.CheckChargeAllowed();
            if (chargeTarget != null)
            {
                validMoves.Add(typeof(StartCharge));
            }
            if (Owner.currentTarget.Dist(Owner.transform) < Owner.CaDistance)
            {
                validMoves.Add(typeof(CopperAssault));
            }
            else
            {
                validMoves.Add(typeof(Jump));
                validMoves.Add(typeof(ElectronOrb));
            }

            Type newState = validMoves[UnityEngine.Random.Range(0, validMoves.Count)];

            if (newState == typeof(StartCharge))
            {
                Owner.currentTarget = chargeTarget;
            }
            else if (newState == typeof(Jump))
            {
                Vector2 farthest = Owner.FarthestPlayer().transform.position;
                farthest.x = Mathf.Clamp(farthest.x, Owner.arenaMinX, Owner.arenaMaxX);
                farthest.y = Mathf.Clamp(farthest.y + 1, Owner.arenaMinY, Owner.arenaMaxY);
                Owner.relocateDestination = farthest;
            }

            Debug.Log("Chosen state: " + newState);
            Owner.stateMachine.SwitchState(newState);
            bool turnDir = Owner.currentTarget.transform.position.x > Owner.transform.position.x;
            if (newState == typeof(StartCharge))
            {
                turnDir = !turnDir;
            }
            Owner.spriteRenderer.flipX = turnDir;
            coroutineActive = false;
        }
    }

    public class StartCharge : State<ElectroGolem>
    {
        // Step back a little

        private int Xdirection;
        private float maxTime = 0.5f;
        private float time = 0f;
        public StartCharge(ElectroGolem owner) : base(owner) { }

        public override void OnEnter()
        {
            Owner.animator.SetInteger("state", 1);
            Owner.animator.speed = -0.5f;
            time = 0;
            Xdirection = (int)Mathf.Sign((Owner.transform.position - Owner.currentTarget.transform.position).x);
            Owner.rb.velocity = Vector2.zero;
            Owner.rb.AddForce(new Vector2(Xdirection * 4000, 0));
        }

        public override void OnUpdate()
        {
            Debug.Log(time);
            time += Time.fixedDeltaTime;
            if (time > maxTime)
            {
                Owner.stateMachine.SwitchState(typeof(Charge));
            }
        }

        public override void OnExit()
        {
            Owner.rb.velocity = new Vector2(0, Owner.rb.velocity.y);
        }
    }

    public class Charge : State<ElectroGolem>
    {
        // Charge in direction very quickly until hitting something or reaching a max distance

        private float startX;
        private int Xdirection;
        private float acceleration = 30000f;
        private float maxSpeed = 40;
        private float maxDistance = 25;
        private float maxTime = 3;
        private float time;
        public Charge(ElectroGolem owner) : base(owner) { }

        public override void OnEnter()
        {
            Owner.animator.speed = 1;
            time = 0;
            Xdirection = -(int)Mathf.Sign((Owner.transform.position - Owner.currentTarget.transform.position).x);
            startX = Owner.transform.position.x;
            Owner.ChargeCollider.SetActive(true);
        }

        public override void OnUpdate()
        {
            time += Time.fixedDeltaTime;
            if (Owner.rb.velocity.x < maxSpeed)
            {
                Debug.Log(new Vector2(acceleration * Xdirection, 0));
                Owner.rb.AddForce(new Vector2(acceleration * Xdirection * Time.fixedDeltaTime, 0));
            }
            if (time >= maxTime || Mathf.Abs(Owner.transform.position.x - startX) > maxDistance)
            {
                Owner.stateMachine.SwitchState(typeof(Idle));
            }
        }

        public override void OnExit()
        {
            Owner.rb.velocity = new Vector2(0, Owner.rb.velocity.y);
            Owner.ChargeCollider.SetActive(false);
        }
    }

    public class Jump : State<ElectroGolem>
    {

        // Jump towards destination

        private float Yspeed = 20;
        private float Xaccel = 3;
        private float targetY = 30;
        private float targetYOffset = 9;
        private float startDelay = 0.5f;
        private float maxTime = 6;
        private float time;
        private float Xdirection;

        public Jump(ElectroGolem owner) : base(owner) { }

        public override void OnEnter()
        {
            if(Owner.rb.isKinematic) { Owner.rb.isKinematic = false; }
            Owner.hoverParticles.Play(true);
            Owner.animator.SetInteger("state", 2);
            time = 0;
            targetY = Mathf.Min(Owner.relocateDestination.y + targetYOffset, Owner.arenaMaxY);
            Xdirection = Mathf.Sign(Owner.relocateDestination.x - Owner.transform.position.x);
            Owner.StartCoroutine(Start());
        }

        public override void OnUpdate()
        {
            time += Time.deltaTime;
            if (time > startDelay)
            {
                Owner.rb.velocity += new Vector2(Xaccel * Xdirection * Time.deltaTime, 0);
            }

            if (time > maxTime || Owner.transform.position.y >= targetY)
            {
                Owner.stateMachine.SwitchState(typeof(Hover));
            }
        }

        public override void OnExit()
        {

        }

        private IEnumerator Start()
        {
            yield return new WaitForSeconds(startDelay);
            Owner.rb.isKinematic = true;
            Owner.rb.velocity = new Vector2(0, Yspeed);

        }

    }

    public class Hover : State<ElectroGolem>
    {

        // Hover towards destination
        
        private float speed = 17;
        private float maxTime = 6;
        private float time;
        private float Xdirection;

        public Hover(ElectroGolem owner) : base(owner) { }

        public override void OnEnter()
        {
            time = 0;
            Owner.rb.isKinematic = true;
            Owner.rb.gravityScale = 0;
            Xdirection = Mathf.Sign(Owner.relocateDestination.x - Owner.transform.position.x);
            Owner.rb.velocity = new Vector2(speed * Xdirection, 0);

        }

        public override void OnUpdate()
        {
            time += Time.deltaTime;
            if (time > maxTime || Mathf.Abs(Owner.relocateDestination.x - Owner.transform.position.x) < 1)
            {
                Owner.rb.gravityScale = 3;
                Owner.rb.velocity = new Vector2(Owner.rb.velocity.x / 5, 0);
                Owner.rb.isKinematic = false;
                Owner.rb.excludeLayers = Owner.descendingExclude;
                Owner.isDescending = true;
                Owner.hoverParticles.Stop(true, ParticleSystemStopBehavior.StopEmitting);
                Owner.stateMachine.SwitchState(typeof(Idle));
            }
        }

        public override void OnExit()
        {

        }

    }

    public class CopperAssault : State<ElectroGolem>, IProjectileOwner
    {

        // Shoot copper projectiles that explode

        private float startDelay = 0.6f;
        private float fireDelay = 0.03f;
        private float endDelay = 1.5f;

        private int count = 12;
        private float maxSpread = 20;
        private float projectileForce = 20;

        public CopperAssault(ElectroGolem owner) : base(owner) { }

        public override void OnEnter()
        {
            Owner.animator.SetInteger("state", 3);
            Owner.StartCoroutine(Process());
        }

        public override void OnUpdate()
        {

        }

        public override void OnExit()
        {
        }

        private IEnumerator Process()
        {
            yield return new WaitForSeconds(Time.deltaTime * 3);
            Owner.animator.speed = 0;
            yield return new WaitForSeconds(startDelay);

            Owner.animator.speed = 1;
            for (int i = 0; i < count; i++)
            {
                PhysicsProjectile p = UnityEngine.Object.Instantiate(Owner.CopperAssaultProjectile, Owner.CaOffset.position, Quaternion.identity).GetComponent<PhysicsProjectile>();
                p.Setup(this);

                float angle = Vector2.SignedAngle(Vector2.right, Owner.currentTarget.transform.position - Owner.CaOffset.position);
                angle += UnityEngine.Random.Range(-maxSpread, maxSpread);
                angle *= Mathf.Deg2Rad;
                Vector2 dir = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)).normalized;

                p.rb.AddForce(projectileForce * dir, ForceMode2D.Impulse);
                p.rb.AddTorque(20);
                yield return new WaitForSeconds(fireDelay);
            }
            Owner.animator.speed = 0;

            yield return new WaitForSeconds(endDelay);
            Owner.animator.speed = 1;
            Owner.stateMachine.SwitchState(typeof(Idle));
        }

        public bool OnProjectileHit(Collider2D other, GameObject p)
        {
            if ((Owner.groundMask & (1 << other.gameObject.layer)) != 0)
            {
                Rigidbody2D rb = p.GetComponent<Rigidbody2D>();
                rb.velocity = Vector2.zero;
                rb.freezeRotation = true;
                rb.isKinematic = true;
                return false;
            }
            if (other.GetComponent<CharacterStatus>() != null)
            {
                // Explode
                UnityEngine.Object.Instantiate(Owner.CopperAssaultExplosion, p.transform.position, p.transform.rotation);
                return true;
            }
            return false;
        }
    }

    public class ElectronOrb : State<ElectroGolem>, IProjectileOwner
    {

        // Shoot electric orb that floats in a sine pattern

        private float startDelay = 0.6f;
        private float endDelay = 1.5f;

        public ElectronOrb(ElectroGolem owner) : base(owner) { }

        public override void OnEnter()
        {
            Owner.animator.SetInteger("state", 4);
            Owner.StartCoroutine(Process());
        }

        public override void OnUpdate()
        {

        }

        public override void OnExit()
        {
        }

        private IEnumerator Process()
        {
            yield return new WaitForSeconds(Time.deltaTime * 3);
            Owner.animator.speed = 0;
            yield return new WaitForSeconds(startDelay);
            Owner.animator.speed = 1;

            SineProjectile p = UnityEngine.Object.Instantiate(Owner.ElectronOrbProjectile, Owner.EoOffset.position, Quaternion.identity).GetComponent<SineProjectile>();
            p.Setup(1, (int)Mathf.Sign(Owner.currentTarget.transform.position.x - Owner.transform.position.x), this);

            yield return new WaitForSeconds(endDelay);
            Owner.stateMachine.SwitchState(typeof(Idle));
        }

        public bool OnProjectileHit(Collider2D other, GameObject p)
        {
            CharacterStatus status = other.GetComponent<CharacterStatus>();
            if (status != null)
            {
                status.TakeDamage(Owner.ElectronOrbDamage);
                return true;
            }
            return false;
        }
    }
}

