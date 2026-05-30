using RhythmBase.Adofai.Components;
using RhythmBase.Global.Components.Vector;

namespace RhythmBase.Adofai.Events;
/// <summary>
/// Represents an event that adds a particle effect to the level.
/// </summary>
[JsonObjectSerializable]
public class AddParticle : BaseEvent, IBeginningEvent, IImageFileEvent
{
	/// <inheritdoc/>
	public override EventType Type => EventType.AddParticle;

	/// <summary>
	/// Gets or sets the tag associated with the particle effect.
	/// </summary>
	public string Tag { get; set; } = string.Empty;

	/// <summary>
	/// Gets or sets the decoration image used for the particle effect.
	/// </summary>
	public FileReference DecorationImage { get; set; } = string.Empty;

	/// <summary>
	/// Gets or sets the random texture tiling for the particle effect.
	/// </summary>
	public PointNI RandomTextureTiling { get; set; } = new PointNI(1, 1);

	/// <summary>
	/// Gets or sets the simulation space for the particle effect.
	/// </summary>
	public SimulationSpace SimulationSpace { get; set; } = SimulationSpace.Local;

	/// <summary>
	/// Gets or sets the start rotation of the particle effect.
	/// </summary>
	public PointN StartRotation { get; set; } = new PointN(0, 0);

	/// <summary>
	/// Gets or sets the color gradient of the particle effect.
	/// </summary>
	public ColorGradient Color { get; set; } = new ColorGradient();

	/// <summary>
	/// Gets or sets the velocity limit over the lifetime of the particles.
	/// </summary>
	public ValueRange VelocityLimitOverLifetime { get; set; } = new ValueRange(0);

	/// <summary>
	/// Gets or sets the color gradient over the lifetime of the particles.
	/// </summary>
	public ColorGradient ColorOverLifetime { get; set; } = new ColorGradient();
	/// <summary>
	/// Gets or sets the size range of an object over its lifetime.
	/// </summary>
	public ValueRange SizeOverLifetime { get; set; } = new ValueRange(100);
	/// <summary>
	/// Gets or sets the maximum number of particles.
	/// </summary>
	public int MaxParticles { get; set; } = 1000;

	/// <summary>
	/// Gets or sets a value indicating whether the particle effect should autoplay.
	/// </summary>
	public bool AutoPlay { get; set; } = true;

	/// <summary>
	/// Gets or sets the duration of the particle effect playback in seconds.
	/// </summary>
	public int PlayDuration { get; set; } = 5;

	/// <summary>
	/// Gets or sets a value indicating whether the particle effect should loop.
	/// </summary>
	public bool Loop { get; set; } = false;

	/// <summary>
	/// Gets or sets the lifetime range of the particles.
	/// </summary>
	public ValueRange ParticleLifetime { get; set; } = new ValueRange(10);

	/// <summary>
	/// Gets or sets the size of the particles.
	/// </summary>
	public SizeN ParticleSize { get; set; } = new SizeN(100, 100);

	/// <summary>
	/// Gets or sets the velocity range of the particles in the X and Y directions.
	/// </summary>
	public ValueRangePair Velocity { get; set; } = new ValueRangePair(new(0), new(0));

	/// <summary>
	/// Gets or sets the rotation of the particles over time.
	/// </summary>
	public ValueRange RotationOverTime { get; set; } = new ValueRange(0);

	/// <summary>
	/// Gets or sets the shape type of the particle emission.
	/// </summary>
	public EmissionShapeType ShapeType { get; set; } = EmissionShapeType.Circle;

	/// <summary>
	/// Gets or sets the radius of the emission shape.
	/// </summary>
	public float ShapeRadius { get; set; } = 1f;

	/// <summary>
	/// Gets or sets the arc of the emission shape in degrees.
	/// </summary>
	public float Arc { get; set; } = 360f;

	/// <summary>
	/// Gets or sets the mode of the arc emission.
	/// </summary>
	public ArcMode ArcMode { get; set; } = ArcMode.Random;

	/// <summary>
	/// Gets or sets the emission rate of the particles.
	/// </summary>
	public ValueRange EmissionRate { get; set; } = new ValueRange(10);

	/// <summary>
	/// Gets or sets the simulation speed of the particle effect.
	/// </summary>
	public float SimulationSpeed { get; set; } = 100f;

	/// <summary>
	/// Gets or sets the random seed for the particle effect.
	/// </summary>
	public int RandomSeed { get; set; } = 0;

	/// <summary>
	/// Gets or sets the position of the particle effect.
	/// </summary>
	public PointN Position { get; set; } = new PointN(0, 0);

	/// <summary>
	/// Gets or sets the reference point for decoration placement.
	/// </summary>
	public DecorationRelativeTo RelativeTo { get; set; } = DecorationRelativeTo.Global;

	/// <summary>
	/// Gets or sets the pivot offset of the particle effect.
	/// </summary>
	public PointN PivotOffset { get; set; } = new PointN(0, 0);

	/// <summary>
	/// Gets or sets the rotation of the particle effect.
	/// </summary>
	public float Rotation { get; set; } = 0;

	/// <summary>
	/// Gets or sets the scale of the particle effect.
	/// </summary>
	public PointN Scale { get; set; } = new PointN(100, 100);

	/// <summary>
	/// Gets or sets the depth of the particle effect.
	/// </summary>
	public int Depth { get; set; } = -1;

	/// <summary>
	/// Gets or sets the parallax effect of the particle effect.
	/// </summary>
	public PointN Parallax { get; set; } = new PointN(0, 0);

	/// <summary>
	/// Gets or sets the parallax offset of the particle effect.
	/// </summary>
	public PointN ParallaxOffset { get; set; } = new PointN(0, 0);

	/// <summary>
	/// Gets or sets a value indicating whether the rotation of the particle effect is locked.
	/// </summary>
	public bool LockRotation { get; set; } = false;

	/// <summary>
	/// Gets or sets a value indicating whether the scale of the particle effect is locked.
	/// </summary>
	public bool LockScale { get; set; } = false;
	IEnumerable<FileReference> IImageFileEvent.ImageFiles => [DecorationImage];
	IEnumerable<FileReference> IFileEvent.Files => [DecorationImage];
}