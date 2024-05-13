using System.Collections;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;
using UnityEngine.UI;

public class ElectroGolem : MonoBehaviour, IStatus, IDamageable
{
    public Rigidbody2D rb;
    [HideInInspector] public CharacterStatus currentTarget;
    [HideInInspector] public Vector2 relocateDestination;

    public GameObject copperAssaultProjectile;
    public GameObject copperAssaultExplosion;

    [SerializeField] private float groundCheckLength;
    [SerializeField] private float groundCheckOffset;
    [SerializeField] public LayerMask groundMask;
    public bool isGrounded;


    public float arenaMinX = -20, arenaMinY = 0, arenaMaxX = 20, arenaMaxY = 20;

    public float MaxHealth;
    public float ChargeYThreshold;

    [HideInInspector] public StateMachine stateMachine = new();

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
        SetupStateMachine();
    }

    private void FixedUpdate()
    {
        isGrounded = CheckGrounded();
        stateMachine.OnFixedUpdate();
    }

    private void SetupStateMachine()
    {
        stateMachine.AddState(new EGStates.Idle(this));

        stateMachine.AddState(new EGStates.StartCharge(this));
        stateMachine.AddState(new EGStates.Charge(this));

        stateMachine.AddState(new EGStates.Jump(this));
        stateMachine.AddState(new EGStates.Hover(this));

        stateMachine.AddState(new EGStates.CopperAssault(this));

        stateMachine.SwitchState(new EGStates.Idle(this));
    }

    public bool CheckChargeAllowed()
    {
        foreach (CharacterStatus player in PlayerConnector.instance.players)
        {
            if (Mathf.Abs(player.transform.position.y - transform.position.y) < ChargeYThreshold)
            {
                currentTarget = player;
                Debug.Log("Charge target found");
                return true;
            }
        }
        Debug.Log("No charge target found");
        return false;
    }

    public void TakeDamage(float damage)
    {
        if (Invulnerable) { return; }
        damage *= DamageTakenMult;
        Health = Mathf.Clamp(Health - damage, 0, MaxHealth);
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
    }
}

namespace EGStates
{
    public class Idle : State<ElectroGolem>
    {

        // Decides next action to perform

        public Idle(ElectroGolem owner) : base(owner) { }

        public override void OnEnter()
        {

        }

        public override void OnUpdate()
        {
            if (!Owner.isGrounded) { return; }
            Owner.rb.velocity = Vector2.zero;

            if (Owner.CheckChargeAllowed())
            {
                Owner.stateMachine.SwitchState(typeof(CopperAssault));
            }
            else
            {
                Owner.relocateDestination = new Vector2(Random.Range(Owner.arenaMinX, Owner.arenaMaxX), Random.Range(Owner.arenaMinY, Owner.arenaMaxY));
                Owner.stateMachine.SwitchState(typeof(Jump));
            }
        }

        public override void OnExit()
        {

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
            time = 0;
            Xdirection = -(int)Mathf.Sign((Owner.transform.position - Owner.currentTarget.transform.position).x);
            startX = Owner.transform.position.x;
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
        }
    }

    public class Jump : State<ElectroGolem>
    {

        // Jump towards destination

        private float Yspeed = 20;
        private float Xaccel = 3;
        private float targetY = 15;
        private float startDelay = 0.5f;
        private float maxTime = 6;
        private float time;
        private float Xdirection;

        public Jump(ElectroGolem owner) : base(owner) { }

        public override void OnEnter()
        {
            time = 0;
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

        private float speed = 10;
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
                Owner.rb.gravityScale = 1;
                Owner.rb.velocity /= 5;
                Owner.rb.isKinematic = false;
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
        private float fireDelay = 0.1f;
        private float endDelay = 1.5f;

        private int count = 10;
        private float maxSpread = 25;
        private float projectileForce = 20;

        public CopperAssault(ElectroGolem owner) : base(owner) { }

        public override void OnEnter()
        {
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
            yield return new WaitForSeconds(startDelay);

            for(int i = 0; i < count; i++)
            {
                PhysicsProjectile p = Object.Instantiate(Owner.copperAssaultProjectile, Owner.transform.position, Quaternion.identity).GetComponent<PhysicsProjectile>();
                p.Setup(this);
                float angle = -Vector2.SignedAngle(Vector2.right, Owner.currentTarget.transform.position - Owner.transform.position);
                angle += Random.Range(-maxSpread, maxSpread);
                Debug.Log(angle);
                angle *= Mathf.Deg2Rad;
                Vector2 dir = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)).normalized;
                p.rb.AddForce(projectileForce * dir, ForceMode2D.Impulse);
                p.rb.AddTorque(20);
                yield return new WaitForSeconds(fireDelay);
            }

            yield return new WaitForSeconds(endDelay);
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
                Object.Instantiate(Owner.copperAssaultExplosion, p.transform.position, p.transform.rotation);
                return true;
            }
            return false;
        }
    }

}
