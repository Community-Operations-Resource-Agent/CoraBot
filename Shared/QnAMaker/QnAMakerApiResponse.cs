using System.Collections.Generic;

namespace Shared.QnAMaker
{
    public class QnAMakerApiResponse
    {
        public List<AnswerObject> Answers { get; set; }
        public string DebugInfo { get; set; }
        public bool ActiveLearningEnabled { get; set; }
    }

    public class AnswerObject
    {
        public List<string> Questions { get; set; }
        public string Answer { get; set; }
        public float Score { get; set; }
        public int Id { get; set; }
        public string Source { get; set; }
        public List<MetadataObject> Metadata { get; set; }
        public ContextObject Context { get; set; }
    }

    public class MetadataObject
    {
        public string Name { get; set; }
        public string Value { get; set; }
    }

    public class ContextObject
    {
        public bool IsContextOnly { get; set; }
        public List<string> Prompts { get; set; }
    }
}
