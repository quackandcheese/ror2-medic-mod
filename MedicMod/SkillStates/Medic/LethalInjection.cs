using EntityStates;
using MedicMod.SkillStates.BaseStates;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;


namespace MedicMod.SkillStates
{
    public class LethalInjection : BaseMeleeAttack
    {
        public override void OnEnter()
        {
            this.hitboxName = "Sword";

            this.damageType = DamageType.PoisonOnHit;
            this.damageCoefficient = Modules.StaticValues.injectionDamageCoefficient;
            this.procCoefficient = 1f;
            this.pushForce = 300f;
            this.bonusForce = Vector3.zero;
            this.baseDuration = 0.5f;
            this.attackStartTime = 0.2f;
            this.attackEndTime = 0.4f;
            this.baseEarlyExitTime = 0.4f;
            this.hitStopDuration = 0.012f;
            this.attackRecoil = 0.5f;
            this.hitHopVelocity = 4f;

            this.swingSoundString = "Play_acrid_m1_slash"; //Play_acrid_m1_bigSlash
            this.hitSoundString = "";
            this.muzzleString = "SwingLeft";
            this.swingEffectPrefab = Modules.Assets.swordSwingEffect;
            this.hitEffectPrefab = Modules.Assets.swordHitImpactEffect;

            this.impactSound = Modules.Assets.swordHitSoundEvent.index;

            base.OnEnter();
        }

        protected override void PlayAttackAnimation()
        {
            base.PlayAttackAnimation();
        }

        protected override void PlaySwingEffect()
        {
            base.PlaySwingEffect();
        }

        protected override void OnHitEnemyAuthority()
        {
            base.OnHitEnemyAuthority();
        }


        public override void OnExit()
        {
            base.OnExit();
        }
    }
}
