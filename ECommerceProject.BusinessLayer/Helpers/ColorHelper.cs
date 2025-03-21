using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceProject.BusinessLayer.Helpers
{
    public static class ColorHelper
    {
        public static string GetNearestColor(string hex)
        {
            var mainColors = new Dictionary<string, (int R, int G, int B)>
        {
            // Kırmızı tonları
            { "Red", (255, 0, 0) },
            { "Dark Red", (139, 0, 0) },
            { "Light Red", (255, 102, 102) },

            // Yeşil tonları
            { "Green", (0, 255, 0) },
            { "Dark Green", (0, 100, 0) },
            { "Light Green", (144, 238, 144) },

            // Mavi tonları
            { "Blue", (0, 0, 255) },
            { "Dark Blue", (0, 0, 139) },
            { "Light Blue", (173, 216, 230) },

            // Sarı & Turuncu tonları
            { "Yellow", (255, 255, 0) },
            { "Gold", (255, 215, 0) },
            { "Orange", (255, 165, 0) },

            // Diğer renkler
            { "Purple", (128, 0, 128) },
            { "Pink", (255, 192, 203) },
            { "Brown", (139, 69, 19) },
            { "Gray", (128, 128, 128) },
            { "Black", (0, 0, 0) },
            { "White", (255, 255, 255) }
        };

            // HEX → RGB çevir
            (int r, int g, int b) = HexToRgb(hex);

            // En yakın rengi bul
            var nearest = mainColors
                .Select(c => new { Color = c.Key, Distance = ColorDistance(r, g, b, c.Value.R, c.Value.G, c.Value.B) })
                .OrderBy(c => c.Distance)
                .First();

            // Eğer "Dark Red", "Light Red" gibi çıktı alıyorsak, bunları "Red" olarak döndür
            return SimplifyColor(nearest.Color);
        }

        static (int, int, int) HexToRgb(string hex)
        {
            hex = hex.TrimStart('#');
            return (
                Convert.ToInt32(hex.Substring(0, 2), 16),
                Convert.ToInt32(hex.Substring(2, 2), 16),
                Convert.ToInt32(hex.Substring(4, 2), 16)
            );
        }

        static double ColorDistance(int r1, int g1, int b1, int r2, int g2, int b2)
        {
            return Math.Sqrt(Math.Pow(r2 - r1, 2) + Math.Pow(g2 - g1, 2) + Math.Pow(b2 - b1, 2));
        }

        static string SimplifyColor(string color)
        {
            if (color.Contains("Red")) return "Red";
            if (color.Contains("Green")) return "Green";
            if (color.Contains("Blue")) return "Blue";
            if (color.Contains("Yellow") || color.Contains("Gold")) return "Yellow";
            if (color.Contains("Orange")) return "Orange";
            if (color.Contains("Purple")) return "Purple";
            if (color.Contains("Pink")) return "Pink";
            if (color.Contains("Brown")) return "Brown";
            if (color.Contains("Gray")) return "Gray";
            if (color.Contains("Black")) return "Black";
            if (color.Contains("White")) return "White";

            return color; // Varsayılan olarak kendi ismiyle döndür
        }
    }
}
