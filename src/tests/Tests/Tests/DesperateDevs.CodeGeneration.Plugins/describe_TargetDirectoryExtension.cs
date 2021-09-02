using DesperateDevs.CodeGeneration.Plugins;
using NSpec;
using Shouldly;

class describe_TargetDirectoryExtension : nspec {

    void safe_dir() {
        it["appends '/Generated'"] = () => "Assets".ToSafeDirectory().ShouldBe("Assets/Generated");
        it["appends 'Generated'"] = () => "Assets/".ToSafeDirectory().ShouldBe("Assets/Generated");
        it["doesn't append"] = () => "Assets/Generated".ToSafeDirectory().ShouldBe("Assets/Generated");
        it["removes trailing '/'"] = () => "Assets/Generated/".ToSafeDirectory().ShouldBe("Assets/Generated");
        it["appends 'Generated'"] = () => "/".ToSafeDirectory().ShouldBe("/Generated");
        it["appends 'Generated'"] = () => "".ToSafeDirectory().ShouldBe("Generated");
        it["appends 'Generated'"] = () => ".".ToSafeDirectory().ShouldBe("Generated");
    }
}
