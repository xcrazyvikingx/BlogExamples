namespace MultitenancyExample.ViewModels
{
    public class DocumentGetViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Content { get; set; }
    }

    public class DocumentPostViewModel
    {
        public string Title { get; set; }
        public string Content { get; set; }
    }
}