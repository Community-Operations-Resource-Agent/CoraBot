namespace Shared.Models
{
    public class Mission : Model
    {
        public string CreatedById { get; set; }

        public string AssignedToId { get; set; }

        public string Description { get; set; }
    }
}
