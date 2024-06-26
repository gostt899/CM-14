using Content.Shared._CM14.Marines.Skills;
using Robust.Shared.GameStates;

namespace Content.Shared._CM14.Storage;

[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
[Access(typeof(CMStorageSystem))]
public sealed partial class StorageSkillRequiredComponent : Component
{
    [IncludeDataField, AutoNetworkedField]
    public Skills Skills;
}
