﻿using Content.Shared._CM14.Armor;
using Content.Shared._CM14.Xenos.Fortify;
using Content.Shared._CM14.Xenos.Rest;
using Content.Shared._CM14.Xenos.Sweep;
using Content.Shared.Actions;
using Content.Shared.Movement.Systems;
using Content.Shared.Popups;
using Content.Shared.StatusEffect;

namespace Content.Shared._CM14.Xenos.Crest;

public sealed class XenoCrestSystem : EntitySystem
{
    [Dependency] private readonly SharedActionsSystem _actions = default!;
    [Dependency] private readonly SharedAppearanceSystem _appearance = default!;
    [Dependency] private readonly MovementSpeedModifierSystem _movementSpeed = default!;
    [Dependency] private readonly SharedPopupSystem _popup = default!;

    public override void Initialize()
    {
        // TODO CM14 resist knockback
        SubscribeLocalEvent<XenoCrestComponent, XenoToggleCrestActionEvent>(OnXenoCrestAction);
        SubscribeLocalEvent<XenoCrestComponent, RefreshMovementSpeedModifiersEvent>(OnXenoCrestRefreshMovementSpeed);
        SubscribeLocalEvent<XenoCrestComponent, CMGetArmorEvent>(OnXenoCrestGetArmor);

        SubscribeLocalEvent<XenoCrestComponent, BeforeStatusEffectAddedEvent>(OnXenoCrestBeforeStatusAdded);

        SubscribeLocalEvent<XenoCrestComponent, XenoFortifyAttemptEvent>(OnXenoCrestFortifyAttempt);
        SubscribeLocalEvent<XenoCrestComponent, XenoTailSweepAttemptEvent>(OnXenoCrestTailSweepAttempt);
        SubscribeLocalEvent<XenoCrestComponent, XenoRestAttemptEvent>(OnXenoCrestRestAttempt);
    }

    private void OnXenoCrestAction(Entity<XenoCrestComponent> xeno, ref XenoToggleCrestActionEvent args)
    {
        if (args.Handled)
            return;

        var attempt = new XenoToggleCrestAttemptEvent();
        RaiseLocalEvent(xeno, ref attempt);

        if (attempt.Cancelled)
            return;

        args.Handled = true;

        xeno.Comp.Lowered = !xeno.Comp.Lowered;
        Dirty(xeno);

        _movementSpeed.RefreshMovementSpeedModifiers(xeno);
        _appearance.SetData(xeno, XenoVisualLayers.Crest, xeno.Comp.Lowered);

        foreach (var (actionId, action) in _actions.GetActions(xeno))
        {
            if (action.BaseEvent is XenoToggleCrestActionEvent)
                _actions.SetToggled(actionId, xeno.Comp.Lowered);
        }
    }

    private void OnXenoCrestRefreshMovementSpeed(Entity<XenoCrestComponent> xeno, ref RefreshMovementSpeedModifiersEvent args)
    {
        if (xeno.Comp.Lowered)
            args.ModifySpeed(xeno.Comp.SpeedMultiplier, xeno.Comp.SpeedMultiplier);
    }

    private void OnXenoCrestGetArmor(Entity<XenoCrestComponent> xeno, ref CMGetArmorEvent args)
    {
        if (xeno.Comp.Lowered)
            args.Armor += xeno.Comp.Armor;
    }

    private void OnXenoCrestBeforeStatusAdded(Entity<XenoCrestComponent> xeno, ref BeforeStatusEffectAddedEvent args)
    {
        if (xeno.Comp.Lowered && args.Key == xeno.Comp.ImmuneToStatus)
            args.Cancelled = true;
    }

    private void OnXenoCrestFortifyAttempt(Entity<XenoCrestComponent> xeno, ref XenoFortifyAttemptEvent args)
    {
        if (xeno.Comp.Lowered)
        {
            _popup.PopupClient(Loc.GetString("cm-xeno-toggle-crest-cant-fortify"), xeno, xeno);
            args.Cancelled = true;
        }
    }

    private void OnXenoCrestTailSweepAttempt(Entity<XenoCrestComponent> xeno, ref XenoTailSweepAttemptEvent args)
    {
        if (xeno.Comp.Lowered)
        {
            _popup.PopupClient(Loc.GetString("cm-xeno-toggle-crest-cant-tail-sweep"), xeno, xeno);
            args.Cancelled = true;
        }
    }

    private void OnXenoCrestRestAttempt(Entity<XenoCrestComponent> xeno, ref XenoRestAttemptEvent args)
    {
        if (xeno.Comp.Lowered)
        {
            _popup.PopupClient(Loc.GetString("cm-xeno-toggle-crest-cant-rest"), xeno, xeno);
            args.Cancelled = true;
        }
    }
}
