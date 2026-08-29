using System;
using System.Collections.Generic;
using SixLabors.ImageSharp.PixelFormats;

namespace GetMajorColors.Services;

/// <summary>
/// Resolves colors to the closest CSS Color Module Level 4 name.
/// </summary>
public sealed class CssColorNameResolver : IColorNameResolver
{
    private static readonly IReadOnlyDictionary<string, Rgb24> NamedColors = new Dictionary<string, Rgb24>(StringComparer.OrdinalIgnoreCase)
    {
        ["AliceBlue"] = new(240, 248, 255),
        ["AntiqueWhite"] = new(250, 235, 215),
        ["Aqua"] = new(0, 255, 255),
        ["Aquamarine"] = new(127, 255, 212),
        ["Azure"] = new(240, 255, 255),
        ["Beige"] = new(245, 245, 220),
        ["Bisque"] = new(255, 228, 196),
        ["Black"] = new(0, 0, 0),
        ["BlanchedAlmond"] = new(255, 235, 205),
        ["Blue"] = new(0, 0, 255),
        ["BlueViolet"] = new(138, 43, 226),
        ["Brown"] = new(165, 42, 42),
        ["BurlyWood"] = new(222, 184, 135),
        ["CadetBlue"] = new(95, 158, 160),
        ["Chartreuse"] = new(127, 255, 0),
        ["Chocolate"] = new(210, 105, 30),
        ["Coral"] = new(255, 127, 80),
        ["CornflowerBlue"] = new(100, 149, 237),
        ["Cornsilk"] = new(255, 248, 220),
        ["Crimson"] = new(220, 20, 60),
        ["Cyan"] = new(0, 255, 255),
        ["DarkBlue"] = new(0, 0, 139),
        ["DarkCyan"] = new(0, 139, 139),
        ["DarkGoldenRod"] = new(184, 134, 11),
        ["DarkGray"] = new(169, 169, 169),
        ["DarkGrey"] = new(169, 169, 169),
        ["DarkGreen"] = new(0, 100, 0),
        ["DarkKhaki"] = new(189, 183, 107),
        ["DarkMagenta"] = new(139, 0, 139),
        ["DarkOliveGreen"] = new(85, 107, 47),
        ["DarkOrange"] = new(255, 140, 0),
        ["DarkOrchid"] = new(153, 50, 204),
        ["DarkRed"] = new(139, 0, 0),
        ["DarkSalmon"] = new(233, 150, 122),
        ["DarkSeaGreen"] = new(143, 188, 143),
        ["DarkSlateBlue"] = new(72, 61, 139),
        ["DarkSlateGray"] = new(47, 79, 79),
        ["DarkSlateGrey"] = new(47, 79, 79),
        ["DarkTurquoise"] = new(0, 206, 209),
        ["DarkViolet"] = new(148, 0, 211),
        ["DeepPink"] = new(255, 20, 147),
        ["DeepSkyBlue"] = new(0, 191, 255),
        ["DimGray"] = new(105, 105, 105),
        ["DimGrey"] = new(105, 105, 105),
        ["DodgerBlue"] = new(30, 144, 255),
        ["FireBrick"] = new(178, 34, 34),
        ["FloralWhite"] = new(255, 250, 240),
        ["ForestGreen"] = new(34, 139, 34),
        ["Fuchsia"] = new(255, 0, 255),
        ["Gainsboro"] = new(220, 220, 220),
        ["GhostWhite"] = new(248, 248, 255),
        ["Gold"] = new(255, 215, 0),
        ["GoldenRod"] = new(218, 165, 32),
        ["Gray"] = new(128, 128, 128),
        ["Grey"] = new(128, 128, 128),
        ["Green"] = new(0, 128, 0),
        ["GreenYellow"] = new(173, 255, 47),
        ["HoneyDew"] = new(240, 255, 240),
        ["HotPink"] = new(255, 105, 180),
        ["IndianRed"] = new(205, 92, 92),
        ["Indigo"] = new(75, 0, 130),
        ["Ivory"] = new(255, 255, 240),
        ["Khaki"] = new(240, 230, 140),
        ["Lavender"] = new(230, 230, 250),
        ["LavenderBlush"] = new(255, 240, 245),
        ["LawnGreen"] = new(124, 252, 0),
        ["LemonChiffon"] = new(255, 250, 205),
        ["LightBlue"] = new(173, 216, 230),
        ["LightCoral"] = new(240, 128, 128),
        ["LightCyan"] = new(224, 255, 255),
        ["LightGoldenRodYellow"] = new(250, 250, 210),
        ["LightGray"] = new(211, 211, 211),
        ["LightGrey"] = new(211, 211, 211),
        ["LightGreen"] = new(144, 238, 144),
        ["LightPink"] = new(255, 182, 193),
        ["LightSalmon"] = new(255, 160, 122),
        ["LightSeaGreen"] = new(32, 178, 170),
        ["LightSkyBlue"] = new(135, 206, 250),
        ["LightSlateGray"] = new(119, 136, 153),
        ["LightSlateGrey"] = new(119, 136, 153),
        ["LightSteelBlue"] = new(176, 196, 222),
        ["LightYellow"] = new(255, 255, 224),
        ["Lime"] = new(0, 255, 0),
        ["LimeGreen"] = new(50, 205, 50),
        ["Linen"] = new(250, 240, 230),
        ["Magenta"] = new(255, 0, 255),
        ["Maroon"] = new(128, 0, 0),
        ["MediumAquaMarine"] = new(102, 205, 170),
        ["MediumBlue"] = new(0, 0, 205),
        ["MediumOrchid"] = new(186, 85, 211),
        ["MediumPurple"] = new(147, 112, 219),
        ["MediumSeaGreen"] = new(60, 179, 113),
        ["MediumSlateBlue"] = new(123, 104, 238),
        ["MediumSpringGreen"] = new(0, 250, 154),
        ["MediumTurquoise"] = new(72, 209, 204),
        ["MediumVioletRed"] = new(199, 21, 133),
        ["MidnightBlue"] = new(25, 25, 112),
        ["MintCream"] = new(245, 255, 250),
        ["MistyRose"] = new(255, 228, 225),
        ["Moccasin"] = new(255, 228, 181),
        ["NavajoWhite"] = new(255, 222, 173),
        ["Navy"] = new(0, 0, 128),
        ["OldLace"] = new(253, 245, 230),
        ["Olive"] = new(128, 128, 0),
        ["OliveDrab"] = new(107, 142, 35),
        ["Orange"] = new(255, 165, 0),
        ["OrangeRed"] = new(255, 69, 0),
        ["Orchid"] = new(218, 112, 214),
        ["PaleGoldenRod"] = new(238, 232, 170),
        ["PaleGreen"] = new(152, 251, 152),
        ["PaleTurquoise"] = new(175, 238, 238),
        ["PaleVioletRed"] = new(219, 112, 147),
        ["PapayaWhip"] = new(255, 239, 213),
        ["PeachPuff"] = new(255, 218, 185),
        ["Peru"] = new(205, 133, 63),
        ["Pink"] = new(255, 192, 203),
        ["Plum"] = new(221, 160, 221),
        ["PowderBlue"] = new(176, 224, 230),
        ["Purple"] = new(128, 0, 128),
        ["RebeccaPurple"] = new(102, 51, 153),
        ["Red"] = new(255, 0, 0),
        ["RosyBrown"] = new(188, 143, 143),
        ["RoyalBlue"] = new(65, 105, 225),
        ["SaddleBrown"] = new(139, 69, 19),
        ["Salmon"] = new(250, 128, 114),
        ["SandyBrown"] = new(244, 164, 96),
        ["SeaGreen"] = new(46, 139, 87),
        ["SeaShell"] = new(255, 245, 238),
        ["Sienna"] = new(160, 82, 45),
        ["Silver"] = new(192, 192, 192),
        ["SkyBlue"] = new(135, 206, 235),
        ["SlateBlue"] = new(106, 90, 205),
        ["SlateGray"] = new(112, 128, 144),
        ["SlateGrey"] = new(112, 128, 144),
        ["Snow"] = new(255, 250, 250),
        ["SpringGreen"] = new(0, 255, 127),
        ["SteelBlue"] = new(70, 130, 180),
        ["Tan"] = new(210, 180, 140),
        ["Teal"] = new(0, 128, 128),
        ["Thistle"] = new(216, 191, 216),
        ["Tomato"] = new(255, 99, 71),
        ["Turquoise"] = new(64, 224, 208),
        ["Violet"] = new(238, 130, 238),
        ["Wheat"] = new(245, 222, 179),
        ["White"] = new(255, 255, 255),
        ["WhiteSmoke"] = new(245, 245, 245),
        ["Yellow"] = new(255, 255, 0),
        ["YellowGreen"] = new(154, 205, 50),
    };

    /// <inheritdoc />
    public string GetName(Rgb24 pixel)
    {
        string bestName = "Unknown";
        double bestDistance = double.MaxValue;

        foreach (var (name, color) in NamedColors)
        {
            double distance = EuclideanDistance(pixel, color);
            if (distance < bestDistance)
            {
                bestDistance = distance;
                bestName = name;
            }
        }

        return bestName;
    }

    private static double EuclideanDistance(Rgb24 a, Rgb24 b)
    {
        double dr = a.R - b.R;
        double dg = a.G - b.G;
        double db = a.B - b.B;
        return (dr * dr) + (dg * dg) + (db * db);
    }
}
