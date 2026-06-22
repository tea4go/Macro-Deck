using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SuchByte.MacroDeck.ActionButton;
using SuchByte.MacroDeck.Plugins;

namespace MacroDeck.Tests;

/// <summary>
/// Tests for <see cref="ConditionAction"/> configuration serialization.
/// Regression coverage for a bug where UpdateConfiguration() serialized the
/// if-branch actions ("_actions") into BOTH the "actions" and "actionsElse"
/// config keys, so editing a condition action in the GUI silently overwrote
/// the else-branch with the if-branch.
/// These tests only exercise the JSON (de)serialization path - they never call
/// Trigger(), so no GUI or VariableManager state is required.
/// </summary>
[TestFixture]
public class ConditionActionTests
{
    // Mirrors the settings Macro Deck uses to (de)serialize plugin actions.
    private static readonly JsonSerializerSettings SerializerSettings = new()
    {
        TypeNameHandling = TypeNameHandling.Auto,
        NullValueHandling = NullValueHandling.Ignore,
        Error = (_, args) => { args.ErrorContext.Handled = true; },
    };

    private static int CountBranch(string configuration, string key)
    {
        var branchJson = JObject.Parse(configuration)[key]!.ToString();
        return JArray.Parse(branchJson).Count;
    }

    [Test]
    public void UpdateConfiguration_KeepsElseBranchSeparateFromIfBranch()
    {
        // If-branch empty, else-branch has one (dependency-free) child action.
        var action = new ConditionAction
        {
            Actions = new List<PluginAction?>(),
            ActionsElse = new List<PluginAction?> { new ConditionAction() },
        };

        // Before the fix, "actionsElse" was overwritten with "_actions" (empty).
        Assert.That(CountBranch(action.Configuration, "actions"), Is.EqualTo(0),
            "if-branch should be empty");
        Assert.That(CountBranch(action.Configuration, "actionsElse"), Is.EqualTo(1),
            "else-branch must retain its own actions, not be overwritten by the if-branch");
    }

    [Test]
    public void RoundTrip_KeepsBothBranchesIndependent()
    {
        var original = new ConditionAction
        {
            Actions = new List<PluginAction?> { new ConditionAction(), new ConditionAction() },
            ActionsElse = new List<PluginAction?> { new ConditionAction() },
        };

        // Serialize + deserialize the way profile loading does.
        var json = JsonConvert.SerializeObject(original, SerializerSettings);
        var restored = JsonConvert.DeserializeObject<ConditionAction>(json, SerializerSettings)!;

        Assert.That(restored.Actions, Has.Count.EqualTo(2), "if-branch count should round-trip");
        Assert.That(restored.ActionsElse, Has.Count.EqualTo(1), "else-branch count should round-trip");
    }

    [Test]
    public void RoundTrip_PreservesConditionFields()
    {
        var original = new ConditionAction
        {
            ConditionType = ConditionType.Button_State,
            ConditionMethod = ConditionMethod.Equals,
            ConditionValue2 = "On",
        };

        var json = JsonConvert.SerializeObject(original, SerializerSettings);
        var restored = JsonConvert.DeserializeObject<ConditionAction>(json, SerializerSettings)!;

        Assert.Multiple(() =>
        {
            Assert.That(restored.ConditionType, Is.EqualTo(ConditionType.Button_State));
            Assert.That(restored.ConditionMethod, Is.EqualTo(ConditionMethod.Equals));
            Assert.That(restored.ConditionValue2, Is.EqualTo("On"));
        });
    }
}
