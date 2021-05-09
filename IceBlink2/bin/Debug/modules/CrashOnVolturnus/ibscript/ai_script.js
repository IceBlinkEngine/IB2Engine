// ActionToTake must be set along with CombatTarget SpellToCast, as applicable
var closestPC = inputs.GetValue("closestPC");
var distToClosestPC = parseInt(inputs.GetValue("distToClosestPC"));

if (isNaN(distToClosestPC)) {
    // The parameter was not an integer for some reason
    outputs.SetValue("ActionToTake", "Move");
} else {
    if (distToClosestPC <= 1) {
        outputs.SetValue("ActionToTake", "Attack");
    } else {
        // In here, we would probably also want to check to see if we should use a rangeed weapon
        outputs.SetValue("ActionToTake", "Move");
    }
}
outputs.SetValue("CombatTarget", closestPC);