using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IceBlink2
{
    public enum AnimationState
    {
        None,
        PcMeleeAttackAnimation,
        PcRangedAttackAnimation,
        PcRangedProjectileAnimation,
        PcRangedEndingAnimation,
        CreatureHitAnimation,
        CreatureMissedAnimation,
        CreatureMeleeAttackAnimation,
        CreatureRangedAttackAnimation,
        CreatureRangedProjectileAnimation,
        CreatureRangedEndingAnimation,
        PcHitAnimation,
        PcMissedAnimation,
        PcCastAttackAnimation,
        PcCastProjectileAnimation,
        PcCastEndingAnimation,
        CreatureCastAttackAnimation,
        CreatureCastProjectileAnimation,
        CreatureCastEndingAnimation,
        CreatureThink,
        CreatureMove,
        DeathAnimation
    }
}
