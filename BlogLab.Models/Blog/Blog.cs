namespace BlogLab.Models.Blog
{
    public class Blog : BlogCreate
    {
        public string UserName { get; set; }
        public int ApplicationUserId { get; set; }
        public DateTime PublishDate { get; set; }
        public DateTime UpdatedDate { get; set; }
    }
}