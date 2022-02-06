namespace StockApp.Core.Factories
{
    public static class GameFactory
    {
        /// <summary>
        /// Es wird ein Spielplan generiert, in dem zwei Vereine gegeneinander spielen, ohne dass ein Verein ein vereinsinternes Spiel hat
        /// </summary>
        /// <param name="countTeams">gerade Anzahl an Mannschaften</param>
        /// <param name="countGameRounds">Anzahl an Spielrunden</param>
        /// <param name="isStartingChange">Wechsel des Anspiels bei jeder zweiten Spielrunde</param>
        /// <returns></returns>
        public static IEnumerable<IFactoryGame> Generiere_SonderSpielPlan_1(int countTeams, int countGameRounds, bool isStartingChange)
        {
            if (countTeams % 2 != 0) yield break;

            // Der erste Verein hat gerade Startnummern (2,4,6,8,10) der andere Verein die ungeraden (1-3-5-7-9)
            // Immer ohne Aussetzer
            // Anzahl der Spiele ist immer Anzahl der Teams / 2
            // Anzahl der Bahnen ist immer Anzhal der Teams / 2

            int _countOfCourts = countTeams / 2;
            int _countOfGames = countTeams / 2;

            int _courtNumberTemp;
            int _gameNumber;

            for (int r = 0; r < countGameRounds; r++)
            {
                //Mit Spiel 1 auf Bahn 1 wird begonnen --> es folgen alle ungeraden, da immer +2 gerechnet wird
                _courtNumberTemp = 1;
                _gameNumber = 1;

                for (int x = 0; x < countTeams / 2; x++)
                {

                    for (int y = 0; y < countTeams / 2; y++)
                    {
                        int _startnumberA;
                        int _startnumberB;
                        if (r % 2 == 0)
                        {
                            _startnumberA = 1 + (y * 2);

                            _startnumberB = _startnumberA + 1 + ((x) * 2);
                            if (_startnumberB > countTeams) { _startnumberB -= countTeams; }
                        }
                        else
                        {
                            _startnumberB = 1 + (y * 2);

                            _startnumberA = _startnumberB + 1 + ((x) * 2);
                            if (_startnumberA > countTeams) { _startnumberA -= countTeams; }
                        }



                        int _courtNumber = _courtNumberTemp + y;
                        if (_courtNumber > _countOfCourts)
                        {
                            int tempBahn = _courtNumber;
                            _courtNumber = (tempBahn - _countOfCourts);
                        }

                        if (_gameNumber > _countOfGames)
                        {
                            int tempSpiel = _gameNumber;
                            _gameNumber = (tempSpiel - _countOfGames);
                        }



                        yield return new FactoryGame(
                                           r + 1,                                                            //Spielrunde
                                           (r - 1) * (countTeams - 1) + (countTeams - x),                    //SpielNummer Gesamt
                                           _gameNumber,                                                    //SpielNummer
                                           _courtNumber,                                                     //Bahnnummer
                                           _startnumberA,                                                    //Startnummer A    
                                           _startnumberB,                                                    //Startnummber B                    
                                           (x % 2 != 0),                                                     //Anspiel A        
                                           false,                                                            //Aussetzer    
                                           isStartingChange);                                                //Anspielwechesel bei Mehrfachrunden

                    }

                    _courtNumberTemp += 2;
                    _gameNumber += 2;

                    //Es wird alles auf 2 gesetzt, es folgen alle geraden, da immer +2 gerechnet wird
                    if (_courtNumberTemp > _countOfCourts) { _courtNumberTemp = 2; }
                    if (_gameNumber > _countOfGames) { _gameNumber = 2; }
                }
            }
        }

        /// <summary>
        /// Es wird ein Standard Spielplan generiert, jeder gegen jeden<p></p>
        /// 
        /// Bei einer ungeraden Anzahl von Mannschaften ist die Startnummer A die Mannschaft die den Aussetzer hat (B ist virtuell und hat eine höhere Startnummer als)<br></br>
        /// Der Aussetzer ist immer auf Bahn 0
        /// Bei gerader Anzahl an Mannschaften und zwei Aussetzer werden zwei viruelle Teams hinzugefügt. Diese haben bei 6 Startern die Startnummer 7 und 8.<br></br>
        /// 
        ///   
        /// </summary>
        /// <param name="teamsCount"></param>
        /// <param name="twoBreaks"></param>
        /// <param name="roundCount"></param>
        /// <param name="isStartingChange"></param>
        /// <returns></returns>
        public static IEnumerable<IFactoryGame> CreateGames(int teamsCount, bool twoBreaks = false, int roundCount = 1, bool isStartingChange = false)
        {
            int iBahnCor = 0;               //Korrektur-Wert für Bahn

            #region Virtual Teams, es wird immer eine Gerade Anzahl an Mannschaften benötigt
            int virtualTeamsCount = 0;

            //Bei ungerade Zahl an Teams ein virtuelles Team hinzufügen
            if (teamsCount % 2 == 1)
            {
                virtualTeamsCount = 1;
            }
            else
            {
                //Gerade Anzahl an Mannschaften
                //Entweder kein Aussetzer oder ZWEI Aussetzer
                if (twoBreaks)
                {
                    virtualTeamsCount = 2;
                }
            }
            #endregion

            int allTeamsCount = teamsCount + virtualTeamsCount;
            //Jede SpielRunde berechnen
            for (int _spielRunde = 1; _spielRunde <= roundCount; _spielRunde++)
            {
                //Über Schleifen die Spiele erstellen, Teams, Bahnen und Anspiel festlegen
                for (int i = 1; i < allTeamsCount; i++)
                {
                    #region Bahn berechnen

                    int _courtNumber = 0;

                    if (allTeamsCount <= teamsCount)    //kein Aussetzer
                    {
                        if (i <= allTeamsCount / 2)
                        {
                            _courtNumber = (allTeamsCount / 2) - i + 1;
                        }
                        else
                        {
                            _courtNumber = i - (allTeamsCount / 2) + 1;
                        }
                        iBahnCor = _courtNumber;
                    }

                    #endregion

                    yield return new FactoryGame(
                                _spielRunde,                                                            //Spielrunde
                                (_spielRunde - 1) * (allTeamsCount - 1) + (allTeamsCount - i),          //SpielNummer Gesamt
                                allTeamsCount - i,                                                      //SpielNummer
                                _courtNumber,                                                           //Bahnnummer
                                i,                                                                      //Startnummer A    
                                allTeamsCount,                                                          //Startnummber B                    
                                (i % 2 != 0),                                                           //Anspiel A        
                                allTeamsCount > teamsCount,                                             //Aussetzer    
                                isStartingChange);                                                      //Anspielwechesel bei Mehrfachrunden



                    for (int k = 1; k <= (allTeamsCount / 2 - 1); k++)
                    {
                        #region Team A festlegen

                        int _startNumberA;

                        if ((i + k) % (allTeamsCount - 1) == 0)
                        {
                            _startNumberA = allTeamsCount - 1;
                        }
                        else
                        {
                            var nrTb = (i + k) % (allTeamsCount - 1);
                            if (nrTb < 0)
                            {
                                nrTb = (allTeamsCount - 1) + nrTb;
                            }
                            _startNumberA = nrTb;
                        }

                        #endregion

                        #region Team B festlegen

                        int _startNumberB;


                        if ((i - k) % (allTeamsCount - 1) == 0)
                        {
                            _startNumberB = allTeamsCount - 1;
                        }
                        else
                        {
                            var nrTa = (i - k) % (allTeamsCount - 1);
                            if (nrTa < 0)
                            {
                                nrTa = allTeamsCount - 1 + nrTa;
                            }
                            _startNumberB = nrTa;
                        }

                        #endregion

                        #region Bahn berechnen

                        _courtNumber = 0;

                        if (_startNumberA <= teamsCount && _startNumberB <= teamsCount)  //kein Aussetzer
                        {
                            if (iBahnCor != k)
                            {
                                _courtNumber = k;
                            }
                            else
                            {
                                _courtNumber = allTeamsCount / 2;
                            }
                        }
                        #endregion

                        yield return new FactoryGame(
                                   _spielRunde,                                                             //Spielrunde
                                   (_spielRunde - 1) * (allTeamsCount - 1) + (allTeamsCount - i),           //SpielNummer Gesamt
                                   allTeamsCount - i,                                                       //SpielNummer
                                   _courtNumber,                                                            //Bahnnummer
                                   _startNumberA,                                                           //Startnummer A    
                                   _startNumberB,                                                           //Startnummber B                    
                                   (k % 2 != 0),                                                            //Anspiel A        
                                   _startNumberA > teamsCount || _startNumberB > teamsCount,                //Aussetzer    
                                   isStartingChange);                                                       //Anspielwechesel bei Mehrfachrunden
                    }
                }
            }
        }


    }

    public class FactoryGame : IFactoryGame
    {
        public FactoryGame(int roundOfGame,
                            int gameNumberOverAll,
                            int gameNumber,
                            int courtNumber,
                            int teamA,
                            int teamB,
                            bool isTeamA_Starting,
                            bool isBreakGame,
                            bool isStartingChange)
        {
            RoundOfGame = roundOfGame;
            CourtNumber = courtNumber;
            GameNumber = gameNumber;
            GameNumberOverAll = gameNumberOverAll;
            IsTeamA_Starting = isTeamA_Starting;
            TeamA = teamA;
            TeamB = teamB;
            IsBreakGame = isBreakGame;

            CheckAnspiel(isStartingChange);
        }


        public int RoundOfGame { get; }
        public int CourtNumber { get; }
        public int GameNumber { get; }
        public int GameNumberOverAll { get; }
        public bool IsTeamA_Starting { get; private set; }
        public int TeamA { get; private set; }
        public int TeamB { get; private set; }
        public bool IsBreakGame { get; }

        public string ToStringExtended()
        {
            string s = $"Round:{RoundOfGame}, GameOA#{GameNumberOverAll}, Game#:{GameNumber}, Court#:{CourtNumber} -->";
            return IsBreakGame
                ? s += $"Aussetzer A{TeamA}:B{TeamB}"
                : s += $" {TeamA}:{TeamB} - Anspiel:{(IsTeamA_Starting ? TeamA : TeamB)}";
        }


        private void CheckAnspiel(bool isStartingChange)
        {
            if (IsBreakGame) return;      //Do nothing if it is a PauseGame

            if (CourtNumber % 2 != 0)
            {
                //Spiel auf Bahn 1, 3, 5, 7,...
                if (IsTeamA_Starting)
                {
                    var t1 = TeamA;
                    var t2 = TeamB;
                    TeamA = t2;
                    TeamB = t1;
                    IsTeamA_Starting = !IsTeamA_Starting;
                }
            }
            else
            {
                //Spiel auf Bahn 2, 4, 6, 8, ...
                if (IsTeamA_Starting)
                {
                    var t1 = TeamA;
                    var t2 = TeamB;
                    TeamA = t2;
                    TeamB = t1;
                    IsTeamA_Starting = !IsTeamA_Starting;
                }
            }

            if (isStartingChange && RoundOfGame % 2 == 0)
            {
                IsTeamA_Starting = !IsTeamA_Starting;
            }
        }
    }

    public interface IFactoryGame
    {
        public int RoundOfGame { get; }
        public int CourtNumber { get; }
        public int GameNumber { get; }
        public int GameNumberOverAll { get; }
        public bool IsTeamA_Starting { get; }
        public int TeamA { get; }
        public int TeamB { get; }
        public bool IsBreakGame { get; }
        public string ToStringExtended();

    }
}
