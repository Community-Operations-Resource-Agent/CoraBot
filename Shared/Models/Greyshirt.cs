namespace Shared.Models
{
    public class Greyshirt : User
    {
        public int GreyshirtNumber { get; set; }

        public Greyshirt() : base()
        {
            this.IsGreyshirt = false;
        }

        public bool IsRegistered()
        {
            return this.GreyshirtNumber != 0;
        }
    }
}
