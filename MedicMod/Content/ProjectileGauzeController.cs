// RoR2, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// RoR2.Projectile.ProjectileGauzeController
using System;
using EntityStates;
using EntityStates.Loader;
using MedicMod.SkillStates;
using RoR2;
using RoR2.Projectile;
using UnityEngine;

namespace MedicMod.Content
{
    /*
    [RequireComponent(typeof(ProjectileController))]
    [RequireComponent(typeof(ProjectileSimple))]
    [RequireComponent(typeof(EntityStateMachine))]
    [RequireComponent(typeof(ProjectileStickOnImpact))]
    public class ProjectileGauzeController : MonoBehaviour
    {
        private struct OwnerInfo
        {
            public readonly GameObject gameObject;

            public readonly CharacterBody characterBody;

            public readonly CharacterMotor characterMotor;

            public readonly Rigidbody rigidbody;

            public readonly EntityStateMachine stateMachine;

            public readonly bool hasEffectiveAuthority;

            public OwnerInfo(GameObject ownerGameObject)
            {
                this = default(OwnerInfo);
                gameObject = ownerGameObject;
                if (!gameObject)
                {
                    return;
                }
                characterBody = gameObject.GetComponent<CharacterBody>();
                characterMotor = gameObject.GetComponent<CharacterMotor>();
                rigidbody = gameObject.GetComponent<Rigidbody>();
                hasEffectiveAuthority = Util.HasEffectiveAuthority(gameObject);
                EntityStateMachine[] components = gameObject.GetComponents<EntityStateMachine>();
                for (int i = 0; i < components.Length; i++)
                {
                    if (components[i].customName == "Hook")
                    {
                        stateMachine = components[i];
                        break;
                    }
                }
            }
        }

        private class BaseState : EntityStates.BaseState
        {
            protected ProjectileGauzeController gauzeController;

            protected Vector3 aimOrigin;

            protected Vector3 position;

            protected bool ownerValid { get; private set; }

            protected ref OwnerInfo owner => ref gauzeController.owner;

            private void UpdatePositions()
            {
                aimOrigin = gauzeController.owner.characterBody.aimOrigin;
                position = base.transform.position + base.transform.up * gauzeController.normalOffset;
            }

            public override void OnEnter()
            {
                base.OnEnter();
                gauzeController = GetComponent<ProjectileGauzeController>();
                ownerValid = (bool)gauzeController && (bool)gauzeController.owner.gameObject;
                Chat.AddMessage($"ownerValid: {ownerValid}, gauzeController: {(bool)gauzeController}, gO: {(bool)gauzeController.owner.gameObject}");
                // For some reason the owner gameObject shows up as null.
                if (ownerValid)
                {
                    UpdatePositions();
                }
            }

            public override void FixedUpdate()
            {
                base.FixedUpdate();
                if (ownerValid)
                {
                    ownerValid &= gauzeController.owner.gameObject;
                    if (ownerValid)
                    {
                        UpdatePositions();
                        FixedUpdateBehavior();
                    }
                }
                if (NetworkServer.active && !ownerValid)
                {
                    Chat.AddMessage($"now ownerValid: {(bool)gauzeController && (bool)gauzeController.owner.gameObject}");
                    Chat.AddMessage("DESTROYED");
                    ownerValid = false;
                    EntityState.Destroy(base.gameObject);
                }
            }

            protected virtual void FixedUpdateBehavior()
            {
                if (base.isAuthority && !gauzeController.OwnerIsInFiringState())
                {
                    outer.SetNextState(new ReturnState());
                }
            }

            protected Ray GetOwnerAimRay()
            {
                if (!owner.characterBody)
                {
                    return default(Ray);
                }
                return owner.characterBody.inputBank.GetAimRay();
            }
        }

        private class FlyState : BaseState
        {
            private float duration;

            public override void OnEnter()
            {
                base.OnEnter();
                duration = gauzeController.maxTravelDistance / gauzeController.GetComponent<ProjectileSimple>().velocity;
            }

            protected override void FixedUpdateBehavior()
            {
                base.FixedUpdateBehavior();
                if (!base.isAuthority)
                {
                    return;
                }
                if (gauzeController.projectileStickOnImpactController.stuck)
                {
                    EntityState entityState = null;
                    if ((bool)gauzeController.projectileStickOnImpactController.stuckBody)
                    {
                        Rigidbody component = gauzeController.projectileStickOnImpactController.stuckBody.GetComponent<Rigidbody>();
                        if ((bool)component && component.mass < gauzeController.yankMassLimit)
                        {
                            CharacterBody component2 = component.GetComponent<CharacterBody>();
                            if (!component2 || !component2.isPlayerControlled || component2.teamComponent.teamIndex != base.projectileController.teamFilter.teamIndex || FriendlyFireManager.ShouldDirectHitProceed(component2.healthComponent, base.projectileController.teamFilter.teamIndex))
                            {
                                entityState = new YankState();
                            }
                        }
                    }
                    if (entityState == null)
                    {
                        entityState = new GripState();
                    }
                    DeductOwnerStock();
                    outer.SetNextState(entityState);
                }
                else if (duration <= base.fixedAge)
                {
                    outer.SetNextState(new ReturnState());
                }
            }

            private void DeductOwnerStock()
            {
                if (!base.ownerValid || !base.owner.hasEffectiveAuthority)
                {
                    return;
                }
                SkillLocator component = base.owner.gameObject.GetComponent<SkillLocator>();
                if ((bool)component)
                {
                    GenericSkill utility = component.utility;
                    if ((bool)utility)
                    {
                        utility.DeductStock(1);
                    }
                }
            }
        }

        private class BaseGripState : BaseState
        {
            protected float currentDistance;

            public override void OnEnter()
            {
                base.OnEnter();
                currentDistance = Vector3.Distance(aimOrigin, position);
            }

            protected override void FixedUpdateBehavior()
            {
                base.FixedUpdateBehavior();
                currentDistance = Vector3.Distance(aimOrigin, position);
                if (base.isAuthority)
                {
                    bool flag = !gauzeController.projectileStickOnImpactController.stuck;
                    bool flag2 = currentDistance < gauzeController.nearBreakDistance;
                    bool flag3 = !gauzeController.OwnerIsInFiringState();
                    if (!base.owner.stateMachine || !((base.owner.stateMachine.state as BaseSkillState)?.IsKeyDownAuthority() ?? false) || flag3 || flag2 || flag)
                    {
                        outer.SetNextState(new ReturnState());
                    }
                }
            }
        }

        private class GripState : BaseGripState
        {
            private float lastDistance;

            private void DeductStockIfStruckNonPylon()
            {
                GameObject victim = gauzeController.projectileStickOnImpactController.victim;
                if ((bool)victim)
                {
                    GameObject gameObject = victim;
                    EntityLocator component = gameObject.GetComponent<EntityLocator>();
                    if ((bool)component)
                    {
                        gameObject = component.entity;
                    }
                    _ = (bool)gameObject.GetComponent<ProjectileController>();
                }
            }

            public override void OnEnter()
            {
                base.OnEnter();
                lastDistance = Vector3.Distance(aimOrigin, position);
                if (base.ownerValid)
                {
                    gauzeController.didStick = true;
                    if ((bool)base.owner.characterMotor)
                    {
                        Vector3 direction = GetOwnerAimRay().direction;
                        Vector3 velocity = base.owner.characterMotor.velocity;
                        velocity = ((Vector3.Dot(velocity, direction) < 0f) ? Vector3.zero : Vector3.Project(velocity, direction));
                        velocity += direction * gauzeController.initialLookImpulse;
                        velocity += base.owner.characterMotor.moveDirection * gauzeController.initiallMoveImpulse;
                        base.owner.characterMotor.velocity = velocity;
                    }
                }
            }

            protected override void FixedUpdateBehavior()
            {
                base.FixedUpdateBehavior();
                float num = gauzeController.acceleration;
                if (currentDistance > lastDistance)
                {
                    num *= gauzeController.escapeForceMultiplier;
                }
                lastDistance = currentDistance;
                if (base.owner.hasEffectiveAuthority && (bool)base.owner.characterMotor && (bool)base.owner.characterBody)
                {
                    Ray ownerAimRay = GetOwnerAimRay();
                    Vector3 normalized = (base.transform.position - base.owner.characterBody.aimOrigin).normalized;
                    Vector3 vector = normalized * num;
                    float time = Mathf.Clamp01(base.fixedAge / gauzeController.lookAccelerationRampUpDuration);
                    float num2 = gauzeController.lookAccelerationRampUpCurve.Evaluate(time);
                    float num3 = Util.Remap(Vector3.Dot(ownerAimRay.direction, normalized), -1f, 1f, 1f, 0f);
                    vector += ownerAimRay.direction * (gauzeController.lookAcceleration * num2 * num3);
                    vector += base.owner.characterMotor.moveDirection * gauzeController.moveAcceleration;
                    base.owner.characterMotor.ApplyForce(vector * (base.owner.characterMotor.mass * Time.fixedDeltaTime), alwaysApply: true, disableAirControlUntilCollision: true);
                }
            }
        }

        private class YankState : BaseGripState
        {
            public static float yankSpeed;

            public static float delayBeforeYanking;

            public static float hoverTimeLimit = 0.5f;

            private CharacterBody stuckBody;

            public override void OnEnter()
            {
                base.OnEnter();
                stuckBody = gauzeController.projectileStickOnImpactController.stuckBody;
            }

            protected override void FixedUpdateBehavior()
            {
                base.FixedUpdateBehavior();
                if (!stuckBody)
                {
                    return;
                }
                if (Util.HasEffectiveAuthority(stuckBody.gameObject))
                {
                    Vector3 vector = aimOrigin - position;
                    IDisplacementReceiver component = stuckBody.GetComponent<IDisplacementReceiver>();
                    if ((bool)(Component)component && base.fixedAge >= delayBeforeYanking)
                    {
                        component.AddDisplacement(vector * (yankSpeed * Time.fixedDeltaTime));
                    }
                }
                if (base.owner.hasEffectiveAuthority && (bool)base.owner.characterMotor && base.fixedAge < hoverTimeLimit)
                {
                    Vector3 velocity = base.owner.characterMotor.velocity;
                    if (velocity.y < 0f)
                    {
                        velocity.y = 0f;
                        base.owner.characterMotor.velocity = velocity;
                    }
                }
            }
        }

        private class ReturnState : BaseState
        {
            private float returnSpeedAcceleration = 240f;

            private float returnSpeed;

            public override void OnEnter()
            {
                base.OnEnter();
                if (base.ownerValid)
                {
                    returnSpeed = gauzeController.projectileSimple.velocity;
                    returnSpeedAcceleration = returnSpeed * 2f;
                }
                if (NetworkServer.active && (bool)gauzeController)
                {
                    gauzeController.projectileStickOnImpactController.Detach();
                    gauzeController.projectileStickOnImpactController.ignoreCharacters = true;
                    gauzeController.projectileStickOnImpactController.ignoreWorld = true;
                }
                Collider component = GetComponent<Collider>();
                if ((bool)component)
                {
                    component.enabled = false;
                }
            }

            protected override void FixedUpdateBehavior()
            {
                base.FixedUpdateBehavior();
                if (!base.rigidbody)
                {
                    return;
                }
                returnSpeed += returnSpeedAcceleration * Time.fixedDeltaTime;
                base.rigidbody.velocity = (aimOrigin - position).normalized * returnSpeed;
                if (NetworkServer.active)
                {
                    Vector3 endPosition = position + base.rigidbody.velocity * Time.fixedDeltaTime;
                    if (HGMath.Overshoots(position, endPosition, aimOrigin))
                    {
                        EntityState.Destroy(base.gameObject);
                    }
                }
            }
        }

        private ProjectileController projectileController;

        private ProjectileStickOnImpact projectileStickOnImpactController;

        private ProjectileSimple projectileSimple;

        public SerializableEntityStateType ownerHookStateType;

        public float acceleration;

        public float lookAcceleration = 4f;

        public float lookAccelerationRampUpDuration = 0.25f;

        public float initialLookImpulse = 5f;

        public float initiallMoveImpulse = 5f;

        public float moveAcceleration = 4f;

        public string enterSoundString;

        public string exitSoundString;

        public string hookDistanceRTPCstring;

        public float minHookDistancePitchModifier;

        public float maxHookDistancePitchModifier;

        public AnimationCurve lookAccelerationRampUpCurve;

        public Transform ropeEndTransform;

        public string muzzleStringOnBody = "Muzzle";

        [Tooltip("The minimum distance the hook can be from the target before it detaches.")]
        public float nearBreakDistance;

        [Tooltip("The maximum distance this hook can travel.")]
        public float maxTravelDistance;

        public float escapeForceMultiplier = 2f;

        public float normalOffset = 1f;

        public float yankMassLimit;

        private Type resolvedOwnerHookStateType;

        private OwnerInfo owner;

        private bool didStick;

        private uint soundID;

        private void Awake()
        {
            Chat.AddMessage("Actually awoken");
            projectileStickOnImpactController = GetComponent<ProjectileStickOnImpact>();
            projectileController = GetComponent<ProjectileController>();
            projectileSimple = GetComponent<ProjectileSimple>();
            resolvedOwnerHookStateType = ownerHookStateType.stateType;
            if ((bool)ropeEndTransform)
            {
                soundID = Util.PlaySound(enterSoundString, ropeEndTransform.gameObject);
            }
        }

        private void FixedUpdate()
        {
            if ((bool)ropeEndTransform)
            {
                float in_value = Util.Remap((ropeEndTransform.transform.position - base.transform.position).magnitude, minHookDistancePitchModifier, maxHookDistancePitchModifier, 0f, 100f);
                AkSoundEngine.SetRTPCValueByPlayingID(hookDistanceRTPCstring, in_value, soundID);
            }
        }

        private void AssignHookReferenceToBodyStateMachine()
        {
            if ((bool)owner.stateMachine && owner.stateMachine.state is GauzeGrapple gauzeGrapple)
            {
                gauzeGrapple.SetHookReference(base.gameObject);
            }
            Transform modelTransform = owner.gameObject.GetComponent<ModelLocator>().modelTransform;
            if (!modelTransform)
            {
                return;
            }
            ChildLocator component = modelTransform.GetComponent<ChildLocator>();
            if ((bool)component)
            {
                Transform transform = component.FindChild(muzzleStringOnBody);
                if ((bool)transform)
                {
                    ropeEndTransform.SetParent(transform, worldPositionStays: false);
                }
            }
        }

        private void Start()
        {
            owner = new OwnerInfo(projectileController.owner);
            AssignHookReferenceToBodyStateMachine();
        }

        private void OnDestroy()
        {
            if ((bool)ropeEndTransform)
            {
                Util.PlaySound(exitSoundString, ropeEndTransform.gameObject);
                UnityEngine.Object.Destroy(ropeEndTransform.gameObject);
            }
            else
            {
                AkSoundEngine.StopPlayingID(soundID);
            }
        }

        private bool OwnerIsInFiringState()
        {
            if ((bool)owner.stateMachine)
            {
                return owner.stateMachine.state.GetType() == resolvedOwnerHookStateType;
            }
            return false;
        }
    }
    */

