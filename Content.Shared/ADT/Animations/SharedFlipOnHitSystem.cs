using Content.Shared.Item.ItemToggle.Components;
using Content.Shared.Standing;
using Content.Shared.Weapons.Melee.Events;
using Robust.Shared.Serialization;
using Robust.Shared.Timing;
using Content.Shared.Mobs;
using Content.Shared.Mobs.Components;

namespace Content.Shared.Animations;

public abstract class SharedFlipOnHitSystem : EntitySystem
{
    [Dependency] private readonly IGameTiming _timing = default!;
    [Dependency] private readonly StandingStateSystem _standingState = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<FlipOnHitComponent, MeleeHitEvent>(OnHit);
    }

    private void OnHit(Entity<FlipOnHitComponent> ent, ref MeleeHitEvent args)
    {
        if (!_timing.IsFirstTimePredicted)
            return;

        if (args.HitEntities.Count == 0)
            return;

        if (TryComp(ent, out ItemToggleComponent? itemToggle) && !itemToggle.Activated)
            return;

        if (_standingState.IsDown(args.User))
            return;


        foreach (var damaged in args.HitEntities)
        {
            if (!HasComp<MobStateComponent>(damaged))
                continue;
            PlayAnimation(damaged);
        }
    }

    protected abstract void PlayAnimation(EntityUid user);
}

[Serializable, NetSerializable]
public sealed class FlipOnHitEvent(NetEntity user) : EntityEventArgs
{
    public NetEntity User = user;
}
