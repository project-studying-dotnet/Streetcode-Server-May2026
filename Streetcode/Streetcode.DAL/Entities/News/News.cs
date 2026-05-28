using Streetcode.DAL.Entities.Media.Images;

namespace Streetcode.DAL.Entities.News
{
    public class News
    {
        public int Id { get; set; }
        required public string Title { get; set; }
        required public string Text { get; set; }
        required public string URL { get; set; }
        public int? ImageId { get; set; }
        public Image? Image { get; set; }
        public DateTime CreationDate { get; set; }
    }
}
