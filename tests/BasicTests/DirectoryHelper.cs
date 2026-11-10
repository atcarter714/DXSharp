namespace BasicTests;

[TestFixture, Category("BuildBaseline")]
public class Validate_Directory_Helper {
    [Test]
    public void Paths_Are_Correct() {
        var assets = Path.Combine(AppContext.BaseDirectory, "Assets", "Shaders");
        Assert.That(File.Exists(Path.Combine(assets, "tests_shader_0.hlsl")), Is.True, assets);
        Assert.That(File.Exists(Path.Combine(assets, "dxs", "dxshaderTest.dxs")), Is.True, assets);
    }
}
