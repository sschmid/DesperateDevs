namespace DesperateDevs.CLI.Utils
{
    // More Styles: https://raw.githubusercontent.com/sindresorhus/cli-spinners/master/spinners.json
    // Preview: https://cdn.rawgit.com/sindresorhus/cli-spinners/dcac74b75e52d4d9fe980e6fce98c2814275739e/screenshot.svg

    public class SpinnerStyle
    {
        public int interval;
        public string[] frames;
    }

    public static class SpinnerStyles
    {
        public static SpinnerStyle magnifyingGlass => new SpinnerStyle {
            interval = 100,
            frames = new[] {
                "🔍    ",
                " 🔍   ",
                "  🔍  ",
                "   🔍 ",
                "  🔍  ",
                " 🔍   "
            }
        };

        public static SpinnerStyle bouncingBar => new SpinnerStyle {
            interval = 80,
            frames = new[] {
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

        public static SpinnerStyle bouncingBall => new SpinnerStyle {
            interval = 80,
            frames = new[] {
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

        public static SpinnerStyle clock => new SpinnerStyle {
            interval = 100,
            frames = new[] {
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

        public static SpinnerStyle magicCat => new SpinnerStyle {
            interval = 120,
            frames = new[] {
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
