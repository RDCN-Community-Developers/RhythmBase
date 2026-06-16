namespace RhythmBase.Global.Components;
partial struct Color
{
	/// <summary>
	/// Creates an Color instance from a color name.
	/// </summary>
	/// <param name="name">The name of the color.</param>
	/// <returns>An Color instance corresponding to the specified color name.</returns>
	/// <exception cref="ArgumentException">Thrown when the color name is invalid.</exception>
	public static Color FromName(string name)
	{
		return TryFromName(name, out Color color) ? color : throw new ArgumentException($"Invalid color name: {name}.");
	}
	/// <summary>
	/// Tries to create an Color instance from a color name.
	/// </summary>
	/// <param name="name">The name of the color.</param>
	/// <param name="color">When this method returns, contains the Color instance created from the color name, if the conversion succeeded, or the default value if the conversion failed.</param>
	/// <returns>true if the color name was converted successfully; otherwise, false.</returns>
	public static bool TryFromName(ReadOnlySpan<char> name, out Color color)
	{
		Span<char> nameLower = new char[name.Length];
		name.ToLowerInvariant(nameLower);
		Color? color2 = nameLower switch
		{
			"aliceblue" => AliceBlue,
			"antiquewhite" => AntiqueWhite,
			"aqua" => Aqua,
			"aquamarine" => Aquamarine,
			"azure" => Azure,
			"beige" => Beige,
			"bisque" => Bisque,
			"black" => Black,
			"blanchedalmond" => BlanchedAlmond,
			"blue" => Blue,
			"blueviolet" => BlueViolet,
			"brown" => Brown,
			"burlywood" => BurlyWood,
			"cadetblue" => CadetBlue,
			"chartreuse" => Chartreuse,
			"chocolate" => Chocolate,
			"coral" => Coral,
			"cornflowerblue" => CornflowerBlue,
			"cornsilk" => Cornsilk,
			"crimson" => Crimson,
			"cyan" => Cyan,
			"darkblue" => DarkBlue,
			"darkcyan" => DarkCyan,
			"darkgoldenrod" => DarkGoldenrod,
			"darkgray" => DarkGray,
			"darkgreen" => DarkGreen,
			"darkkhaki" => DarkKhaki,
			"darkmagenta" => DarkMagenta,
			"darkolivegreen" => DarkOliveGreen,
			"darkorange" => DarkOrange,
			"darkorchid" => DarkOrchid,
			"darkred" => DarkRed,
			"darksalmon" => DarkSalmon,
			"darkseagreen" => DarkSeaGreen,
			"darkslateblue" => DarkSlateBlue,
			"darkslategray" => DarkSlateGray,
			"darkturquoise" => DarkTurquoise,
			"darkviolet" => DarkViolet,
			"deeppink" => DeepPink,
			"deepskyblue" => DeepSkyBlue,
			"dimgray" => DimGray,
			"dodgerblue" => DodgerBlue,
			"firebrick" => Firebrick,
			"floralwhite" => FloralWhite,
			"forestgreen" => ForestGreen,
			"fuchsia" => Fuchsia,
			"gainsboro" => Gainsboro,
			"ghostwhite" => GhostWhite,
			"gold" => Gold,
			"goldenrod" => Goldenrod,
			"gray" => Gray,
			"grey" => Gray,
			"green" => Green,
			"greenyellow" => GreenYellow,
			"honeydew" => Honeydew,
			"hotpink" => HotPink,
			"indianred" => IndianRed,
			"indigo" => Indigo,
			"ivory" => Ivory,
			"khaki" => Khaki,
			"lavender" => Lavender,
			"lavenderblush" => LavenderBlush,
			"lawngreen" => LawnGreen,
			"lemonchiffon" => LemonChiffon,
			"lightblue" => LightBlue,
			"lightcoral" => LightCoral,
			"lightcyan" => LightCyan,
			"lightgoldenrodyellow" => LightGoldenrodYellow,
			"lightgray" => LightGray,
			"lightgreen" => LightGreen,
			"lightpink" => LightPink,
			"lightsalmon" => LightSalmon,
			"lightseagreen" => LightSeaGreen,
			"lightskyblue" => LightSkyBlue,
			"lightslategray" => LightSlateGray,
			"lightsteelblue" => LightSteelBlue,
			"lightyellow" => LightYellow,
			"lime" => Lime,
			"limegreen" => LimeGreen,
			"linen" => Linen,
			"magenta" => Magenta,
			"maroon" => Maroon,
			"mediumaquamarine" => MediumAquamarine,
			"mediumblue" => MediumBlue,
			"mediumorchid" => MediumOrchid,
			"mediumpurple" => MediumPurple,
			"mediumseagreen" => MediumSeaGreen,
			"mediumslateblue" => MediumSlateBlue,
			"mediumspringgreen" => MediumSpringGreen,
			"mediumturquoise" => MediumTurquoise,
			"mediumvioletred" => MediumVioletRed,
			"midnightblue" => MidnightBlue,
			"mintcream" => MintCream,
			"mistyrose" => MistyRose,
			"moccasin" => Moccasin,
			"navajowhite" => NavajoWhite,
			"navy" => Navy,
			"oldlace" => OldLace,
			"olive" => Olive,
			"olivedrab" => OliveDrab,
			"orange" => Orange,
			"orangered" => OrangeRed,
			"orchid" => Orchid,
			"palegoldenrod" => PaleGoldenrod,
			"palegreen" => PaleGreen,
			"paleturquoise" => PaleTurquoise,
			"palevioletred" => PaleVioletRed,
			"papayawhip" => PapayaWhip,
			"peachpuff" => PeachPuff,
			"peru" => Peru,
			"pink" => Pink,
			"plum" => Plum,
			"powderblue" => PowderBlue,
			"purple" => Purple,
			"red" => Red,
			"rosybrown" => RosyBrown,
			"royalblue" => RoyalBlue,
			"saddlebrown" => SaddleBrown,
			"salmon" => Salmon,
			"sandybrown" => SandyBrown,
			"seagreen" => SeaGreen,
			"seashell" => SeaShell,
			"sienna" => Sienna,
			"silver" => Silver,
			"skyblue" => SkyBlue,
			"slateblue" => SlateBlue,
			"slategray" => SlateGray,
			"snow" => Snow,
			"springgreen" => SpringGreen,
			"steelblue" => SteelBlue,
			"tan" => Tan,
			"teal" => Teal,
			"thistle" => Thistle,
			"tomato" => Tomato,
			"turquoise" => Turquoise,
			"violet" => Violet,
			"wheat" => Wheat,
			"white" => White,
			"whitesmoke" => WhiteSmoke,
			"yellow" => Yellow,
			"yellowgreen" => YellowGreen,
			"transparent" => Transparent,
			"empty" => Empty,
			_ => null,
		};
		if (color2 is null)
		{
			color = default;
			return false;
		}
		color = color2.Value;
		return true;
	}
	/// <summary>
	/// Tries to create an Color instance from a color name.
	/// </summary>
	/// <param name="name">The name of the color.</param>
	/// <param name="color">When this method returns, contains the Color instance created from the color name, if the conversion succeeded, or the default value if the conversion failed.</param>
	/// <returns>true if the color name was converted successfully; otherwise, false.</returns>
	public static bool TryFromName(string name, out Color color) => TryFromName((ReadOnlySpan<char>)name, out color);
	/// <summary>
	/// Tries to get the name(s) of the color.
	/// </summary>
	/// <param name="names">When this method returns, contains the name(s) of the color if the conversion succeeded, or an empty array if the conversion failed.</param>
	/// <returns>true if the color name(s) were found; otherwise, false.</returns>
	/// <remarks>
	/// For colors with multiple names, the CMYK name is preferred.
	/// </remarks>
	public readonly bool TryGetName(out string[] names)
	{
		names = _color switch
		{
			4293982463u => ["AliceBlue"],
			4294634455u => ["AntiqueWhite"],
			4278255615u => ["Cyan", "Aqua"],
			4286578644u => ["Aquamarine"],
			4293984255u => ["Azure"],
			4294309340u => ["Beige"],
			4294960324u => ["Bisque"],
			4278190080u => ["Black"],
			4294962125u => ["BlanchedAlmond"],
			4278190335u => ["Blue"],
			4287245282u => ["BlueViolet"],
			4289014314u => ["Brown"],
			4292786311u => ["BurlyWood"],
			4284456608u => ["CadetBlue"],
			4286578432u => ["Chartreuse"],
			4291979550u => ["Chocolate"],
			4294934352u => ["Coral"],
			4284782061u => ["CornflowerBlue"],
			4294965468u => ["Cornsilk"],
			4292613180u => ["Crimson"],
			4278190219u => ["DarkBlue"],
			4278225803u => ["DarkCyan"],
			4290283019u => ["DarkGoldenrod"],
			4289309097u => ["DarkGray"],
			4278215680u => ["DarkGreen"],
			4290623339u => ["DarkKhaki"],
			4287299723u => ["DarkMagenta"],
			4283788079u => ["DarkOliveGreen"],
			4294937600u => ["DarkOrange"],
			4288230092u => ["DarkOrchid"],
			4287299584u => ["DarkRed"],
			4293498490u => ["DarkSalmon"],
			4287609995u => ["DarkSeaGreen"],
			4282924427u => ["DarkSlateBlue"],
			4281290575u => ["DarkSlateGray"],
			4278243025u => ["DarkTurquoise"],
			4287889619u => ["DarkViolet"],
			4294907027u => ["DeepPink"],
			4278239231u => ["DeepSkyBlue"],
			4285098345u => ["DimGray"],
			4280193279u => ["DodgerBlue"],
			4289864226u => ["Firebrick"],
			4294966000u => ["FloralWhite"],
			4280453922u => ["ForestGreen"],
			4294902015u => ["Magenta", "Fuchsia"],
			4292664540u => ["Gainsboro"],
			4294506751u => ["GhostWhite"],
			4294956800u => ["Gold"],
			4292519200u => ["Goldenrod"],
			4286611584u => ["Gray", "Grey"],
			4278222848u => ["Green"],
			4289593135u => ["GreenYellow"],
			4293984240u => ["Honeydew"],
			4294928820u => ["HotPink"],
			4291648604u => ["IndianRed"],
			4283105410u => ["Indigo"],
			4294967280u => ["Ivory"],
			4293977740u => ["Khaki"],
			4293322490u => ["Lavender"],
			4294963445u => ["LavenderBlush"],
			4286381056u => ["LawnGreen"],
			4294965965u => ["LemonChiffon"],
			4289583334u => ["LightBlue"],
			4293951616u => ["LightCoral"],
			4292935679u => ["LightCyan"],
			4294638290u => ["LightGoldenrodYellow"],
			4292072403u => ["LightGray"],
			4287688336u => ["LightGreen"],
			4294948545u => ["LightPink"],
			4294942842u => ["LightSalmon"],
			4280332970u => ["LightSeaGreen"],
			4287090426u => ["LightSkyBlue"],
			4286023833u => ["LightSlateGray"],
			4289774814u => ["LightSteelBlue"],
			4294967264u => ["LightYellow"],
			4278255360u => ["Lime"],
			4281519410u => ["LimeGreen"],
			4294635750u => ["Linen"],
			4286578688u => ["Maroon"],
			4284927402u => ["MediumAquamarine"],
			4278190285u => ["MediumBlue"],
			4290401747u => ["MediumOrchid"],
			4287852763u => ["MediumPurple"],
			4282168177u => ["MediumSeaGreen"],
			4286277870u => ["MediumSlateBlue"],
			4278254234u => ["MediumSpringGreen"],
			4282962380u => ["MediumTurquoise"],
			4291237253u => ["MediumVioletRed"],
			4279834992u => ["MidnightBlue"],
			4294311930u => ["MintCream"],
			4294960353u => ["MistyRose"],
			4294960309u => ["Moccasin"],
			4294958765u => ["NavajoWhite"],
			4278190208u => ["Navy"],
			4294833638u => ["OldLace"],
			4286611456u => ["Olive"],
			4285238819u => ["OliveDrab"],
			4294944000u => ["Orange"],
			4294919424u => ["OrangeRed"],
			4292505814u => ["Orchid"],
			4293847210u => ["PaleGoldenrod"],
			4288215960u => ["PaleGreen"],
			4289720046u => ["PaleTurquoise"],
			4292571283u => ["PaleVioletRed"],
			4294963157u => ["PapayaWhip"],
			4294957753u => ["PeachPuff"],
			4291659071u => ["Peru"],
			4294951115u => ["Pink"],
			4292714717u => ["Plum"],
			4289781990u => ["PowderBlue"],
			4286578816u => ["Purple"],
			4294901760u => ["Red"],
			4290547599u => ["RosyBrown"],
			4282477025u => ["RoyalBlue"],
			4287317267u => ["SaddleBrown"],
			4294606962u => ["Salmon"],
			4294222944u => ["SandyBrown"],
			4281240407u => ["SeaGreen"],
			4294964718u => ["SeaShell"],
			4288696877u => ["Sienna"],
			4290822336u => ["Silver"],
			4287090411u => ["SkyBlue"],
			4285160141u => ["SlateBlue"],
			4285563024u => ["SlateGray"],
			4294966010u => ["Snow"],
			4278255487u => ["SpringGreen"],
			4282811060u => ["SteelBlue"],
			4291998860u => ["Tan"],
			4278222976u => ["Teal"],
			4292394968u => ["Thistle"],
			4294927175u => ["Tomato"],
			4282441936u => ["Turquoise"],
			4293821166u => ["Violet"],
			4294303411u => ["Wheat"],
			uint.MaxValue => ["White"],
			4294309365u => ["WhiteSmoke"],
			4294967040u => ["Yellow"],
			4288335154u => ["YellowGreen"],
			16777215u => ["Transparent"],
			0u => ["Empty"],
			_ => [],
		};
		return names.Length != 0;
	}
	///<summary>
	///Gets the predefined color of alice blue, or #FFF0F8FF.
	///</summary>
	public static readonly Color AliceBlue = new(4293982463u);
	///<summary>
	///Gets the predefined color of antique white, or #FFFAEBD7.
	///</summary>
	public static readonly Color AntiqueWhite = new(4294634455u);
	///<summary>
	///Gets the predefined color of aqua, or #FF00FFFF.
	///</summary>
	public static readonly Color Aqua = new(4278255615u);
	///<summary>
	///Gets the predefined color of aquamarine, or #FF7FFFD4.
	///</summary>
	public static readonly Color Aquamarine = new(4286578644u);
	///<summary>
	///Gets the predefined color of azure, or #FFF0FFFF.
	///</summary>
	public static readonly Color Azure = new(4293984255u);
	///<summary>
	///Gets the predefined color of beige, or #FFF5F5DC.
	///</summary>
	public static readonly Color Beige = new(4294309340u);
	///<summary>
	///Gets the predefined color of bisque, or #FFFFE4C4.
	///</summary>
	public static readonly Color Bisque = new(4294960324u);
	///<summary>
	///Gets the predefined color of black, or #FF000000.
	///</summary>
	public static readonly Color Black = new(4278190080u);
	///<summary>
	///Gets the predefined color of blanched almond, or #FFFFEBCD.
	///</summary>
	public static readonly Color BlanchedAlmond = new(4294962125u);
	///<summary>
	///Gets the predefined color of blue, or #FF0000FF.
	///</summary>
	public static readonly Color Blue = new(4278190335u);
	///<summary>
	///Gets the predefined color of blue violet, or #FF8A2BE2.
	///</summary>
	public static readonly Color BlueViolet = new(4287245282u);
	///<summary>
	///Gets the predefined color of brown, or #FFA52A2A.
	///</summary>
	public static readonly Color Brown = new(4289014314u);
	///<summary>
	///Gets the predefined color of burly wood, or #FFDEB887.
	///</summary>
	public static readonly Color BurlyWood = new(4292786311u);
	///<summary>
	///Gets the predefined color of cadet blue, or #FF5F9EA0.
	///</summary>
	public static readonly Color CadetBlue = new(4284456608u);
	///<summary>
	///Gets the predefined color of chartreuse, or #FF7FFF00.
	///</summary>
	public static readonly Color Chartreuse = new(4286578432u);
	///<summary>
	///Gets the predefined color of chocolate, or #FFD2691E.
	///</summary>
	public static readonly Color Chocolate = new(4291979550u);
	///<summary>
	///Gets the predefined color of coral, or #FFFF7F50.
	///</summary>
	public static readonly Color Coral = new(4294934352u);
	///<summary>
	///Gets the predefined color of cornflower blue, or #FF6495ED.
	///</summary>
	public static readonly Color CornflowerBlue = new(4284782061u);
	///<summary>
	///Gets the predefined color of cornsilk, or #FFFFF8DC.
	///</summary>
	public static readonly Color Cornsilk = new(4294965468u);
	///<summary>
	///Gets the predefined color of crimson, or #FFDC143C.
	///</summary>
	public static readonly Color Crimson = new(4292613180u);
	///<summary>
	///Gets the predefined color of cyan, or #FF00FFFF.
	///</summary>
	public static readonly Color Cyan = new(4278255615u);
	///<summary>
	///Gets the predefined color of dark blue, or #FF00008B.
	///</summary>
	public static readonly Color DarkBlue = new(4278190219u);
	///<summary>
	///Gets the predefined color of dark cyan, or #FF008B8B.
	///</summary>
	public static readonly Color DarkCyan = new(4278225803u);
	///<summary>
	///Gets the predefined color of dark goldenrod, or #FFB8860B.
	///</summary>
	public static readonly Color DarkGoldenrod = new(4290283019u);
	///<summary>
	///Gets the predefined color of dark gray, or #FFA9A9A9.
	///</summary>
	public static readonly Color DarkGray = new(4289309097u);
	///<summary>
	///Gets the predefined color of dark green, or #FF006400.
	///</summary>
	public static readonly Color DarkGreen = new(4278215680u);
	///<summary>
	///Gets the predefined color of dark khaki, or #FFBDB76B.
	///</summary>
	public static readonly Color DarkKhaki = new(4290623339u);
	///<summary>
	///Gets the predefined color of dark magenta, or #FF8B008B.
	///</summary>
	public static readonly Color DarkMagenta = new(4287299723u);
	///<summary>
	///Gets the predefined color of dark olive green, or #FF556B2F.
	///</summary>
	public static readonly Color DarkOliveGreen = new(4283788079u);
	///<summary>
	///Gets the predefined color of dark orange, or #FFFF8C00.
	///</summary>
	public static readonly Color DarkOrange = new(4294937600u);
	///<summary>
	///Gets the predefined color of dark orchid, or #FF9932CC.
	///</summary>
	public static readonly Color DarkOrchid = new(4288230092u);
	///<summary>
	///Gets the predefined color of dark red, or #FF8B0000.
	///</summary>
	public static readonly Color DarkRed = new(4287299584u);
	///<summary>
	///Gets the predefined color of dark salmon, or #FFE9967A.
	///</summary>
	public static readonly Color DarkSalmon = new(4293498490u);
	///<summary>
	///Gets the predefined color of dark sea green, or #FF8FBC8B.
	///</summary>
	public static readonly Color DarkSeaGreen = new(4287609995u);
	///<summary>
	///Gets the predefined color of dark slate blue, or #FF483D8B.
	///</summary>
	public static readonly Color DarkSlateBlue = new(4282924427u);
	///<summary>
	///Gets the predefined color of dark slate gray, or #FF2F4F4F.
	///</summary>
	public static readonly Color DarkSlateGray = new(4281290575u);
	///<summary>
	///Gets the predefined color of dark turquoise, or #FF00CED1.
	///</summary>
	public static readonly Color DarkTurquoise = new(4278243025u);
	///<summary>
	///Gets the predefined color of dark violet, or #FF9400D3.
	///</summary>
	public static readonly Color DarkViolet = new(4287889619u);
	///<summary>
	///Gets the predefined color of deep pink, or #FFFF1493.
	///</summary>
	public static readonly Color DeepPink = new(4294907027u);
	///<summary>
	///Gets the predefined color of deep sky blue, or #FF00BFFF.
	///</summary>
	public static readonly Color DeepSkyBlue = new(4278239231u);
	///<summary>
	///Gets the predefined color of dim gray, or #FF696969.
	///</summary>
	public static readonly Color DimGray = new(4285098345u);
	///<summary>
	///Gets the predefined color of dodger blue, or #FF1E90FF.
	///</summary>
	public static readonly Color DodgerBlue = new(4280193279u);
	///<summary>
	///Gets the predefined color of firebrick, or #FFB22222.
	///</summary>
	public static readonly Color Firebrick = new(4289864226u);
	///<summary>
	///Gets the predefined color of floral white, or #FFFFFAF0.
	///</summary>
	public static readonly Color FloralWhite = new(4294966000u);
	///<summary>
	///Gets the predefined color of forest green, or #FF228B22.
	///</summary>
	public static readonly Color ForestGreen = new(4280453922u);
	///<summary>
	///Gets the predefined color of fuchsia, or #FFFF00FF.
	///</summary>
	public static readonly Color Fuchsia = new(4294902015u);
	///<summary>
	///Gets the predefined color of gainsboro, or #FFDCDCDC.
	///</summary>
	public static readonly Color Gainsboro = new(4292664540u);
	///<summary>
	///Gets the predefined color of ghost white, or #FFF8F8FF.
	///</summary>
	public static readonly Color GhostWhite = new(4294506751u);
	///<summary>
	///Gets the predefined color of gold, or #FFFFD700.
	///</summary>
	public static readonly Color Gold = new(4294956800u);
	///<summary>
	///Gets the predefined color of goldenrod, or #FFDAA520.
	///</summary>
	public static readonly Color Goldenrod = new(4292519200u);
	///<summary>
	///Gets the predefined color of gray, or #FF808080.
	///</summary>
	public static readonly Color Gray = new(4286611584u);
	///<summary>
	///Gets the predefined color of grey, or #FF808080.
	///</summary>
	public static readonly Color Grey = new(4286611584u);
	///<summary>
	///Gets the predefined color of green, or #FF008000.
	///</summary>
	public static readonly Color Green = new(4278222848u);
	///<summary>
	///Gets the predefined color of green yellow, or #FFADFF2F.
	///</summary>
	public static readonly Color GreenYellow = new(4289593135u);
	///<summary>
	///Gets the predefined color of honeydew, or #FFF0FFF0.
	///</summary>
	public static readonly Color Honeydew = new(4293984240u);
	///<summary>
	///Gets the predefined color of hot pink, or #FFFF69B4.
	///</summary>
	public static readonly Color HotPink = new(4294928820u);
	///<summary>
	///Gets the predefined color of indian red, or #FFCD5C5C.
	///</summary>
	public static readonly Color IndianRed = new(4291648604u);
	///<summary>
	///Gets the predefined color of indigo, or #FF4B0082.
	///</summary>
	public static readonly Color Indigo = new(4283105410u);
	///<summary>
	///Gets the predefined color of ivory, or #FFFFFFF0.
	///</summary>
	public static readonly Color Ivory = new(4294967280u);
	///<summary>
	///Gets the predefined color of khaki, or #FFF0E68C.
	///</summary>
	public static readonly Color Khaki = new(4293977740u);
	///<summary>
	///Gets the predefined color of lavender, or #FFE6E6FA.
	///</summary>
	public static readonly Color Lavender = new(4293322490u);
	///<summary>
	///Gets the predefined color of lavender blush, or #FFFFF0F5.
	///</summary>
	public static readonly Color LavenderBlush = new(4294963445u);
	///<summary>
	///Gets the predefined color of lawn green, or #FF7CFC00.
	///</summary>
	public static readonly Color LawnGreen = new(4286381056u);
	///<summary>
	///Gets the predefined color of lemon chiffon, or #FFFFFACD.
	///</summary>
	public static readonly Color LemonChiffon = new(4294965965u);
	///<summary>
	///Gets the predefined color of light blue, or #FFADD8E6.
	///</summary>
	public static readonly Color LightBlue = new(4289583334u);
	///<summary>
	///Gets the predefined color of light coral, or #FFF08080.
	///</summary>
	public static readonly Color LightCoral = new(4293951616u);
	///<summary>
	///Gets the predefined color of light cyan, or #FFE0FFFF.
	///</summary>
	public static readonly Color LightCyan = new(4292935679u);
	///<summary>
	///Gets the predefined color of light goldenrod yellow, or #FFFAFAD2.
	///</summary>
	public static readonly Color LightGoldenrodYellow = new(4294638290u);
	///<summary>
	///Gets the predefined color of light gray, or #FFD3D3D3.
	///</summary>
	public static readonly Color LightGray = new(4292072403u);
	///<summary>
	///Gets the predefined color of light green, or #FF90EE90.
	///</summary>
	public static readonly Color LightGreen = new(4287688336u);
	///<summary>
	///Gets the predefined color of light pink, or #FFFFB6C1.
	///</summary>
	public static readonly Color LightPink = new(4294948545u);
	///<summary>
	///Gets the predefined color of light salmon, or #FFFFA07A.
	///</summary>
	public static readonly Color LightSalmon = new(4294942842u);
	///<summary>
	///Gets the predefined color of light sea green, or #FF20B2AA.
	///</summary>
	public static readonly Color LightSeaGreen = new(4280332970u);
	///<summary>
	///Gets the predefined color of light sky blue, or #FF87CEFA.
	///</summary>
	public static readonly Color LightSkyBlue = new(4287090426u);
	///<summary>
	///Gets the predefined color of light slate gray, or #FF778899.
	///</summary>
	public static readonly Color LightSlateGray = new(4286023833u);
	///<summary>
	///Gets the predefined color of light steel blue, or #FFB0C4DE.
	///</summary>
	public static readonly Color LightSteelBlue = new(4289774814u);
	///<summary>
	///Gets the predefined color of light yellow, or #FFFFFFE0.
	///</summary>
	public static readonly Color LightYellow = new(4294967264u);
	///<summary>
	///Gets the predefined color of lime, or #FF00FF00.
	///</summary>
	public static readonly Color Lime = new(4278255360u);
	///<summary>
	///Gets the predefined color of lime green, or #FF32CD32.
	///</summary>
	public static readonly Color LimeGreen = new(4281519410u);
	///<summary>
	///Gets the predefined color of linen, or #FFFAF0E6.
	///</summary>
	public static readonly Color Linen = new(4294635750u);
	///<summary>
	///Gets the predefined color of magenta, or #FFFF00FF.
	///</summary>
	public static readonly Color Magenta = new(4294902015u);
	///<summary>
	///Gets the predefined color of maroon, or #FF800000.
	///</summary>
	public static readonly Color Maroon = new(4286578688u);
	///<summary>
	///Gets the predefined color of medium aquamarine, or #FF66CDAA.
	///</summary>
	public static readonly Color MediumAquamarine = new(4284927402u);
	///<summary>
	///Gets the predefined color of medium blue, or #FF0000CD.
	///</summary>
	public static readonly Color MediumBlue = new(4278190285u);
	///<summary>
	///Gets the predefined color of medium orchid, or #FFBA55D3.
	///</summary>
	public static readonly Color MediumOrchid = new(4290401747u);
	///<summary>
	///Gets the predefined color of medium purple, or #FF9370DB.
	///</summary>
	public static readonly Color MediumPurple = new(4287852763u);
	///<summary>
	///Gets the predefined color of medium sea green, or #FF3CB371.
	///</summary>
	public static readonly Color MediumSeaGreen = new(4282168177u);
	///<summary>
	///Gets the predefined color of medium slate blue, or #FF7B68EE.
	///</summary>
	public static readonly Color MediumSlateBlue = new(4286277870u);
	///<summary>
	///Gets the predefined color of medium spring green, or #FF00FA9A.
	///</summary>
	public static readonly Color MediumSpringGreen = new(4278254234u);
	///<summary>
	///Gets the predefined color of medium turquoise, or #FF48D1CC.
	///</summary>
	public static readonly Color MediumTurquoise = new(4282962380u);
	///<summary>
	///Gets the predefined color of medium violet red, or #FFC71585.
	///</summary>
	public static readonly Color MediumVioletRed = new(4291237253u);
	///<summary>
	///Gets the predefined color of midnight blue, or #FF191970.
	///</summary>
	public static readonly Color MidnightBlue = new(4279834992u);
	///<summary>
	///Gets the predefined color of mint cream, or #FFF5FFFA.
	///</summary>
	public static readonly Color MintCream = new(4294311930u);
	///<summary>
	///Gets the predefined color of misty rose, or #FFFFE4E1.
	///</summary>
	public static readonly Color MistyRose = new(4294960353u);
	///<summary>
	///Gets the predefined color of moccasin, or #FFFFE4B5.
	///</summary>
	public static readonly Color Moccasin = new(4294960309u);
	///<summary>
	///Gets the predefined color of navajo white, or #FFFFDEAD.
	///</summary>
	public static readonly Color NavajoWhite = new(4294958765u);
	///<summary>
	///Gets the predefined color of navy, or #FF000080.
	///</summary>
	public static readonly Color Navy = new(4278190208u);
	///<summary>
	///Gets the predefined color of old lace, or #FFFDF5E6.
	///</summary>
	public static readonly Color OldLace = new(4294833638u);
	///<summary>
	///Gets the predefined color of olive, or #FF808000.
	///</summary>
	public static readonly Color Olive = new(4286611456u);
	///<summary>
	///Gets the predefined color of olive drab, or #FF6B8E23.
	///</summary>
	public static readonly Color OliveDrab = new(4285238819u);
	///<summary>
	///Gets the predefined color of orange, or #FFFFA500.
	///</summary>
	public static readonly Color Orange = new(4294944000u);
	///<summary>
	///Gets the predefined color of orange red, or #FFFF4500.
	///</summary>
	public static readonly Color OrangeRed = new(4294919424u);
	///<summary>
	///Gets the predefined color of orchid, or #FFDA70D6.
	///</summary>
	public static readonly Color Orchid = new(4292505814u);
	///<summary>
	///Gets the predefined color of pale goldenrod, or #FFEEE8AA.
	///</summary>
	public static readonly Color PaleGoldenrod = new(4293847210u);
	///<summary>
	///Gets the predefined color of pale green, or #FF98FB98.
	///</summary>
	public static readonly Color PaleGreen = new(4288215960u);
	///<summary>
	///Gets the predefined color of pale turquoise, or #FFAFEEEE.
	///</summary>
	public static readonly Color PaleTurquoise = new(4289720046u);
	///<summary>
	///Gets the predefined color of pale violet red, or #FFDB7093.
	///</summary>
	public static readonly Color PaleVioletRed = new(4292571283u);
	///<summary>
	///Gets the predefined color of papaya whip, or #FFFFEFD5.
	///</summary>
	public static readonly Color PapayaWhip = new(4294963157u);
	///<summary>
	///Gets the predefined color of peach puff, or #FFFFDAB9.
	///</summary>
	public static readonly Color PeachPuff = new(4294957753u);
	///<summary>
	///Gets the predefined color of peru, or #FFCD853F.
	///</summary>
	public static readonly Color Peru = new(4291659071u);
	///<summary>
	///Gets the predefined color of pink, or #FFFFC0CB.
	///</summary>
	public static readonly Color Pink = new(4294951115u);
	///<summary>
	///Gets the predefined color of plum, or #FFDDA0DD.
	///</summary>
	public static readonly Color Plum = new(4292714717u);
	///<summary>
	///Gets the predefined color of powder blue, or #FFB0E0E6.
	///</summary>
	public static readonly Color PowderBlue = new(4289781990u);
	///<summary>
	///Gets the predefined color of purple, or #FF800080.
	///</summary>
	public static readonly Color Purple = new(4286578816u);
	///<summary>
	///Gets the predefined color of red, or #FFFF0000.
	///</summary>
	public static readonly Color Red = new(4294901760u);
	///<summary>
	///Gets the predefined color of rosy brown, or #FFBC8F8F.
	///</summary>
	public static readonly Color RosyBrown = new(4290547599u);
	///<summary>
	///Gets the predefined color of royal blue, or #FF4169E1.
	///</summary>
	public static readonly Color RoyalBlue = new(4282477025u);
	///<summary>
	///Gets the predefined color of saddle brown, or #FF8B4513.
	///</summary>
	public static readonly Color SaddleBrown = new(4287317267u);
	///<summary>
	///Gets the predefined color of salmon, or #FFFA8072.
	///</summary>
	public static readonly Color Salmon = new(4294606962u);
	///<summary>
	///Gets the predefined color of sandy brown, or #FFF4A460.
	///</summary>
	public static readonly Color SandyBrown = new(4294222944u);
	///<summary>
	///Gets the predefined color of sea green, or #FF2E8B57.
	///</summary>
	public static readonly Color SeaGreen = new(4281240407u);
	///<summary>
	///Gets the predefined color of sea shell, or #FFFFF5EE.
	///</summary>
	public static readonly Color SeaShell = new(4294964718u);
	///<summary>
	///Gets the predefined color of sienna, or #FFA0522D.
	///</summary>
	public static readonly Color Sienna = new(4288696877u);
	///<summary>
	///Gets the predefined color of silver, or #FFC0C0C0.
	///</summary>
	public static readonly Color Silver = new(4290822336u);
	///<summary>
	///Gets the predefined color of sky blue, or #FF87CEEB.
	///</summary>
	public static readonly Color SkyBlue = new(4287090411u);
	///<summary>
	///Gets the predefined color of slate blue, or #FF6A5ACD.
	///</summary>
	public static readonly Color SlateBlue = new(4285160141u);
	///<summary>
	///Gets the predefined color of slate gray, or #FF708090.
	///</summary>
	public static readonly Color SlateGray = new(4285563024u);
	///<summary>
	///Gets the predefined color of snow, or #FFFFFAFA.
	///</summary>
	public static readonly Color Snow = new(4294966010u);
	///<summary>
	///Gets the predefined color of spring green, or #FF00FF7F.
	///</summary>
	public static readonly Color SpringGreen = new(4278255487u);
	///<summary>
	///Gets the predefined color of steel blue, or #FF4682B4.
	///</summary>
	public static readonly Color SteelBlue = new(4282811060u);
	///<summary>
	///Gets the predefined color of tan, or #FFD2B48C.
	///</summary>
	public static readonly Color Tan = new(4291998860u);
	///<summary>
	///Gets the predefined color of teal, or #FF008080.
	///</summary>
	public static readonly Color Teal = new(4278222976u);
	///<summary>
	///Gets the predefined color of thistle, or #FFD8BFD8.
	///</summary>
	public static readonly Color Thistle = new(4292394968u);
	///<summary>
	///Gets the predefined color of tomato, or #FFFF6347.
	///</summary>
	public static readonly Color Tomato = new(4294927175u);
	///<summary>
	///Gets the predefined color of turquoise, or #FF40E0D0.
	///</summary>
	public static readonly Color Turquoise = new(4282441936u);
	///<summary>
	///Gets the predefined color of violet, or #FFEE82EE.
	///</summary>
	public static readonly Color Violet = new(4293821166u);
	///<summary>
	///Gets the predefined color of wheat, or #FFF5DEB3.
	///</summary>
	public static readonly Color Wheat = new(4294303411u);
	///<summary>
	///Gets the predefined color of white, or #FFFFFFFF.
	///</summary>
	public static readonly Color White = new(uint.MaxValue);
	///<summary>
	///Gets the predefined color of white smoke, or #FFF5F5F5.
	///</summary>
	public static readonly Color WhiteSmoke = new(4294309365u);
	///<summary>
	///Gets the predefined color of yellow, or #FFFFFF00.
	///</summary>
	public static readonly Color Yellow = new(4294967040u);
	///<summary>
	///Gets the predefined color of yellow green, or #FF9ACD32.
	///</summary>
	public static readonly Color YellowGreen = new(4288335154u);
	///<summary>
	///Gets the predefined color of white transparent, or #00FFFFFF.
	///</summary>
	public static readonly Color Transparent = new(16777215u);
	///<summary>
	///Gets the predefined empty color (black transparent), or #00000000.
	///</summary>
	public static readonly Color Empty = new(0u);
}