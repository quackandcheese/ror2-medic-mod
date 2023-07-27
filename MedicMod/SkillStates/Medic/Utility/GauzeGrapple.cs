using EntityStates;
using RoR2.Projectile;
using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using R2API;
using MedicMod.Content;
using EntityStates.Loader;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;
using MedicMod.Modules;

namespace MedicMod.SkillStates
{
    // Create arc from player to target
    // Calculate position of a dot moving across in that arc
    // Draw the gauze from the player position to that dot

    // COPIED FireHook.cs
    // TODO: Make custom gauze projectile controller
    /*
    public class GauzeGrapple : BaseSkillState
    {
        [SerializeField]
        public GameObject projectilePrefab;

        public static float damageCoefficient;

        public static GameObject muzzleflashEffectPrefab;

        public static string fireSoundString;

        public GameObject hookInstance;

        protected ProjectileStickOnImpact hookStickOnImpact;

        private bool isStuck;

        private bool hadHookInstance;

        private uint soundID;

        public override void OnEnter()
        {
            this.projectilePrefab = Modules.Projectiles.gauzePrefab;

            fireSoundString = "HenryBombThrow";

            damageCoefficient = FireHook.damageCoefficient;

            base.OnEnter();
            if (base.isAuthority)
            {
                Ray aimRay = GetAimRay();
                FireProjectileInfo fireProjectileInfo = new FireProjectileInfo
                {
                    position = aimRay.origin,
                    rotation = Quaternion.LookRotation(aimRay.direction),
                    crit = base.characterBody.RollCrit(),
                    damage = this.damageStat * GauzeGrapple.damageCoefficient,
                    force = 0f,
                    damageColorIndex = DamageColorIndex.Default,
                    procChainMask = default(ProcChainMask),
                    projectilePrefab = projectilePrefab,
                    owner = base.gameObject
                };
                ProjectileManager.instance.FireProjectile(fireProjectileInfo);
            }
            //EffectManager.SimpleMuzzleFlash(muzzleflashEffectPrefab, base.gameObject, "MuzzleLeft", transmit: false);
            Util.PlaySound(FireHook.fireSoundString, base.gameObject);
            //PlayAnimation("Grapple", "FireHookIntro");
        }

        public void SetHookReference(GameObject hook)
        {
            hookInstance = hook;
            hookStickOnImpact = hook.GetComponent<ProjectileStickOnImpact>();
            hadHookInstance = true;
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if ((bool)hookStickOnImpact)
            {
                if (hookStickOnImpact.stuck && !isStuck)
                {
                    //PlayAnimation("Grapple", "FireHookLoop");
                }
                isStuck = hookStickOnImpact.stuck;
            }
            if (base.isAuthority && !hookInstance && hadHookInstance)
            {
                outer.SetNextStateToMain();
            }
        }

        public override void OnExit()
        {
            //PlayAnimation("Grapple", "FireHookExit");
            //EffectManager.SimpleMuzzleFlash(muzzleflashEffectPrefab, base.gameObject, "MuzzleLeft", transmit: false);
            base.OnExit();
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Pain;
        }
    }
    */

    internal class GauzeGrapple : BaseSkillState
    {
        public override void OnEnter()
        {
            base.OnEnter();

            FireProjectileInfo fireProjectileInfo = new FireProjectileInfo
            {
                crit = base.RollCrit(),
                damage = base.characterBody.damage,
                damageColorIndex = DamageColorIndex.Default,
                damageTypeOverride = new DamageType?(DamageType.Stun1s),
                force = 0f,
                owner = base.gameObject,
                position = base.GetAimRay().origin,
                procChainMask = default(ProcChainMask),
                projectilePrefab = Projectiles.gauzePrefab,
                rotation = Util.QuaternionSafeLookRotation(base.GetAimRay().direction),
                useFuseOverride = false,
                useSpeedOverride = false,
                target = null
            };
            ProjectileManager.instance.FireProjectile(fireProjectileInfo);
            this.dashVector = base.inputBank.aimDirection;
            base.characterDirection.forward = this.dashVector;
            //base.PlayAnimation("FullBody, Override", "hook");
            //Util.PlaySound(FireGrenades.attackSoundString, base.gameObject);
            //EffectManager.SimpleMuzzleFlash(Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Common/VFX/MuzzleflashSmokeRing.prefab").WaitForCompletion(), base.gameObject, "hookFireMuzzle", false);
        }

        internal void NoCollision()
        {
            this.noCollision = true;
            //base.GetModelAnimator().SetBool("miss", true);
        }

        internal void HookCollision(bool entityCollision, Vector3 vector, HealthComponent component = null)
        {
            this.collision = true;
            //base.GetModelAnimator().SetBool("hit", true);
            this.hookPosition = vector;

            if (component)
            {
                this.healthComponent = component;
            }
            if (entityCollision)
            {
                //base.GetModelAnimator().SetBool("kick", true);
                base.characterBody.outOfCombatStopwatch = 0f;
            }
            else
            {
                //base.GetModelAnimator().SetBool("ground", true);
            }

            DeductOwnerStock();
        }

        private void DeductOwnerStock()
        {
            if (!base.isAuthority)
            {
                return;
            }

            SkillLocator component = base.gameObject.GetComponent<SkillLocator>();
            if ((bool)component)
            {
                GenericSkill utility = component.utility;
                if ((bool)utility)
                {
                    utility.DeductStock(1);
                }
            }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (characterMotor)
            {
                base.characterMotor.velocity = Vector3.zero;
            }

            if (this.collision && base.characterMotor && base.characterDirection)
            {
                if (inputBank)
                {
                    Vector2 vector = Util.Vector3XZToVector2XY(base.inputBank.moveVector);
                    if (vector != Vector2.zero)
                    {
                        vector.Normalize();
                        Vector3 normalized = new Vector3(vector.x, 0f, vector.y).normalized;
                        base.characterDirection.moveVector = normalized;
                    }
                }
                base.characterDirection.forward = this.dashVector;
                if (Vector3.Distance(base.transform.position, this.hookPosition) > 7f)
                {
                    base.characterMotor.rootMotion += this.dashVector * this.dashVelocity;
                }
                else
                {
                    this.outer.SetNextStateToMain();
                }
            }
            if (!collision && noCollision && isAuthority)
            {
                this.outer.SetNextStateToMain();
            }
            if (fixedAge >= maxDuration && isAuthority)
            {
                this.outer.SetNextStateToMain();
            }
        }

        public override void OnExit()
        {
            base.OnExit();
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Skill;
        }

        private float maxDuration = 1.1f;

        private bool collision;

        private float dashVelocity = 2.5f;

        private Vector3 dashVector;

        private Vector3 hookPosition;

        private bool noCollision;

        private new HealthComponent healthComponent;
    }
}


