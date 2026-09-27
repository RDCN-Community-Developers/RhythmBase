/// <summary>
/// Scaffolding for the <see cref="_g_infoRootClassType_"/> JSON upgrade pass of this project.
/// Derive from this type and implement <see cref="AddUpgraders"/> with the concrete registrations.
/// The discriminator resolution, version gating and data-source wiring are provided here so every
/// project behaves consistently.
/// </summary>
internal abstract class _g_mtpName__g_registryId_UpgraderBase
{
	private readonly JsonUpgradePipeline _pipeline;

	protected _g_mtpName__g_registryId_UpgraderBase()
	{
		JsonUpgradePipeline pipeline = new();
		// Event arrays live either under "events" or as the document root array.
		pipeline.AddDiscriminator("$.events[*]", "type", ResolveEventType);
		pipeline.AddDiscriminator("$[*]", "type", ResolveEventType);
		AddUpgraders(pipeline);
		_pipeline = pipeline;
	}

	/// <summary>Registers this project's concrete upgraders.</summary>
	protected abstract void AddUpgraders(JsonUpgradePipeline pipeline);

	/// <summary>
	/// Returns the source version of the document. The default uses the version already known to the host
	/// (for example a value read from a separate manifest); override to read it from the document instead.
	/// </summary>
	protected virtual int ReadSourceVersion(ReadOnlySequence<byte> source, MetadataJsonSerializerOptions options) => options.Version;

	/// <summary>Creates the sub-project upgrade state, or <see langword="null"/> when it needs none.</summary>
	protected virtual JsonUpgradeState? CreateState() => null;

	/// <summary>Wraps a data source so its JSON is upgraded before it reaches the deserializer.</summary>
	public IJsonDataSource WrapSource(IJsonDataSource source, MetadataJsonSerializerOptions options, int targetVersion)
	{
		if (!options.UpgradeToLatest)
			return source;
		return new JsonUpgradingDataSource(source, _pipeline, targetVersion, s => ReadSourceVersion(s, options), CreateState());
	}

	/// <summary>
	/// Resolves the discriminator string to this project's enum key. Override when the project has a generated
	/// <c>EnumConverter.TryParse(ref Utf8JsonReader, out TEnum)</c> overload; the default matches nothing.
	/// </summary>
	protected virtual bool ResolveEventType(ref Utf8JsonReader reader, out int key)
	{
		key = 0;
		return false;
	}
}
