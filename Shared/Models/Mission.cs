namespace Shared.Models
{
    public class Mission : Model
    {
        public string CreatedById { get; set; }

        public string AssignedToId { get; set; }

        public string Description { get; set; }
        public string ShortId { get; set; }

        public Mission() : base()
        {
            this.ShortId = this.Id.Substring(0, 5);
        }
    }
}
