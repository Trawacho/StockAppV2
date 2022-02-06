namespace StockApp.Core.Wettbewerb.Teambewerb
{

    public interface IPlayer
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string LicenseNumber { get; set; }
    }


    /// <summary>
    /// Spieler im TeamBewerb
    /// </summary>
    public class Player : TBasePlayer, IPlayer
    {

        #region Constructors

        private Player()
        {
            LicenseNumber = string.Empty;
        }


        #endregion

        public static IPlayer Create(string lastName, string firstName)
        {
            return new Player()
            {
                LastName = lastName,
                FirstName = firstName
            };
        }

        public static IPlayer Create() => Create("lastname", "firstname");
    }
}