    public class ProjectileGauzeController : MonoBehaviour
    {
        private void Start()
        {
            this.stickComponent = base.GetComponent<ProjectileStickOnImpact>();
            this.controllerComponent = base.GetComponent<ProjectileController>();
            if (this.controllerComponent && this.controllerComponent.owner)
            {
                this.stateMachine = Array.Find<EntityStateMachine>(this.controllerComponent.owner.GetComponents<EntityStateMachine>(), (EntityStateMachine element) => element.customName == "Hook");
                medicTracker = this.controllerComponent.owner.GetComponent<MedicTracker>();
            }
        }

        private void FixedUpdate()
        {
            if (!this.owner)
            {
                this.controller = base.GetComponent<ProjectileController>();
                if (this.controller && this.controller.Networkowner)
                {
                    if (controller.ghost)
                    {
                        this.ghost = this.controller.ghost.gameObject;
                        this.line = this.ghost.GetComponent<LineRenderer>();
                        this.start = this.ghost.transform.GetChild(1);
                    }
                    this.owner = this.controller.owner;
                    ModelLocator component = this.owner.GetComponent<ModelLocator>();
                    if (component && component.modelTransform)
                    {
                        ChildLocator component2 = component.modelTransform.GetComponent<ChildLocator>();
                        if (component2)
                        {
                            this.muzzle = component2.FindChild("Muzzle");
                        }
                    }
                }
            }
            if (this.muzzle && this.line && this.start)
            {
                this.line.SetPosition(0, this.start.position);
                this.line.SetPosition(1, this.muzzle.position);
            }
            if (this.stickComponent && this.stickComponent.stuck && this.stickComponent.victim)
            {
                CharacterBody victimBody = this.stickComponent.victim.GetComponent<CharacterBody>();
                if (victimBody)
                {
                    GauzeGrapple grapple;
                    if (this.stateMachine && (grapple = (this.stateMachine.state as GauzeGrapple)) != null)
                    {
                        grapple.HookCollision(true, base.transform.position, victimBody.healthComponent);
                    }
                    SetStateOnHurt victimHurt = victimBody.GetComponent<SetStateOnHurt>();
                    if (victimHurt)
                    {
                        victimHurt.SetStun(1.5f);
                    }
                }
                else
                {
                    GauzeGrapple grapple;
                    if (this.stateMachine && (grapple = (this.stateMachine.state as GauzeGrapple)) != null)
                    {
                        grapple.HookCollision(false, base.transform.position, null);
                    }
                }
            }
        }

        private void OnDisable()
        {
            GauzeGrapple utility;
            if (this.stateMachine && (utility = (this.stateMachine.state as GauzeGrapple)) != null)
            {
                utility.NoCollision();
            }
        }

        private MedicTracker medicTracker;

        private ProjectileStickOnImpact stickComponent;

        private ProjectileController controllerComponent;

        private EntityStateMachine stateMachine;

        [SerializeField]
        public GameObject owner;

        [SerializeField]
        public GameObject ghost;

        [SerializeField]
        public ProjectileController controller;

        [SerializeField]
        public Transform muzzle;

        [SerializeField]
        public LineRenderer line;

        [SerializeField]
        public Transform start;
    }
}