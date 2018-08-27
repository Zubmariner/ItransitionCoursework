namespace iCoursework.Models
{
    public class Step
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public int Index { get; set; }
        public virtual Instruction Instruction { get; set; }
        public string Text { get; set; }
        public string Image { get; set; }
    }
}