using EntityStates;
using MedicMod.Content;
using MedicMod.Modules;
using RoR2;
using RoR2.Skills;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace MedicMod.SkillStates
{
    public class BaseActive : BaseSkillState
    {
        private MedicTracker medicTracker;

        public override void OnEnter()
        {
            base.OnEnter();
            medicTracker = base.gameObject.GetComponent<MedicTracker>();
            medicTracker.enabled = true;
            medicTracker.mode = MedicTracker.Mode.Grappling;

            SkillLocator skillLocator = base.skillLocator;
            GenericSkill genericSkill = (skillLocator != null) ? skillLocator.primary : null;
            if (genericSkill)
            {
                this.TryOverrideSkill(genericSkill);
                genericSkill.onSkillChanged += this.TryOverrideSkill;
            }
            if (base.isAuthority)
            {
                //this.loopPtr = LoopSoundManager.PlaySoundLoopLocal(base.gameObject, this.loopSound);
            }
        }

        public override void OnExit()
        {
            /*if (this.loopPtr.isValid)
            {
                LoopSoundManager.StopSoundLoopLocal(this.loopPtr);
            }*/
            medicTracker.enabled = false;

            SkillLocator skillLocator = base.skillLocator;
            GenericSkill genericSkill = (skillLocator != null) ? skillLocator.primary : null;
            if (genericSkill)
            {
                genericSkill.onSkillChanged -= this.TryOverrideSkill;
            }
            if (this.overriddenSkill)
            {
                this.overriddenSkill.UnsetSkillOverride(this, this.primaryOverride, GenericSkill.SkillOverridePriority.Contextual);
            }
            base.OnExit();
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (base.isAuthority && (!base.IsKeyDownAuthority() || base.characterBody.isSprinting))
            {
                this.outer.SetNextStateToMain();
            }
        }

        private void TryOverrideSkill(GenericSkill skill)
        {
            if (skill && !this.overriddenSkill && !skill.HasSkillOverrideOfPriority(GenericSkill.SkillOverridePriority.Contextual))
            {
                this.overriddenSkill = skill;
                this.overriddenSkill.SetSkillOverride(this, this.primaryOverride, GenericSkill.SkillOverridePriority.Contextual);
                this.overriddenSkill.stock = base.skillLocator.utility.stock;
            }
        }

        [SerializeField]
        public SkillDef primaryOverride = MedicMod.Modules.Survivors.Medic.gauzeSkillDef;

        //[SerializeField]
        //public LoopSoundDef loopSound;

        private GenericSkill overriddenSkill;

        //private LoopSoundManager.SoundLoopPtr loopPtr;
    }
}
