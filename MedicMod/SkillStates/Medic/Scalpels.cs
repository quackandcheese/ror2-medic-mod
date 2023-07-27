using EntityStates;
using RoR2;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace MedicMod.SkillStates
{
    public class Scalpels : BaseSkillState
    {
        // TODO: Reduce knockback

        public static float damageCoefficient = Modules.StaticValues.scalpelDamageCoefficient;
        public static float procCoefficient = 1f;
        public static float baseDuration = 0.8f;
        public static float force = 800f; // If this is knockback -- decrease
        public static float recoil = 3f;
        public static float range = 256f;
        public static GameObject tracerEffectPrefab = RoR2.LegacyResourcesAPI.Load<GameObject>("Prefabs/Effects/Tracers/TracerGoldGat");
        public static float delayBetweenBulletsMax = 0.1f;

        public static int scalpelAmount = 4;
        public static float intermittent = 0.25f;

        private float duration;
        private float fireTime;
        private int shotTimes;
        private string muzzleString;



        private float delayBetweenBullets = 0f;

        public override void OnEnter()
        {
            base.OnEnter();
            this.duration = Shoot.baseDuration / this.attackSpeedStat;
            this.fireTime = 0.2f * this.duration;
            base.characterBody.SetAimTimer(2f);
            this.muzzleString = "Muzzle";

            base.PlayAnimation("LeftArm, Override", "ShootGun", "ShootGun.playbackRate", 1.8f);
        }

        public override void OnExit()
        {
            base.OnExit();
        }

        private void Fire()
        {

            if (shotTimes < scalpelAmount)
            {
                shotTimes++;

                base.characterBody.AddSpreadBloom(1.5f);
                //EffectManager.SimpleMuzzleFlash(EntityStates.Commando.CommandoWeapon.FirePistol2.muzzleEffectPrefab, base.gameObject, this.muzzleString, false);
                Util.PlaySound("Play_merc_sword_swing", base.gameObject);

                if (base.isAuthority)
                {
                    Ray aimRay = base.GetAimRay();
                    //base.AddRecoil(-1f * Shoot.recoil, -2f * Shoot.recoil, -0.5f * Shoot.recoil, 0.5f * Shoot.recoil);

                    new BulletAttack
                    {
                        bulletCount = 1,
                        aimVector = aimRay.direction,
                        origin = aimRay.origin,
                        damage = Shoot.damageCoefficient * this.damageStat,
                        damageColorIndex = DamageColorIndex.Default,
                        damageType = DamageType.Generic,
                        falloffModel = BulletAttack.FalloffModel.DefaultBullet,
                        maxDistance = Shoot.range,
                        force = Shoot.force,
                        hitMask = LayerIndex.CommonMasks.bullet,
                        minSpread = 0f,
                        maxSpread = 0f,
                        isCrit = base.RollCrit(),
                        owner = base.gameObject,
                        muzzleName = muzzleString,
                        smartCollision = true,
                        procChainMask = default(ProcChainMask),
                        procCoefficient = procCoefficient,
                        radius = 0.75f,
                        sniper = false,
                        stopperMask = LayerIndex.CommonMasks.bullet,
                        weapon = null,
                        tracerEffectPrefab = Shoot.tracerEffectPrefab,
                        spreadPitchScale = 0f,
                        spreadYawScale = 0f,
                        queryTriggerInteraction = QueryTriggerInteraction.UseGlobal,
                        hitEffectPrefab = EntityStates.Commando.CommandoWeapon.FirePistol2.hitEffectPrefab,
                    }.Fire();
                }
            }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            delayBetweenBullets += Time.deltaTime;
            if (delayBetweenBullets >= delayBetweenBulletsMax)
            {
                Fire();
                delayBetweenBullets = 0f;
            }

            if (base.fixedAge >= this.fireTime)
            {

            }

            if (base.fixedAge >= this.duration && base.isAuthority)
            {
                this.outer.SetNextStateToMain();
                return;
            }
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.PrioritySkill;
        }
    }
}
