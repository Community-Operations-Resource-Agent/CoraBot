namespace Shared.Models
{
    public class Mission : Model
    {
        public string CreatedById { get; set; }

        public string AssignedToId { get; set; }

        public string Instructions { get; set; }
    }
}
