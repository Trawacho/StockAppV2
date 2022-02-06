namespace StockApp.Core.Converter
{
    public static class GameGroupStringConverter
    {
        public static string Convert(int gameGroup)
        {
            return gameGroup switch
            {
                0 => "-",
                1 => "A",
                2 => "B",
                3 => "C",
                4 => "D",
                5 => "E",
                6 => "F",
                7 => "G",
                8 => "H",
                9 => "I",
                10 => "J",
                _ => string.Empty,
            };
        }

        public static int Convert(string gameGroup)
        {
            return gameGroup switch
            {
                "A" => 1,
                "B" => 2,
                "C" => 3,
                "D" => 4,
                "E" => 5,
                "F" => 6,
                "G" => 7,
                "H" => 8,
                "I" => 9,
                "J" => 10,
                _ => 0
            };
        }
    }
}
