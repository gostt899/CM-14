﻿using Content.Shared.FixedPoint;
using Robust.Shared.GameStates;
using Robust.Shared.Utility;
using static Robust.Shared.Utility.SpriteSpecifier;

namespace Content.Shared._CM14.Xenos.Pheromones;

[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
[Access(typeof(SharedXenoPheromonesSystem))]
public sealed partial class XenoRecoveryPheromonesComponent : Component
{
    [DataField, AutoNetworkedField]
    public SpriteSpecifier Icon = new Rsi(new ResPath("/Textures/_CM14/Interface/xeno_pheromones_hud.rsi"), "recovery");

    [DataField, AutoNetworkedField]
    public FixedPoint2 Multiplier;

    public override bool SessionSpecific => true;
}
