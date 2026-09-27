namespace RhythmBase.RhythmDoctor.Components;

#pragma warning disable CS1591
public static class CollectionBuilders
{
	public static Room BuildRoom(ReadOnlySpan<byte> rooms)
	{
		Room result = default;
		foreach (byte item in rooms)
		{
			if (item > RoomCapacity) continue;
			result[item] = true;
		}
		return result;
	}
	public static Order BuildOrder(ReadOnlySpan<int> indices)
	{
		Order order = new(indices.ToArray());
		return order;
    }
	public static PlayerTypeGroup BuildPlayerTypeGroup(ReadOnlySpan<PlayerType> playerTypes)
	{
		PlayerTypeGroup group = default;
		for (int i = 0; i < RowCapacity && i < playerTypes.Length; i++)
		{
			group[i] = playerTypes[i];
		}
		return group;
    }
}