namespace DesperateDevs.Cli.Utils
{
    // More Styles: https://raw.githubusercontent.com/sindresorhus/cli-spinners/master/spinners.json
    // Preview: https://cdn.rawgit.com/sindresorhus/cli-spinners/dcac74b75e52d4d9fe980e6fce98c2814275739e/screenshot.svg

    public class SpinnerStyle
    {
        public int Interval;
        public string[] Frames;
    }

    public static class SpinnerStyles
    {
        public static SpinnerStyle MagnifyingGlass => new SpinnerStyle
        {
            Interval = 100,
            Frames = new[]
            {
                "🔍    ",
                " 🔍   ",
                "  🔍  ",
                "   🔍 ",
                "  🔍  ",
                " 🔍   "
            }
        };

        public static SpinnerStyle BouncingBar => new SpinnerStyle
        {
            Interval = 80,
            Frames = new[]
            {
                "[    ]",
                "[=   ]",
                "[==  ]",
                "[=== ]",
                "[ ===]",
                "[  ==]",
                "[   =]",
                "[    ]",
                "[   =]",
                "[  ==]",
                "[ ===]",
                "[====]",
                "[=== ]",
                "[==  ]",
                "[=   ]"
            }
        };

        public static SpinnerStyle BouncingBall => new SpinnerStyle
        {
            Interval = 80,
            Frames = new[]
            {
                "( ●    )",
                "(  ●   )",
                "(   ●  )",
                "(    ● )",
                "(     ●)",
                "(    ● )",
                "(   ●  )",
                "(  ●   )",
                "( ●    )",
                "(●     )"
            }
        };

        public static SpinnerStyle Clock => new SpinnerStyle
        {
            Interval = 100,
            Frames = new[]
            {
                "🕛 ",
                "🕐 ",
                "🕑 ",
                "🕒 ",
                "🕓 ",
                "🕔 ",
                "🕕 ",
                "🕖 ",
                "🕗 ",
                "🕘 ",
                "🕙 ",
                "🕚 "
            }
        };

        public static SpinnerStyle MagicCat => new SpinnerStyle
        {
            Interval = 120,
            Frames = new[]
            {
                "   ∧＿∧\n" +
                " （｡･ω･｡)つ━☆        \n" +
                " ⊂　  ノ             \n" +
                " しーＪ               \n",

                "   ∧＿∧\n" +
                " （｡･ω･｡)つ━☆・       \n" +
                " ⊂　  ノ             \n" +
                " しーＪ               \n",

                "   ∧＿∧\n" +
                " （｡^ω^｡)つ━☆・*      \n" +
                " ⊂　  ノ             \n" +
                " しーＪ               \n",

                "   ∧＿∧\n" +
                " （｡^ω^｡)つ━☆・*      \n" +
                " ⊂　  ノ      ・゜     \n" +
                " しーＪ               \n",

                "   ∧＿∧\n" +
                " （｡^ω^｡)つ━☆・*      \n" +
                " ⊂　  ノ      ・゜+.   \n" +
                " しーＪ               \n",

                "   ∧＿∧\n" +
                " （｡･ω･｡)つ━☆・*       \n" +
                " ⊂　  ノ      ・゜+.   \n" +
                " しーＪ      °。  *´   \n",

                "   ∧＿∧\n" +
                " （｡･ω･｡)つ━☆・*      \n" +
                " ⊂　  ノ      ・゜+.   \n" +
                " しーＪ      °。+ *´   \n",
            }
        };
    }
}
