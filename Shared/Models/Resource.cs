namespace Shared.Models
{
    public class Resource : Model
    {
        public string CreatedById { get; set; }

        public string Category { get; set; }

        public string Name { get; set; }

        public int Quantity { get; set; }

        public bool IsRecordComplete { get; set; }
    }
}
