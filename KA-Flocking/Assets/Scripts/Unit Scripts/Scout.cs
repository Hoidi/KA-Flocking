using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Flock/Units/Scout")]
public class Scout : Unit
{
    public override void Initialize()
    {
        attackMode = "meleeAttack";
        attackTime = 1.4f;
    }
}
/*
OBSERVATIONS ABOUT SCOUTS. TRYING TO GET A FEW SCOUTS TO BE ABLE TO LEAD MOST OF AN ARMY TO LONELY ENEMY TARGETS 
With a low neighbour radius ~3.5f the infantry will not follow the scouts enough.
Increasing the weight of the scout to 10x the amount barely helped (This will ofc )
Increasing the neighbour radius to 8f (and decreasing avoidance multiplier by half) helped a bit together with the increase in weight (Increases lagg significantly)
    Seems that the infantry are not inclined enough to change direction to that of scouts, especially if there are external factors (altitude of map or enemies in conflicting directions)
    This might be because of the increased neighbour radius
Scouts tend to flock together mostly.
Current hypothesis. The most common reason for the flock to split of is because the scouts get too far away from the infantry and as such the infantry no longer follows them
This could perhaps be solved by increasing the cohesion factor of the scouts, further increase to the weight of the scouts, increasing the weight for infantry to align with scouts significantly
    Increased cohesion
        did however notice that it worked better with only 1 scout rather than 3, probably because scouts also had higher weights to go with scouts
        Trippling the alignment factor for scouts made it such that groups of ~5 infantry diverged from the army
    Increased alignment for infantry (1->3)
        made the groups larger ~10. Which is still insufficient
    Increasing the alignment for infantry to align with scouts, but not infantry and remove the weight of the added weight to scouts:
        By making the scouts have the same value as 20 infantry we got about 4 times as large groups (half of the entire army) to move away, however half of those eventually got too far away from the scouts
        Test to do the same thing with cohesion
            Does not seem to help at all
    Adding more scouts evenly dispersed in the army
        Used 5 scouts for around 150 infantry and it seemed to help a lot, around 60% of the army goes together to find the other troops infantry
            Adding 15 scouts / 150 infantry resulted in 90% of the army go together however 1/3 of the infantry diverged later on, altitude differences still seem very important though
            Multiple conflicting enemies still resulted in a mostly cohesive group, with a few infantry men often diverging alone at the end
At the moment I believe that scout work decently and I'll try reverting some of the old changes and seeing if I can still reach this behaviour
    Revert neighbour radius to 3.5
        A lot less lag, however it's harder to keep one cohesive structure in the army
        if you disregard that the army is hard to keep cohesive then it works almost equally well
Scouts are almost always in the group that goes out, this means that it will be hard for groups of infantry to find their friends again. 
    Removing the added weight from scouts if you are a scout
        Did not seem to have any affect
    Increase avoidance behaviour for scouts if you are a scout to disperse them more
        No affect
Scouts lag the game alot
    Give them access to all units instead of doing a Physics.Overlapsphere
        Did not help and made code harder to read
    Use layer masks to make sure you only look at troops and objects
        Didn't help significantly
    Use layer masks such that scouts only see enemies
        Destroys their fight of flee behaviour since they don't see sufficient allies
    Only look at larger ranges in the fight or flee behaviour
        Lags a bit but is still significantly better
*/